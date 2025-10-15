using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    public Vector3 lastCheckpointPos;
    public Quaternion lastCheckpointRot;
    private float fallThreshold = -5f;

    private void Start()
    {
        lastCheckpointPos = transform.position;
        lastCheckpointRot = transform.rotation;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }
    [ClientRpc]
    public void SetCheckpointClientRpc(Vector3 checkpointPos, Quaternion checkpointRot)
    {
        lastCheckpointPos = checkpointPos;
        lastCheckpointRot = checkpointRot;
        Debug.Log($"Checkpoint mis à jour : {lastCheckpointPos}, rotation : {lastCheckpointRot.eulerAngles}");
    }

    private void Respawn()
    {
        transform.position = lastCheckpointPos + Vector3.up * 2f;
        transform.rotation = lastCheckpointRot;
        Debug.Log("Respawn au checkpoint !");
    }
}
