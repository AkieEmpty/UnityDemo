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

        [LabelText("��������")] public string skillName;
        [LabelText("���֡��")] public int maxFrameCount;
        [LabelText("֡��")] public int frameRote = 30;
        [NonSerialized, OdinSerialize]
        [LabelText("���ܶ�������")] public SkillAnimationData skillAnimationData = new SkillAnimationData();
        [NonSerialized, OdinSerialize]
        [LabelText("������Ч����")] public SkillAudioData skillAudioData = new SkillAudioData();
    }
}
