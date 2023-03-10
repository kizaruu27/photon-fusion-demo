using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private SessionListUIHandler sessionListUIHandler;
    
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

    public enum MatchmakingMode
    {
        AutoHostOrClient,
        Shared
    }

    public MatchmakingMode mode;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);

        if (networkRunner != null)
            networkRunner = networkRunnerInScene;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            switch (mode)
            {
                case MatchmakingMode.AutoHostOrClient :
                    StartGame(GameMode.AutoHostOrClient, "Test Session", SceneManager.GetActiveScene().buildIndex);
                    break;
                case MatchmakingMode.Shared :
                    StartGame(GameMode.Shared, "Test Session", SceneManager.GetActiveScene().buildIndex);
                    break;
            }
        }
    }

    async void StartGame(GameMode mode, string sessionName, int scene)
    {
        networkRunner.ProvideInput = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        
        Debug.Log("Network Successfull");
        
        if (mode == GameMode.AutoHostOrClient)
            Debug.Log("Auto Host");
        else if (mode == GameMode.Shared)
            Debug.Log("Shared Mode");
    }

    async void JoinLobby()
    {
        Debug.Log("Join Lobby Started");

        string lobbyID = "OurLobbyID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
            Debug.Log($"Unable to join lobby {lobbyID}");
        else 
            Debug.Log("Join Lobby OK");
    }

    public void OnJoinLobby()
    {
        JoinLobby();
    }

    public void CreateGame(string sessionName, int scene)
    {
        // Debug.Log($"Create session {sessionName} scene {sceneName} build Index {SceneUtility.GetBuildIndexByScenePath($"Scene/{sceneName}")}");
        StartGame(GameMode.Host, sessionName, scene);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");
        StartGame(GameMode.Client, sessionInfo.Name, SceneManager.GetActiveScene().buildIndex);

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = Vector3.up * 2;
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        playerList.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        
        // move input
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        data.movementInput = new Vector3(xInput, 0, zInput);
        
        // jump input
        data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
        
        // fire input
        data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));
        
        // set the input data to network
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionListUIHandler == null)
            return;

        if (sessionList.Count == 0)
        {
            Debug.Log("No Session Found");
            sessionListUIHandler.OnNoSessionFound();
        }
        else
        {
            sessionListUIHandler.ClearList();
            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);
                Debug.Log($"Found Session {sessionInfo.Name} player count: {sessionInfo.PlayerCount}");
            }
        }
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
}
