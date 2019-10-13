using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlyCam : MonoBehaviour
{
    public float cameraSensitivity = 90;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;
    public float rotationSpeed = 120;

    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(2, 2);

    [Header("UI")]
    public Text speed_Text;

    private Vector2 mouseDelta;
    private Vector2 mouseAbsolute;
    private Vector2 smoothMouse;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        FlyControls();
    }

    private void FlyControls()
    {
        //Rotation
        mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1.0f / smoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1.0f / smoothing.y);
        mouseAbsolute = smoothMouse;

        transform.Rotate(-Vector3.right * mouseAbsolute.y * Time.deltaTime);
        transform.Rotate(Vector3.up * mouseAbsolute.x * Time.deltaTime);

        // Camera Keyboard rotation (Z)
        if (Input.GetKey(KeyCode.Z))
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.X))
            transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

        //Position
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * normalMoveSpeed * fastMoveFactor * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * normalMoveSpeed * slowMoveFactor * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * normalMoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * normalMoveSpeed * fastMoveFactor * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * normalMoveSpeed * slowMoveFactor * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * normalMoveSpeed * Time.deltaTime;
        }

        //Speed
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            normalMoveSpeed += Input.GetAxis("Mouse ScrollWheel") * normalMoveSpeed;
            speed_Text.text = "Speed: " + normalMoveSpeed;
        }
    }
}