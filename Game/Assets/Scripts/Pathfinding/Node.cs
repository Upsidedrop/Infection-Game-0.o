using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    //In case I forger: 
    public int gCost; // Distance from start
    public int hCost; // Distance from target

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY){
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    
    }

    public int fCost{
        get{
            return hCost + gCost;
        }
    }
}
