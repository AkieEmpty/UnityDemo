
using System;
using AkieEmpty.Animations;
using AkieEmpty.InputSystem;
using Codice.CM.Common;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerController : CharacterControllerBase<PlayerView>
    {
        [SerializeField] CharacterConfig characterConfig;
        [SerializeField] SkillPlayer skillPlayer;
        private PlayerState playerState;

        public CharacterConfig CharacterConfig { get => characterConfig; }
        public PlayerView View { get => view; }
        public AnimationController AnimationController { get => view.AnimationController; }

        public SkillPlayer SkillPlayer { get=> skillPlayer; }
        public override void Init()
        {           
            base.Init();
            view.Init(characterConfig);
            ChangedState(PlayerState.Idle);
        }
        private void Update()
        {
       
        }
        public void ChangedState(PlayerState playerState)
        {
            this.playerState = playerState;
            switch (playerState)
            {
                case PlayerState.Idle:
                    stateMachine.ChangeState<PlayerIdleState>();
                    break;
                case PlayerState.Move:
                    stateMachine.ChangeState<PlayerMoveState>();
                    break;
                case PlayerState.Skill:
                    stateMachine.ChangeState<PlayerSkillState>();
                    break;
            }
        }
        public override void MoveHandle(Vector3 input)
        {
           characterController.Move(input);
        }
        public override void RotateHandle(Vector3 moveDir)
        {
            view.transform.rotation = Quaternion.Slerp(view.transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * characterConfig.rotateSpeed);
        }


    }
}
