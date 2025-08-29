using System;
using AkieEmpty.SkillRuntime;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AnimationTrackItem : TrackItemBase<AnimationTrack>
    {
        private AnimationTrack track;
        private AnimationTrackItemStyle trackItemStyle;
        private Action<int, int> applyDragAction;
        private Action<int, float, SkillAnimationEvent> moveTrackItemAction;
        private bool mouseDrag = false;
        private float startDargPosX;
        private int startDragFrameIndex;
        private SkillAnimationEvent animationFrameData;

        public AnimationTrackItemStyle TrackItemStyle { get => trackItemStyle; }
        public SkillAnimationEvent AnimationFrameData { get=>animationFrameData; }

        public void Init(int startFrameIndex,int frameUnitWdith,AnimationTrack track,TrackStyleBase trackStyle,SkillAnimationEvent animationFrameData)
        {
            this.FrameIndex = startFrameIndex;
            this.frameUnitWidth = frameUnitWdith;
            this.track = track;
            this.animationFrameData = animationFrameData;
            trackItemStyle = new AnimationTrackItemStyle();
            trackItemStyle.Init(trackStyle);

            itemStyleBase = trackItemStyle;
            normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
            selectColor = new Color(0.388f, 0.850f, 0.905f, 0.75f);

            trackItemStyle.RegisterMouseCallback<MouseDownEvent>(OnMouseDown);
            trackItemStyle.RegisterMouseCallback<MouseUpEvent>(OnMouseUp);
            trackItemStyle.RegisterMouseCallback<MouseOutEvent>(OnMouseOut);
            trackItemStyle.RegisterMouseCallback<MouseMoveEvent>(OnMouseMove);

           

            ResetView(frameUnitWdith);
        }
        public void SetApplyDragAction(Action<int, int> action)
        {
            applyDragAction = action;
        }
        public void SetMoveTrackItemAction(Action<int, float,SkillAnimationEvent> action)
        {
            moveTrackItemAction = action;
        }
      
        private void OnMouseDown(MouseDownEvent evt)
        {
            startDargPosX = evt.mousePosition.x;
            startDragFrameIndex = FrameIndex;
            mouseDrag = true;
            Select();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (mouseDrag) ApplyDrag();
            mouseDrag = false;
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (mouseDrag) ApplyDrag();
            mouseDrag = false;
        }

        private void ApplyDrag()
        {
            applyDragAction?.Invoke(startDragFrameIndex, FrameIndex);   
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (mouseDrag)
            {
                float offsetPos = evt.mousePosition.x - startDargPosX;
                moveTrackItemAction.Invoke(startDragFrameIndex, offsetPos, AnimationFrameData);
            }
        }
        public override void ResetView(int frameUnitWdith)
        {
            base.ResetView(frameUnitWdith);

            trackItemStyle.SetTitle(AnimationFrameData.animationClip.name);
            //位置计算
            trackItemStyle.SetPosition(FrameIndex * frameUnitWdith);
            trackItemStyle.SetWidth(AnimationFrameData.durationFrame * frameUnitWdith);

            //计算动画总帧数
            int animationClipFrameCount = (int)(AnimationFrameData.animationClip.length * AnimationFrameData.animationClip.frameRate);
            // 计算动画结束线的位置
            if (animationClipFrameCount > AnimationFrameData.durationFrame)
            {
                trackItemStyle.ToggleOverLine(false);
            }
            else
            {
                trackItemStyle.ToggleOverLine(true);
                trackItemStyle.SetOverLinePos(animationClipFrameCount, frameUnitWdith);
            }
        }
       
        public override void OnConfigChanged()
        {
            animationFrameData = track.FrameDataDic[FrameIndex];
        }
      

    }
}