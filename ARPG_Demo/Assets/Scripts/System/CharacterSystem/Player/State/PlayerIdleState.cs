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
            AnimationClip idleClip = GetAnimationByName("Idle");
            AnimationController.PlaySingleAniamtion(idleClip);
        }

        public override void Update()
        {
            Vector2 input = PlayerInput.GetMoveAxis();
            if (Mathf.Abs(input.x) > 0 || Mathf.Abs(input.y) > 0)
            {
                PlayerController.ChangedState(PlayerState.Move);
            }
        }
    }
}
