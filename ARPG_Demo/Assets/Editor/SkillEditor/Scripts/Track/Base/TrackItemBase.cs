using System;
using UnityEngine;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// 轨道格子基类
    /// </summary>
    public abstract class TrackItemBase
    {
        protected int frameUnitWdith;
        public abstract void Select();
        public abstract void OnSelect();
        public abstract void OnUnSelect();
        public virtual void ResetView() => ResetView(frameUnitWdith);
        public virtual void ResetView(int frameUnitWdith) => this.frameUnitWdith = frameUnitWdith;
        public virtual void OnConfigChanged() { }
    }
    public abstract class TrackItemBase<T> :TrackItemBase where T:TrackBase
    {
        private Action<TrackItemBase> showTrackInspecotrAction;
        protected TrackItemStyleBase itemStyleBase { get; set; }
        protected Color normalColor;
        protected Color selectColor;
        public void SetShowTrackInspecotrAction(Action<TrackItemBase> action)
        {
            showTrackInspecotrAction = action;
        }
        public override void Select()
        {
            //打开Inspecotr面板
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