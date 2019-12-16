using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
  public float mouseSensitivity = 100f;

  public Transform playerBody;

  float yRotation = 0;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update()
  {
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    yRotation -= mouseY;
    yRotation = Mathf.Clamp(yRotation, -90f, 90);

    transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    playerBody.Rotate(Vector3.up * mouseX);
  }
}
