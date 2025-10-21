using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Bullet : MonoBehaviour
{
    public Vector3 direction;
    public float bumpForce = 10f;
    public string playertag;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playertag))
        {
            if (!other.CompareTag("Player")) return;

            ThirdPersonController controller = other.GetComponent<ThirdPersonController>();

            if (controller == null) return;

            direction.y = 0.1f;

            controller.ApplyExternalForce(direction * bumpForce);

            GameObject.Destroy(gameObject);
        }
    }
}
