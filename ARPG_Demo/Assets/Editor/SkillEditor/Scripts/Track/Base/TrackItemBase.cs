using System;
using UnityEngine;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// ������ӻ���
    /// </summary>
    public abstract class TrackItemBase
    {
        protected int frameUnitWidth;
        public abstract void Select();
        public abstract void OnSelect();
        public abstract void OnUnSelect();
        public virtual void ResetView() => ResetView(frameUnitWidth);
        public virtual void ResetView(int frameUnitWdith) => this.frameUnitWidth = frameUnitWdith;
        public virtual void OnConfigChanged() { }
    }
    public abstract class TrackItemBase<T> :TrackItemBase where T:TrackBase
    {
        private Action<TrackItemBase> showTrackInspecotrAction;
        protected Color normalColor;
        protected Color selectColor;
        protected TrackItemStyleBase itemStyleBase { get; set; }
        public int FrameIndex { get; set; }
        
        public void SetShowTrackInspecotrAction(Action<TrackItemBase> action)
        {
            showTrackInspecotrAction = action;
        }
        public override void Select()
        {
            //��Inspecotr���
            showTrackInspecotrAction?.Invoke(this);
        }
        public override void OnSelect()
        {
            itemStyleBase.SetBGColor(selectColor);
        }
        public override void OnUnSelect()
        {
            itemStyleBase.SetBGColor(normalColor);
        }
    }
}