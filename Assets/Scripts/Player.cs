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

    public float playerWidth = .3f;
    public float playerHeight = 2f;


    float horizontal;
    float vertical;
    float mouseHorizontal;
    float mouseVertical;
    bool jumpRequest;
    Vector3 velocity;
    float verticalMoment = 0;



    InputActionMap actionMap;

    private void OnEnable()
    {
        cam = Camera.main.transform;
        world = FindAnyObjectByType<World>();

        actionMap = InputSystem.actions.FindActionMap("Player");

    }

    private void Update()
    {
        GetPlayerInputs();
        CalculateVelocity();

        if (jumpRequest) Jump();


        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.left * mouseVertical);

        transform.Translate(velocity,Space.World);
    }

    void Jump()
    {
        verticalMoment = jumpForce;
        isGrounded = jumpRequest = false;
    }
    void CalculateVelocity()
    {
        if (verticalMoment > gravity)
            verticalMoment += Time.fixedDeltaTime * gravity;
        velocity = Time.fixedDeltaTime * (isSprinting ? sprintSpeed : walkingSpeed) *
            (transform.forward * vertical + transform.right * horizontal);

        velocity += Vector3.up * verticalMoment * Time.deltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = CheckDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = CheckUpSpeed(velocity.y);
    }
    void GetPlayerInputs()
    {
        Vector2 moveInput = actionMap.FindAction("Move").ReadValue<Vector2>();
        horizontal = moveInput.x;
        vertical = moveInput.y;

        Vector2 mouseInput = actionMap.FindAction("Look").ReadValue<Vector2>();
        mouseHorizontal = mouseInput.x;
        mouseVertical = mouseInput.y;

        jumpRequest = actionMap.FindAction("Jump").ReadValue<float>() == 1;
        isSprinting = actionMap.FindAction("Sprint").ReadValue<float>() == 1;
    }

    float CheckDownSpeed(float downSpeed)
    {
        float yOffset = transform.position.y + downSpeed;
        if (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, yOffset, transform.position.z - playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, yOffset, transform.position.z - playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, yOffset, transform.position.z + playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, yOffset, transform.position.z + playerWidth)))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return downSpeed;
        }
    }
    float CheckUpSpeed(float upSpeed)
    {
        float yOffset = transform.position.y + upSpeed + playerHeight;
        if (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, yOffset, transform.position.z - playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, yOffset, transform.position.z - playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, yOffset, transform.position.z + playerWidth))
            || world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, yOffset, transform.position.z + playerWidth)))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return upSpeed;
        }
    }

    public bool front
    {
        get
        {
            if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth))
                || world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z + playerWidth)))
                return true;
            return false;
        }
    }

    public bool back
    {
        get
        {
            if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth))
                || world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - playerWidth)))
                return true;
            return false;
        }
    }
    public bool right
    {
        get
        {
            if (world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z))
                || world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + 1, transform.position.z)))
                return true;
            return false;
        }
    }

    public bool left
    {
        get
        {
            if (world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z))
                || world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + 1, transform.position.z)))
                return true;
            return false;
        }
    }
}
