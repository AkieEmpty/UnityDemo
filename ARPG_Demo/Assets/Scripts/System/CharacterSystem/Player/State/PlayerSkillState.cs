using AkieEmpty.Animations;
using AkieEmpty.SkillRuntime;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerSkillState:PlayerStateBase
    {
        private SkillPlayer skillPlayer;
        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            skillPlayer = PlayerController.SkillPlayer;

        }
        public override void Enter()
        {
            // TODO:测试技能播放逻辑
            SkillConfig skillConfig = ResSystem.LoadAsset<SkillConfig>("TempSkillConfig");
            skillPlayer.PlaySkill(skillConfig, OnSkillEnd, OnRootMotion);
        }


        private void OnRootMotion(Vector3 deltaPosition, Quaternion quaternion)
        {
            deltaPosition.y = -CharacterConfig.gravity * Time.deltaTime;
            PlayerController.MoveHandle(deltaPosition);
        }
        private void OnSkillEnd()
        {
            PlayerController.ChangedState(PlayerState.Idle);
        }
      
    }
}
