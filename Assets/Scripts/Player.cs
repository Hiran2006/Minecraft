using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool isGrounded = false;
    public bool isSprinting = false;

    Transform cam;
    World world;

    public float walkingSpeed = 3f;
    public float sprintSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    public float playerWidth = .5f;


    float horizontal;
    float vertical;
    float mouseHorizontal;
    float mouseVertical;
    bool jumpRequest;
    Vector3 velocity;


    InputAction moveAction;
    InputAction mouseAction;
    InputAction jumpAction;

    private void OnEnable()
    {
        cam = Camera.main.transform;
        world = FindAnyObjectByType<World>();

        moveAction = InputSystem.actions.FindAction("Move");
        mouseAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");

    }

    private void Update()
    {
        GetPlayerInputs();

        velocity = Time.deltaTime * walkingSpeed * ((transform.forward * vertical) + (transform.right * horizontal));
        velocity += gravity * Time.deltaTime * Vector3.up;

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.left * mouseVertical);

        transform.Translate(velocity,Space.World);
    }

    void GetPlayerInputs()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontal = moveInput.x;
        vertical = moveInput.y;

        Vector2 mouseInput = mouseAction.ReadValue<Vector2>();
        mouseHorizontal = mouseInput.x;
        mouseVertical = mouseInput.y;

        jumpRequest = jumpAction.ReadValue<float>() == 1;
    }
}
