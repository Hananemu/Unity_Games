using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.81f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f;
    public float maxVertRotation = 90.0f;
    public float minVertRotation = -90.0f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private Camera playerCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVertRotation, maxVertRotation);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Check if the character is grounded
        bool isGrounded = controller.isGrounded;

        // Reset velocity if grounded and not jumping
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Determine current speed (walk or run)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Create movement vector relative to the camera's forward direction
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnDisable()
    {
        // Unlock the cursor when the game is paused or not in focus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}