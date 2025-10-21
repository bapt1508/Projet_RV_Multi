using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/ShootActivity")]
    public class ShootActivity : Activity
    {
        [Header("Shooting Settings")]
        public GameObject projectilePrefab;
        public float fireRate = 1f;
        public float projectileSpeed = 20f;
        public float rotationSpeed = 3f;
        public string playerTag = "Player";

        private float lastFireTime = 0f;
        private Transform playerTransform;

        public override void Enter(BaseStateMachine stateMachine)
        {
            lastFireTime = -1f / fireRate;

            // Trouver le joueur le plus proche
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            if (players == null || players.Length == 0)
                return;

            float closestDistance = Mathf.Infinity;
            GameObject closestPlayer = null;

            foreach (GameObject player in players)
            {
                if (player == null)
                    continue;

                float distance = Vector3.Distance(stateMachine.transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer != null)
                playerTransform = closestPlayer.transform;
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

            if (projectile.TryGetComponent(out Rigidbody rb) && projectile.TryGetComponent(out Bullet bullet))
            {
                Vector3 direction = (playerTransform.position - stateMachine.transform.position).normalized;
                bullet.direction = stateMachine.transform.position;
                rb.velocity = direction * projectileSpeed;
            }

            GameObject.Destroy(projectile, 5f);
        }

        public override void Exit(BaseStateMachine stateMachine)
        {
            playerTransform = null;
        }
    }
}
