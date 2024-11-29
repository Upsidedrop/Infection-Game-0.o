using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;

    //In case I forger: 
    public int gCost; // Distance from start
    public int hCost; // Distance from target

    public Node(bool _walkable, Vector3 _worldPosition){
        walkable = _walkable;
        worldPosition = _worldPosition;
    }

    public int fCost{
        get{
            return hCost + gCost;
        }
    }
}
