using System.Collections;
using System.Collections.Generic;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public abstract class CharacterControllerBase<V> : MonoBehaviour,IStateMachineOwner where V: CharacterViewBase
    {
        [SerializeField]protected CharacterController characterController;
        [SerializeField]protected V view;
        protected StateMachine stateMachine;

       

        private void Start()
        {
            //≤‚ ‘
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
