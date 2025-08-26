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
            editorWindow.titleContent = new GUIContent("���ܱ༭��");
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
            loadEditorSceneButton = Root.Q<Button>("LoadEditorSceneButton") ?? throw new InvalidOperationException("�Ҳ����ؼ�: LoadEditorSceneButton");
            loadOldSceneButton = Root.Q<Button>("LoadOldSceneButton") ?? throw new InvalidOperationException("�Ҳ����ؼ�: LoadOldSceneButton");
            basicInfoButton = Root.Q<Button>("BasicInfoButton") ?? throw new InvalidOperationException("�Ҳ����ؼ�: BasicInfoButton");
            previewCharacterPrefabObjectField = Root.Q<ObjectField>("PreviewCharacterPrefabObjectField") ?? throw new InvalidOperationException("�Ҳ����ؼ�: PreviewCharacterPrefabObjectField");
            previewCharacterObjectField = Root.Q<ObjectField>("PreviewCharacterObjectField") ?? throw new InvalidOperationException("�Ҳ����ؼ�: PreviewCharacterObjectField");
            skillConfigObjectField = Root.Q<ObjectField>("SkillConfigObjectField") ?? throw new InvalidOperationException("�Ҳ����ؼ�: SkillConfigObjectField");

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
            // ��������������ʵ����
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
            //ˢ�����֡�������
            if (SkillConfig == null) maxFrameCountField.value = 100;
            else maxFrameCountField.value = SkillConfig.maxFrameCount;
            // ˢ�¹��
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
            timerShaft = Root.Q<IMGUIContainer>("TimerShaft") ?? throw new InvalidOperationException("�Ҳ����ؼ�: TimerShaft");
            selectLine = Root.Q<IMGUIContainer>("SelectLine") ?? throw new InvalidOperationException("�Ҳ����ؼ�: SelectLine");
            mainContentView = Root.Q<ScrollView>("MainContentView") ?? throw new InvalidOperationException("�Ҳ����ؼ�: MainContentView");
            contentContainer = mainContentView.Q<VisualElement>("unity-content-container") ?? throw new InvalidOperationException("�Ҳ����ؼ�: unity-content-container");
            contentViewPort = mainContentView.Q<VisualElement>("unity-content-viewport") ?? throw new InvalidOperationException("�Ҳ����ؼ�: unity-content-viewport");

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
                // ���Ƴ��ߡ��ı�
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
            // �жϵ�ǰѡ��֡�Ƿ�����ͼ��Χ��
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
            timerShaft.MarkDirtyLayout(); // ��־Ϊ��Ҫ���»��Ƶ�
            selectLine.MarkDirtyLayout(); // ��־Ϊ��Ҫ���»��Ƶ�
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
            previouFrameButton = Root.Q<Button>("PreviouFrameButton")??throw new InvalidOperationException("�Ҳ����ؼ�: PreviouFrameButton");
            playButton = Root.Q<Button>("PlayButton") ??throw new InvalidOperationException("�Ҳ����ؼ�: PlayButton");
            nextFrameButton = Root.Q<Button>("NextFrameButton") ??throw new InvalidOperationException("�Ҳ����ؼ�: NextFrameButton");
            currentFrameField = Root.Q<IntegerField>("CurrentFrameField") ??throw new InvalidOperationException("�Ҳ����ؼ�: CurrentFrameField");
            maxFrameCountField = Root.Q<IntegerField>("FrameCountField") ??throw new InvalidOperationException("�Ҳ����ؼ�: FrameCountField");

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
            if (skillConfig == null) return;//û�����ò���ʼ�����
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
            //��������ļ���null,������
            if (skillConfig==null)
            {
                DestoryTrack();
            }
            else
            {
                //�������б�����û������,˵��û�й��,��ǰ�û��������õ�,��Ҫ��ʼ�����
                if (trackList.Count==0)
                {
                    InitTrack();
                }
                //������ͼ
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
            // ��������һ������
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

