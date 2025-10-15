using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FallSpawner : MonoBehaviour
{
    public string playertag;
    public Transform BaseSpawnPoint;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Fall Collision détectée avec : " + other.gameObject.name);

        if (other.CompareTag(playertag))
            
        {
            CharacterController cr = other.GetComponentInParent<CharacterController>();
            cr.enabled = false;
            var playerRespawn = other.GetComponentInParent<PlayerRespawn>();
            other.transform.SetPositionAndRotation(
                playerRespawn.lastCheckpointPos + UnityEngine.Vector3.up * 2f,
                playerRespawn.lastCheckpointRot
            );
            StartCoroutine(ReenableCR(cr));
        }
    }
    public IEnumerator ReenableCR(CharacterController cr)
    {
        yield return new WaitForEndOfFrame();
        cr.enabled = true;
    }
}
