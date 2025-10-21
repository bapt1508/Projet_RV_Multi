using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/AlignToPlayerActivity")]
    public class AlignToPlayerActivity : Activity
    {
        [Header("Alignment Settings")]
        public float rotationSpeed = 5f;
        public string playerTag = "Player";

        public override void Enter(BaseStateMachine stateMachine)
        {
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
            {
                stateMachine.SetData("playerTransform", closestPlayer.transform);
            }
        }

        public override void Execute(BaseStateMachine stateMachine)
        {
            if (stateMachine == null)
                return;

            Transform playerTransform = stateMachine.GetData<Transform>("playerTransform");
            if (playerTransform == null)
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
        }

        public override void Exit(BaseStateMachine stateMachine)
        {
            stateMachine.RemoveData("playerTransform");
        }
    }
}
