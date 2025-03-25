////////////////////////////
/// NetworkManagerHUD    ///
/// Selcuk Cengiz - 2024 ///
////////////////////////////

using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerHUD : MonoBehaviour
{
    private string ip = "127.0.0.1";
    private UnityTransport transport;

    private int ping;

    private void Start()
    {
        transport = gameObject.GetComponent<UnityTransport>();
    }
    private void GetPing()
    {
        if (!NetworkManager.Singleton.IsListening)
            return;

        ulong serverClientID = NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId;
        ping = Mathf.RoundToInt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(serverClientID)) / 2;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(new Vector2(15, 15), new Vector2(200, NetworkManager.Singleton.IsListening ? 115 : 140)), "Network Manager Hud");
        GUI.Label(new Rect(new Vector2(20, 32), new Vector2(200, 100)), "Connect status:");

        string status = NetworkManager.Singleton.IsListening ? NetworkManager.Singleton.IsClient ?
            (NetworkManager.Singleton.IsServer ? "Host" : "Client") : "Server" : "Closed";

        GUI.contentColor = NetworkManager.Singleton.IsListening ? Color.green : Color.red;
        GUI.Label(new Rect(new Vector2(115, 32), new Vector2(200, 100)), status);
        GUI.contentColor = Color.white;

        if (!NetworkManager.Singleton.IsListening)
        {
            ip = GUI.TextField(new Rect(new Vector2(20, 130), new Vector2(190, 20)), ip);

            if (GUI.Button(new Rect(new Vector2(20, 55), new Vector2(190, 20)), "Host"))
            {
                transport.SetConnectionData(ip, 7777);
                NetworkManager.Singleton.StartHost();
            }
            if (GUI.Button(new Rect(new Vector2(20, 80), new Vector2(190, 20)), "Client"))
            {
                transport.SetConnectionData(ip, 7777);
                NetworkManager.Singleton.StartClient();

                InvokeRepeating(nameof(GetPing), 0, 1);
            }
            if (GUI.Button(new Rect(new Vector2(20, 105), new Vector2(190, 20)), "Server"))
            {
                transport.SetConnectionData(ip, 7777);
                NetworkManager.Singleton.StartServer();
            }
        }
        else
        {
            GUI.Label(new Rect(new Vector2(20, 55), new Vector2(190, 20)), "IP:" + transport.ConnectionData.Address);
            GUI.Label(new Rect(new Vector2(20, 75), new Vector2(190, 20)), "PING:" + ping);

            if (GUI.Button(new Rect(new Vector2(20, 105), new Vector2(190, 20)), "Disconnect"))
            {
                CancelInvoke(nameof(GetPing));

                NetworkManager.Singleton.Shutdown();
            }
        }
    }
}