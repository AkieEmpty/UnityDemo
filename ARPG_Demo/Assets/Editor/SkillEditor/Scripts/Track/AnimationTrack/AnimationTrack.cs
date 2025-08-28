using System.Collections.Generic;
using AkieEmpty.SkillRuntime;
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
        private readonly ISkillEditorWindow skillEditorWindow;
        private readonly Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();
        private SingeleLineTrackStyle skillTrackStyle;
        public Dictionary<int, SkillAnimationEvent> FrameDataDic => skillEditorSystem.SkillConfig.skillAnimationData.FrameDataDic;
        public AnimationTrack(ISkillEditorSystem skillEditorSystem,ISkillEditorWindow skillEditorWindow)
        {
            this.skillEditorSystem = skillEditorSystem;
            this.skillEditorWindow = skillEditorWindow;
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

            foreach(var item in trackItemDic)
            {
                skillTrackStyle.RemoveItem(item.Value.TrackItemStyle.root);
            }

            trackItemDic.Clear();

            if (skillEditorSystem.SkillConfig == null) return;

            // 根据数据绘制TrackItem
            foreach (var item in FrameDataDic)
            {
                CreateItem(item.Key, item.Value);
            }
        }
        private void CreateItem(int frameIndex, SkillAnimationEvent animationFrameData)
        {
            AnimationTrackItem trackItem = new AnimationTrackItem();
            trackItem.Init(frameIndex,frameUnitWidth, this , skillTrackStyle, animationFrameData);
            trackItem.SetApplyDragAction(ApplyDrag);
            trackItem.SetMoveTrackItemAction(MoveTrackItem);
            trackItem.SetShowTrackInspecotrAction(ShowTrackInspector);
            trackItemDic.Add(frameIndex, trackItem);
        }

      

        public override void DeleteTrackItem(int frameIndex)
        {
            FrameDataDic.Remove(frameIndex);
            if(trackItemDic.Remove(frameIndex,out AnimationTrackItem trackItem))
            {
                skillTrackStyle.RemoveItem(trackItem.TrackItemStyle.root);
            }
            skillEditorSystem.SaveConfig();
        }

        private void ShowTrackInspector(TrackItemBase trackItem)
        {
            skillEditorWindow.ShowTrackInspector(this, trackItem);
        }

        private void MoveTrackItem(int startDragFrameIndex, float offsetPos, SkillAnimationEvent animationFrameData)
        {
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;
            bool checkDrag = false;
            if (targetFrameIndex < 0) return; // 不考虑拖拽到负数的情况
         
            //检查左边
            if (offsetFrame < 0) checkDrag = skillEditorSystem.CheckFrameIndexOnDrag(startDragFrameIndex, targetFrameIndex,  true);
            //检查右边
            else if (offsetFrame > 0) checkDrag = skillEditorSystem.CheckFrameIndexOnDrag(startDragFrameIndex, targetFrameIndex + animationFrameData.durationFrame,  false);
            else return;
            if (checkDrag)
            {
                AnimationTrackItem trackItem = trackItemDic[startDragFrameIndex];
                // 如果超过右侧边界，拓展边界
                skillEditorSystem.CheckMaxFrameCount(targetFrameIndex, animationFrameData.durationFrame);
                // 确定修改的数据
                trackItem.FrameIndex = targetFrameIndex;
                // 刷新视图
                trackItem.ResetView(frameUnitWidth);
            }
        }

        private void ApplyDrag(int startDragFrameIndex, int frameIndex)
        {
            if (startDragFrameIndex == frameIndex) return;

            if (FrameDataDic.Remove(startDragFrameIndex, out SkillAnimationEvent animationFrameData))
            {
                FrameDataDic.Add(frameIndex, animationFrameData);
                trackItemDic.Remove(startDragFrameIndex, out AnimationTrackItem animationTrackItem);
                trackItemDic.Add(frameIndex, animationTrackItem);
                SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
                skillEditorSystem.SaveConfig();
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
                SkillAnimationEvent animationData = new SkillAnimationEvent()
                {
                    animationClip = clip,
                    durationFrame = durationFrame,
                    transitionTime = 0.25f
                };

                // 保存新增的动画数据
                FrameDataDic.Add(selectFrameIndex, animationData);
                skillEditorSystem.SaveConfig();

                // 创建一个新的Item
                CreateItem(selectFrameIndex, animationData);
            }
        }
        public override void OnConfigChanged()
        {
            foreach (var item in trackItemDic.Values)
            {
                item.OnConfigChanged();
            }
        }

        #region 鼠标交互
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
        #endregion

        public override void Destory()
        {
            skillTrackStyle.Destory();
        }
    }
}