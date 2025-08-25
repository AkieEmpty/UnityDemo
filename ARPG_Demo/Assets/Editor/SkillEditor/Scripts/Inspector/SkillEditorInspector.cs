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
            // �����Ѿ�����Inspector����ˢ������
            if (Instance != null) Instance.Show();

        }
        private void OnDestroy()
        {
            // ˵������ж��
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

            // TODO:Ŀǰֻ�ж�����һ�����
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

        #region �������
        private Label clipFrameLabel;
        private Toggle rootMotionToggle;
        private Label isLoopLable;
        private IntegerField durationField;

        private void DrawAniamtionTrackItem(AnimationTrackItem animationTrackItem)
        {
            trackItemFrameIndex = animationTrackItem.FrameIndex;
            // ������Դ
            ObjectField animationClipAssetField = new ObjectField("������Դ");
            animationClipAssetField.objectType = typeof(AnimationClip);
            animationClipAssetField.value = animationTrackItem.AnimationFrameData.animationClip;
            animationClipAssetField.RegisterValueChangedCallback(AnimationClipAssetFiedlValueChanged);
            root.Add(animationClipAssetField);

            // ���˶�
            rootMotionToggle = new Toggle("Ӧ�ø��˶�");
            rootMotionToggle.value = animationTrackItem.AnimationFrameData.applyRootMotion;
            rootMotionToggle.RegisterValueChangedCallback(RootMotionToggleValueChanged);
            root.Add(rootMotionToggle);

            // �������
            durationField = new IntegerField("�������");
            durationField.value = animationTrackItem.AnimationFrameData.durationFrame;
            durationField.RegisterValueChangedCallback(DurationFieldValueChanged);
            root.Add(durationField);

            // ����ʱ��
            FloatField transitionTimeField = new FloatField("����ʱ��");
            transitionTimeField.value = animationTrackItem.AnimationFrameData.transitionTime;
            transitionTimeField.RegisterValueChangedCallback(TransitionTimeFieldValueChanged);
            root.Add(transitionTimeField);

            // ������ص���Ϣ
            int clipFrameCount = (int)(animationTrackItem.AnimationFrameData.animationClip.length * animationTrackItem.AnimationFrameData.animationClip.frameRate);
            clipFrameLabel = new Label("������Դ����:" + clipFrameCount);
            root.Add(clipFrameLabel);
            isLoopLable = new Label("ѭ������:" + animationTrackItem.AnimationFrameData.animationClip.isLooping);
            root.Add(isLoopLable);

            // ɾ��
            Button deleteButton = new Button(DeleteButtonClick);
            deleteButton.text = "ɾ��";
            deleteButton.style.backgroundColor = new Color(1, 0, 0, 0.5f);
            root.Add(deleteButton);
        }

        private void AnimationClipAssetFiedlValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            AnimationClip clip = evt.newValue as AnimationClip;
            // �޸�������ʾЧ��
            clipFrameLabel.text = "������Դ����:" + ((int)(clip.length * clip.frameRate));
            isLoopLable.text = "ѭ������:" + clip.isLooping;
            // ���浽����
            (currentTrackItem as AnimationTrackItem).AnimationFrameData.animationClip = clip;
            skillEditorSystem.SaveConfig();
            currentTrackItem.ResetView();
        }

        private void RootMotionToggleValueChanged(ChangeEvent<bool> evt)
        {
            (currentTrackItem as AnimationTrackItem).AnimationFrameData.applyRootMotion = evt.newValue;
            skillEditorSystem.SaveConfig();
        }

        private void DurationFieldValueChanged(ChangeEvent<int> evt)
        {
            int value = evt.newValue;
            // ��ȫУ��
            if (skillEditorSystem.CheckFrameIndexOnDrag(trackItemFrameIndex + value, trackItemFrameIndex, false))
            {
                // �޸����ݣ�ˢ����ͼ
                (currentTrackItem as AnimationTrackItem).AnimationFrameData.durationFrame = value;
                int frameIndex = (currentTrackItem as AnimationTrackItem).FrameIndex;
                skillEditorSystem.CheckMaxFrameCount(frameIndex, value);
                skillEditorSystem.SaveConfig();
                currentTrackItem.ResetView();
            }
            else
            {
                durationField.value = evt.previousValue;
            }
        }

        private void TransitionTimeFieldValueChanged(ChangeEvent<float> evt)
        {
            (currentTrackItem as AnimationTrackItem).AnimationFrameData.transitionTime = evt.newValue;
        }

        private void DeleteButtonClick()
        {
            currentTrack.DeleteTrackItem(trackItemFrameIndex); // �˺����ṩ�����ˢ����ͼ�߼�
            Selection.activeObject = null;
        }
        #endregion
    }
}
