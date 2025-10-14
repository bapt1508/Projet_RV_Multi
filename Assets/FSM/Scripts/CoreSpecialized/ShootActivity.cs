using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/ShootActivity")]
    public class ShootActivity : Activity
    {
        [Header("Shooting Settings")]
        public GameObject projectilePrefab; 
        public float fireRate = 1f;         
        public float projectileSpeed = 10f;
        public float rotationSpeed = 3f;
        public string playerTag = "Player"; 

        private float lastFireTime = 0f;
        private Transform playerTransform;

        public override void Enter(BaseStateMachine stateMachine)
        {
            
            lastFireTime = -1f / fireRate;

            
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                playerTransform = player.transform;
        }

        public override void Execute(BaseStateMachine stateMachine)
        {
            if (stateMachine == null || projectilePrefab == null || playerTransform == null)
                return;

            Vector3 direction = playerTransform.position - stateMachine.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;


            Quaternion targetRotation = Quaternion.LookRotation(direction);


            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Time.time - lastFireTime >= 1f / fireRate)
            {
                Shoot(stateMachine);
                lastFireTime = Time.time;
            }
        }

        private void Shoot(BaseStateMachine stateMachine)
        {
            
            GameObject projectile = GameObject.Instantiate(
                projectilePrefab,
                stateMachine.transform.position + stateMachine.transform.forward * 1f, 
                stateMachine.transform.rotation
            );

            
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (playerTransform.position - stateMachine.transform.position).normalized;
                rb.velocity = direction * projectileSpeed;
            }

            
            GameObject.Destroy(projectile, 5f);
        }

        public override void Exit(BaseStateMachine stateMachine)
        {
            
        }
    }
}
