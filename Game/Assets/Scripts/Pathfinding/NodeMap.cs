using Unity.Mathematics;
using UnityEngine;

public class NodeMap : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public  float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start(){
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

                grid[x,y] = new Node(walkable, worldPos);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPostition){
        float percentX = (worldPostition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPostition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x,y];
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null){
            foreach(Node n in grid){
                Gizmos.color = n.walkable? Color.blue : Color.red;
                Gizmos.DrawSphere(n.worldPosition, nodeRadius);
            }
        }
    }
}
