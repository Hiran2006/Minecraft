using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float gravity = -9.81f;

    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 7f;

    [SerializeField] float playerWidth = .5f;

    float horizontalInput;
    float verticalInput;
    float mouseXInput;
    float mouseYInput;

    float verticalMomentum = 0;

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
        float speed = isSprintRequest ? sprintSpeed : walkSpeed;
        velocity = Time.deltaTime * speed * (transform.forward * verticalInput + transform.right * horizontalInput);
        verticalMomentum += gravity * Time.deltaTime;

        if (isJumpRequest)
        {
            verticalMomentum = jumpForce;
            isJumpRequest = false;
            isGrounded = false;
        }
        velocity.y += verticalMomentum * Time.deltaTime;
        verticalMomentum -= verticalMomentum * Time.deltaTime;
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
        if (world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed, -playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed, -playerWidth)))
        {
            verticalMomentum = 0;
            isGrounded = true;
            return transform.position.y-(int)transform.position.y;
        }
        else
        {
            isGrounded = false;
            return speed;
        }
    }

    float UpSpeed(float speed)
    {
        if (world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed  +2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed+2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed+2f, -playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed+2f, -playerWidth)))
        {

            verticalMomentum = 0;

            return transform.position.y - (int)transform.position.y;
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
