using System;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    [CreateAssetMenu(menuName = "Config/Character/SkillConfig", fileName = "SkillConfig")]
    public class SkillConfig : ConfigBase
    {
#if UNITY_EDITOR
        private static Action configValidateAction;
        public static void SetValidateAction(Action action) => configValidateAction = action;
        private void OnValidate() => configValidateAction?.Invoke();
#endif

        [LabelText("技能名称")]public string skillName;
        [LabelText("最大帧数")]public int maxFrameCount;


    }
}
