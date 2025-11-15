using Fusion;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ItemPickup] Trigger con: {other.name}");

        
        if (!Object.HasStateAuthority)
            return;

       
        var player = other.GetComponentInParent<PlayerScore>();
        if (player == null)
        {
            Debug.Log("[ItemPickup] El objeto que entró no tiene PlayerScore");
            return;
        }

        Debug.Log("[ItemPickup] Item recogido por jugador, sumando punto...");
        player.AddPoint();

        var spawner = FindObjectOfType<ItemSpawner>();
        if (spawner != null)
            spawner.OnItemCollected();

        Runner.Despawn(Object);
    }
}
