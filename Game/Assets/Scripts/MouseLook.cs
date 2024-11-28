using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity;

    [SerializeField]
    Transform playerBody;

    float xRotation = 0;

    InputAction mouseTurn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        mouseTurn = InputSystem.actions.FindAction("Look");

        mouseTurn.performed += context =>
        {
            Vector2 mousePos = context.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

            xRotation -= mousePos.y;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

            playerBody.Rotate(Vector3.up, mousePos.x);
        };
    }
}
