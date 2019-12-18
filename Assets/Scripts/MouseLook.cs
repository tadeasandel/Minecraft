using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
  [Header("Mouse Setup")]
  public float mouseSensitivity = 100f;

  public Transform playerBody;

  float yRotation = 0;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  // process of input into camera rotation
  void Update()
  {
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    yRotation -= mouseY;
    // clamps the Y axis for rotation, so that camera cannot rotate forever
    yRotation = Mathf.Clamp(yRotation, -90f, 90);

    transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    playerBody.Rotate(Vector3.up * mouseX);
  }
}
