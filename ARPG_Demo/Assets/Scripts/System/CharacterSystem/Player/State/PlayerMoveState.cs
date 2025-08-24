using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkieEmpty.InputSystem;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerMoveState : PlayerStateBase
    {
        private bool applyRootMotionForMove;
        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            applyRootMotionForMove = playerController.CharacterConfig.applyRootMotionForMove;
        }
        public override void Enter()
        {

            playerController.PlayAnimation("Move");
        }

        public override void Update()
        {
            float h = PlayerInput.GetHorizontalAxis();
            float v = PlayerInput.GetVerticalAxis();
            if (h == 0 && v == 0)
            {
                playerController.ChangedState(PlayerState.Idle);
            }
            //if (!applyRootMotionForMove)
            //{

            //}

                // 处理移动
            Vector3 input = new Vector3(h, 0, v);
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
            Vector3 motion = Time.deltaTime * moveDir;
            motion.y = -9.8f * Time.deltaTime;
            playerController.MoveHandle(motion);
            playerController.RotateHandle(moveDir);

        }
    }
}
