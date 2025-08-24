
using System;
using AkieEmpty.InputSystem;
using Codice.CM.Common;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerController : CharacterControllerBase
    {
        [SerializeField] CharacterConfig characterConfig;

        public CharacterConfig CharacterConfig => characterConfig;

        private PlayerState playerState;
        public override void Init()
        {
            //测试
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;



            base.Init();
            playerView.Init();
            ChangedState(PlayerState.Idle);
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
            }
        }
        public override void MoveHandle(Vector3 input)
        {
           characterController.Move(input* characterConfig.moveSpeed);
        }
        public override void RotateHandle(Vector3 moveDir)
        {
            playerView.transform.rotation = Quaternion.Slerp(playerView.transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * characterConfig.rotateSpeed);
        }
        public void PlayAnimation(string animationName)
        {
            if (characterConfig.animationDic.TryGetValue(animationName, out AnimationClip animationClip))
            {
                playerView.PlayAnimation(animationClip);
            }
        }
    }
}
