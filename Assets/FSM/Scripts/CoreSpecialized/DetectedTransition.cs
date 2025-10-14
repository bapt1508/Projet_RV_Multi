using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Transition/DetectedTransition")]
    public class DetectedTransition : Transition
    {
        [Header("Detection Settings")]
        public float detectionRange = 5f; 
        public string playerTag = "Player"; 

        public override bool Decide(BaseStateMachine stateMachine)
        {
            if (stateMachine == null || stateMachine.transform == null)
                return false;

            
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player == null)
                return false;

            
            float distance = Vector3.Distance(stateMachine.transform.position, player.transform.position);

            
            return distance <= detectionRange;
        }
    }
}
