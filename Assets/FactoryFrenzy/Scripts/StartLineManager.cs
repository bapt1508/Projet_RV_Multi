using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class StartLineManager : NetworkBehaviour
{
    public TMP_Text countdownText;
    public int countdownTime = 3;
    public Canvas canva;

    private NetworkVariable<bool> raceStarted = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            foreach (var player in FindObjectsOfType<StarterAssets.ThirdPersonController>())
            {
                player.canMove = false;
            }
            canva.enabled = true;
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
    private void UpdateCountdownClientRpc(int timeLeft)
    {
        Debug.Log("Update");
        if (countdownText == null) return;

        if (timeLeft > 0)
            countdownText.text = timeLeft.ToString();
        else
            countdownText.text = "GO!";

        countdownText.fontSize = timeLeft == 0 ? 120 : 120;
        countdownText.color = timeLeft == 0 ? Color.green : Color.white;
    }

    [ClientRpc]
    private void EnableMovementClientRpc()
    {
        foreach (var player in FindObjectsOfType<StarterAssets.ThirdPersonController>())
        {
            player.canMove = true;
        }

        StartCoroutine(ClearCountdownText());
    }

    private IEnumerator ClearCountdownText()
    {
        yield return new WaitForSeconds(1f);
        countdownText.text = "";
        canva.enabled = false;
    }
}
