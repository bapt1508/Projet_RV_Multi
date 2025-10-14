using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Transition/AlignedTransition")]
    public class AlignedTransition : Transition
    {
        [Header("Alignment Settings")]
        [Tooltip("Angle maximum (en degrés) pour considérer le joueur comme aligné")]
        public float maxAngle = 30f;
        public string playerTag = "Player";

        public override bool Decide(BaseStateMachine stateMachine)
        {
            if (stateMachine == null || stateMachine.transform == null)
                return false;

            // Trouver le joueur le plus proche
            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
            if (players == null || players.Length == 0)
                return false;

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

            if (closestPlayer == null)
                return false;

            // Calcul de l'angle
            Vector3 toPlayer = closestPlayer.transform.position - stateMachine.transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude < 0.001f)
                return false;

            toPlayer.Normalize();
            float angle = Vector3.Angle(stateMachine.transform.forward, toPlayer);

            return angle <= maxAngle;
        }
    }
}
