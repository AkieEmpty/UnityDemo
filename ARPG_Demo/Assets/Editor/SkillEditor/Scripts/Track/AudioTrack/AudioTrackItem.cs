using AkieEmpty.SkillRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AudioTrackItem:TrackItemBase<AudioTrack>
    {
        private ISkillEditorSystem skillEditorSystem;
        private MultilineTrackStyle.ChildTrack childTrackStyle;
        private AudioTrackItemStyle trackItemStyle;
        private SkillAudioEvent skillAudioEvent;
        private bool mouseDrag = false;
        private float startDargPosX;
        private int startDragFrameIndex;
        public void Init(ISkillEditorSystem skillEditorSystem,int frameUnitWidth ,SkillAudioEvent skillAudioEvent, MultilineTrackStyle.ChildTrack childTrackStyle)
        {
            this.skillEditorSystem = skillEditorSystem;
            this.childTrackStyle = childTrackStyle;
            this.skillAudioEvent = skillAudioEvent;
            trackItemStyle = new AudioTrackItemStyle();
            itemStyleBase = trackItemStyle;
           

            normalColor = new Color(1f, 0.6f, 0f, 0.5f);
            selectColor = new Color(1f, 0.6f, 0f, 0.7f);

          
            this.childTrackStyle.trackRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            this.childTrackStyle.trackRoot.RegisterCallback<DragExitedEvent>(OnDragExited);

            ResetView(frameUnitWidth);
        }       
        public void Destory()
        {
            childTrackStyle.Destory();
        }

        #region 视图
        public override void ResetView(int frameUnitWdith)
        {
            base.ResetView(frameUnitWdith);
            if(skillAudioEvent.Clip!=null)
            {
                if (!trackItemStyle.IsInit)
                {
                    trackItemStyle.Init(skillEditorSystem, skillAudioEvent, childTrackStyle);
                    trackItemStyle.RegisterMouseCallback<MouseDownEvent>(OnMouseDown);
                    trackItemStyle.RegisterMouseCallback<MouseUpEvent>(OnMouseUp);
                    trackItemStyle.RegisterMouseCallback<MouseOutEvent>(OnMouseOut);
                    trackItemStyle.RegisterMouseCallback<MouseMoveEvent>(OnMouseMove);
                }
                trackItemStyle.ResetView(frameUnitWdith, skillAudioEvent);
            }
        }
        public void SetTrackName(string name)
        {
            childTrackStyle.SetTrackName(name);
        }
        #endregion

        #region 鼠标交互
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
           
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (mouseDrag)
            {
                float offsetPos = evt.mousePosition.x - startDargPosX;
                int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
                int targetFrameIndex = startDragFrameIndex + offsetFrame;
                if (targetFrameIndex < 0 || offsetFrame == 0) return; // 不考虑拖拽到负数的情况
                // 确定修改的数据
                FrameIndex = targetFrameIndex;
                skillAudioEvent.FrameIndex = FrameIndex;
                // 如果超过右侧边界，拓展边界
                skillEditorSystem.CheckAndExtendMaxFrameCount(targetFrameIndex, (int)(skillAudioEvent.Clip.length * skillEditorSystem.SkillConfig.frameRote));
                // 刷新视图
                ResetView(frameUnitWidth);
            }
        }
        #endregion

        #region 资源拖拽
        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            // 监听用户拖拽的是否是动画
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            AudioClip clip = objs[0] as AudioClip;
            if (clip != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }
        private void OnDragExited(DragExitedEvent evt)
        {
            // 监听用户拖拽的是否是动画
            UnityEngine.Object[] objs = DragAndDrop.objectReferences;
            AudioClip clip = objs[0] as AudioClip;
            if (clip != null)
            {
                int selectFrameIndex = SkillEditorSystem.GetFrameIndexByMousePos(evt.localMousePosition.x, skillEditorSystem.SkillEditorConfig.CurrentFrameUnitWidth);
                if(selectFrameIndex>0)
                {
                    //构建音效数据
                    skillAudioEvent.Clip = clip;
                    skillAudioEvent.FrameIndex= selectFrameIndex;
                    skillAudioEvent.PlayCount = 1;
                    skillAudioEvent.Voluem = 1;

                    this.FrameIndex = selectFrameIndex;

                    skillEditorSystem.SaveConfig();
                    ResetView();
                }
            }
        }
        #endregion
    }
}
