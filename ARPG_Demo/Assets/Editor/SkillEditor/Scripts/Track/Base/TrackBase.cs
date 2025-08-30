using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// ¹ìµÀ»ùÀà
    /// </summary>
    public abstract class TrackBase 
    {
        protected ISkillEditorController skillEditorSystem;
        protected ISkillEditorWindow skillEditorWindow;
        public int FrameUnitWidth => skillEditorSystem.SkillEditorConfig.CurrentFrameUnitWidth;
        public virtual void Init(ISkillEditorController skillEditorSystem, ISkillEditorWindow skillEditorWindow, VisualElement menuParent, VisualElement trackParent)
        {
            this.skillEditorSystem = skillEditorSystem;
            this.skillEditorWindow = skillEditorWindow;
        }
        public virtual void Destory()
        {

        }
        public virtual void ResetView()
        {
           
        }
        public virtual void DeleteTrackItem(int frameIndex) { }

        public virtual void OnConfigChanged() { }
        public virtual void TickView(int frameIndex) { }
        public virtual void OnPlay(int startFrameIndex) { }
        public virtual void OnStop() { }
    }
}