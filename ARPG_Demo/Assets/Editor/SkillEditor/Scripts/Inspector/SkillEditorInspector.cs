using System;
using AkieEmpty.SkillEditor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace AkieEmpty.SkillEditor
{
    [CustomEditor(typeof(SkillEditorWindow))]
    public class SkillEditorInspector : Editor
    {
        public static SkillEditorInspector Instance;
        private static TrackItemBase currentTrackItem;
        private static TrackBase currentTrack;
        private static ISkillEditorSystem skillEditorSystem;
        public static void SetTrackItem(ISkillEditorSystem skillEditorSystem, TrackItemBase trackItem, TrackBase track)
        {
            if (SkillEditorInspector.skillEditorSystem == null) SkillEditorInspector.skillEditorSystem = skillEditorSystem;
            if (currentTrackItem != null)
            {
                currentTrackItem.OnUnSelect();
            }
            currentTrackItem = trackItem;
            currentTrackItem.OnSelect();
            currentTrack = track;
            // 避免已经打开了Inspector，不刷新数据
            if (Instance != null) Instance.Show();

        }
        private void OnDestroy()
        {
            // 说明窗口卸载
            if (currentTrackItem != null)
            {
                currentTrackItem.OnUnSelect();
                currentTrackItem = null;
                currentTrack = null;
            }
        }
        private VisualElement root;
        public override VisualElement CreateInspectorGUI()
        {
            Instance = this;
            root = new VisualElement();
            Show();
            return root;
        }

        private void Show()
        {
            Clean();
            if (currentTrackItem == null) return;

            // TODO:目前只有动画这一种情况
            if (currentTrackItem.GetType() == typeof(AnimationTrackItem))
            {
                DrawAniamtionTrackItem((AnimationTrackItem)currentTrackItem);
            }

        }

        private void Clean()
        {
            if (root != null)
            {
                for (int i = root.childCount - 1; i >= 0; i--)
                {
                    root.RemoveAt(i);
                }
            }
        }
        private int trackItemFrameIndex;
        public void SetTrackItemFrameIndex(int trackItemFrameIndex)
        {
            this.trackItemFrameIndex = trackItemFrameIndex;
        }

        #region 动画轨道
        private Label clipFrameLabel;
        private Toggle rootMotionToggle;
        private Label isLoopLable;
        private IntegerField durationField;
        private FloatField transitionTimeField;
        private int oldDurationValue;
        private float oldTransitionTimeValue;
        private void DrawAniamtionTrackItem(AnimationTrackItem animationTrackItem)
        {
            trackItemFrameIndex = animationTrackItem.FrameIndex;
            // 动画资源
            ObjectField animationClipAssetField = new ObjectField("动画资源");
            animationClipAssetField.objectType = typeof(AnimationClip);
            animationClipAssetField.value = animationTrackItem.AnimationFrameData.animationClip;
            animationClipAssetField.RegisterValueChangedCallback(AnimationClipAssetFiedlValueChanged);
            root.Add(animationClipAssetField);

            // 根运动
            rootMotionToggle = new Toggle("应用根运动");
            rootMotionToggle.value = animationTrackItem.AnimationFrameData.applyRootMotion;
            rootMotionToggle.RegisterValueChangedCallback(RootMotionToggleValueChanged);
            root.Add(rootMotionToggle);

            // 轨道长度
            durationField = new IntegerField("轨道长度");
            durationField.value = animationTrackItem.AnimationFrameData.durationFrame;
            durationField.RegisterCallback<FocusInEvent>(DurationFieldFocusIn);
            durationField.RegisterCallback<FocusOutEvent>(DurationFieldFocusOut);
            root.Add(durationField);

            // 过渡时间
            transitionTimeField = new FloatField("过渡时间");
            transitionTimeField.value = animationTrackItem.AnimationFrameData.transitionTime;
            durationField.RegisterCallback<FocusInEvent>(TransitionTimeFieldFocusIn);
            durationField.RegisterCallback<FocusOutEvent>(TransitionTimeFieldFocusOut);
            root.Add(transitionTimeField);

            // 动画相关的信息
            int clipFrameCount = (int)(animationTrackItem.AnimationFrameData.animationClip.length * animationTrackItem.AnimationFrameData.animationClip.frameRate);
            clipFrameLabel = new Label("动画资源长度:" + clipFrameCount);
            root.Add(clipFrameLabel);
            isLoopLable = new Label("循环动画:" + animationTrackItem.AnimationFrameData.animationClip.isLooping);
            root.Add(isLoopLable);

            // 删除
            Button deleteButton = new Button(DeleteButtonClick);
            deleteButton.text = "删除";
            deleteButton.style.backgroundColor = new Color(1, 0, 0, 0.5f);
            root.Add(deleteButton);
        }

        private void AnimationClipAssetFiedlValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            AnimationClip clip = evt.newValue as AnimationClip;
            // 修改自身显示效果
            clipFrameLabel.text = "动画资源长度:" + ((int)(clip.length * clip.frameRate));
            isLoopLable.text = "循环动画:" + clip.isLooping;
            // 保存到配置
            (currentTrackItem as AnimationTrackItem).AnimationFrameData.animationClip = clip;
            skillEditorSystem.SaveConfig();
            currentTrackItem.ResetView();
        }

        private void RootMotionToggleValueChanged(ChangeEvent<bool> evt)
        {
            (currentTrackItem as AnimationTrackItem).AnimationFrameData.applyRootMotion = evt.newValue;
            skillEditorSystem.SaveConfig();
        }
        //焦点进入
        private void DurationFieldFocusIn(FocusInEvent evt)
        {
            oldDurationValue = durationField.value;
        }
        //焦点退出
        private void DurationFieldFocusOut(FocusOutEvent evt)
        {
            if(oldDurationValue!=durationField.value)
            {
                int value = durationField.value;
                // 安全校验
                if (skillEditorSystem.CheckFrameIndexOnDrag(trackItemFrameIndex + value, trackItemFrameIndex, false))
                {
                    // 修改数据，刷新视图
                    ((AnimationTrackItem)currentTrackItem).AnimationFrameData.durationFrame = value;
                    int frameIndex = (currentTrackItem as AnimationTrackItem).FrameIndex;
                    skillEditorSystem.CheckAndExtendMaxFrameCount(frameIndex, value);
                    skillEditorSystem.SaveConfig();
                    currentTrackItem.ResetView();
                }
                else
                {
                    durationField.value = oldDurationValue;
                }
            }
        }
        private void TransitionTimeFieldFocusIn(FocusInEvent evt)
        {
            oldTransitionTimeValue = transitionTimeField.value;
        }
        private void TransitionTimeFieldFocusOut(FocusOutEvent evt)
        {
            if(oldDurationValue!= transitionTimeField.value)
            {
                ((AnimationTrackItem)currentTrackItem).AnimationFrameData.transitionTime = transitionTimeField.value;
            }
        }
        private void DeleteButtonClick()
        {
            currentTrack.DeleteTrackItem(trackItemFrameIndex); // 此函数提供保存和刷新视图逻辑
            Selection.activeObject = null;
        }
        #endregion
    }
}
