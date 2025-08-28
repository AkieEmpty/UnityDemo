using AkieEmpty.SkillRuntime;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{

    public class AudioTrack : TrackBase
    {
        private const string TrackName = "音效配置";
        private readonly ISkillEditorSystem skillEditorSystem;
        private readonly ISkillEditorWindow skillEditorWindow;
        private MultilineTrackStyle skillTrackStyle;
        private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();
        public List<SkillAudioEvent> FrameDataDic => skillEditorSystem.SkillConfig.skillAudioData.FrameDataDic;
        public AudioTrack(ISkillEditorSystem skillEditorSystem, ISkillEditorWindow skillEditorWindow) 
        {
            this.skillEditorSystem = skillEditorSystem;
            this.skillEditorWindow = skillEditorWindow;
        }
        public override void Init(VisualElement menuParent, VisualElement trackParent, int frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            skillTrackStyle = new MultilineTrackStyle();
            skillTrackStyle.Init(menuParent, trackParent, TrackName, AddChildTrack, SpawnChildTrack, CheckDeleteChildTrack, UpdateChildTrackName);

            ResetView();
        }

        public override void ResetView(int frameUnitWidth)
        {
            base.ResetView(frameUnitWidth);
            foreach (AudioTrackItem item in trackItemList)
            {
                item.Destory();
            }
            trackItemList.Clear();
            foreach (SkillAudioEvent item in FrameDataDic)
            {
                CreateItem(item);
            }
          
        }

        private void CreateItem(SkillAudioEvent skillAudioEvent)
        {
            AudioTrackItem item = new AudioTrackItem();
            float frameRote = skillEditorSystem.SkillConfig.frameRote;
            MultilineTrackStyle.ChildTrack childTrack = skillTrackStyle.AddChildTrack();
            item.Init(frameUnitWidth, frameRote, skillAudioEvent, childTrack);
            item.SetTrackName(skillAudioEvent.TrackName);
            trackItemList.Add(item);
        }


        private void AddChildTrack()
        {
            SkillAudioEvent skillAudioEvent = new SkillAudioEvent();
            CreateItem(skillAudioEvent);
            FrameDataDic.Add(skillAudioEvent);
            skillEditorSystem.SaveConfig();    
        }
        private bool CheckDeleteChildTrack(int index)
        {
            if (index < 0 || index >= FrameDataDic.Count)
            {
                return false;
            }

            if (FrameDataDic[index] != null)
            {
                FrameDataDic.RemoveAt(index);
                skillEditorSystem.SaveConfig();
                return true;
            }
            return false;
        }
        private void SpawnChildTrack(int index1,int index2)
        {
            SkillAudioEvent tempAudioEvent = FrameDataDic[index1];
            FrameDataDic[index1] = FrameDataDic[index2];
            FrameDataDic[index2] = tempAudioEvent;
            //保存交给窗口的退出机制
        }
        private void UpdateChildTrackName(MultilineTrackStyle.ChildTrack childTrack, string newName)
        {
            //同步给配置
            FrameDataDic[childTrack.Index].TrackName = newName;
            skillEditorSystem.SaveConfig();
        }
        public override void Destory()
        {
            skillTrackStyle.Destory();
    }
    }
}