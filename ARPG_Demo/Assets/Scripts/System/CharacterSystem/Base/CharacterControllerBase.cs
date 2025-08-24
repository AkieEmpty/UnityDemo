using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public abstract class CharacterControllerBase : MonoBehaviour,IStateMachineOwner
    {
        [SerializeField]protected CharacterController characterController;
        [SerializeField]protected PlayerView playerView;
        protected StateMachine stateMachine;

        private void Start()
        {
            Init();
        }
        public virtual void Init()
        {
            stateMachine = new StateMachine();
            stateMachine.Init(this);
        }
        public virtual void MoveHandle(Vector3 input) { }
        public virtual void RotateHandle(Vector3 moveDir) { }


    }
}
