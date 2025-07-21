using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    private CharacterController controller;
    private PlayerInputActions inputActions;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [SerializeField] public ThirdPersonCamera cameraScript;


    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField]
    private float smoothInputSpeed = .2f;

    [Header("Speed Settings")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float baseSpeed = 5.0f;
    [SerializeField] private float speedLerpTime = 1f;

    private Vector2 inputVector;
    private Vector2 smoothInputVector;
    private Vector2 smoothInputVelocity;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }


    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();


    void Start()
    {
        playerSpeed = baseSpeed;
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -1f;     // Чтобы не завис в воздухе
        }

        //ввода от нового Input System
        inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        //плваное движение WASD
        smoothInputVector = Vector2.SmoothDamp(smoothInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);

        Vector3 move = new Vector3(smoothInputVector.x, 0, smoothInputVector.y);
        move = transform.TransformDirection(move); // Чтобы движение шло относительно направления

        bool isSprinting = inputActions.Player.Sprint.IsPressed();
        bool isMoving = inputVector.magnitude > 0.1f;

        // Управление ускорением Shift
        float targetSpeed = isSprinting ? runSpeed : baseSpeed;

        if (isSprinting)
        {
            playerSpeed = runSpeed; // Мгновенный спринт
        }
        else if (isMoving)
        {
            // Плавное торможение после спринта
            playerSpeed = Mathf.MoveTowards(playerSpeed, baseSpeed, Time.deltaTime * (runSpeed - baseSpeed) / speedLerpTime);
        }
        else
        {
            playerSpeed = baseSpeed; // Сброс при остановке
        }

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Прыжок
        if (inputActions.Player.Jump.IsPressed() && groundedPlayer)
        {
            Debug.Log("На земле");
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Применение гравитации
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        if (inputActions.Player.Move.IsPressed())
        {
            playerRotateRealetivCamera();
        }
    }


    private float r;
    public void playerRotateRealetivCamera()
    {
        float angel = Mathf.SmoothDampAngle(player.eulerAngles.y, cameraScript.getYaw(), ref r, cameraScript.getCamSmoothTime()); // не получаю даные с скрипта
        player.rotation = Quaternion.Euler(0, angel, 0);
    }
}
