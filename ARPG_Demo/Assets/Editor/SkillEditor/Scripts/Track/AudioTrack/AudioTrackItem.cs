using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkieEmpty.SkillRuntime;

namespace AkieEmpty.SkillEditor
{
    public class AudioTrackItem:TrackItemBase<AudioTrack>
    {
        private MultilineTrackStyle.ChildTrack childTrack;
        private AudioTrackItemStyle itemStyle;
        public void Init(int frameUnitWidth,float frameRote ,SkillAudioEvent skillAudioEvent, MultilineTrackStyle.ChildTrack childTrack)
        {
            this.childTrack = childTrack;

            itemStyle = new AudioTrackItemStyle();
            itemStyleBase = itemStyle;
            itemStyle.Init(frameUnitWidth, frameRote, skillAudioEvent, childTrack);
        }

        public void Destory()
        {
            childTrack.Destory();
        }
        public void SetTrackName(string name)
        {
            childTrack.SetTrackName(name);
        }
    }
}
