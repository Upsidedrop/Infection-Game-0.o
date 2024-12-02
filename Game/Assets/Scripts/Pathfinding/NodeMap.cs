using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class NodeMap : MonoBehaviour
{
    public Vector2 gridWorldSize;
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
        FetchGrid();
    }

    void FetchGrid(){
        string file = File.ReadAllText("Assets/Grid.json");
        StoredGridMap decompiledGrid = JsonUtility.FromJson<StoredGridMap>(file);
        gridSizeX = decompiledGrid.nodeColumns.Length;
        gridSizeY = decompiledGrid.nodeColumns[0].rows.Length;
        grid = new Node[gridSizeX, gridSizeY];

        for(int x = 0; x < gridSizeX; ++x){
            for(int y = 0; y < gridSizeY; ++y){
                Vector3 worldPos = decompiledGrid.nodeColumns[x].rows[y];
                grid[x,y] = new Node(worldPos,x,y);
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
        int x = Mathf.FloorToInt((worldPosition.x + worldDimensions.x / 2) * (gridDimensions.x / worldDimensions.x));
        int y = Mathf.FloorToInt((worldPosition.z + worldDimensions.y / 2) * (gridDimensions.y / worldDimensions.y));
        return new(x, y);
    }

}
