using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : NetworkBehaviour
{
    private List<Transform> spawnPoints = new();
    private int nextSpawnIndex = 0;
    public GameObject SpawnPoints;

    private void Awake()
    {

        foreach (Transform child in SpawnPoints.transform)
            spawnPoints.Add(child);

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnAllPlayersNextFrame());
        }
    }


    private IEnumerator SpawnAllPlayersNextFrame()
    {
        yield return null;
        SpawnAllPlayers();
    }
    private void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {

            var player = client.PlayerObject;
            if (player == null) continue;
            var playerData = player.GetComponent<PlayerData>();
            var spawnPos = GetNextSpawnPosition();
            var playerSpawn = player.GetComponent<PlayerRespawn>();
            playerSpawn.lastCheckpointPos = spawnPos.transform.position;
            playerSpawn.lastCheckpointRot = spawnPos.transform.rotation;

            playerData.TeleportTo(spawnPos.position, spawnPos.rotation);
        }
    }

    private Transform GetNextSpawnPosition()
    {
        if (spawnPoints.Count == 0)
            return new GameObject("TempSpawn").transform;

        var spawn = spawnPoints[nextSpawnIndex % spawnPoints.Count];
        nextSpawnIndex++;
        return spawn;
    }
}
