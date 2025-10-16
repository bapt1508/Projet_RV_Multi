using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class FanBehaviour : MonoBehaviour
{
    [Header("Fan Settings")]
    public float rotationSpeed = 200f; // en tours par minute (rpm)
    public float pushForce = 1f;       // force appliquée au joueur
    public Vector3 pushDirection = Vector3.forward; // direction du souffle du ventilateur
    public Transform fanModel;

    private float degreesPerSecond;

    void Start()
    {
        // Convertit la vitesse en degrés par seconde
        degreesPerSecond = (rotationSpeed / 60f) * 360f;
    }

    void Update()
    {
        // Fait tourner le ventilateur autour de l’axe Y
        fanModel.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime, Space.Self);
    }

    void OnTriggerStay(Collider other)
    {
        // Si c’est le joueur ou un objet avec un Rigidbody
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<ThirdPersonController>();

            if (controller != null)
            {
                // Direction du vent dans le monde
                Vector3 worldPushDir = transform.TransformDirection(pushDirection.normalized);

                controller.ApplyExternalForce(worldPushDir * pushForce);
            }
        }
    }
}
