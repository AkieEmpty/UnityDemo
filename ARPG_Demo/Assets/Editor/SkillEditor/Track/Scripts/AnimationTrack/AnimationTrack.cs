using System.Collections.Generic;
using AkieEmpty.CharacterSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// 动画轨道
    /// </summary>
    public class AnimationTrack : TrackBase
    {
        private const string TrackName = "动画配置";
        private readonly ISkillEditorSystem skillEditorSystem;
        private readonly Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();
        private SingeleLineTrackStyle skillTrackStyle;
        private Dictionary<int, AnimationFrameData> FrameDataDic => skillEditorSystem.SkillConfig.skillAnimationData.FrameDataDic;
        public AnimationTrack(ISkillEditorSystem skillEditorSystem)
        {
            this.skillEditorSystem = skillEditorSystem;

        }
        public override void Init(VisualElement menuParent, VisualElement trackParent, int frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            skillTrackStyle = new SingeleLineTrackStyle();
            skillTrackStyle.Init(menuParent, trackParent, TrackName);
            skillTrackStyle.RegisterDragUpdatedCallback(OnDragUpdate);
            skillTrackStyle.RegisterDragExitedCallback(OnDragExited);

            ResetView();
        }

        public override void ResetView(int frameUnitWidth)
        {
            base.ResetView(frameUnitWidth);
            skillTrackStyle.RemoveAllItem();
            trackItemDic.Clear();

            if (skillEditorSystem.SkillConfig == null) return;

            // 根据数据绘制TrackItem
            foreach (var item in FrameDataDic)
            {
                CreateItem(item.Key, item.Value);
                skillEditorSystem.SaveConfig();
            }
        }
        private void CreateItem(int frameIndex, AnimationFrameData animationFrameData)
        {
            AnimationTrackItem trackItem = new AnimationTrackItem();
            trackItem.Init(skillTrackStyle, animationFrameData);
            trackItemDic.Add(frameIndex, trackItem);
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            // 监听用户拖拽的是否是动画
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            AnimationClip clip = objs[0] as AnimationClip;
            if (clip != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        private void OnDragExited(DragExitedEvent evt)
        {
            // 监听用户拖拽的是否是动画
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            AnimationClip clip = objs[0] as AnimationClip;
            if (clip != null)
            {
                int selectFrameIndex = SkillEditorSystem.GetFrameIndexByMousePos(evt.localMousePosition.x,skillEditorSystem.SkillEditorConfig.CurrentFrameUnitWidth);
                PlaceAnimationOnTrack(selectFrameIndex, clip);
            }
        }

        private void PlaceAnimationOnTrack(int selectFrameIndex, AnimationClip clip)
        {
            // 放置动画资源
            // 当前选中的位置检测能否放置动画
            
            bool canPlace = true;
            int durationFrame = -1; // -1代表可以用原本AniamtionClip的持续时间
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in FrameDataDic)
            {
                // 不允许选中帧在TrackItem中间（动画事件的起点到他的终点之间）
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Value.durationFrame + item.Key)
                {
                    // 不能放置
                    canPlace = false;
                    break;
                }

                //选中某个TrackItem左边
                if (item.Key > selectFrameIndex)
                {
                    int tempOffset = item.Key - selectFrameIndex;
                    if (tempOffset < currentOffset)
                    {
                        currentOffset = tempOffset;
                        nextTrackItem = item.Key;
                    }
                }
            }

            if (canPlace)
            {
                // 如果右边有其他TrackItem，要考虑Track不能重叠的问题
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    //根据剩余空间进行片段裁剪
                    if (offset < 0) durationFrame = clipFrameCount;
                    else durationFrame = currentOffset;
                }
                // 右边啥都没有
                else durationFrame = clipFrameCount;

                // 构建动画数据
                AnimationFrameData animationEvent = new AnimationFrameData()
                {
                    animationClip = clip,
                    durationFrame = durationFrame,
                    transitionTime = 0.25f
                };

                // 保存新增的动画数据
                FrameDataDic.Add(selectFrameIndex, animationEvent);
                skillEditorSystem.SaveConfig();

                // 创建一个新的Item
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }
}