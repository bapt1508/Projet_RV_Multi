using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Transition/ReturnToIdleTransition")]
    public class ReturnToIdleTransition : Transition
    {
        [Header("Detection Settings")]
        public float detectionRange = 5f;
        public string playerTag = "Player";

        public override bool Decide(BaseStateMachine stateMachine)
        {
            if (stateMachine == null || stateMachine.transform == null)
                return false;

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

            // Si au moins un joueur est dans le rayon
            return  closestDistance >= detectionRange;
        }
    }
}
