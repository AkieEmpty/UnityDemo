using System;
using AkieEmpty.CharacterSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public interface ISkillEditorWindow
    {
        public ObjectField PreviewCharacterPrefabObjectField { get; }
        public ObjectField PreviewCharacterObjectField { get; }
        public ObjectField SkillConfigObjectField { get; }
    }
    public class SkillEditorWindow : EditorWindow,ISkillEditorWindow
    {
        private const string EditorWindowPath = "Assets/Editor/SkillEditor/EditorWindow/Assets/SkillEditorWindow.uxml";
        private SkillConfig skillConfig;
        private SkillEditorSystem editorSystem;
        private VisualElement Root => rootVisualElement;

        [MenuItem("SkillEditor/SkillEditorWindow")]
        public static void ShowExample()
        {
            SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
            wnd.titleContent = new GUIContent("技能编辑器");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorWindowPath);
            VisualElement labelFromUXML = visualTree.Instantiate();
            Root.Add(labelFromUXML);
        
            editorSystem = new SkillEditorSystem(this, skillConfig);

            InitMenu();
        }
        public void OnDestroy()
        {
           
            UnInitMenu();
        }
        #region Menu
        private Button loadEditorSceneButton;
        private Button loadOldSceneButton;
        private Button basicInfoButton;
        public ObjectField PreviewCharacterPrefabObjectField { get; private set; }
        public ObjectField PreviewCharacterObjectField { get; private set; }
        public ObjectField SkillConfigObjectField { get; private set; }
        private void InitMenu()
        {
            loadEditorSceneButton = Root.Q<Button>("LoadEditorSceneButton") ?? throw new InvalidOperationException("找不到控件: LoadEditorSceneButton");
            loadOldSceneButton = Root.Q<Button>("LoadOldSceneButton") ?? throw new InvalidOperationException("找不到控件: LoadOldSceneButton");
            basicInfoButton = Root.Q<Button>("BasicInfoButton") ?? throw new InvalidOperationException("找不到控件: BasicInfoButton");
            PreviewCharacterPrefabObjectField = Root.Q<ObjectField>("PreviewCharacterPrefabObjectField") ?? throw new InvalidOperationException("找不到控件: PreviewCharacterPrefabObjectField");
            PreviewCharacterObjectField = Root.Q<ObjectField>("PreviewCharacterObjectField") ?? throw new InvalidOperationException("找不到控件: PreviewCharacterObjectField");
            SkillConfigObjectField = Root.Q<ObjectField>("SkillConfigObjectField") ?? throw new InvalidOperationException("找不到控件: SkillConfigObjectField");

            loadEditorSceneButton.clicked += LoadEditorSceneButtonClick;
            loadOldSceneButton.clicked += LoadOldSceneButtonClick;
            basicInfoButton.clicked += BasicInfoButtonClick;

            PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldValueChanged);
            PreviewCharacterObjectField.RegisterValueChangedCallback(PreviewCharacterObjectFieldValueChanged);
            SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldValueChanged);
        }
        private void UnInitMenu()
        {
            loadEditorSceneButton.clicked -= LoadEditorSceneButtonClick;
            loadOldSceneButton.clicked -= LoadOldSceneButtonClick;
            basicInfoButton.clicked -= BasicInfoButtonClick;

            PreviewCharacterPrefabObjectField.UnregisterValueChangedCallback(PreviewCharacterPrefabObjectFieldValueChanged);
            PreviewCharacterObjectField.UnregisterValueChangedCallback(PreviewCharacterObjectFieldValueChanged);
            SkillConfigObjectField.UnregisterValueChangedCallback(SkillConfigObjectFieldValueChanged);

            Selection.activeObject = null;
        }
        #region CallBack
        private void LoadEditorSceneButtonClick()
        {
            editorSystem.LoadEditorScene();
        }
        private void LoadOldSceneButtonClick()
        {
            editorSystem.LoadOldScene();
        }
        private void BasicInfoButtonClick()
        {
            if (skillConfig != null)
                Selection.activeObject = skillConfig;
        }
        private void PreviewCharacterPrefabObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            editorSystem.SetPreviewFromPrefab(evt);
        }
        private void PreviewCharacterObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            editorSystem.SetPreviewFromSceneObject(evt);
        }
        private void SkillConfigObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            skillConfig = (SkillConfig)evt.newValue;
            Selection.activeObject = skillConfig;
        }
        #endregion

        #endregion

        #region TimeShift
        private IMGUIContainer timerShaft;
        private IMGUIContainer selectLine;
        private VisualElement contentContainer;
        private VisualElement contentViewPort;

        public void InitTimeShift()
        {

        }
        #endregion
    }
}

