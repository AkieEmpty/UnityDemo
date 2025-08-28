using System;
using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AkieEmpty.SkillRuntime
{
    [CreateAssetMenu(menuName = "Config/Character/SkillConfig", fileName = "SkillConfig")]
    public class SkillConfig : ConfigBase
    {
#if UNITY_EDITOR
        private static Action configValidateAction;
        public static void SetValidateAction(Action action) => configValidateAction = action;
        private void OnValidate() => configValidateAction?.Invoke();
#endif

        [LabelText("技能名称")] public string skillName;
        [LabelText("最大帧数")] public int maxFrameCount;
        [LabelText("帧率")] public int frameRote = 30;
        [NonSerialized, OdinSerialize]
        [LabelText("技能动画数据")] public SkillAnimationData skillAnimationData = new SkillAnimationData();
        [NonSerialized, OdinSerialize]
        [LabelText("技能音效数据")] public SkillAudioData skillAudioData = new SkillAudioData();
    }
}
