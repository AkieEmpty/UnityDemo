using AkieEmpty.SkillRuntime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{

    public class AudioTrack : TrackBase
    {
        private const string TrackName = "音效配置";
        private MultilineTrackStyle skillTrackStyle;
        private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();
        public List<SkillAudioEvent> FrameDataDic => skillEditorSystem.SkillConfig.skillAudioData.FrameDataDic;
       
        public override void Init(ISkillEditorSystem skillEditorSystem,ISkillEditorWindow skillEditorWindow, VisualElement menuParent, VisualElement trackParent)
        {
            base.Init(skillEditorSystem, skillEditorWindow,menuParent, trackParent);
            skillTrackStyle = new MultilineTrackStyle();
            skillTrackStyle.Init(menuParent, trackParent, TrackName, AddChildTrack, SpawnChildTrack, CheckDeleteChildTrack, UpdateChildTrackName);

            ResetView();
        }

        #region 视图
        public override void ResetView()
        {
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
            item.Init(skillEditorSystem, FrameUnitWidth, skillAudioEvent, childTrack);
            item.SetTrackName(skillAudioEvent.TrackName);
            item.SetShowTrackInspecotrAction(ShowTrackInspector);
            trackItemList.Add(item);
        }

        private void ShowTrackInspector(TrackItemBase trackItem)
        {
            skillEditorWindow.ShowTrackInspector(this, trackItem);
        }
        #endregion

        #region 子轨道
        private void AddChildTrack()
        {
            SkillAudioEvent skillAudioEvent = new SkillAudioEvent();
            FrameDataDic.Add(skillAudioEvent);
            CreateItem(skillAudioEvent);           
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
        #endregion

      

        public override void Destory()
        {
            skillTrackStyle.Destory();
    }
    }
}