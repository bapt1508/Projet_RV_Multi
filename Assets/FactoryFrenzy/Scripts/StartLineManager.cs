using System.Collections;
using StarterAssets;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class StartLineManager : NetworkBehaviour
{
    [Header("UI")]
    public TMP_Text countdownText;
    public Canvas canva;

    [Header("Countdown Settings")]
    public int countdownTime = 3;

    private NetworkVariable<bool> raceStarted = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            DisableMovementClientRpc();

            ShowCanvasClientRpc(true);

            Debug.Log("ca commence");
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        int currentTime = countdownTime;

        while (currentTime > 0)
        {
            UpdateCountdownClientRpc(currentTime);
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        UpdateCountdownClientRpc(0);
        raceStarted.Value = true;

        EnableMovementClientRpc();
    }

    [ClientRpc]
    private void DisableMovementClientRpc()
    {
        foreach (var player in FindObjectsOfType<ThirdPersonController>())
        {
            player.canMove = false;
        }
    }

    [ClientRpc]
    private void EnableMovementClientRpc()
    {
        foreach (var player in FindObjectsOfType<ThirdPersonController>())
        {
            player.canMove = true;
        }

        StartCoroutine(ClearCountdownText());
    }

    [ClientRpc]
    private void ShowCanvasClientRpc(bool show)
    {
        if (canva != null)
            canva.enabled = show;
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(int timeLeft)
    {
        if (countdownText == null) return;

        if (timeLeft > 0)
            countdownText.text = timeLeft.ToString();
        else
            countdownText.text = "GO!";

        countdownText.fontSize = 120;
        countdownText.color = (timeLeft == 0) ? Color.green : Color.white;
    }

    private IEnumerator ClearCountdownText()
    {
        yield return new WaitForSeconds(1f);
        countdownText.text = "";
        canva.enabled = false;
    }
}
