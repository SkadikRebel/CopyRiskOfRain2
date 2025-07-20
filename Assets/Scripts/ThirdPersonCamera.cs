using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;


    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Clamp Settings")]
    [SerializeField] public float minVerticalAngle = -90f;
    [SerializeField] public float maxVerticalAngle = 90f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 60f;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;

    private Camera cam;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Camera.Enable();
        inputActions.Camera.MouseRotation.performed += OnLook;
        inputActions.Camera.MouseRotation.canceled += OnLook;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Camera.MouseRotation.performed -= OnLook;
        inputActions.Camera.MouseRotation.canceled -= OnLook;
        inputActions.Camera.Disable();
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        //Character rotation by mouse

        if (inputActions.Player.Move.IsPressed())
        {
     
            target.Rotate(0, (lookInput.x * rotationSpeed * Time.deltaTime), 0);
        }

        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        transform.LookAt(target.position + Vector3.up * 1.5f);

        //zoom

        cameraZoom();

    }

    private void cameraZoom()
    {

        Vector2 scroll = inputActions.Camera.Zoom.ReadValue<Vector2>();
        float scrollY = scroll.y;
        if (Mathf.Abs(scrollY) > 0.01f)
        {
            cam.fieldOfView -= scrollY * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    public void RotateCharacterToCameraDirection()
    {
        // Получаем forward в плоскости XZ от камеры
        Vector3 cameraForward = target.forward;
        cameraForward.y = 0f; // игнорируем вертикаль
        cameraForward.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        target.rotation = targetRotation;
    }
}