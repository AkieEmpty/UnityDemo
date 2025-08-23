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
        protected int frameUnitWidth;
        public virtual void Init(VisualElement menuParent, VisualElement trackParent, int frameUnitWidth)
        {
            this.frameUnitWidth = frameUnitWidth;
        }
        public virtual void ResetView()
        {
            ResetView(frameUnitWidth);
        }

        public virtual void ResetView(int frameUnitWidth)
        {
            this.frameUnitWidth = frameUnitWidth;
        }
        public virtual void DeleteTrackItem(int frameIndex) { }

        public virtual void OnConfigChanged() { }
    }
}