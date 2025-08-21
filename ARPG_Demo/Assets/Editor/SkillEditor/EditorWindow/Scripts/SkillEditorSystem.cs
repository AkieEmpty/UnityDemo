using AkieEmpty.CharacterSystem;
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
    public class SkillEditorSystem :Object
    {
        
        private readonly ISkillEditorWindow editorWindow;
        private readonly SkillEditorConfig skillEditorConfig;
        private SkillConfig SkillConfig => editorWindow.SkillConfig;
        public SkillEditorSystem(ISkillEditorWindow editorWindow, SkillEditorConfig skillEditorConfig) 
        {
            this.editorWindow = editorWindow;
            this.skillEditorConfig = skillEditorConfig;
        } 

        #region 菜单
      
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
        //public void SetSkillConfig(SkillConfig SkillConfig)
        //{
        //    this.SkillConfig = SkillConfig;
        //    if (SkillConfig == null) UpdateCurrentMaxFrameCount(100);
        //    else UpdateCurrentMaxFrameCount(SkillConfig.maxFrameCount);
            
        //}
        #endregion

        #region TimeShaft
        private int currentSelectFrameIndex;
        private int currentMaxFrameCount;

        private int CurrentSelectFrameIndex
        {
            get => currentSelectFrameIndex;
            set
            {
                if (currentSelectFrameIndex == value) return;
                // 如果超出范围，更新最大帧
                if (value > CurrentMaxFrameCount) UpdateCurrentMaxFrameCount(value);
                currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentMaxFrameCount);
                editorWindow.UpdateTimerShaftView();
            }
        }
        private int CurrentMaxFrameCount
        {
            get => currentMaxFrameCount;
            set
            {
                if (currentMaxFrameCount == value) return;
                currentMaxFrameCount = value;
                if (SkillConfig != null) SkillConfig.maxFrameCount = value;
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
            int frameIndex = GetFrameIndexByMousePos(mouseX);
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
        private int GetFrameIndexByMousePos(float x)
        {
            return Mathf.RoundToInt(x / skillEditorConfig.CurrentFrameUnitWidth);
        }
        #endregion

        #region Console
        public void PreviouFrame()
        {
            UpdateCurrentSelectFrame(CurrentSelectFrameIndex + 1);
        }
        public void Play()
        {

        }

        public void NextFrame()
        {           
            UpdateCurrentSelectFrame(CurrentSelectFrameIndex-1);
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

    }
}
