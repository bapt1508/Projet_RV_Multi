using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetGameState : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var approvalHandler = NetworkManager.Singleton.GetComponent<ConnectionApprovalHandler>();
        if (approvalHandler != null)
        {
            approvalHandler.IsGameStarted = true;
            Debug.Log("Partie commencée : nouvelles connexions bloquées");
        }
    }

    
}
