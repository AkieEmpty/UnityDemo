using System.Collections.Generic;
using AkieEmpty.CharacterSystem;
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
        public void SaveConfig();
    }
    public class SkillEditorSystem :Object, ISkillEditorSystem
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
            if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);
            Transform parent = GameObject.Find(previewCharacterParentPath).transform;
            if (parent != null && parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
            // 实例化新的
            if (previewObj != null)
            {
                currentPreviewCharacterObj = Instantiate(previewObj, Vector3.zero, Quaternion.identity, parent);
                currentPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                return currentPreviewCharacterObj;
            }
            return null;

        }
        public void SetPreviewFromSceneObject(ChangeEvent<Object> evt)
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

      
        #endregion

        #region Console
        public void PreviouFrame()
        {
            UpdateCurrentSelectFrame(CurrentSelectFrameIndex - 1);
        }
        public void Play()
        {

        }

        public void NextFrame()
        {           
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

        public void SaveConfig()
        {
            if (SkillConfig != null)
            {
                EditorUtility.SetDirty(SkillConfig);
                AssetDatabase.SaveAssetIfDirty(SkillConfig);
            }
        }


    }
}
