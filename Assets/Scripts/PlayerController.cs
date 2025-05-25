using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{   
    public float moveSpeed = 5f;
    public float gravity = -30f;          // ⭐ 加大重力，加快上升與下墜速度
    public float jumpHeight = 1.5f;       // ⭐ 跳躍高度（不變）
    public float fallMultiplier = 2.5f;   // ⭐ 下墜時額外加速倍率
    public Transform cam;
    public bool canMove = true;
    public bool isMoving = false;
    public bool isJumping = false;

    [Header("鏡頭設定")]
    public float mouseSensitivity = 50f; // ✅ 預設可以從 20f ~ 100f 自行調整

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        if (!canMove) return;

        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movement input
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move.normalized * moveSpeed * Time.deltaTime);

        // jumping
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);

        // Camera rotation
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        
        // ✅ Detect movement and jumping
        isMoving = input.sqrMagnitude > 0.1f;
        isJumping = !isGrounded && velocity.y > 0;
    }
}
