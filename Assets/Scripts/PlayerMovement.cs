using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;  // Default idle direction (down)

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get movement input from Legacy Input System
        float moveX = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right Arrow
        float moveY = Input.GetAxisRaw("Vertical");    // W/S or Up/Down Arrow
        
        moveInput = new Vector2(moveX, moveY).normalized;

        // If moving, update last move direction
        if (moveInput.magnitude > 0)
        {
            lastMoveDirection = moveInput;
        }

        // Update Animator parameters
        animator.SetFloat("Speed", moveInput.magnitude);

        // If moving, use current direction, else use last direction
        animator.SetFloat("Horizontal", moveInput.magnitude > 0 ? moveInput.x : lastMoveDirection.x);
        animator.SetFloat("Vertical", moveInput.magnitude > 0 ? moveInput.y : lastMoveDirection.y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}