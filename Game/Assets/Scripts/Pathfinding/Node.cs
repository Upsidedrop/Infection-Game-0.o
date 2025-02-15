using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    //In case I forger: 
    public int gCost; // Distance from start
    public int hCost; // Distance from target
    public Node parent;
    public int heapIndex;

    public Node(Vector3 _worldPosition, int _gridX, int _gridY){
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    
    }

    public int fCost{
        get{
            return hCost + gCost;
        }
    }

    public int HeapIndex{
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare){
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0){
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
