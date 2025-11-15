using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class SimpleNetworkManager : MonoBehaviour
{
    [Header("Prefab del NetworkRunner")]
    public NetworkRunner runnerPrefab;

    private NetworkRunner runnerInstance;

    public void StartHost()
    {
        Debug.Log("[NETWORK] HOST BUTTON PRESSED");
        StartNetwork(GameMode.Host);
    }

    public void StartClient()
    {
        Debug.Log("[NETWORK] CLIENT BUTTON PRESSED");
        StartNetwork(GameMode.Client);
    }

    private async void StartNetwork(GameMode mode)
    {
        Debug.Log($"[NETWORK] StartNetwork() modo: {mode}");

        // Para evitar dos runners simultáneos
        if (runnerInstance != null)
        {
            Debug.LogWarning("[NETWORK] Runner ya existe. Ignorando.");
            return;
        }

        // Crear Runner desde el prefab
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.name = "NetworkRunner";
        runnerInstance.ProvideInput = true;

        Debug.Log("[NETWORK] Iniciando sesión en Photon Fusion...");

        StartGameArgs args = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "taller_sala_1",
            PlayerCount = 2,
        };

        var result = await runnerInstance.StartGame(args);

        if (!result.Ok)
        {
            Debug.LogError($"[NETWORK] StartGame FAILED → {result.ShutdownReason}");
            runnerInstance = null;
        }
        else
        {
            Debug.Log($"[NETWORK] StartGame OK como {mode}");
        }
    }
}
