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

        [LabelText("技能名称")] public string skillName;
        [LabelText("最大帧数")] public int maxFrameCount;
        [LabelText("帧率")] public int frameRote = 30;
        [NonSerialized, OdinSerialize]
        [LabelText("技能动画数据")] public SkillAnimationData skillAnimationData = new SkillAnimationData();

    }
    [Serializable]
    public class SkillAnimationData
    {
        [NonSerialized, OdinSerialize]
        [LabelText("帧数据"),DictionaryDrawerSettings(KeyLabel = "帧数", ValueLabel = "动画帧数据")]
        private Dictionary<int, AnimationFrameData> frameData = new Dictionary<int, AnimationFrameData>();


        public Dictionary<int, AnimationFrameData> FrameDataDic => frameData;
    }

    public abstract class FrameDataBase { }
    [Serializable]
    public class AnimationFrameData : FrameDataBase
    {
        [LabelText("动画片段")]public AnimationClip animationClip;
        [LabelText("过渡时间")]public float transitionTime;

#if UNITY_EDITOR
        [LabelText("持续帧数")] public int durationFrame;
        public bool applyRootMotion;
#endif
    }
}
