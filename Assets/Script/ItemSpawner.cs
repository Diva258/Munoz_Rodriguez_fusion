using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : NetworkBehaviour
{
    [Header("Prefab del item")]
    public NetworkPrefabRef itemPrefab;

    [Header("Cantidad máxima de items")]
    public int maxItems = 3;

    [Header("Límites del mapa")]
    public float minX = -24.6f;
    public float maxX =  24.6f;
    public float minZ = -28f;
    public float maxZ =  28f;
    public float heightY = 1f;

    [Header("Distancia mínima a jugadores")]
    public float minDistanceToPlayers = 3f;

    private List<NetworkObject> items = new List<NetworkObject>();

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            SpawnItems();
        }
    }

    void SpawnItems()
    {
        for (int i = items.Count; i < maxItems; i++)
        {
            Vector3 pos = GetRandomPositionFarFromPlayers();
            var newItem = Runner.Spawn(itemPrefab, pos, Quaternion.identity);
            items.Add(newItem);
        }
    }

    Vector3 GetRandomPositionFarFromPlayers()
    {
        for (int tries = 0; tries < 20; tries++)
        {
            Vector3 pos = new Vector3(
                Random.Range(minX, maxX),
                heightY,
                Random.Range(minZ, maxZ)
            );

            if (IsFarFromPlayers(pos))
                return pos;
        }

        // Si no encuentra buen lugar después de varios intentos
        return new Vector3(0f, heightY, 0f);
    }

    bool IsFarFromPlayers(Vector3 position)
    {
        var players = FindObjectsOfType<PlayerScore>();

        foreach (var player in players)
        {
            if (Vector3.Distance(position, player.transform.position) < minDistanceToPlayers)
                return false;
        }

        return true;
    }

    public void OnItemCollected()
    {
        if (!Runner.IsServer) return;

        items.RemoveAll(item => item == null);

        if (items.Count < maxItems)
        {
            SpawnItems();
        }
    }
}
