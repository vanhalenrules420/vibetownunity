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

        LoginUserAsync();
    
    }

    public void SetLocalPlayer(GameObject player, ulong ClientID)
    {
        _localPlayerGameObject = player;
        _playerName = "Player" + ClientID + Guid.NewGuid().ToString().Substring(0, 8);

        Debug.Log(_playerName);
    }


    async Task LoginUserAsync()
    {
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
        VivoxService.Instance.JoinPositionalChannelAsync(
            channelName, 
            ChatCapability.TextAndAudio, 
            new Channel3DProperties(25, 5, 1.0f, AudioFadeModel.InverseByDistance)
        ); 
    }

    void Update()
{
    if (Time.time > _nextPosUpdate && VivoxService.Instance.IsLoggedIn)
    {
        if (_localPlayerGameObject != null && VivoxService.Instance.ActiveChannels.ContainsKey(channelName))
        {
            VivoxService.Instance.Set3DPosition(_localPlayerGameObject, channelName);
        }
        _nextPosUpdate += 0.3f; // Update every 0.3 seconds
    }
}

public void MuteToggle()
{
    if(VivoxService.Instance.IsInputDeviceMuted)
    {
        VivoxService.Instance.UnmuteInputDevice();
    }
    else
    {
        VivoxService.Instance.MuteInputDevice();
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