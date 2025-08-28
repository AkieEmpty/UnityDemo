using AkieEmpty.SkillRuntime;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AudioTrackItemStyle:TrackItemStyleBase
    {
        private const string trackItemAssetPath = "Assets/Editor/SkillEditor/Assets/Track/TrackItem/AudioTrackItem.uxml";
        private Label titleLabel;
        public void Init(float frameUnitWidth, float frameRote, SkillAudioEvent skillAudioEvent, MultilineTrackStyle.ChildTrack childTrack)
        {
            if (skillAudioEvent.Clip != null)
            {
                titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
                root = titleLabel;
                childTrack.InitContent(root);
                SetTitle(skillAudioEvent.Clip.name);
                SetWidth(frameUnitWidth * skillAudioEvent.Clip.length * frameRote);
                SetPosition(frameUnitWidth * skillAudioEvent.FrameIndex);
            }
           
        }

        public virtual void SetTitle(string title)
        {
            titleLabel.text = title;
        }
    }
}
