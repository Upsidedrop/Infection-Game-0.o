using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class NodeMap : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    [HideInInspector]
    public int gridSizeX, gridSizeY;

    public int MaxSize{
        get{
            return gridSizeX * gridSizeY;
        }
    }

    void Awake(){
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid(){
        grid = new Node[gridSizeX,gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; ++x){
            for(int y = 0; y < gridSizeY; ++y){
                Vector3 worldPos = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPos, nodeRadius,unwalkableMask));

                grid[x,y] = new Node(walkable, worldPos,x,y);
            }
        }
    }

    public List<Node> GetNeighbors(Node node){
        List<Node> neighbors = new List<Node>();

        for(int x = -1; x <=1;++x){
            for(int y = -1; y <=1;++y){
                if(x == 0 && y == 0){
                    continue;
                }

                int checkX = node.gridX +x;
                int checkY = node.gridY +y;
                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        } 
        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition){
        Vector2Int location = IndexFromWorldPoint(worldPosition, gridWorldSize, new(gridSizeX, gridSizeY));
        return grid[location.x,location.y];
    }

    public Vector2Int IndexFromWorldPoint(Vector3 worldPosition, Vector2 worldDimensions, Vector2Int gridDimensions){
        float percentX = (worldPosition.x + worldDimensions.x / 2) / worldDimensions.x;
        float percentY = (worldPosition.z + worldDimensions.y / 2) / worldDimensions.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridDimensions.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridDimensions.y - 1) * percentY);
        return new(x, y);
    }

}
