using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(Collider))]
public class Bumper : MonoBehaviour
{
    [Header("Bumper Settings")]
    public float bumpForce = 10f;
    [Tooltip("Ajoute une force verticale (0 = aucun saut, 1 = même force que horizontale)")]
    public float verticalBoost = 0.4f;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Cherche le CharacterController ou le script de déplacement
        ThirdPersonController controller = other.GetComponent<ThirdPersonController>();
        if (controller == null) return;

        Vector3 direction = (other.transform.position - transform.position).normalized;

        direction.y += verticalBoost;
        direction.Normalize();

        controller.ApplyExternalForce(direction * bumpForce);

    }
}