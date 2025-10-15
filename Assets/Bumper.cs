using System.Collections;
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
