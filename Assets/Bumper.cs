/*using System.Collections;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bumpForce = 5f;
    public string playertag;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision détectée avec : " + other.gameObject.name);

        if (other.CompareTag(playertag))
        {
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            CharacterController characterController = rb?.GetComponentInParent<CharacterController>();
               
            

            if (rb != null && characterController != null)
            {
                Vector3 direction = rb.transform.position-transform.position;
                

                
                characterController.enabled = false;
                rb.isKinematic = false;
               



                other.transform.SetPositionAndRotation(new Vector3(other.transform.position.x,0.3f, other.transform.position.z), other.transform.rotation);
                
                direction.y = 0.5f;
                
                Debug.Log(direction);
                rb.AddForce(direction * bumpForce, ForceMode.Impulse);


                StartCoroutine(RenableController(characterController, rb));
            }
        }
    }

    private IEnumerator RenableController(CharacterController cr, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.4f);

       
        rb.isKinematic = true;
        

        cr.enabled = true;
    }
}
*/

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bumper : MonoBehaviour
{
    [Header("Bumper Settings")]
    public float bumpForce = 10f;
    public float bumpDuration = 0.3f;
    public string playerTag = "Player";
    [Tooltip("Ajoute une force verticale (0 = aucun saut, 1 = même force que horizontale)")]
    public float verticalBoost = 0.4f;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Cherche le CharacterController ou le script de déplacement
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller == null) return;

        Vector3 direction = (other.transform.position - transform.position).normalized;

        //Ajout d’une composante verticale contrôlée
        direction.y += verticalBoost;
        direction.Normalize();

        // Lance la coroutine qui pousse le joueur
        StartCoroutine(ApplyBump(controller, direction));
    }

    private System.Collections.IEnumerator ApplyBump(CharacterController controller, Vector3 direction)
    {
        float timer = 0f;

        while (timer < bumpDuration)
        {
            controller.Move(direction * bumpForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}