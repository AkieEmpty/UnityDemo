using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace AkieEmpty.SkillRuntime
{
    [Serializable]
    public class SkillAudioData
    {
        [NonSerialized, OdinSerialize]
        [LabelText("音效帧数据")]
        private List<SkillAudioEvent> frameDataDic = new List<SkillAudioEvent>();


        public List<SkillAudioEvent> FrameDataDic => frameDataDic;
    }
}
