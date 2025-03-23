using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust speed

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;

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
        
        // Normalize so diagonal movement is not faster
        moveInput = new Vector2(moveX, moveY).normalized;

        // Update Animator parameters
        animator.SetFloat("Speed", moveInput.magnitude);
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
    }

    private void FixedUpdate()
    {
        // Move the player using Rigidbody
        rb.linearVelocity = moveInput * moveSpeed;
    }
}