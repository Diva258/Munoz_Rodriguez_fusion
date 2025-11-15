using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs de jugador")]
    public NetworkObject hostPrefab;    // Prefab del HOST (cápsula)
    public NetworkObject clientPrefab;  // Prefab del CLIENT (cilindro)

    private bool gameStarted = false;
    private int connectedPlayers = 0;

    // =========================================================
    //  JUGADORES / ESCENAS
    // =========================================================

    // Se llama cuando un jugador entra a la sesión
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        connectedPlayers++;
        Debug.Log($"[Fusion] OnPlayerJoined {player}. Conectados: {connectedPlayers}");

        // Solo el servidor (host) decide cuándo empezar
        if (runner.IsServer && !gameStarted && connectedPlayers >= 2)
        {
            gameStarted = true;
            Debug.Log("[Fusion] Dos jugadores conectados, cargando escena 'Game'...");
            runner.LoadScene("Game");
        }
    }

    // Se llama cuando un jugador sale de la sesión
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        connectedPlayers = Mathf.Max(connectedPlayers - 1, 0);
        Debug.Log($"[Fusion] PlayerLeft: {player}. Conectados: {connectedPlayers}");
    }

    // Se llama cuando la escena cargada por Fusion termina de cargar
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[Fusion] OnSceneLoadDone: {sceneName}");

        // Solo nos interesa la escena de juego, y solo el servidor spawnea
        if (!runner.IsServer || sceneName != "Game")
            return;

        foreach (var player in runner.ActivePlayers)
        {
            // Si es el jugador LOCAL del servidor => lo tratamos como HOST
            bool esHost = (player == runner.LocalPlayer);

            NetworkObject prefabToSpawn = esHost ? hostPrefab : clientPrefab;

            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-4f, 4f),
                1f,
                UnityEngine.Random.Range(-4f, 4f)
            );

            // Spawneamos el objeto de red
            NetworkObject obj = runner.Spawn(prefabToSpawn, spawnPos, Quaternion.identity, player);

            // Marcamos si este jugador es Host o Client en su PlayerScore
            PlayerScore score = obj.GetComponent<PlayerScore>();
            if (score != null)
            {
                score.IsHost = esHost;
                Debug.Log($"[Fusion] Marcado {(esHost ? "HOST" : "CLIENT")} para {player}");
            }
            else
            {
                Debug.LogWarning("[Fusion] El prefab no tiene PlayerScore agregado.");
            }

            Debug.Log($"[Fusion] Spawned {(esHost ? "HOST" : "CLIENT")} player for {player} en escena Game");
        }
    }

    // Se llama cuando Fusion empieza a cargar una escena
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("[Fusion] OnSceneLoadStart");
    }

    // =========================================================
    //  INPUT (WASD ORIENTADO POR LA CÁMARA)
    // =========================================================

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();

        // Cámara local (cada peer usa la suya)
        Camera cam = Camera.main;

        Vector3 camForward = Vector3.forward;
        Vector3 camRight   = Vector3.right;

        if (cam != null)
        {
            camForward = cam.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            camRight = cam.transform.right;
            camRight.y = 0;
            camRight.Normalize();
        }

        // --- SOLO WASD ---
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;
        if (Input.GetKey(KeyCode.W)) v += 1f;

        Vector3 moveDir = camForward * v + camRight * h;

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        data.moveDirection = moveDir;
        input.Set(data);
    }

    // =========================================================
    //  CALLBACKS OBLIGATORIOS (VACÍOS)
    // =========================================================

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // No usamos input missing por ahora
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"[Fusion] Shutdown: {shutdownReason}");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[Fusion] Conectado al servidor");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"[Fusion] Desconectado del servidor: {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // No usamos auth custom
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"[Fusion] Falló conexión: {reason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // No usamos mensajes custom
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // No usamos lista de sesiones en este taller
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // No usamos auth custom
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // No usamos host migration
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // No usamos datos fiables custom
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // No usamos datos fiables custom
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // No usamos AOI por ahora
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // No usamos AOI por ahora
    }
}
