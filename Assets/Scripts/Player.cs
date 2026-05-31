using System;
using UnityEngine;

[RequireComponent(typeof(ToolBar))]
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
    ToolBar toolBar;


    Vector3 velocity;

    Camera cam;

    public Transform destroyBlock;
    public Transform placeBlock;

    void Start()
    {
        cam = Camera.main;
        world = FindAnyObjectByType<World>();
        toolBar = GetComponent<ToolBar>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GetInputs();
        ApplyPhysics();
        UpdateHighlightBlock();
    }
    struct RaycastHitVoxel
    {
        public Vector3 point;       // exact intersection point
        public Vector3Int block;    // block hit
        public Vector3 normal;      // face normal
    }

    RaycastHitVoxel? RaycastVoxel(
        Vector3 start,
        Vector3 direction,
        float maxDistance,
        Func<Vector3Int, bool> isTransparent
    )
    {
        direction = direction.normalized;

        Vector3Int current = Vector3Int.FloorToInt(start);

        Vector3Int step = new Vector3Int(
            direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0),
            direction.y > 0 ? 1 : (direction.y < 0 ? -1 : 0),
            direction.z > 0 ? 1 : (direction.z < 0 ? -1 : 0)
        );

        Vector3 nextBoundary = new Vector3(
            step.x > 0 ? current.x + 1 : current.x,
            step.y > 0 ? current.y + 1 : current.y,
            step.z > 0 ? current.z + 1 : current.z
        );

        Vector3 tMax = new Vector3(
            direction.x != 0 ? (nextBoundary.x - start.x) / direction.x : float.MaxValue,
            direction.y != 0 ? (nextBoundary.y - start.y) / direction.y : float.MaxValue,
            direction.z != 0 ? (nextBoundary.z - start.z) / direction.z : float.MaxValue
        );

        Vector3 tDelta = new Vector3(
            direction.x != 0 ? Mathf.Abs(1f / direction.x) : float.MaxValue,
            direction.y != 0 ? Mathf.Abs(1f / direction.y) : float.MaxValue,
            direction.z != 0 ? Mathf.Abs(1f / direction.z) : float.MaxValue
        );

        float distance = 0f;
        Vector3 normal = Vector3.zero;

        while (distance <= maxDistance)
        {
            // 🔴 If current block is solid → hit
            if (!isTransparent(current))
            {
                return new RaycastHitVoxel
                {
                    point = start + direction * distance,
                    block = current,
                    normal = normal
                };
            }

            // Move to next voxel boundary
            if (tMax.x < tMax.y && tMax.x < tMax.z)
            {
                current.x += step.x;
                distance = tMax.x;
                tMax.x += tDelta.x;
                normal = new Vector3(-step.x, 0, 0);
            }
            else if (tMax.y < tMax.z)
            {
                current.y += step.y;
                distance = tMax.y;
                tMax.y += tDelta.y;
                normal = new Vector3(0, -step.y, 0);
            }
            else
            {
                current.z += step.z;
                distance = tMax.z;
                tMax.z += tDelta.z;
                normal = new Vector3(0, 0, -step.z);
            }

            // 🛑 Stop if we exceeded max distance
            if (distance > maxDistance)
                break;
        }

        return null; // nothing hit within range
    }

    private void UpdateHighlightBlock()
    {
        RaycastHitVoxel? hit = RaycastVoxel(
            cam.transform.position,
            cam.transform.forward,
            5f,
            pos => !world.IsSolidBlock(pos)
        );
        if (hit != null)
        {

            destroyBlock.gameObject.SetActive(true);
            placeBlock.gameObject.SetActive(true);
            destroyBlock.position = hit.Value.block;
            placeBlock.position = hit.Value.block + hit.Value.normal;
        }
        else
        {
            destroyBlock.gameObject.SetActive(false);
            placeBlock.gameObject.SetActive(false);
        }
    }

    void ApplyPhysics()
    {

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

        if (!(velocity.z > 0 && front || velocity.z < 0 && back))
            velocity.z = 0;

        if (!(velocity.x > 0 && right || velocity.x < 0 && left))
            velocity.x = 0;

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
            return transform.position.y - (int)transform.position.y;
        }
        else
        {
            isGrounded = false;
            return speed;
        }
    }

    float UpSpeed(float speed)
    {
        if (world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed + 2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed + 2f, playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(playerWidth, speed + 2f, -playerWidth)) ||
            world.IsSolidBlock(transform.position + new Vector3(-playerWidth, speed + 2f, -playerWidth)))
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
        if(destroyBlock.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                world.EditChunk(destroyBlock.position, 0);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.EditChunk(placeBlock.position, (byte)toolBar.selectedBlockID);
            }
        }
    }
}
