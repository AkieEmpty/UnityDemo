using System.Collections.Generic;
using AkieEmpty.CharacterSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// �������
    /// </summary>
    public class AnimationTrack : TrackBase
    {
        private const string TrackName = "��������";
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

            // �������ݻ���TrackItem
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
            // �����û���ק���Ƿ��Ƕ���
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            AnimationClip clip = objs[0] as AnimationClip;
            if (clip != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        private void OnDragExited(DragExitedEvent evt)
        {
            // �����û���ק���Ƿ��Ƕ���
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
            // ���ö�����Դ
            // ��ǰѡ�е�λ�ü���ܷ���ö���
            
            bool canPlace = true;
            int durationFrame = -1; // -1���������ԭ��AniamtionClip�ĳ���ʱ��
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in FrameDataDic)
            {
                // ������ѡ��֡��TrackItem�м䣨�����¼�����㵽�����յ�֮�䣩
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Value.durationFrame + item.Key)
                {
                    // ���ܷ���
                    canPlace = false;
                    break;
                }

                //ѡ��ĳ��TrackItem���
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
                // ����ұ�������TrackItem��Ҫ����Track�����ص�������
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    //����ʣ��ռ����Ƭ�βü�
                    if (offset < 0) durationFrame = clipFrameCount;
                    else durationFrame = currentOffset;
                }
                // �ұ�ɶ��û��
                else durationFrame = clipFrameCount;

                // ������������
                AnimationFrameData animationEvent = new AnimationFrameData()
                {
                    animationClip = clip,
                    durationFrame = durationFrame,
                    transitionTime = 0.25f
                };

                // ���������Ķ�������
                FrameDataDic.Add(selectFrameIndex, animationEvent);
                skillEditorSystem.SaveConfig();

                // ����һ���µ�Item
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }
}