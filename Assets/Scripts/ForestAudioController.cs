using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForestAudioController : MonoBehaviour
{
    public AudioSource forestSound;  // Reference to the AudioSource
    
    [Header("UI")]
    public Button muteButton;       // Reference to the Mute/Unmute Button
    public TextMeshProUGUI buttonText;         // Reference to the Button Text

    private bool isMuted = false;   // Track mute state

    void Start()
    {
        // Start playing the forest sound when the scene loads
        if (forestSound != null)
        {
            forestSound.Play();
        }

        // Set initial button text
        UpdateButtonText();
    }

    // Toggle mute/unmute when the button is clicked
    public void ToggleMute()
    {
        if (forestSound != null)
        {
            isMuted = !isMuted;  // Switch mute state
            forestSound.mute = isMuted;

            // Update button text
            UpdateButtonText();
        }
    }

    // Update the button text based on mute status
    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isMuted ? "Unmute Audio" : "Mute Audio";
        }
    }
}