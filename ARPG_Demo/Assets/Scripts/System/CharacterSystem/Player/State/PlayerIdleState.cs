using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkieEmpty.InputSystem;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerIdleState:PlayerStateBase
    {
        public override void Enter()
        {
            playerController.PlayAnimation("Idle");
        }

        public override void Update()
        {
            Vector2 input = PlayerInput.GetMoveInput();
            if (Mathf.Abs(input.x) > 0 || Mathf.Abs(input.y) > 0)
            {
                playerController.ChangedState(PlayerState.Move);
            }
        }
    }
}
