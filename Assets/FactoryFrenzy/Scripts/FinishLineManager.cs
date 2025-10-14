using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;

public class FinishLineManager : NetworkBehaviour
{
    [Header("UI")]
    public TMP_Text raceResultText;
    public GameObject resultPanel;  // le panneau de résultats

    private List<ulong> finishOrder = new List<ulong>(); // liste des ID des joueurs arrivés
    private bool raceEnded = false;

    private float raceEndDelay = 10f; // délai avant fin de course
    private Coroutine endCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || raceEnded) return;

        // Vérifie si c’est un joueur
        var player = other.GetComponent<NetworkObject>();
        if (player != null && player.CompareTag("Player"))
        {
            ulong playerId = player.OwnerClientId;

            // Si le joueur n’est pas déjà classé
            if (!finishOrder.Contains(playerId))
            {
                finishOrder.Add(playerId);
                int position = finishOrder.Count;

                // Signaler à tout le monde le classement du joueur
                AnnounceFinishClientRpc(playerId, position);

                // Si c’est le premier arrivé → lancer la fin du timer
                if (position == 1)
                {
                    endCoroutine = StartCoroutine(EndRaceAfterDelay());
                }
            }
        }
    }

    private IEnumerator EndRaceAfterDelay()
    {
        yield return new WaitForSeconds(raceEndDelay);
        raceEnded = true;

        // Informer tout le monde que la course est terminée
        DisplayResultsClientRpc();
    }

    [ClientRpc]
    private void AnnounceFinishClientRpc(ulong playerId, int position)
    {
        string playerName = $"Joueur {playerId}";
        Debug.Log($"{playerName} a terminé {position}e !");
    }

    [ClientRpc]
    private void DisplayResultsClientRpc()
    {
        // Récupère tous les joueurs pour faire la liste finale
        var allPlayers = FindObjectsOfType<ThirdPersonController>();
        string results = "<b>Résultats de la course</b>\n\n";

        // Classe les joueurs arrivés
        int rank = 1;
        foreach (ulong playerId in finishOrder)
        {
            results += $"{rank}ᵉ : Joueur {playerId}\n";
            rank++;
        }

        // Ajoute ceux qui n'ont pas fini
        foreach (var p in allPlayers)
        {
            if (!finishOrder.Contains(p.GetComponent<NetworkObject>().OwnerClientId))
            {
                results += $"Joueur {p.GetComponent<NetworkObject>().OwnerClientId} non arrivé\n";
            }
        }

        // Afficher le résultat sur le client local
        if (resultPanel != null) resultPanel.SetActive(true);
        if (raceResultText != null) raceResultText.text = results;
    }
}
