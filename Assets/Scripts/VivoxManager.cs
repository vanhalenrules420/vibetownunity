using System;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using System.Threading.Tasks;

public class VivoxManager : MonoBehaviour
{
    public static VivoxManager Instance;

    private string channelName = "VibeTown-MainChannel";

    float _nextPosUpdate;
    GameObject _localPlayerGameObject;
    string _playerName;

    async void Start()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();

        VivoxService.Instance.LoggedIn += onLoggedIn;
        VivoxService.Instance.LoggedOut += onLoggedOut;

    }

    public void SetLocalPlayer(GameObject player, ulong clientId)
    {
        _localPlayerGameObject = player;
        _playerName = "Player" + clientId;

        LoginUserAsync();
    }


    async Task LoginUserAsync()
    {
        Debug.Log(_playerName);
        
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
        Debug.Log("Logged In");
        JoinPositionalChannel();

    }

    private void onLoggedOut()
    {
        Debug.Log("Logged Out");
    }

       public void JoinPositionalChannel()
    {
        VivoxService.Instance.JoinGroupChannelAsync(
            channelName, 
            ChatCapability.TextAndAudio
            ); 
    }

    void Update()
    {
        if(VivoxService.Instance == null) return;
        
        if (Time.time > _nextPosUpdate && VivoxService.Instance.IsLoggedIn)
        {
            AdjustVoiceForProximity();
            _nextPosUpdate = Time.time + 0.3f; // Update every 0.3 seconds
        }
    }

    void AdjustVoiceForProximity()
{
    if (_localPlayerGameObject == null) return;

    foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
    {
        if (player != _localPlayerGameObject)
        {

        float distance = Vector3.Distance(_localPlayerGameObject.transform.position, player.transform.position);

        if (VivoxService.Instance.ActiveChannels.ContainsKey(channelName))
        {
            foreach (var participant in VivoxService.Instance.ActiveChannels[channelName])
            {                
                // if(participant.DisplayName == "Player" + player.GetComponent<NetworkObject>().OwnerClientId)
                // {
                //     participant.SetLocalVolume((int)distance * -2);
                // }
            }
        }
        }
    }
}

public void MuteToggle()
{
    if (VivoxService.Instance.IsInputDeviceMuted)
    {
        VivoxService.Instance.UnmuteInputDevice();
        Debug.Log("Microphone Unmuted");
    }
    else
    {
        VivoxService.Instance.MuteInputDevice();
        Debug.Log("Microphone Muted");
    }
}

private async void OnDestroy()
{

    if (VivoxService.Instance.IsLoggedIn)
    {
        await VivoxService.Instance.LogoutAsync();
        Debug.Log($"Vivox session logged out for {_playerName}");
    }
}
}