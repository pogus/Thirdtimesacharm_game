using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Gameplay";

    private void Start()
    {
        // Ensure NetworkManager is ready
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening)
        {
            Debug.Log("NetworkManager is ready.");
        }
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully.");
            StartCoroutine(LoadGameplaySceneAfterDelay());
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server started successfully.");
            StartCoroutine(LoadGameplaySceneAfterDelay());
        }
        else
        {
            Debug.LogError("Failed to start server.");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully.");
            // Subscribe to OnClientConnected callback to ensure we wait for client connection
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            Debug.LogError("Failed to start client.");
        }
    }

    // Waits for scene to load before continuing
    private IEnumerator LoadGameplaySceneAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }

    // Called when a client connects (or tries to connect)
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected.");

        // Now that the client is connected, spawn the player
        if (NetworkManager.Singleton.IsHost)
        {
            // Host spawns players for itself and clients
            SpawnPlayer(clientId);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client waits for host to spawn players
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject == null)
            {
                Debug.Log($"Spawning player for client {clientId}.");
                GameObject playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
                GameObject playerInstance = Instantiate(playerPrefab);

                var networkObject = playerInstance.GetComponent<NetworkObject>();
                networkObject.SpawnAsPlayerObject(clientId, true);  // This is important for assigning ownership
            }
        }
        else
        {
            Debug.LogWarning($"Client {clientId} not found or already spawned.");
        }
    }

    // Unified OnDestroy method to handle cleanup
    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            // Unsubscribe from the client connected event
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            // Additionally unsubscribe from scene event if needed
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
    }

    // Check if the player is ready to spawn after a scene load
    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            Debug.Log($"Scene '{sceneEvent.SceneName}' loaded successfully.");
            HandlePlayerSpawnAfterSceneLoad(sceneEvent);
        }
    }

    // Ensure player is spawned once the scene is loaded
    private void HandlePlayerSpawnAfterSceneLoad(SceneEvent sceneEvent)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // Host spawns players for both itself and clients
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
            }
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client waits for the host to spawn players
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
    }
}
