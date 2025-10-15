/*using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class Checkpoint : NetworkBehaviour
{
    [Header("Checkpoint Settings")]
    public Material inactiveMaterial;
    public Material activeMaterial;
    public MeshRenderer meshRenderer;

    private bool isActive = false;

    private void Start()
    {
        if (meshRenderer != null && inactiveMaterial != null)
            meshRenderer.material = inactiveMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var player = other.GetComponent<NetworkObject>();
        if (player != null && player.CompareTag("Player"))
        {
            ulong playerId = player.OwnerClientId;

            ActivateCheckpointClientRpc(playerId, transform.position);

            ActivateVisualsClientRpc();
        }
    }

    [ClientRpc]
    private void ActivateCheckpointClientRpc(ulong playerId, Vector3 checkpointPos)
    {
        var allPlayers = FindObjectsOfType<ThirdPersonController>();

        foreach (var p in allPlayers)
        {
            if (p.GetComponent<NetworkObject>().OwnerClientId == playerId)
            {
                var respawn = p.GetComponent<PlayerRespawn>();
                if (respawn != null)
                    respawn.SetCheckpoint(checkpointPos, transform.rotation);

            }
        }
    }

    [ClientRpc]
    private void ActivateVisualsClientRpc()
    {
        if (!isActive && meshRenderer != null && activeMaterial != null)
        {
            meshRenderer.material = activeMaterial;
            isActive = true;
        }
    }
}
*/

using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class Checkpoint : NetworkBehaviour
{
    [Header("Checkpoint Settings")]
    public Material inactiveMaterial;
    public Material activeMaterial;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        if (meshRenderer != null && inactiveMaterial != null)
            meshRenderer.material = inactiveMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var player = other.GetComponent<NetworkObject>();
        if (player != null && player.CompareTag("Player"))
        {
            ulong playerId = player.OwnerClientId;

            // On envoie au client spécifique via ClientRpcParams
            ActivateCheckpointClientRpc(transform.position, transform.rotation, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerId }
                }
            });
        }
    }

    [ClientRpc]
    private void ActivateCheckpointClientRpc(Vector3 checkpointPos, Quaternion checkpointRot, ClientRpcParams clientRpcParams = default)
    {
        var localPlayer = FindObjectsOfType<ThirdPersonController>();

        foreach (var p in localPlayer)
        {
            if (p.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                var respawn = p.GetComponent<PlayerRespawn>();
                if (respawn != null)
                    respawn.SetCheckpointClientRpc(checkpointPos, checkpointRot);

                if (meshRenderer != null && activeMaterial != null)
                    meshRenderer.material = activeMaterial;
            }
        }
    }
}

