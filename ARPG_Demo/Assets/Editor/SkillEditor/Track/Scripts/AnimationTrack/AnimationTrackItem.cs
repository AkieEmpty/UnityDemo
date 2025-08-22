using AkieEmpty.CharacterSystem;

namespace AkieEmpty.SkillEditor
{
    public class AnimationTrackItem : TrackItemBase<AnimationTrack>
    {
        private AnimationTrackItemStyle trackItemStyle;
        public void Init(SingeleLineTrackStyle trackStyle, AnimationFrameData animationFrameData)
        {

            trackItemStyle = new AnimationTrackItemStyle();
            trackItemStyle.Init(trackStyle);
        }
    }
}