using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    public float rayLength;

    InputAction interact;

    public InventoryManager manager;

    private void Start()
    {
        interact = InputSystem.actions.FindAction("Interact");

        interact.performed += context =>
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayLength))
            {
                if (hit.collider.gameObject.CompareTag("Item"))
                {
                    manager.TryPickup(hit.collider.gameObject);
                }
            }
        };
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward) * rayLength);
    }
}
