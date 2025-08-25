using AkieEmpty.InputSystem;
using JKFrame;
using UnityEngine;
namespace AkieEmpty.CharacterSystem
{
    public class PlayerMoveState : PlayerStateBase
    {
        private bool applyRootMotionForMove;

        private float runTransition;
        private bool isRun;
        private float speed;
        private float h;
        private float v;
        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            applyRootMotionForMove = CharacterConfig.applyRootMotionForMove;
        }
        public override void Enter()
        {
            runTransition = 0;
            if (applyRootMotionForMove) AnimationController.SetRootMotionAction(OnRootMotion);
            AnimationClip walkClip = GetAnimationByName("Walk");
            AnimationClip runClip = GetAnimationByName("Run");
            AnimationController.PlayBlendAnimation(walkClip, runClip);
            AnimationController.SetBlendWeight(1);
        }
        public override void Exit()
        {
            if (applyRootMotionForMove) AnimationController.ClearRootMotionAction();
        }
        public override void Update()
        {
            if (CheckStateChanged()) return;

            //TODO:临时测试
            if(Input.GetKeyDown(KeyCode.Q))
            {
                PlayerController.ChangedState(PlayerState.Skill);
                return;
            }

            
            SwitchWalkAndRun();

            ControlCharacter();
        }
        private void ControlCharacter()
        {
            speed = Mathf.Lerp(CharacterConfig.walkSpeed, CharacterConfig.runSpeed, runTransition);

            Vector3 input = new Vector3(h, 0, v);
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;
            if (!applyRootMotionForMove)
            {
              
                Vector3 motion = Time.deltaTime * moveDir;
                motion.y = -9.8f * Time.deltaTime * speed;
                PlayerController.MoveHandle(motion);
            }

            PlayerController.RotateHandle(moveDir);
        }
        private bool CheckStateChanged()
        {
            h = PlayerInput.GetHorizontalAxis();
            v = PlayerInput.GetVerticalAxis();
            if (Mathf.Abs(h) < 0.1 && Mathf.Abs(v) < 0.1)
            {
                PlayerController.ChangedState(PlayerState.Idle);
                return true;
            }
            return false;
        }
        private void SwitchWalkAndRun()
        {
            //长按版
            //if (PlayerInput.GetKeyDown(InputKey.LeftShift)) isRun = true;
            //else if(PlayerInput.GetKeyUp(InputKey.LeftShift)) isRun = false;

            //切换版
            if (PlayerInput.GetKeyDown(InputKey.LeftShift)) isRun = !isRun;

            if (isRun) runTransition = Mathf.Clamp(runTransition + Time.deltaTime * CharacterConfig.walkAndRunTransitionSpeed, 0, 1);
            else runTransition = Mathf.Clamp(runTransition - Time.deltaTime * CharacterConfig.walkAndRunTransitionSpeed, 0, 1);

            AnimationController.SetBlendWeight(1 - runTransition);
        }
        private void OnRootMotion(Vector3 deltaPosition, Quaternion quaternion)
        {
            AnimationController.Speed = speed;
            deltaPosition.y = -CharacterConfig.gravity * Time.deltaTime;
            PlayerController.MoveHandle(deltaPosition);
        }
    }
}
