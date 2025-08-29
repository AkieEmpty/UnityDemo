using System;
using AkieEmpty.SkillRuntime;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AudioTrackItemStyle:TrackItemStyleBase
    {
        private const string trackItemAssetPath = "Assets/Editor/SkillEditor/Assets/Track/TrackItem/AudioTrackItem.uxml";
        private ISkillEditorSystem skillEditorSystem;
        private VisualElement mainDragArea;
        private Label titleLabel;
        public bool IsInit {  get; private set; }
        public void Init(ISkillEditorSystem skillEditorSystem, SkillAudioEvent skillAudioEvent, MultilineTrackStyle.ChildTrack childTrack)
        {
            this.skillEditorSystem = skillEditorSystem;
            if (!IsInit && skillAudioEvent.Clip != null)
            {
                titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
                root = titleLabel;
                mainDragArea = root.Q<VisualElement>("Main");
                childTrack.InitContent(root);
                IsInit = true;
            }
           
        }

        public virtual void SetTitle(string title)
        {
            titleLabel.text = title;
        }

        public void RegisterMouseCallback<T>(EventCallback<T> action) where T : EventBase<T>, new()
        {
            mainDragArea.RegisterCallback<T>(action);
        }

        public void ResetView(int frameUnitWdith, SkillAudioEvent skillAudioEvent)
        {
            SetTitle(skillAudioEvent.Clip.name);
            SetWidth(frameUnitWdith * skillAudioEvent.Clip.length * skillEditorSystem.SkillConfig.frameRote);
            SetPosition(frameUnitWdith * skillAudioEvent.FrameIndex);
        }
    }
}
