using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private SessionListUIHandler sessionListUIHandler;

    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

    Dictionary<int, NetworkPlayer> mapTokenWithIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();

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
                case MatchmakingMode.AutoHostOrClient:
                    StartGame(GameMode.AutoHostOrClient, GameManager.instance.GetConnectionToken(),  "Test Session", SceneManager.GetActiveScene().buildIndex);
                    break;
                case MatchmakingMode.Shared:
                    StartGame(GameMode.Shared, GameManager.instance.GetConnectionToken(),  "Test Session", SceneManager.GetActiveScene().buildIndex);
                    break;
            }
        }
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        //Buat NetworkRunner baru
        networkRunner = Instantiate(networkRunner);
        networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(hostMigrationToken);

        Debug.Log($"Host migration started");
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player)
            return ConnectionTokenUtils.HashToken(GameManager.instance.GetConnectionToken());
        else
        {
            var token = runner.GetPlayerConnectionToken();

            if (token != null)
                return ConnectionTokenUtils.HashToken(token);

            Debug.LogError("Get Player Token Invalid");

            return 0;
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        mapTokenWithIDWithNetworkPlayer.Add(token, networkPlayer);
    }
    
    async void StartGame(GameMode mode, byte[] connectionToken, string sessionName, int scene)
    {
        networkRunner.ProvideInput = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ConnectionToken = connectionToken
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
        StartGame(GameMode.Host, GameManager.instance.GetConnectionToken(), sessionName, scene);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");
        StartGame(GameMode.Client, GameManager.instance.GetConnectionToken(),  sessionInfo.Name, SceneManager.GetActiveScene().buildIndex);

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = Vector3.up * 2;
        if (runner.IsServer)
        {
            int playerToken = GetPlayerToken(runner, player);

            Debug.Log($"OnPlayerSERVERJoined, Token {playerToken}");

            if(mapTokenWithIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                Debug.Log($"Conncetion Token Lama: {playerToken}");
                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);
            }
            else
            {
                Debug.Log($"Spawn New Player Token: {playerToken}");
                NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
                playerList.Add(player, networkPlayerObject);

                networkPlayer.token = playerToken;

                mapTokenWithIDWithNetworkPlayer[playerToken] = networkPlayerObject;
            }
        }
        else
            Debug.Log("OnPlayerJoined");
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

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");

        await networkRunner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
        Debug.Log("OnShutdown");

        FindObjectOfType<BasicSpawner>().StartHostMigration(hostMigrationToken);
        StartHostMigration(hostMigrationToken);
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(HostMigrationToken hostMigrationToken)
    {
        networkRunner.ProvideInput = true;

        return networkRunner.StartGame(new StartGameArgs()
        {
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = HostMigrationResume,
            ConnectionToken = GameManager.instance.GetConnectionToken()
        });
    }

    void HostMigrationResume(NetworkRunner runner)
    {
        Debug.Log("Host Migration Resume Started");

        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNetworkObject.TryGetBehaviour<PlayerController>(out var characterController))
                runner.Spawn(resumeNetworkObject,
                    position: characterController.ReadPosition(),
                    rotation: characterController.ReadRotation(),
                    onBeforeSpawned: (runner, newNetworkObject) =>
            {
                newNetworkObject.CopyStateFrom(resumeNetworkObject);

                //Map the connection token with the new Network player
                if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                {
                    // Store Player token for reconnection
                    FindObjectOfType<BasicSpawner>().SetConnectionTokenMapping(oldNetworkPlayer.token, newNetworkObject.GetComponent<NetworkPlayer>());
                }
            });
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
}
