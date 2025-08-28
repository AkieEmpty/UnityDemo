using System.Collections.Generic;
using AkieEmpty.SkillRuntime;
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

            // �������ݻ���TrackItem
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
            if (targetFrameIndex < 0) return; // ��������ק�����������
         
            //������
            if (offsetFrame < 0) checkDrag = skillEditorSystem.CheckFrameIndexOnDrag(startDragFrameIndex, targetFrameIndex,  true);
            //����ұ�
            else if (offsetFrame > 0) checkDrag = skillEditorSystem.CheckFrameIndexOnDrag(startDragFrameIndex, targetFrameIndex + animationFrameData.durationFrame,  false);
            else return;
            if (checkDrag)
            {
                AnimationTrackItem trackItem = trackItemDic[startDragFrameIndex];
                // ��������Ҳ�߽磬��չ�߽�
                skillEditorSystem.CheckMaxFrameCount(targetFrameIndex, animationFrameData.durationFrame);
                // ȷ���޸ĵ�����
                trackItem.FrameIndex = targetFrameIndex;
                // ˢ����ͼ
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
                SkillAnimationEvent animationData = new SkillAnimationEvent()
                {
                    animationClip = clip,
                    durationFrame = durationFrame,
                    transitionTime = 0.25f
                };

                // ���������Ķ�������
                FrameDataDic.Add(selectFrameIndex, animationData);
                skillEditorSystem.SaveConfig();

                // ����һ���µ�Item
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

        #region ��꽻��
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
        #endregion

        public override void Destory()
        {
            skillTrackStyle.Destory();
        }
    }
}