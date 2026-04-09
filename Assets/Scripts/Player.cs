using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;

    float horizontalInput;
    float verticalInput;
    float mouseXInput;
    float mouseYInput;

    Vector3 velocity;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        GetInputs();

        velocity = Time.deltaTime * walkSpeed * (transform.forward * verticalInput + transform.right * horizontalInput);

        transform.Rotate(Vector3.up * mouseXInput);
        cam.transform.Rotate(Vector3.left * mouseYInput);

        transform.Translate(velocity, Space.World);
    }

    void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        mouseXInput = Input.GetAxis("Mouse X");
        mouseYInput = Input.GetAxis("Mouse Y");
    }
}
