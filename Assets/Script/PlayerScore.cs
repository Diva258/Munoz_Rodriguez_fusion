using Fusion;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    [Networked] public int Score { get; set; }

    // Ahora el rol también es Networked, para que todos sepan quién es Host
    [Networked] public bool IsHost { get; set; }

    public void AddPoint()
    {
        if (!Object.HasStateAuthority) return;

        Score++;

        if (Score >= 3)
        {
            RPC_Win();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Win()
    {
        var ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.ShowWinner(IsHost);
        }
    }
}
