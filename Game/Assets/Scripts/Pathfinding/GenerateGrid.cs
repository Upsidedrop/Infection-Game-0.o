using UnityEngine;
using System.IO;
using System;
using UnityEngine.Assertions.Must;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

[ExecuteInEditMode]
public class GenerateGrid : MonoBehaviour
{    
    NodeMap grid;
    List<Vector3> dynamicNodes = new();
    public bool placeNode;
    public bool clearNodes;
    public bool removeLastNode;
    public Vector3 currentNodePos;
    public float gizmoSize = 1;
    public float gizmoSpacing = 0.4f;
    public Vector2Int gridDimensions = new(8, 8);
    public Vector2 worldDimensions = new(50, 50);
    public bool writeFile;
    void OnValidate(){
        if(placeNode){
            placeNode = false;
            dynamicNodes.Add(currentNodePos);
        }
        if(clearNodes){
            clearNodes = false;
            dynamicNodes.Clear();
        }
        if(removeLastNode ){
            removeLastNode = false;
            if(dynamicNodes.Count == 0){
                return;
            }
            dynamicNodes.RemoveAt(dynamicNodes.Count - 1);
        }
        if(writeFile){
            writeFile = false;
            Write();
        }
    }
    void Awake(){
        grid = GetComponent<NodeMap>();
    }

    void OnDrawGizmosSelected(){
        Vector2Int[] nodeIndicies = new Vector2Int[dynamicNodes.Count];
        Gizmos.color = Color.green;
        List<Vector3> toDestroy = new();
        for(int i = 0; i < dynamicNodes.Count; ++i){
            Gizmos.DrawSphere(dynamicNodes[i], gizmoSize);
            Vector2Int index = grid.IndexFromWorldPoint(-dynamicNodes[i], worldDimensions, gridDimensions);
            if(nodeIndicies.Contains(index)){
                toDestroy.Add(dynamicNodes[i]);
            }
            nodeIndicies[i] = index;
        }
        foreach(Vector3 victim in toDestroy){
            dynamicNodes.Remove(victim);
        }
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(currentNodePos, gizmoSize);
        Vector2 nodeSize = worldDimensions / gridDimensions;
        for(int i = 0; i < gridDimensions.x; ++i){
            for(int j = 0; j < gridDimensions.y; ++j){
                Gizmos.color = nodeIndicies.Contains(new(i,j))? Color.green : Color.gray;
                Gizmos.DrawWireCube(
                    ((new Vector3(worldDimensions.x, 0, worldDimensions.y) - 
                      new Vector3(nodeSize.x, 0, nodeSize.y)) / 2) + 
                      Vector3.left * i * nodeSize.x + 
                      Vector3.back * j * nodeSize.y,
                      new(nodeSize.x - gizmoSpacing, 2, nodeSize.y - gizmoSpacing)
                    );
            }
        }
    }
    [SerializeField]
    StoredGridMap map;
    void Write(){
        map = new();
        map.nodeColumns = new NodeColumn[gridDimensions.x];
        for(int i = 0; i < map.nodeColumns.Length; ++i){
            map.nodeColumns[i] = new();
            map.nodeColumns[i].rows = new StoredNode[gridDimensions.y];
        }
        foreach(Vector3 node in dynamicNodes){
            Vector2Int gridPos = grid.IndexFromWorldPoint(node, worldDimensions, gridDimensions);
            StoredNode index;
            index = new();
            index.worldPosition = new();
            index.worldPosition = node;
            map.nodeColumns[gridPos.x].rows[gridPos.y] = index;
        }
        string mapJson = JsonUtility.ToJson(map);
        print(mapJson);
        File.WriteAllText("Assets/Grid.json", mapJson);
    }
}

[Serializable]
public class StoredGridMap{
    public NodeColumn[] nodeColumns;
}

[Serializable]
public class NodeColumn{
    public StoredNode[] rows;
}

[Serializable]
public class StoredNode{
    public Vector3 worldPosition;
}
