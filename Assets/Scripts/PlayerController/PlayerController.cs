using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float jumpHeight = 2.0f;
    public float gravity = 20.0f;
    public float fallSpeedMultiplier = 1.5f;
    public float airControlFactor = 0.3f;

    [Header("Mouse Settings")]
    public Camera playerCamera;
    public float mouseSensitivity = 2.0f;
    public float maxLookAngle = 80.0f;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0.0f;
    private float verticalSpeed = 0.0f;
    private float rotationY = 0.0f;
    private Vector3 airMoveDirection = Vector3.zero;

    [Header("Key Bindings")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(!InventoryManager.instance._inventoryPanel.activeSelf)
        {
            HandleMouseLook();
        }

        HandleMovement();
    }

    void HandleMouseLook()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rotationX, 0);
    }

    void HandleMovement()
    {
        if (controller.isGrounded)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = (Input.GetKey(forwardKey) ? 1 : 0) - (Input.GetKey(backwardKey) ? 1 : 0);
            float curSpeedY = (Input.GetKey(rightKey) ? 1 : 0) - (Input.GetKey(leftKey) ? 1 : 0);

            curSpeedX *= (Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed);
            curSpeedY *= (Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed);

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetKey(jumpKey))
            {
                verticalSpeed = Mathf.Sqrt(2 * jumpHeight * gravity);
            }

            airMoveDirection = moveDirection;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime * fallSpeedMultiplier;

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float airControlX = ((Input.GetKey(forwardKey) ? 1 : 0) - (Input.GetKey(backwardKey) ? 1 : 0)) * walkSpeed * airControlFactor;
            float airControlY = ((Input.GetKey(rightKey) ? 1 : 0) - (Input.GetKey(leftKey) ? 1 : 0)) * walkSpeed * airControlFactor;

            airMoveDirection += (forward * airControlX + right * airControlY) * Time.deltaTime;
            airMoveDirection = Vector3.ClampMagnitude(airMoveDirection, walkSpeed);

            moveDirection.x = airMoveDirection.x;
            moveDirection.z = airMoveDirection.z;
        }

        moveDirection.y = verticalSpeed;

        controller.Move(moveDirection * Time.deltaTime);
    }
}
