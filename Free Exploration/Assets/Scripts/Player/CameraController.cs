using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] float mouseSensitivity = 75.0f;

    float xRotation = 0.0f;
    float yRotation = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouse = MouseInput();

        player.Rotate(Vector3.up * mouse.x);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    Vector2 MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);

        yRotation += mouseX;
        if (yRotation > 360.0f) yRotation = 0.0f;
        else if (yRotation < 0.0f) yRotation = 360.0f;

        return new Vector2(mouseX, mouseY);
    }
}
