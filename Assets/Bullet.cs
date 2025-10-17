using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bumpForce = 5f;
    public string playertag;
    void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag(playertag))
        {
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            CharacterController characterController = rb?.GetComponentInParent<CharacterController>();



            if (rb != null && characterController != null)
            {
                //Vector3 direction = transform.position - rb.transform.position; // changer la direction
                Vector3 direction = other.transform.position - transform.position;


                characterController.enabled = false;
                rb.isKinematic = false;




                //other.transform.SetPositionAndRotation(new Vector3(other.transform.position.x, 0.3f, other.transform.position.z), other.transform.rotation);
                //direction.x = direction.x * bumpForce;
                //direction.z = direction.z * bumpForce;
                direction.y = 0.1f;

                Debug.Log(direction);
                rb.AddForce(direction*bumpForce, ForceMode.Impulse);


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
