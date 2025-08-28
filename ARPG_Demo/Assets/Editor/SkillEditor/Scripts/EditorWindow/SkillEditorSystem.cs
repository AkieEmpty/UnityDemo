using System;
using System.Collections.Generic;
using System.Linq;
using AkieEmpty.SkillRuntime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public struct TimeShaftLayout
    {  
        public readonly int index;
        public readonly float startOffset;
        public readonly int tickStep;
        public TimeShaftLayout(int index, float startOffset, int tickStep)
        {
            this.index = index;
            this.startOffset = startOffset;
            this.tickStep = tickStep;
        }
    }

    public interface ISkillEditorSystem
    {
        public SkillConfig SkillConfig { get; }
        public SkillEditorConfig SkillEditorConfig { get; }
        public bool CheckFrameIndexOnDrag(int selfIndex, int targetIndex, bool isLeft);
        public void CheckMaxFrameCount(int frameIndex, int durationFrame);
        public void SaveConfig();
    }
    public class SkillEditorSystem : ISkillEditorSystem
    {
        
        private readonly ISkillEditorWindow editorWindow;
        private readonly SkillEditorConfig skillEditorConfig;
        public SkillConfig SkillConfig => editorWindow.SkillConfig;
        public SkillEditorConfig SkillEditorConfig => skillEditorConfig;

        public SkillEditorSystem(ISkillEditorWindow editorWindow, SkillEditorConfig skillEditorConfig) 
        {
            this.editorWindow = editorWindow;
            this.skillEditorConfig = skillEditorConfig;
        } 

        #region Menu
      
        private const string previewCharacterParentPath = "PreviewCharacterRoot";
        private GameObject currentPreviewCharacterObj;
        private string oldScenePath;

        
        /// <summary>
        /// 加载编辑器场景
        /// </summary>
        public void LoadEditorScene(string skillEditorScenePath)
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            // 当前是编辑器场景，但是玩家依然点击了加载编辑器场景，没有意义
            if (currentScenePath == skillEditorScenePath) return;
            oldScenePath = currentScenePath;
            EditorSceneManager.OpenScene(skillEditorScenePath);
        }
        /// <summary>
        /// 加载旧场景
        /// </summary>
        public void LoadOldScene()
        {
            if (!string.IsNullOrEmpty(oldScenePath))
            {
                string currentScenePath = EditorSceneManager.GetActiveScene().path;
                // 当前场景和旧场景是同一个场景，没有切换意义
                if (currentScenePath == oldScenePath) return;
                EditorSceneManager.OpenScene(oldScenePath);
            }
            else Debug.LogWarning("场景不存在！");
        }
        public GameObject CreatePreviewCharacter(GameObject previewObj)
        {
            // 值相等，设置无效
            if (previewObj == currentPreviewCharacterObj) return currentPreviewCharacterObj;
            // 销毁旧的
            if (currentPreviewCharacterObj != null) SkillEditorWindow.DestroyImmediate(currentPreviewCharacterObj);
            Transform parent = GameObject.Find(previewCharacterParentPath).transform;
            if (parent != null && parent.childCount > 0)
            {
                SkillEditorWindow.DestroyImmediate(parent.GetChild(0).gameObject);
            }
            // 实例化新的
            if (previewObj != null)
            {
                currentPreviewCharacterObj = SkillEditorWindow.Instantiate(previewObj, Vector3.zero, Quaternion.identity, parent);
                currentPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                return currentPreviewCharacterObj;
            }
            return null;

        }
        public void SetPreviewFromSceneObject(ChangeEvent<UnityEngine.Object> evt)
        {
            currentPreviewCharacterObj = (GameObject)evt.newValue;
        }   
        #endregion

        #region TimeShaft
        private int currentSelectFrameIndex;
        private int currentMaxFrameCount = -1;

        public int CurrentSelectFrameIndex
        {
            get => currentSelectFrameIndex;
            set
            {
                if (currentSelectFrameIndex == value) return;
                // 如果超出范围，更新最大帧
                if (value > CurrentMaxFrameCount) CurrentMaxFrameCount =value ;
                currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentMaxFrameCount);
                editorWindow.UpdateTimerShaftView();
                editorWindow.UpdateConsoleField();
                TickSkill(currentSelectFrameIndex);
            }
        }
        public int CurrentMaxFrameCount
        {
            get => currentMaxFrameCount;
            set
            {
                if (currentMaxFrameCount == value) return;
                currentMaxFrameCount = value;
                if (SkillConfig != null) SkillConfig.maxFrameCount = value;
                editorWindow.UpdateConsoleField();
                editorWindow.UpdateTrackContentSzie();
            }
        }


        private float CurrentSelectFramePos => CurrentSelectFrameIndex * skillEditorConfig.CurrentFrameUnitWidth;

        public TimeShaftLayout CalculateTimeShaftLayout(float ContentOffsetPos)
        {
            //计算起始索引
            int index = Mathf.CeilToInt(ContentOffsetPos / skillEditorConfig.CurrentFrameUnitWidth);
            //计算绘制起点的偏移,(范围为0 ~ 单位帧宽)
            float startOffset = 0;
            if (index > 0) startOffset = skillEditorConfig.CurrentFrameUnitWidth - (ContentOffsetPos % skillEditorConfig.CurrentFrameUnitWidth);
            //计算步长
            int tickStep = SkillEditorConfig.maxFrameWidthLV + 1 - (skillEditorConfig.CurrentFrameUnitWidth / SkillEditorConfig.StandFrameUnitWidth);
            tickStep = tickStep / 2; // 可能 1 / 2 = 0的情况
            if (tickStep == 0) tickStep = 1; // 避免为0

            return new TimeShaftLayout(index, startOffset, tickStep);
        }
        public void SelectFrameIndexFromMouseDown(float mouseX)
        {
            IsPlaying = false;
            int frameIndex = GetFrameIndexByMousePos(mouseX,skillEditorConfig.CurrentFrameUnitWidth);
            if (frameIndex == CurrentSelectFrameIndex) return;
            CurrentSelectFrameIndex = frameIndex;
        }
        public bool TryGetSelectFrameViewportX(float contentOffsetPos, out float framePosX)
        {
            framePosX = 0;
            if (CurrentSelectFramePos >= contentOffsetPos)
            {
                framePosX = CurrentSelectFramePos - contentOffsetPos;
                return true;
            }
            return false;
        }
        public static int GetFrameIndexByMousePos(float x,int frameUnitWidth)
        {
            return Mathf.RoundToInt(x / frameUnitWidth);
        }
        public void CheckMaxFrameCount(int frameIndex, int durationFrame)
        {
            // 如果超过右侧边界，拓展边界
            if (frameIndex + durationFrame >SkillConfig.maxFrameCount)
            {
                // 保存配置导致对象无效，重新引用
                CurrentMaxFrameCount = frameIndex + durationFrame;
            }
        }

        #endregion

        #region Console
        public void PreviouFrame()
        {
            IsPlaying = false;
            UpdateCurrentSelectFrame(CurrentSelectFrameIndex - 1);
        }
        public void Play()
        {
            IsPlaying = !IsPlaying;
        }

        public void NextFrame()
        {
            IsPlaying = false;
            UpdateCurrentSelectFrame(CurrentSelectFrameIndex + 1);
        }

        public void UpdateCurrentSelectFrame(int currentFrame)
        {
            CurrentSelectFrameIndex = currentFrame;
        }

        public void UpdateCurrentMaxFrameCount(int maxFrameCount)
        {
            CurrentMaxFrameCount = maxFrameCount;
        }
        #endregion

        #region Track
        public bool CheckFrameIndexOnDrag(int selfIndex, int targetIndex, bool isLeft)
        {
            foreach (var item in SkillConfig.skillAnimationData.FrameDataDic)
            {
                if (item.Key == selfIndex) continue;
                // 向左移动 && 原先在其右边 && 目标没有重叠
                if (isLeft && selfIndex > item.Key && targetIndex < item.Key + item.Value.durationFrame)
                {
                    return false;

                }
                // 向右移动 && 原先在其左边 && 目标没有重叠
                else if (!isLeft && selfIndex < item.Key && targetIndex > item.Key)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Preview
        private DateTime startTime;
        private int startFrameIndex;
        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                if (isPlaying)
                {
                    startTime = DateTime.UtcNow;
                    startFrameIndex = currentSelectFrameIndex;
                }
            }
        }

        public void UpdateTimer()
        {
            if(isPlaying)
            {
                float timer =(float)DateTime.UtcNow.Subtract(startTime).TotalSeconds;
                //确定时间轴的帧率
                float frameRote;
                if (SkillConfig != null) frameRote = SkillConfig.frameRote;
                else frameRote = SkillEditorConfig.defaultFrameRote;
                //根据时间差计算选中帧
                CurrentSelectFrameIndex = (int)((timer * frameRote) + startFrameIndex);
                // 到达最后一帧自动暂停
                if (CurrentSelectFrameIndex == CurrentMaxFrameCount)
                {
                    IsPlaying = false;
                    CurrentSelectFrameIndex = 0;//回归起点
                }
            }
        }

        public void TickSkill(int frameIndex)
        {
            // 驱动技能表现
            if (SkillConfig != null && currentPreviewCharacterObj != null)
            {
                Animator animator = currentPreviewCharacterObj.GetComponent<Animator>();
                // 根据帧找到目前是哪个动画
                Dictionary<int,SkillAnimationEvent> frameData = SkillConfig.skillAnimationData.FrameDataDic;


                Vector3 rootMositionTotalPosition = ComputeAccumulatedRootMotion(frameIndex, animator, frameData);
                SampleAndApplyPose(frameIndex, animator, frameData);

                currentPreviewCharacterObj.transform.position = rootMositionTotalPosition;
            }
        }

        /// <summary>
        /// 采样动画并添加姿势
        /// </summary>
        private void SampleAndApplyPose(int frameIndex, Animator animator, Dictionary<int, SkillAnimationEvent> frameData)
        {

            // 找到距离这一帧左边最近的一个动画，也就是当前要播放的动画
            int currentOffset = int.MaxValue; // 最近的索引距离当前选中帧的差距
            int animationEventIndex = -1;
            foreach (var item in frameData)
            {
                int tempOffset = frameIndex - item.Key;
                if (tempOffset > 0 && tempOffset < currentOffset)
                {
                    currentOffset = tempOffset;
                    animationEventIndex = item.Key;
                }
            }
            if (animationEventIndex != -1)
            {
                SkillAnimationEvent animationEvent = frameData[animationEventIndex];
                // 动画资源总帧数
                float clipFrameCount = animationEvent.animationClip.length * animationEvent.animationClip.frameRate;
                // 计算当前的播放进度
                float progress = currentOffset / clipFrameCount;
                // 循环动画的处理
                if (progress > 1 && animationEvent.animationClip.isLooping)
                {
                    progress -= (int)progress; // 只留小数部分
                }
                animator.applyRootMotion = animationEvent.applyRootMotion;
                animationEvent.animationClip.SampleAnimation(currentPreviewCharacterObj, progress * animationEvent.animationClip.length);
            }
        }
        /// <summary>
        /// 计算累计根运动位移
        /// </summary>
        private Vector3 ComputeAccumulatedRootMotion(int frameIndex, Animator animator, Dictionary<int, SkillAnimationEvent> frameData)
        {
            Vector3 rootMositionTotalPosition = Vector3.zero;
            // 利用有序字典数据结构来达到有序计算的目的
            SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameData);
            int[] keys = frameDataSortedDic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                int key = keys[i];
                SkillAnimationEvent animationFrameData = frameDataSortedDic[key];
                // 只考虑根运动配置的动画
                if (animationFrameData.applyRootMotion == false) continue;
                int nextKeyFrame = 0;
                if (i + 1 < keys.Length) nextKeyFrame = keys[i + 1];
                // 最后一个动画 下一个关键帧计算采用整个技能的帧长度
                else nextKeyFrame = SkillConfig.maxFrameCount;

                bool isBreak = false;
                if (nextKeyFrame > frameIndex)
                {
                    nextKeyFrame = frameIndex;
                    isBreak = true;
                }

                // 持续帧数 = 下一个动画的帧数 - 这个动画的开始时间
                int durationFrameCount = nextKeyFrame - key;
                if (durationFrameCount > 0)
                {
                    // 动画资源总总帧数
                    float clipFrameCount = animationFrameData.animationClip.length * SkillConfig.frameRote;
                    // 计算总的播放进度
                    float totalProgress = durationFrameCount / clipFrameCount;
                    // 播放次数
                    int playTimes = 0;
                    // 最终一次不完整的播放，也就是进度<1
                    float lastProgress = 0;
                    // 只有循环动画才需要多次采样
                    if (animationFrameData.animationClip.isLooping)
                    {
                        playTimes = (int)totalProgress;
                        lastProgress = totalProgress - (int)totalProgress;
                    }
                    else
                    {
                        // 不循环的动画，播放进度>1也等于1,
                        if (totalProgress >= 1)
                        {
                            playTimes = 1;
                            lastProgress = 0;
                        }
                        else
                        {
                            lastProgress = totalProgress - (int)totalProgress;
                        }
                    }
                    animator.applyRootMotion = true;
                    // 完整播放部分的采样
                    if (playTimes >= 1)
                    {
                        animationFrameData.animationClip.SampleAnimation(currentPreviewCharacterObj, animationFrameData.animationClip.length);
                        Vector3 pos = currentPreviewCharacterObj.transform.position;
                        rootMositionTotalPosition += pos * playTimes;
                    }
                    // 不完整的部分采样
                    if (lastProgress > 0)
                    {
                        animationFrameData.animationClip.SampleAnimation(currentPreviewCharacterObj, lastProgress * animationFrameData.animationClip.length);
                        Vector3 pos = currentPreviewCharacterObj.transform.position;
                        rootMositionTotalPosition += pos;
                    }
                }
                if (isBreak) break;
            }

            return rootMositionTotalPosition;
        }


        #endregion
        public void SaveConfig()
        {
            if (SkillConfig != null)
            {
                EditorUtility.SetDirty(SkillConfig);
                AssetDatabase.SaveAssetIfDirty(SkillConfig);
                editorWindow.ResetTrackData();
            }
        }

      

       
    }
}
