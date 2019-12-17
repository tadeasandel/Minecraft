using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [SerializeField] CharacterController controller;

  [SerializeField] float speed;
  [SerializeField] float normalSpeed = 7f;
  [SerializeField] float runSpeed = 10f;

  [SerializeField] float gravity = -9.81f;
  [SerializeField] float jumpHeight = 3f;

  [SerializeField] Transform groundCheck;
  [SerializeField] float groundDistance = 0.4f;
  [SerializeField] LayerMask groundMask;

  [SerializeField] float doubleMoveReduction = 1.7f;

  [Range(-1, 1)] [SerializeField] float x;
  [Range(-1, 1)] [SerializeField] float z;

  Vector3 velocity;
  [SerializeField] bool isGrounded;

  float timeSinceJump;

  void Update()
  {
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    if (Input.GetKey(KeyCode.LeftShift))
    {
      speed = runSpeed;
    }
    else
    {
      speed = normalSpeed;
    }

    if (isGrounded && velocity.y < 0)
    {
      velocity.y = -2f;
      timeSinceJump = 0f;
    }

    x = Input.GetAxis("Horizontal");
    z = Input.GetAxis("Vertical");

    Vector3 moveForward = transform.right * x;
    Vector3 moveRight = transform.forward * z;

    if ((x > 0 && z > 0) || (x < 0 && z < 0) || (x > 0 && z < 0) || (x < 0 && z > 0))
    {
      x /= doubleMoveReduction;
      z /= doubleMoveReduction;
      speed /= doubleMoveReduction;
    }

    controller.Move(moveForward * speed * Time.deltaTime);
    controller.Move(moveRight * speed * Time.deltaTime);

    if (Input.GetButtonDown("Jump") && isGrounded)
    {
      velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    velocity.y += gravity * Time.deltaTime;

    controller.Move(velocity * Time.deltaTime);
  }

  public void MoveTo(Vector3 playerNewPos)
  {
    controller.enabled = false;
    transform.position = playerNewPos;
    controller.enabled = true;
  }
}
