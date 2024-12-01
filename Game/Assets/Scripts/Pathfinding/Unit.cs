using System.Collections;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Unit : MonoBehaviour
{
    public Transform target;
    public float speed = 5;
    Vector3[] path;
    int targetIndex;
    CharacterController controller;

    void Awake(){
        controller = GetComponent<CharacterController>();
    }

    void Start(){
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
        if(pathSuccessful){
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator FollowPath(){
        Vector3 currentWaypoint = path[0];

        while(true){
            if(CloseEnough(new(transform.position.x, 0 ,transform.position.z), currentWaypoint, 0.2f)){
                ++targetIndex;
                if(targetIndex >= path.Length){
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            controller.Move((currentWaypoint - new Vector3(transform.position.x, 0 ,transform.position.z)).normalized * speed * Time.deltaTime);
            yield return null;
        }
    }
    void OnDrawGizmos(){
        if(path != null){
            for (int i = targetIndex; i < path.Length; ++i){
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex){
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else{
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
    bool CloseEnough(Vector3 a, Vector3 b, float margin){
        return Vector3.Distance(a, b) < margin;
    }
}
