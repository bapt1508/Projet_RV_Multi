using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/AlignToPlayerActivity")]
    public class AlignToPlayerActivity : Activity
    {
        [Header("Alignment Settings")]
        public float rotationSpeed = 3f;    
        public string playerTag = "Player"; 

        private Transform playerTransform;

        public override void Enter(BaseStateMachine stateMachine)
        {
            
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                playerTransform = player.transform;
        }

        public override void Execute(BaseStateMachine stateMachine)
        {
            if (stateMachine == null || playerTransform == null)
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
            // Rien de spécial à la sortie
        }
    }
}
