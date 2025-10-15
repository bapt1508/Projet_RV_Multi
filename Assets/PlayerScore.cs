using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    // Score synchronisé automatiquement entre les clients
    public NetworkVariable<int> Score = new NetworkVariable<int>(
        10,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );








    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int amount)
    {
        Score.Value = amount;
        Debug.Log("test");
    }
}
