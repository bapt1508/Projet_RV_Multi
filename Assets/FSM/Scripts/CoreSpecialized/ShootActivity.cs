using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/ShootActivity")]
    public class ShootActivity : Activity
    {
        [Header("Shooting Settings")]
        public GameObject projectilePrefab;
        public float fireRate = 1f;
        public float projectileSpeed = 15f;
        public float rotationSpeed = 3f;
        public string playerTag = "Player";

        public override void Enter(BaseStateMachine stateMachine)
        {
            stateMachine.SetData("lastFireTime", -1f / fireRate);

            Transform playerTransform = FindClosestPlayer(stateMachine);
            stateMachine.SetData("playerTransform", playerTransform);
        }

        public override void Execute(BaseStateMachine stateMachine)
        {
            float lastFireTime = stateMachine.GetData<float>("lastFireTime");
            Transform playerTransform = stateMachine.GetData<Transform>("playerTransform");

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
                Shoot(stateMachine, playerTransform);
                stateMachine.SetData("lastFireTime", Time.time);
            }
        }

        private void Shoot(BaseStateMachine stateMachine, Transform playerTransform)
        {
            GameObject projectile = GameObject.Instantiate(
                projectilePrefab,
                stateMachine.transform.Find("AXLE 40MM").transform.position + stateMachine.transform.forward * 1f,
                stateMachine.transform.rotation
            );

            if (projectile.TryGetComponent(out Rigidbody rb) && projectile.TryGetComponent(out Bullet bullet))
            {
                Vector3 direction = playerTransform.position - stateMachine.transform.position;
                direction.y = 0f;
                direction = direction.normalized;
                bullet.direction = direction;
                rb.velocity = direction * projectileSpeed;
            }

            GameObject.Destroy(projectile, 5f);
        }

        private Transform FindClosestPlayer(BaseStateMachine stateMachine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            if (players == null || players.Length == 0)
                return null;

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

            return closestPlayer?.transform;
        }

        public override void Exit(BaseStateMachine stateMachine)
        {
            stateMachine.RemoveData("playerTransform");
            stateMachine.RemoveData("lastFireTime");
        }
    }
}

