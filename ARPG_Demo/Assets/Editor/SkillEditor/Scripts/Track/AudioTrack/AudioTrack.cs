using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{

    public class AudioTrack : TrackBase
    {
        private const string TrackName = "“Ù–ß≈‰÷√";
        private readonly ISkillEditorSystem skillEditorSystem;
        private readonly ISkillEditorWindow skillEditorWindow;
        private MultilineTrackStyle skillTrackStyle;

        public AudioTrack(ISkillEditorSystem skillEditorSystem, ISkillEditorWindow skillEditorWindow) 
        {
            this.skillEditorSystem = skillEditorSystem;
            this.skillEditorWindow = skillEditorWindow;
        }
        public override void Init(VisualElement menuParent, VisualElement trackParent, int frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            skillTrackStyle = new MultilineTrackStyle();
            skillTrackStyle.Init(menuParent, trackParent, TrackName, CheckAddChildTrack, CheckDeleteChildTrack);
        }

        private bool CheckAddChildTrack()
        {
            return true;
        }
        private bool CheckDeleteChildTrack(int index)
        {
            return true;
        }
        public override void Destory()
        {
            skillTrackStyle.Destory();
    }
    }
}