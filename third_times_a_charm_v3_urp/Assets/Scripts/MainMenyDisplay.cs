using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenyDisplay : MonoBehaviour
{
    [SerializeField] private string GameplaySceneName = "Gameplay";

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Single);
    }
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(GameplaySceneName, LoadSceneMode.Single);

    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();

    }



}
