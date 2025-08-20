using AkieEmpty.CharacterSystem;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class SkillEditorSystem :Object
    {
        private readonly ISkillEditorWindow editorWindow;
        public readonly SkillConfig skillConfig;
        public SkillEditorSystem(ISkillEditorWindow editorWindow,SkillConfig skillConfig) 
        {
            this.editorWindow = editorWindow;
            this.skillConfig = skillConfig;
        } 

        #region 菜单
        private const string skillEditorScenePath = "Assets/Editor/SkillEditor/Scene/SkillEditorScene.unity";
        private const string previewCharacterParentPath = "PreviewCharacterRoot";
        private GameObject currentPreviewCharacterObj;
        private string oldScenePath;
        /// <summary>
        /// 加载编辑器场景
        /// </summary>
        public void LoadEditorScene()
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
      
        public void SetPreviewFromPrefab(ChangeEvent<Object> evt)
        {
            // 避免在其他场景实例化
            if (skillEditorScenePath != EditorSceneManager.GetActiveScene().path)
            {
                editorWindow.PreviewCharacterPrefabObjectField.value = null;
                return;
            }
            // 值相等，设置无效
            if (evt.newValue == currentPreviewCharacterObj) return;

            // 销毁旧的
            if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);
            Transform parent = GameObject.Find(previewCharacterParentPath).transform;
            if (parent != null && parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
            // 实例化新的
            if (evt.newValue != null)
            {
                currentPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
                currentPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                editorWindow.PreviewCharacterObjectField.value = currentPreviewCharacterObj;
            }
        }
        public void SetPreviewFromSceneObject(ChangeEvent<Object> evt)
        {
            currentPreviewCharacterObj = (GameObject)evt.newValue;
        }

        #endregion

       
    }
}
