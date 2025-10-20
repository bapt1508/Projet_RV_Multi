using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLineManager : NetworkBehaviour
{
    [Header("UI")]
    private List<ulong> finishOrder = new List<ulong>();
    private bool raceEnded = false;

    private float raceEndDelay = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || raceEnded) return;

        var playerNetworkObject = other.GetComponent<NetworkObject>();
        if (playerNetworkObject != null && playerNetworkObject.CompareTag("Player"))
        {
            ulong playerId = playerNetworkObject.OwnerClientId;

            if (!finishOrder.Contains(playerId))
            {
                finishOrder.Add(playerId);
                int position = finishOrder.Count;

                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
                {
                    var playerObject = client.PlayerObject;
                    if (playerObject != null)
                    {
                        var playerData = playerObject.GetComponent<PlayerData>();
                        if (playerData != null)
                        {
                            playerData.DisableMovementAndCollisionsClientRpc();
                        }

                        var playerScore = playerObject.GetComponent<PlayerScore>();
                        if (playerScore != null)
                            playerScore.AddScoreServerRpc(position);
                    }
                }

                if (position == 1)
                {
                    StartCoroutine(EndRaceAfterDelay());
                }
            }
        }
    }

    private IEnumerator EndRaceAfterDelay()
    {
        yield return new WaitForSeconds(raceEndDelay);
        raceEnded = true;
        Debug.Log("Fin de la partie !");

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerData = client.PlayerObject?.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.DisableMovementAndCollisionsClientRpc();
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene("ScoreBoard", LoadSceneMode.Single);
    }
}
