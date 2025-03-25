using UnityEngine;
using Unity.Netcode;
using Unity.Services.Vivox;
using Unity.Services.Authentication;

public class PlayerNetwork : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down; // Default idle direction

    private NetworkVariable<Vector2> networkedPosition = new NetworkVariable<Vector2>(
        Vector2.zero, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            networkedPosition.Value = transform.position;
        }
    }

    void Start()
    {
        if (IsOwner) VivoxManager.Instance.SetLocalPlayer(this.gameObject, OwnerClientId);

    }
    private void Update()
    {
        if (IsOwner)
        {
            float moveX = Input.GetAxisRaw("Horizontal");  
            float moveY = Input.GetAxisRaw("Vertical");    

            moveInput = new Vector2(moveX, moveY).normalized;

            if (moveInput.magnitude > 0)
            {
                lastMoveDirection = moveInput;
                MoveServerRpc(moveInput);
                UpdateAnimationServerRpc(moveInput.x, moveInput.y, moveInput.magnitude);
            }
            else
            {
                // If no movement, set speed to 0 to switch to idle animation
                UpdateAnimationServerRpc(lastMoveDirection.x, lastMoveDirection.y, 0);
            }

            // Locally update the Animator parameters for instant feedback
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetFloat("Horizontal", moveInput.magnitude > 0 ? moveInput.x : lastMoveDirection.x);
            animator.SetFloat("Vertical", moveInput.magnitude > 0 ? moveInput.y : lastMoveDirection.y);
        }

        transform.position = Vector2.Lerp(transform.position, networkedPosition.Value, Time.deltaTime * 10f);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 movement)
    {
        if (movement.magnitude > 0)
        {
            Vector2 newPosition = networkedPosition.Value + (movement * moveSpeed * Time.deltaTime);
            networkedPosition.Value = newPosition;
        }
    }

    [ServerRpc]
    private void UpdateAnimationServerRpc(float horizontal, float vertical, float speed)
    {
        UpdateAnimationClientRpc(horizontal, vertical, speed);
    }

    [ClientRpc]
    private void UpdateAnimationClientRpc(float horizontal, float vertical, float speed)
    {
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }
}