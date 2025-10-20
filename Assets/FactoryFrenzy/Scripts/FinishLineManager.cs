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
    [SerializeField] private TextMeshProUGUI countdownText;

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
                            playerData.DisableMovementAndCollisionsClientRpc();

                        var playerScore = playerObject.GetComponent<PlayerScore>();
                        if (playerScore != null)
                            playerScore.AddScoreServerRpc(position);
                    }
                }

                if (position == 1)
                {
                    StartCoroutine(EndRaceCountdown());
                    StartCountdownClientRpc((int)raceEndDelay);
                }
            }
        }
    }

    [ClientRpc]
    private void StartCountdownClientRpc(int duration)
    {
        var instance = FindObjectOfType<FinishLineManager>();
        if (instance != null)
            instance.StartCoroutine(instance.RunCountdown(duration));
    }

    private IEnumerator RunCountdown(int duration)
    {
        if (countdownText == null) yield break;

        countdownText.gameObject.SetActive(true);

        for (int i = duration; i > 0; i--)
        {
            countdownText.text = $"Fin de la course dans {i}s";
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Fin de la course !";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
    private IEnumerator EndRaceCountdown()
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
