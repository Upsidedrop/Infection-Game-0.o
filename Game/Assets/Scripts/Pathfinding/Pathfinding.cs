using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public LayerMask unwalkableMask;
    PathRequestManager requestManager;
    NodeMap grid;

    public void StartFindPath(Vector3 startPos, Vector3 targetPos){
        StartCoroutine(FindPath(startPos, targetPos));
    }

    void Awake(){
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<NodeMap>();
    }
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        if(!Physics.CheckSphere(startNode.worldPosition, 0.01f,unwalkableMask) && !Physics.CheckSphere(targetNode.worldPosition, 0.01f,unwalkableMask)){
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0){
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if(currentNode == targetNode){
                    pathSuccess = true;

                    break;
                }
                foreach(Node neighbor in grid.GetNeighbors(currentNode)){
                    if(Physics.Raycast(neighbor.worldPosition + Vector3.up, currentNode.worldPosition - neighbor.worldPosition, Vector3.Distance(neighbor.worldPosition, currentNode.worldPosition), unwalkableMask) || closedSet.Contains(neighbor)){
                        print("safsa");
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode,neighbor);
                    if(newMovementCostToNeighbor < neighbor.gCost  || !openSet.Contains(neighbor)){
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if(!openSet.Contains(neighbor)){
                            openSet.Add(neighbor);
                        }
                        else{
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess){
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        List<Vector3> pathPositions = new();
        foreach(Node node in path){
            pathPositions.Add(node.worldPosition);
        }
        Vector3[] wayPoints = pathPositions.ToArray();
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path){
        List<Vector3> waypoints = new();
        Vector2 dirOld = Vector2.zero;

        for(int i = 1; i < path.Count; ++i){
            Vector2 dirNew = new(path[i].gridX - path[i-1].gridX, path[i].gridY - path[i - 1].gridY);
            print($"old: {dirOld} new: {dirNew}");
            if(true/*dirNew != dirOld*/){
                waypoints.Add(path[i].worldPosition);
                dirOld = dirNew;
            }
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB){
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distX >distY){
            return distY * 14 + (distX-distY)*10;
        }
        return distX * 14 + (distY-distX) * 10;
    }
}
