using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Vector3 dimension;
    float gravity;
    World world;
    InputAction look;
    InputAction move;

    public Transform cam;
    private void OnEnable()
    {
        look = InputSystem.actions["Look"];
        move = InputSystem.actions["Move"];
        gravity = Physics.gravity.y;
        world = GameObject.FindAnyObjectByType<World>();
    }

    private void Update()
    {
        ApplyRotation();
        ApplyMovement();
        ApplyGravity();
    }

    void ApplyMovement()
    {
        Vector2 input = move.ReadValue<Vector2>() * Time.deltaTime;
        Vector3 newPos = transform.position + transform.forward * input.y + transform.right * input.x;
        MoveForward(ref newPos);
        MoveBackward(ref newPos);
        MoveRight(ref newPos);
        MoveLeft(ref newPos);
        transform.position = newPos;
    }
    void MoveForward(ref Vector3 newPos)
    {
        if(IsSolidBlock(new Vector3(transform.position.x,newPos.y-dimension.y,newPos.z+dimension.z),
            new Vector3(transform.position.x, newPos.y + dimension.y, newPos.z+dimension.z))){
                newPos.z = (int)transform.position.z + 1 - dimension.z;
        }
    }
    void MoveBackward(ref Vector3 newPos)
    {
        if (IsSolidBlock(new Vector3(transform.position.x, newPos.y - dimension.y, newPos.z - dimension.z),
            new Vector3(transform.position.x, newPos.y + dimension.y, newPos.z - dimension.z)))
        {
            newPos.z = (int)transform.position.z + dimension.z;
        }
    }
    void MoveRight(ref Vector3 newPos)
    {
        if (IsSolidBlock(new Vector3(newPos.x+dimension.x, newPos.y - dimension.y, transform.position.z),
            new Vector3(newPos.x + dimension.x, newPos.y + dimension.y, transform.position.z)))
        {
            newPos.x = (int)transform.position.x + 1 - dimension.z;
        }
    }

    void MoveLeft(ref Vector3 newPos)
    {
        if (IsSolidBlock(new Vector3(newPos.x - dimension.x, newPos.y - dimension.y, transform.position.z),
            new Vector3(newPos.x - dimension.x, newPos.y + dimension.y, transform.position.z)))
        {
            newPos.x = (int)transform.position.x + dimension.z;
        }
    }

    void ApplyRotation()
    {
        Vector2 input = Time.deltaTime* 5 * look.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, input.x);
        Vector3 camR = cam.localEulerAngles - Vector3.right * input.y;
        Mathf.Clamp(camR.x, -90, 90);
        cam.localEulerAngles = camR;
    }



    void ApplyGravity()
    {
        Vector3 newPos = transform.position + gravity * Time.deltaTime * Vector3.up;
        if (IsSolidBlock(new Vector3(newPos.x + dimension.x, newPos.y - dimension.y, newPos.z + dimension.z))
            || IsSolidBlock(new Vector3(newPos.x + dimension.x, newPos.y - dimension.y, newPos.z - dimension.z))
            || IsSolidBlock(new Vector3(newPos.x - dimension.x, newPos.y - dimension.y, newPos.z + dimension.z))
            || IsSolidBlock(new Vector3(newPos.x - dimension.x, newPos.y - dimension.y, newPos.z - dimension.z)))
        {
            newPos.y = (int)(newPos.y + dimension.y);
        }

        transform.position = newPos;
    }

    bool IsSolidBlock(Vector3 block)
    {
        return world.blockType[world.GetBlock(block)].isSolid;
    }
    bool IsSolidBlock(Vector3 from, Vector3 to)
    {
        for (float x = from.x; x <= to.x; x++)
        {
            for (float y = from.y; y <= to.y; y++)
            {
                for (float z = from.z; z <= to.z; z++)
                {
                    if (IsSolidBlock(new Vector3(x, y, z)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
