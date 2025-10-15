using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.lineact.lit.FSM
{
    [CreateAssetMenu(menuName = "LIT/FSM/Activity/IdleActivity")]
    public class IdleActivity : Activity
    {
        public float speed = 360f; 
        public override void Enter(BaseStateMachine stateMachine)
        {
        }
        public override void Execute(BaseStateMachine stateMachine)
        {
            if (stateMachine != null && stateMachine.transform != null)
            {
                
                stateMachine.transform.Rotate(Vector3.up * speed * Time.deltaTime);
            }
        }
        public override void Exit(BaseStateMachine stateMachine)
        {
                
        }
    }
}

