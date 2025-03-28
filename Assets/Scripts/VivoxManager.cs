using System;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using System.Threading.Tasks;
using Mirror;

public class VivoxManager : NetworkBehaviour
{
    private string channelName = "VibeTown-MainChannel";

    float _nextPosUpdate;

    [SyncVar] public string _playerName;
    [SyncVar] public string playerID;

    async void Start()
    {
        if(!isLocalPlayer) return;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        GameManager.Instance.voiceChatStatusText.text = "Initializing Unity Services";

        await VivoxService.Instance.InitializeAsync();
        GameManager.Instance.voiceChatStatusText.text = "Initializing Vivox";

        VivoxService.Instance.LoggedIn += onLoggedIn;
        VivoxService.Instance.LoggedOut += onLoggedOut;

        _playerName = "Player" + GetComponent<NetworkIdentity>().netId;

        await LoginUserAsync();
    }


    async Task LoginUserAsync()
    {
        GameManager.Instance.voiceChatStatusText.text = "Logging In Vivox";
        
        // For this example, the VivoxService is initialized.
        var loginOptions = new LoginOptions()
        {
            DisplayName = _playerName,
            EnableTTS = false
        };
        await VivoxService.Instance.LoginAsync(loginOptions);
    }

    private void onLoggedIn()
    {
        GameManager.Instance.voiceChatStatusText.text = "Logged In Vivox";
        playerID = AuthenticationService.Instance.PlayerId;
        JoinPositionalChannel();

    }

    private void onLoggedOut()
    {
        Debug.Log("Logged Out");
    }

       public void JoinPositionalChannel()
    {
        GameManager.Instance.voiceChatStatusText.text = "Joining Channel";

        VivoxService.Instance.JoinGroupChannelAsync(
            channelName, 
            ChatCapability.TextAndAudio); 
    }

    void Update()
    {
        if(VivoxService.Instance == null) return;
        if(!isLocalPlayer) return;
        
        if (Time.time > _nextPosUpdate && VivoxService.Instance.IsLoggedIn)
        {
            GameManager.Instance.voiceChatStatusText.text = "Voice Enabled";
            AdjustVoiceForProximity();
            _nextPosUpdate = Time.time + 0.3f; // Update every 0.3 seconds
        }
    }

    void AdjustVoiceForProximity()
    {
    foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
    {
        if (player != gameObject)
        {

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (VivoxService.Instance.ActiveChannels.ContainsKey(channelName))
        {
            foreach (var participant in VivoxService.Instance.ActiveChannels[channelName])
            {                
                Debug.Log(participant.PlayerId + "|" + player.GetComponent<VivoxManager>().playerID);

                if(participant.PlayerId == player.GetComponent<VivoxManager>().playerID)
                {
                    participant.SetLocalVolume((int)distance * -2);
                }
            }
        }
        }
    }
}

private async void OnDestroy()
{
    if(VivoxService.Instance == null) return;

    if (VivoxService.Instance.IsLoggedIn)
    {
        await VivoxService.Instance.LogoutAsync();
        Debug.Log($"Vivox session logged out for {_playerName}");
    }
}
}