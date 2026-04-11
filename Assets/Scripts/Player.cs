using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float gravity = -9.81f;

    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 5f;

    [SerializeField] float playerWidth = .5f;

    float horizontalInput;
    float verticalInput;
    float mouseXInput;
    float mouseYInput;

    float momentum;

    bool isJumpRequest;
    bool isSprintRequest;
    bool isGrounded;

    World world;


    Vector3 velocity;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        world = FindAnyObjectByType<World>();
    }

    private void Update()
    {
        GetInputs();

        velocity = Time.deltaTime * walkSpeed * (transform.forward * verticalInput + transform.right * horizontalInput);
        velocity += Vector3.up * gravity * Time.deltaTime;

        if (isJumpRequest)
        {
            velocity.y = jumpForce;
            isJumpRequest = false;
            isGrounded = false;
        }
        if (velocity.y < 0)
            velocity.y = DownSpeed(velocity.y);
        if (velocity.y > 0)
            velocity.y = UpSpeed(velocity.y);

        if (velocity.z > 0 && front || velocity.z < 0 && back) ;
        else velocity.z = 0;

        if (velocity.x > 0 && right || velocity.x < 0 && left) ;
        else velocity.x = 0;

        transform.Rotate(Vector3.up * mouseXInput);
        cam.transform.Rotate(Vector3.left * mouseYInput);

        transform.Translate(velocity, Space.World);
    }

    float DownSpeed(float speed)
    {
        if (world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed + 2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed + 2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed + 2f, -playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed + 2f, -playerWidth)))
        {
            isGrounded = true;
            return 0;
        }
        else
        {
            isGrounded = false;
            return speed;
        }
    }

    float UpSpeed(float speed)
    {
        if (world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed, -playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed, -playerWidth)))
        {
            return 0;
        }
        else
        {
            return speed;
        }
    }

    bool front
    {
        get
        {
            if (!world.IsSolidBlock(transform.position + playerWidth * Vector3.forward) &&
                !world.IsSolidBlock(transform.position + playerWidth * Vector3.forward + Vector3.up)
            ) return true;
            return false;
        }
    }

    bool back
    {
        get
        {
            if (!world.IsSolidBlock(transform.position - playerWidth * Vector3.forward) &&
                !world.IsSolidBlock(transform.position - playerWidth * Vector3.forward + Vector3.up)
            ) return true;
            return false;
        }
    }

    bool right
    {
        get
        {
            if (!world.IsSolidBlock(transform.position + playerWidth * Vector3.right) &&
                !world.IsSolidBlock(transform.position + playerWidth * Vector3.right + Vector3.up)
            ) return true;
            return false;
        }
    }

    bool left
    {
        get
        {
            if (!world.IsSolidBlock(transform.position - playerWidth * Vector3.right) &&
                !world.IsSolidBlock(transform.position - playerWidth * Vector3.right + Vector3.up)
            ) return true;
            return false;
        }
    }


    void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint"))
        {
            isSprintRequest = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprintRequest = false;
        }
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumpRequest = true;
        }
    }
}
