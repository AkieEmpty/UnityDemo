using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkieEmpty.SkillRuntime
{
    [Serializable]
    public class SkillAnimationEvent : SkillFrameEventBase
    {
        [LabelText("动画片段")]public AnimationClip animationClip;
        [LabelText("过渡时间")]public float transitionTime;

#if UNITY_EDITOR
        [LabelText("持续帧数")] public int durationFrame;
        public bool applyRootMotion;
#endif
    }
}
