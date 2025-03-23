using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    
    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Create the player input component if not already attached
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.actions = CreateInputActions();
        }
        
        // Get the move action from the player input
        moveAction = playerInput.actions.FindAction("Move");
        if (moveAction == null)
        {
            Debug.LogError("Move action not found in the input actions asset!");
        }
    }
    
    private void OnEnable()
    {
        // Enable the move action
        moveAction?.Enable();
        
        // Subscribe to the performed callback
        if (moveAction != null)
        {
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from the performed callback
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            moveAction.Disable();
        }
    }
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // Get the input value from the context
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Reset the input value when the action is canceled
        moveInput = Vector2.zero;
    }
    
    private void FixedUpdate()
    {
        // Apply movement based on the input
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
    
    private InputActionAsset CreateInputActions()
    {
        // Create a new input action asset
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        
        // Create a new action map
        var actionMap = new InputActionMap("Player");
        
        // Create a move action with WASD binding
        var moveAction = actionMap.AddAction("Move", binding: "<Keyboard>/w,<Keyboard>/s,<Keyboard>/a,<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        
        // Add the action map to the asset
        asset.AddActionMap(actionMap);
        
        return asset;
    }
}
