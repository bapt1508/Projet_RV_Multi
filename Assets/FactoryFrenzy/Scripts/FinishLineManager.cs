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


    private List<ulong> finishOrder = new List<ulong>(); // liste des ID des joueurs arrivés
    private bool raceEnded = false;

    private float raceEndDelay = 10f; // délai avant fin de course


    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || raceEnded) return;

        // Vérifie si c’est un joueur
        var player = other.GetComponent<NetworkObject>();
        if (player != null && player.CompareTag("Player"))
        {
            Debug.Log("joueur detecté");
            ulong playerId = player.OwnerClientId;

            // Si le joueur n’est pas déjà classé
            if (!finishOrder.Contains(playerId))
            {
                finishOrder.Add(playerId);
                int position = finishOrder.Count;

                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
                {
                    Debug.Log("client trouvé");
                    var playerScore = client.PlayerObject.GetComponent<PlayerScore>();
                    if (playerScore != null)
                        Debug.Log("client actif");
                    playerScore.AddScoreServerRpc(position);
                }

                // Si c’est le premier arrivé → lancer la fin du timer
                if (position == 1)
                {
                    Debug.Log("premier joueur");
                    StartCoroutine(EndRaceAfterDelay());
                }
            }
        }
    }

    private IEnumerator EndRaceAfterDelay()
    {
        yield return new WaitForSeconds(raceEndDelay);
        raceEnded = true;
        Debug.Log("fin de partie");
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerData = client.PlayerObject?.GetComponent<PlayerData>();
            if (playerData != null)
            {
                playerData.ThirdPersonController.canMove = false;
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene("ScoreBoard", LoadSceneMode.Single);


    }

    

    
}
