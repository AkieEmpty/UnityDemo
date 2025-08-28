using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace AkieEmpty.SkillRuntime
{
    [Serializable]
    public class SkillAnimationData
    {
        [NonSerialized, OdinSerialize]
        [LabelText("帧数据"),DictionaryDrawerSettings(KeyLabel = "帧数", ValueLabel = "动画帧数据")]
        private Dictionary<int, SkillAnimationEvent> frameDataDic = new Dictionary<int, SkillAnimationEvent>();


        public Dictionary<int, SkillAnimationEvent> FrameDataDic => frameDataDic;
    }
}
