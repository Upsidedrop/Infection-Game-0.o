using UnityEngine;
using System.IO;
using System;
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
    public bool showGrid = true;
    public float gizmoSize = 1;
    public float gizmoSpacing = 0.4f;
    public Vector2Int gridDimensions = new(8, 8);
    public bool readFile;
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
        if(readFile){
            readFile = false;
            FetchGrid();
        }
    }

    void FetchGrid(){
        string file = File.ReadAllText("Assets/Grid.json");
        StoredGridMap decompiledGrid = JsonUtility.FromJson<StoredGridMap>(file);
        gridDimensions.x = decompiledGrid.nodeColumns.Length;
        gridDimensions.y = decompiledGrid.nodeColumns[0].rows.Length;
        dynamicNodes = new();

        for(int x = 0; x < gridDimensions.x; ++x){
            for(int y = 0; y < gridDimensions.y; ++y){
                Vector3 worldPos = decompiledGrid.nodeColumns[x].rows[y];
                dynamicNodes.Add(worldPos);
            }
        }
    }

    void Awake(){
        grid = GetComponent<NodeMap>();
    }

    void OnDrawGizmosSelected(){
        Vector2Int[] nodeIndicies = new Vector2Int[dynamicNodes.Count];
        for(int i = 0; i < nodeIndicies.Length; ++i){
            nodeIndicies[i] = Vector2Int.one * -1;
        }
        Gizmos.color = Color.green;
        List<Vector3> toDestroy = new();
        for(int i = 0; i < dynamicNodes.Count; ++i){
            Gizmos.DrawSphere(dynamicNodes[i], gizmoSize);
            Vector2Int index = grid.IndexFromWorldPoint(-dynamicNodes[i], grid.gridWorldSize, gridDimensions);
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
        if(!showGrid){
            return;   
        }
        Vector2 nodeSize = grid.gridWorldSize / gridDimensions;
        for(int i = 0; i < gridDimensions.x; ++i){
            for(int j = 0; j < gridDimensions.y; ++j){
                Gizmos.color = nodeIndicies.Contains(new(i,j))? Color.green : Color.gray;
                Gizmos.DrawWireCube(
                    ((new Vector3(grid.gridWorldSize.x, 0, grid.gridWorldSize.y) - 
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
            map.nodeColumns[i].rows = new Vector3[gridDimensions.y];
        }
        foreach(Vector3 node in dynamicNodes){
            Vector2Int gridPos = grid.IndexFromWorldPoint(node, grid.gridWorldSize, gridDimensions);
            map.nodeColumns[gridPos.x].rows[gridPos.y] = node;
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
    public Vector3[] rows;
}
