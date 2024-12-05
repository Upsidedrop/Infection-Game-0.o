using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoosterBehavior : MonoBehaviour
{
    public LayerMask unwalkable;
    Vector3 nest;
    public NodeMap gridObject;
    HashSet<Vector2Int> outerMemory = new();
    HashSet<Vector2Int> totalMemory = new();
    public CharacterController controller;
    Vector3[] path;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        nest = gridObject.NodeFromWorldPoint(transform.position).worldPosition; // So that the nest is on a node

        CheckSquare(3);

        
        Vector2Int nextPos = outerMemory.ToList()[Random.Range(0, outerMemory.Count)];
        PathRequestManager.RequestPath(transform.position, gridObject.grid[nextPos.x, nextPos.y].worldPosition, OnPathFound);
    }

    void Update(){
        CheckSquare(3);
    }

    IEnumerator ReturnToNest(){
        PathRequestManager.RequestPath(transform.position, nest, OnPathFound);
        yield return new WaitUntil(() => CloseEnough(new(transform.position.x, 0 ,transform.position.z), nest, 0.2f));
        totalMemory.Clear();
        outerMemory.Clear();
        CheckSquare(3);
        Vector2Int nextPos = outerMemory.ToList()[Random.Range(0, outerMemory.Count)];
        PathRequestManager.RequestPath(transform.position, gridObject.grid[nextPos.x, nextPos.y].worldPosition, OnPathFound);
    }

    void AddNode(Vector2Int node){
        if(totalMemory.Contains(node)){
            return;
        }
        totalMemory.Add(node);
        outerMemory.Add(node);
        Vector2Int[] temp = new Vector2Int[outerMemory.Count];
        outerMemory.CopyTo(temp);
        foreach(Vector2Int index in temp){
            foreach(Vector2Int neighbor in CheckSides(index)){
                if(!totalMemory.Contains(neighbor)){
                    goto exit;
                }
            }
            outerMemory.Remove(index);
            exit:
            continue;
        }
    }
    readonly Vector2Int[] directions = {new(0, 1), new(1,0), new(0, -1), new(-1, 0)};
    List<Vector2Int> CheckSides(Vector2Int node){
        List<Vector2Int> sides = new();
        foreach(Vector2Int direction in directions){
            
            if(node.x + direction.x >= gridObject.gridSizeX || node.x + direction.x  < 0 || node.y + direction.y  >= gridObject.gridSizeY || node.y + direction.y < 0){
                continue;
            }
            if(!Physics.Raycast(
                gridObject.grid[node.x, node.y].worldPosition,
                gridObject.grid[node.x + direction.x, node.y + direction.y].worldPosition - gridObject.grid[node.x, node.y].worldPosition,
                Vector3.Distance(gridObject.grid[node.x, node.y].worldPosition, gridObject.grid[node.x + direction.x, node.y + direction.y].worldPosition),
                unwalkable)){
                sides.Add(node + direction);
            }
        }
        
        return sides;
    }
    void CheckSquare(int size){
        Vector2Int pos = gridObject.IndexFromWorldPoint(transform.position);
        for(int i = 0; i < size; ++i){
            if(pos.x + i - Mathf.FloorToInt(size/2) >= gridObject.gridSizeX || pos.x + i - Mathf.FloorToInt(size/2) < 0){
                continue;
            }
            for(int j = 0; j < size; ++j){
                if(pos.y + j - Mathf.FloorToInt(size/2) >= gridObject.gridSizeY || pos.y + j - Mathf.FloorToInt(size/2) < 0){
                   continue;
                }
                Vector2Int index = pos + new Vector2Int(i,j) - Vector2Int.one * Mathf.FloorToInt(size/2);
                if(!Physics.Raycast(transform.position, gridObject.grid[index.x, index.y].worldPosition - transform.position, Vector3.Distance(gridObject.grid[index.x, index.y].worldPosition,transform.position),unwalkable)){
                    AddNode(index);
                }
            }
        }
    }
    void OnDrawGizmosSelected(){
        if(gridObject.grid == null){
            return;
        }
        foreach(Vector2Int vector2Int in outerMemory){
            Gizmos.DrawSphere(gridObject.grid[vector2Int.x, vector2Int.y].worldPosition,0.2f);
        }
        foreach(Vector2Int vector2Int in totalMemory){
            Gizmos.color = Color.red;
            if(!outerMemory.Contains(vector2Int)){
                Gizmos.DrawSphere(gridObject.grid[vector2Int.x, vector2Int.y].worldPosition,0.2f);
            }
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
        if(pathSuccessful){
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator FollowPath(){
        int targetIndex = 0;
        if(path.Length <= 0){
            goto FinishPath;
        }
        Vector3 currentWaypoint = path[0];

        while(true){
            if(CloseEnough(new(transform.position.x, 0 ,transform.position.z), currentWaypoint, 0.2f)){
                ++targetIndex;
                if(targetIndex >= path.Length){
                    goto FinishPath;
                }
                currentWaypoint = path[targetIndex];
            }
            controller.Move((currentWaypoint - new Vector3(transform.position.x, 0 ,transform.position.z)).normalized * 5 * Time.deltaTime);
            yield return null;
        }
        FinishPath:
            Vector2Int nextPos = outerMemory.ToList()[Random.Range(0, outerMemory.Count)];
            PathRequestManager.RequestPath(transform.position, gridObject.grid[nextPos.x, nextPos.y].worldPosition, OnPathFound);
    }
    bool CloseEnough(Vector3 a, Vector3 b, float margin){
        return Vector3.Distance(a, b) < margin;
    }
}
