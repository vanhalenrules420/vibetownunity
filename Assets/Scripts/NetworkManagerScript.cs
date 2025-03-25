using UnityEngine;
using Unity.Netcode;

public class NetworkManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;

   private void Start()
{
    if (NetworkManager.Singleton == null)
    {
        Debug.LogError("NetworkManager is missing from the scene.");
        return;
    }

    // Subscribe to the client connection event BEFORE starting the host
    NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    Debug.Log("OnClientConnectedCallback registered.");
}

    private void SpawnPlayer(ulong clientId)
{
    if (!NetworkManager.Singleton.IsServer) return; // Ensure this only runs on the server

    Debug.Log($"Spawning player for client {clientId}");

    if (playerPrefab == null)
    {
        Debug.LogError("playerPrefab is not assigned in NetworkManagerScript.");
        return;
    }

    GameObject player = Instantiate(playerPrefab);
    NetworkObject netObj = player.GetComponent<NetworkObject>();

    if (netObj == null)
    {
        Debug.LogError("Player prefab is missing a NetworkObject component!");
        return;
    }

    netObj.SpawnAsPlayerObject(clientId);
    }


    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
