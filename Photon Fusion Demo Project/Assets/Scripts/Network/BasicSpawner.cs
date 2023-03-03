using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>();

    public enum MatchmakingMode
    {
        AutoHostOrClient,
        Shared
    }

    public MatchmakingMode mode;
    
    private void Start()
    {
        switch (mode)
        {
            case MatchmakingMode.AutoHostOrClient :
                StartGame(GameMode.AutoHostOrClient);
                break;
            case MatchmakingMode.Shared :
                StartGame(GameMode.Shared);
                break;
        }
    }

    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        
        Debug.Log("Network Successfull");
        
        if (mode == GameMode.AutoHostOrClient)
            Debug.Log("Auto Host");
        else if (mode == GameMode.Shared)
            Debug.Log("Shared Mode");
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

        // if (Input.GetKey(KeyCode.W))
        //     data.movementInput += Vector3.forward;
        //
        // if (Input.GetKey(KeyCode.S))
        //     data.movementInput += Vector3.back;
        //
        // if (Input.GetKey(KeyCode.A))
        //     data.movementInput += Vector3.left;
        //
        // if (Input.GetKey(KeyCode.D))
        //     data.movementInput += Vector3.right;

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

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
}
