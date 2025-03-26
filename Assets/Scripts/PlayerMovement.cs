using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down; // Default idle direction (down)

    Vector2 lastMoveInput;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return; // Only local player processes input

        if(GameManager.Instance.cinemachineCamera.Target.TrackingTarget == null)
        {
            GameManager.Instance.cinemachineCamera.Target.TrackingTarget = this.transform;     
        }

        float moveX = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right Arrow
        float moveY = Input.GetAxisRaw("Vertical");    // W/S or Up/Down Arrow

        var moveInput = new Vector2(moveX, moveY).normalized;
        
        if(lastMoveInput != moveInput)
        {
            CmdMove(moveInput);
            lastMoveInput = moveInput;
        }
    }

    [Command]
    private void CmdMove(Vector2 input)
    {
        if (input.magnitude > 0)
        {
            lastMoveDirection = input;
        }

        // Server processes movement
        rb.linearVelocity = input * moveSpeed;

        // Update Animator parameters (NetworkAnimator will sync this)
        animator.SetFloat("Speed", input.magnitude);
        animator.SetFloat("Horizontal", input.magnitude > 0 ? input.x : lastMoveDirection.x);
        animator.SetFloat("Vertical", input.magnitude > 0 ? input.y : lastMoveDirection.y);
    }
}