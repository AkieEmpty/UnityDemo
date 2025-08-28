using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkieEmpty.SkillRuntime
{
    [Serializable]
    public class SkillAudioEvent:SkillFrameEventBase
    {
#if UNITY_EDITOR
        [LabelText("轨道名称")]public string TrackName = "音效轨道";
#endif
        [LabelText("音效")] public AudioClip Clip;
        [LabelText("音量")] public float Voluem = 1;
        [LabelText("帧数索引")] public int FrameIndex = -1;
        [LabelText("播放次数")] public int PlayCount;
    }
}
