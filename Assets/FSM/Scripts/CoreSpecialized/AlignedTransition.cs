using System.Collections;
using System.Collections.Generic;
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

            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player == null)
                return false;

            Vector3 toPlayer = player.transform.position - stateMachine.transform.position;
            toPlayer.y = 0f; 

            if (toPlayer.sqrMagnitude < 0.001f)
                return false;

            
            toPlayer.Normalize();

          
            float angle = Vector3.Angle(stateMachine.transform.forward, toPlayer);

            
            return angle <= maxAngle;
        }
    }
}
