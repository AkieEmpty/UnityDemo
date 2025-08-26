using System;
using System.Collections.Generic;
using AkieEmpty.CharacterSystem;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace AkieEmpty.SkillEditor
{
   
    public interface ISkillEditorWindow
    {
        public SkillConfig SkillConfig {  get; }

        public void ResetTrackData();
        public void ShowTrackInspector(TrackBase track, TrackItemBase trackItem);
        public void UpdateConsoleField();
        public void UpdateTimerShaftView();
        public void UpdateTrackContentSzie();
    }
    public class SkillEditorWindow : EditorWindow, ISkillEditorWindow
    {
        private const string EditorWindowPath = "Assets/Editor/SkillEditor/Assets/EditorWindow/SkillEditorWindow.uxml";
        private const string SkillEditorScenePath = "Assets/Editor/SkillEditor/Scene/SkillEditorScene.unity";
        private static SkillEditorWindow editorWindow;
        private SkillEditorConfig skillEditorConfig;
        private SkillEditorSystem editorSystem;
        private SkillConfig skillConfig;
        public SkillConfig SkillConfig { get => skillConfig; }

        private VisualElement Root => rootVisualElement;
        private ScrollView mainContentView;

        [MenuItem("SkillEditor/SkillEditorWindow")]
        public static void ShowExample()
        {
            editorWindow = GetWindow<SkillEditorWindow>();
            editorWindow.titleContent = new GUIContent("技能编辑器");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorWindowPath);
            VisualElement labelFromUXML = visualTree.Instantiate();
            Root.Add(labelFromUXML);

            skillEditorConfig = new SkillEditorConfig();
            SkillConfig.SetValidateAction(ResetSkillConfigObjectField);
            editorSystem = new SkillEditorSystem(this, skillEditorConfig);


            InitMenu();
            InitTimeShift();
            InitTrackView();
            InitConsole();

            if(skillConfig != null)skillConfigObjectField.value = skillConfig;
        }
     
        #region Menu
        private Button loadEditorSceneButton;
        private Button loadOldSceneButton;
        private Button basicInfoButton;
        private ObjectField previewCharacterPrefabObjectField;
        private ObjectField previewCharacterObjectField;
        private ObjectField skillConfigObjectField;
        private void InitMenu()
        {
            loadEditorSceneButton = Root.Q<Button>("LoadEditorSceneButton") ?? throw new InvalidOperationException("找不到控件: LoadEditorSceneButton");
            loadOldSceneButton = Root.Q<Button>("LoadOldSceneButton") ?? throw new InvalidOperationException("找不到控件: LoadOldSceneButton");
            basicInfoButton = Root.Q<Button>("BasicInfoButton") ?? throw new InvalidOperationException("找不到控件: BasicInfoButton");
            previewCharacterPrefabObjectField = Root.Q<ObjectField>("PreviewCharacterPrefabObjectField") ?? throw new InvalidOperationException("找不到控件: PreviewCharacterPrefabObjectField");
            previewCharacterObjectField = Root.Q<ObjectField>("PreviewCharacterObjectField") ?? throw new InvalidOperationException("找不到控件: PreviewCharacterObjectField");
            skillConfigObjectField = Root.Q<ObjectField>("SkillConfigObjectField") ?? throw new InvalidOperationException("找不到控件: SkillConfigObjectField");

            loadEditorSceneButton.clicked += LoadEditorSceneButtonClick;
            loadOldSceneButton.clicked += LoadOldSceneButtonClick;
            basicInfoButton.clicked += BasicInfoButtonClick;

            previewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldValueChanged);
            previewCharacterObjectField.RegisterValueChangedCallback(PreviewCharacterObjectFieldValueChanged);
            skillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldValueChanged);

           
        }
        private void LoadEditorSceneButtonClick() 
        {
            editorSystem.LoadEditorScene(SkillEditorScenePath);
        }
        private void LoadOldSceneButtonClick()
        {
            editorSystem.LoadOldScene();
        }
        private void BasicInfoButtonClick()
        {
            if (SkillConfig != null)
                Selection.activeObject = SkillConfig;
        }
        private void PreviewCharacterPrefabObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            // 避免在其他场景实例化
            if (SkillEditorScenePath != EditorSceneManager.GetActiveScene().path)
            {
                previewCharacterPrefabObjectField = null;
            }
            else
            {
                previewCharacterObjectField.value = editorSystem.CreatePreviewCharacter((GameObject)evt.newValue);
            }
        }
        private void PreviewCharacterObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            editorSystem.SetPreviewFromSceneObject(evt);
        }
        private void SkillConfigObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            skillConfig = ((SkillConfig)evt.newValue);
            //刷新最大帧数输入框
            if (SkillConfig == null) maxFrameCountField.value = 100;
            else maxFrameCountField.value = SkillConfig.maxFrameCount;
            // 刷新轨道
            ResetTrack();
            Selection.activeObject = (SkillConfig)evt.newValue;
        }
        private void ResetSkillConfigObjectField()
        {
            SkillConfig tempSkillConfig = SkillConfig;
            skillConfigObjectField.value = null;
            skillConfigObjectField.value = tempSkillConfig;
        }
        #endregion

        #region TimeShift

        private IMGUIContainer timerShaft;
        private IMGUIContainer selectLine;
        private VisualElement contentContainer;
        private VisualElement contentViewPort;
        private bool timerShaftIsMouseEnter;
        private float ContentOffsetPos => Mathf.Abs(contentContainer.transform.position.x);
      
        private void InitTimeShift()
        {
            timerShaft = Root.Q<IMGUIContainer>("TimerShaft") ?? throw new InvalidOperationException("找不到控件: TimerShaft");
            selectLine = Root.Q<IMGUIContainer>("SelectLine") ?? throw new InvalidOperationException("找不到控件: SelectLine");
            mainContentView = Root.Q<ScrollView>("MainContentView") ?? throw new InvalidOperationException("找不到控件: MainContentView");
            contentContainer = mainContentView.Q<VisualElement>("unity-content-container") ?? throw new InvalidOperationException("找不到控件: unity-content-container");
            contentViewPort = mainContentView.Q<VisualElement>("unity-content-viewport") ?? throw new InvalidOperationException("找不到控件: unity-content-viewport");

            selectLine.onGUIHandler += DrawSelectLine;
            timerShaft.onGUIHandler += DrawTimerShaft;
           
            timerShaft.RegisterCallback<WheelEvent>(TimerShaftWheel);
            timerShaft.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
            timerShaft.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
            timerShaft.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
            timerShaft.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);
        }
        private void DrawTimerShaft()
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            Rect rect = timerShaft.contentRect;

            TimeShaftLayout layout = editorSystem.CalculateTimeShaftLayout(ContentOffsetPos);
            int index = layout.index;
            for (float i = layout.startOffset; i < rect.width; i += skillEditorConfig.CurrentFrameUnitWidth)
            {
                // 绘制长线、文本
                if (index % layout.tickStep == 0)
                {
                    Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                    string indexStr = index.ToString();
                    GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
                }
                else Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
                index += 1;
            }
            Handles.EndGUI();
        }
        private void DrawSelectLine()
        {
            // 判断当前选中帧是否在视图范围内
            if(editorSystem.TryGetSelectFrameViewportX(ContentOffsetPos, out float framPosX))
            {
                Handles.BeginGUI();
                Handles.color = Color.white;
                Handles.DrawLine(new Vector3(framPosX, 0), new Vector3(framPosX, contentViewPort.contentRect.height + timerShaft.contentRect.height));
                Handles.EndGUI();
            }
        }
        private void TimerShaftWheel(WheelEvent evt)
        {
            int delta = (int)evt.delta.y;
            skillEditorConfig.CurrentFrameUnitWidth
                = Mathf.Clamp(skillEditorConfig.CurrentFrameUnitWidth - delta,
                SkillEditorConfig.StandFrameUnitWidth,
                SkillEditorConfig.maxFrameWidthLV * SkillEditorConfig.StandFrameUnitWidth);

            UpdateTimerShaftView();
            UpdateTrackContentSzie();
            ResetTrack();
        }
        private void TimerShaftMouseDown(MouseDownEvent evt)
        {
            timerShaftIsMouseEnter = true;
            editorSystem.SelectFrameIndexFromMouseDown(evt.localMousePosition.x + ContentOffsetPos);
        }
        private void TimerShaftMouseMove(MouseMoveEvent evt)
        {
            if (!timerShaftIsMouseEnter) return;
            editorSystem.SelectFrameIndexFromMouseDown(evt.localMousePosition.x + ContentOffsetPos);
        }
        private void TimerShaftMouseUp(MouseUpEvent evt) => timerShaftIsMouseEnter = false;
        private void TimerShaftMouseOut(MouseOutEvent evt) => timerShaftIsMouseEnter = false;
        public void UpdateTimerShaftView()
        {
            timerShaft.MarkDirtyLayout(); // 标志为需要重新绘制的
            selectLine.MarkDirtyLayout(); // 标志为需要重新绘制的
        }


        #endregion

        #region Console
        private Button previouFrameButton;
        private Button playButton;
        private Button nextFrameButton;
        private IntegerField currentFrameField;
        private IntegerField maxFrameCountField;

        private void InitConsole()
        {
            previouFrameButton = Root.Q<Button>("PreviouFrameButton")??throw new InvalidOperationException("找不到控件: PreviouFrameButton");
            playButton = Root.Q<Button>("PlayButton") ??throw new InvalidOperationException("找不到控件: PlayButton");
            nextFrameButton = Root.Q<Button>("NextFrameButton") ??throw new InvalidOperationException("找不到控件: NextFrameButton");
            currentFrameField = Root.Q<IntegerField>("CurrentFrameField") ??throw new InvalidOperationException("找不到控件: CurrentFrameField");
            maxFrameCountField = Root.Q<IntegerField>("FrameCountField") ??throw new InvalidOperationException("找不到控件: FrameCountField");

            previouFrameButton.clicked += OnPreviouFrameButtonClick;
            playButton.clicked += OnPlayButtonClick;
            nextFrameButton.clicked += OnNextFrameButtonClick;
            currentFrameField.RegisterValueChangedCallback(OnCurrentFrameFieldValueChanged);
            maxFrameCountField.RegisterValueChangedCallback(OnMaxFrameCountValueChanged);

            if(SkillConfig!=null)
            {
                maxFrameCountField.value = SkillConfig.maxFrameCount;
            }
        }
      
        private void OnPreviouFrameButtonClick()
        {
            editorSystem.PreviouFrame();
        }

        private void OnPlayButtonClick()
        {
           editorSystem.Play();
        }

        private void OnNextFrameButtonClick()
        {
            editorSystem.NextFrame();
        }

        private void OnCurrentFrameFieldValueChanged(ChangeEvent<int> evt)
        {
            editorSystem.UpdateCurrentSelectFrame(evt.newValue);
        }

        private void OnMaxFrameCountValueChanged(ChangeEvent<int> evt)
        {
            editorSystem.UpdateCurrentMaxFrameCount(evt.newValue);
        }
        public void UpdateConsoleField()
        {
            currentFrameField.value = editorSystem.CurrentSelectFrameIndex;
            maxFrameCountField.value = editorSystem.CurrentMaxFrameCount;
        }

        #endregion

        #region Track
        private VisualElement trackMenuList;
        private VisualElement trackContentList;   
        private readonly List<TrackBase> trackList = new List<TrackBase>();

        private void InitTrackView()
        {
            trackMenuList = Root.Q<VisualElement>("TrackMenuList");
            trackContentList = Root.Q<VisualElement>("TrackContentList");         
            mainContentView.verticalScroller.valueChanged += TrackContentViewScrollViewValueChanged;
            UpdateTrackContentSzie();
            InitTrack();
        }
       

        private void InitTrack()
        {
            if (skillConfig == null) return;//没有配置不初始化轨道
            InitAnimationTrack();
            InitAudioTrack();
        }
        private void InitAnimationTrack()
        {
            AnimationTrack animationTrack = new AnimationTrack(editorSystem, this);
            animationTrack.Init(trackMenuList, trackContentList, skillEditorConfig.CurrentFrameUnitWidth);
            trackList.Add(animationTrack);
        }
        private void InitAudioTrack()
        {
            AudioTrack audioTrack = new AudioTrack(editorSystem, this);
            audioTrack.Init(trackMenuList, trackContentList, skillEditorConfig.CurrentFrameUnitWidth);
            trackList.Add(audioTrack);
        }
        private void DestoryTrack()
        {
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].Destory();
            }
            trackList.Clear();
        }
        private void ResetTrack()
        { 
            //如果配置文件是null,清理轨道
            if (skillConfig==null)
            {
                DestoryTrack();
            }
            else
            {
                //如果轨道列表里面没有数据,说明没有轨道,当前用户是有配置的,需要初始化轨道
                if (trackList.Count==0)
                {
                    InitTrack();
                }
                //更新视图
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].ResetView(skillEditorConfig.CurrentFrameUnitWidth);
                }
            }
          
        }
      

        private void TrackContentViewScrollViewValueChanged(float value)
        {
            Vector3 pos = trackMenuList.transform.position;
            pos.y = contentContainer.transform.position.y;
            trackMenuList.transform.position = pos;
        }
      

        public void ResetTrackData()
        {
            // 重新引用一下数据
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].OnConfigChanged();
            }
        }
        public void UpdateTrackContentSzie()
        {
            trackContentList.style.width = skillEditorConfig.CurrentFrameUnitWidth * editorSystem.CurrentMaxFrameCount;

        }
        #endregion

        #region Preview
        public void Update()
        {
            editorSystem.UpdateTimer();
        }
        #endregion

        public void ShowTrackInspector(TrackBase track, TrackItemBase trackItem)
        {
            SkillEditorInspector.SetTrackItem(editorSystem,trackItem,track);
            Selection.activeObject = this;
        }
    }
}

