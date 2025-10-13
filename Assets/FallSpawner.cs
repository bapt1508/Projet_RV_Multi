using System.Collections;
using System.Collections.Generic;
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
            Debug.Log("test");
            CharacterController cr = other.GetComponentInParent<CharacterController>();
            cr.enabled = false;
            other.transform.SetPositionAndRotation(BaseSpawnPoint.position, BaseSpawnPoint.rotation);
            StartCoroutine(ReenableCR(cr));
        }
    }
    public IEnumerator ReenableCR(CharacterController cr)
    {
        yield return new WaitForEndOfFrame();
        cr.enabled = true;
    }
}
