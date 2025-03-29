using TMPro;
using Unity.Cinemachine;
using Unity.Services.Vivox;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CinemachineCamera cinemachineCamera;


    [Header("UI")]
    public TMP_Text voiceChatStatusText;
    public TMP_Text muteButtonText;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void MuteToggle()
    {
    if (VivoxService.Instance.IsInputDeviceMuted)
    {
        VivoxService.Instance.UnmuteInputDevice();
        Debug.Log("Microphone Unmuted");
        muteButtonText.text = "Mute";
    }
    else
    {
        VivoxService.Instance.MuteInputDevice();
        Debug.Log("Microphone Muted");
        muteButtonText.text = "Unmute";
    }
    }
}
