using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ScoreBoardManager : NetworkBehaviour
{
    public GameObject RowPrefab;
    public GameObject Panel;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
            StartCoroutine(WaitForClientsThenDisplayScore());
    }

    private IEnumerator WaitForClientsThenDisplayScore()
    {
        // attends une frame ou deux pour que les clients spawnenet ce script
        yield return new WaitForSeconds(0.2f);

        var pseudosList = new List<FixedString64Bytes>();
        var scoresList = new List<int>();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerData = client.PlayerObject?.GetComponent<PlayerData>();
            var playerScore = client.PlayerObject?.GetComponent<PlayerScore>();

            if (playerData != null && playerScore != null)
            {
                pseudosList.Add(new FixedString64Bytes(playerData.PlayerPseudo.Value));
                scoresList.Add(playerScore.Score.Value);
            }
        }

        DisplayScoreboardClientRpc(pseudosList.ToArray(), scoresList.ToArray());
        EndGameClientRpc();
        StopNetwork();
    }




    [ClientRpc]
    private void DisplayScoreboardClientRpc(FixedString64Bytes[] pseudos, int[] scores)
    {

        foreach (Transform child in Panel.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < pseudos.Length; i++)
        {
            var row = Instantiate(RowPrefab, Panel.transform);
            var texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = pseudos[i].ToString();
            texts[1].text = scores[i].ToString();
        }
        StartCoroutine(UnlockCursorNextFrame());


    }

    private IEnumerator UnlockCursorNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    [ClientRpc]
    private void EndGameClientRpc()
    {
        Debug.Log("Fin de partie reçue - arrêt du client...");
        StopNetwork();
    }

    private void StopNetwork()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log(">> Shutdown Host");
            NetworkManager.Singleton.Shutdown();
            StartCoroutine(WaitForClientToFinish());


        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log(">> Disconnect Client");
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);

        }
    }

    public IEnumerator WaitForClientToFinish()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(NetworkManager.Singleton.gameObject);
    }



}



