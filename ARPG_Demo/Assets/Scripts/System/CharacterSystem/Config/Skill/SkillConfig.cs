using System;
using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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

        [LabelText("��������")] public string skillName;
        [LabelText("���֡��")] public int maxFrameCount;
        [LabelText("֡��")] public int frameRote = 30;
        [NonSerialized, OdinSerialize]
        [LabelText("���ܶ�������")] public SkillAnimationData skillAnimationData = new SkillAnimationData();

    }
    [Serializable]
    public class SkillAnimationData
    {
        [NonSerialized, OdinSerialize]
        [LabelText("֡����"),DictionaryDrawerSettings(KeyLabel = "֡��", ValueLabel = "����֡����")]
        private Dictionary<int, AnimationFrameData> frameData = new Dictionary<int, AnimationFrameData>();


        public Dictionary<int, AnimationFrameData> FrameDataDic => frameData;
    }

    public abstract class FrameDataBase { }
    [Serializable]
    public class AnimationFrameData : FrameDataBase
    {
        [LabelText("����Ƭ��")]public AnimationClip animationClip;
        [LabelText("����ʱ��")]public float transitionTime;

#if UNITY_EDITOR
        [LabelText("����֡��")] public int durationFrame;
        public bool applyRootMotion;
#endif
    }
}
