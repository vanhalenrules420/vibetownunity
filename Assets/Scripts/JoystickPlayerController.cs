using UnityEngine;
using Mirror; // Assuming Mirror is used for networking

public class JoystickPlayerController : NetworkBehaviour
{
    [SerializeField] private Joystick joystick; // Reference to the joystick

    private PlayerMovement playerMovement; // Reference to the PlayerMovement script

    void Start()
    {
        // Check if this is the local player
        if (isLocalPlayer)
        {
            AssignJoystick();
        }
        else
        {
            // Remove JoystickPlayerController script if not the local player
            Destroy(this); // Remove the JoystickPlayerController script
            return;
        }

        // Get the PlayerMovement script attached to the player
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Only handle movement for the local player
        if (isLocalPlayer && joystick != null)
        {
            HandlePlayerMovement();
        }
    }

    // Assign the joystick to the player
    private void AssignJoystick()
    {
        // Find the joystick in the scene (this could be done dynamically)
        joystick = FindObjectOfType<Joystick>();

        if (joystick == null)
        {
            Debug.LogError("Joystick not found!");
        }
    }

    // Handle player movement based on joystick input
    private void HandlePlayerMovement()
    {
        // Get joystick input (Horizontal and Vertical)
        Vector2 input = new Vector2(joystick.Horizontal, joystick.Vertical);
        
        // If there is no input, no movement
        if (input.magnitude > 0)
        {
            Vector2 movement = new Vector2(input.x, input.y).normalized;

            // Move the player using the PlayerMovement script
            playerMovement.Move(movement);
        }
    }
}
