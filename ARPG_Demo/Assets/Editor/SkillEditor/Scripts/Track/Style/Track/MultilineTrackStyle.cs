using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static AkieEmpty.SkillEditor.MultilineTrackStyle;

namespace AkieEmpty.SkillEditor
{
    public class MultilineTrackStyle : TrackStyleBase
    {
        private const string MenuAssetPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackMenu.uxml";
        private const string TrackAssetPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackContent.uxml";
        private const float HeadHeight = 35;//5�Ǽ��
        private const float ItemHeight = 32;//2�ǵײ���߾�

        private readonly List<ChildTrack> childTrackList = new List<ChildTrack>();

        private VisualElement menuItemParent;//�ӹ���Ĳ˵�������

        private Action addChildTrackAction;
        private Action<int,int> spawnChildTrackAction;
        private Action<ChildTrack, string> updateTrackNameAction; 
        private Func<int,bool> deleteChildTrackFunc;

        public void Init(VisualElement trackMenuParent, VisualElement trackContentParent, string title, Action addChildTrackAction, Action<int, int> spawnChildTrackAction, Func<int, bool> deleteChildTrackFunc, Action<ChildTrack, string> updateTrackNameAction)
        {
            this.trackMenuParent = trackMenuParent;
            this.trackContentParent = trackContentParent;
            this.addChildTrackAction = addChildTrackAction;
            this.deleteChildTrackFunc = deleteChildTrackFunc;
            this.spawnChildTrackAction = spawnChildTrackAction;
            this.updateTrackNameAction = updateTrackNameAction;
            trackMenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
            trackContentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];

            trackMenuRoot.Q<Label>().text = title;

            menuItemParent = trackMenuRoot.Q<VisualElement>("TrackMenuList");
            menuItemParent.RegisterCallback<MouseDownEvent>(ItemParentMosueDown);
            menuItemParent.RegisterCallback<MouseMoveEvent>(ItemParentMouseMove);
            menuItemParent.RegisterCallback<MouseUpEvent>(ItemParentMosueUp);
            menuItemParent.RegisterCallback<MouseOutEvent>(ItemParentMosueOut);



            trackMenuParent.Add(trackMenuRoot);
            trackContentParent.Add(trackContentRoot);

            Button addButton = trackMenuRoot.Q<Button>("AddButton");
            addButton.clicked += OnAddButtonClick;
        }

        private void UpdateSize()
        {
            float height = HeadHeight + (childTrackList.Count * ItemHeight);
            //�������   
            trackContentRoot.style.height = height;
            //����˵�
            trackMenuRoot.style.height = height;
            //�ӹ���Ĳ˵�������
            menuItemParent.style.height = childTrackList.Count * ItemHeight;
        }
        public ChildTrack AddChildTrack()
        {
            ChildTrack childTrack = new ChildTrack();
            childTrack.Init(menuItemParent, trackContentRoot, childTrackList.Count, DeleteChildTrack, DeleteChildTrackAndData, updateTrackNameAction);
            childTrackList.Add(childTrack);
            UpdateSize();
            return childTrack;
        }

        private void OnAddButtonClick()
        {
            addChildTrackAction.Invoke();
        }
      
        //ɾ���ӹ��
        private void DeleteChildTrack(ChildTrack childTrack)
        {
            if (deleteChildTrackFunc == null) return;
            //���ϼ������������ж��ܲ���ɾ��
            if (deleteChildTrackFunc(childTrack.Index))
            {
                childTrack.DoDestory();
                childTrackList.RemoveAt(childTrack.Index);
                //���е��ӹ������Ҫ����һ������
                UpdateChildTrackIndex(childTrack.Index);
                UpdateSize();
            }
          
        }
        private void DeleteChildTrackAndData(ChildTrack childTrack)
        {
            childTrack.DoDestory();
            childTrackList.RemoveAt(childTrack.Index);
            //���е��ӹ������Ҫ����һ������
            UpdateChildTrackIndex(childTrack.Index);
            UpdateSize();
        }
        //�����ӹ������
        private void UpdateChildTrackIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < childTrackList.Count; i++)
            {
                childTrackList[i].SetIndex(i);
            }
        }
        //�����ӹ��
        private void SpawnChildTrack(int index1,int index2)
        {
            if(index1!=index2)
            {
                ChildTrack childTemp = childTrackList[index1];
                childTrackList[index1] = childTrackList[index2];
                childTrackList[index2] = childTemp;
                UpdateChildTrackIndex();
                //֪ͨ�ϼ�����ʵ�ʵ����ݱ��
                spawnChildTrackAction?.Invoke(index1,index2);
            }
        }

        #region �ӹ������
        private bool isDragging =false;
        private int selectTrackIndex = -1;
        private void ItemParentMosueDown(MouseDownEvent evt)
        {

            //�رվɵ�
            if (selectTrackIndex != -1)
            {
                childTrackList[selectTrackIndex].UnSelect();
            }
            //ͨ���߶��Ƶ�����ǰ�������ǵڼ����Ӳ˵�
            float mousePosition = evt.localMousePosition.y ;
            selectTrackIndex = GetChildIndexByMousePosition(mousePosition);
            childTrackList[selectTrackIndex].Select();
            //TODO:��ק
            isDragging = true;

        }

        private void ItemParentMouseMove(MouseMoveEvent evt)
        {
            if (selectTrackIndex == -1 || !isDragging) return;
            float mousePosition = evt.localMousePosition.y;
            int mouseTrackIndex = GetChildIndexByMousePosition(mousePosition);
            if(mouseTrackIndex!=selectTrackIndex)
            {
                SpawnChildTrack(selectTrackIndex, mouseTrackIndex);
                selectTrackIndex = mouseTrackIndex;//��ѡ�еĹ������Ϊѡ�еĹ��
            }
        }

        private void ItemParentMosueUp(MouseUpEvent evt)
        {
            isDragging = false;
        }

        private void ItemParentMosueOut(MouseOutEvent evt)
        {
            //ItemParentMouseOut����������������������,��Ϊ����������ǲ����ڵ���ϵ
            //�������Ƿ�����뿪������ѡ�е�����
            if (!menuItemParent.contentRect.Contains(evt.localMousePosition))
            {
                isDragging = false;
            }
        }
        private int GetChildIndexByMousePosition(float mousePositionY)
        {
            int trackIndex = Mathf.FloorToInt(mousePositionY / ItemHeight);
            trackIndex = Mathf.Clamp(trackIndex, 0, childTrackList.Count - 1);
            return trackIndex;
        }

        

        #endregion


        /// <summary>
        /// ���й���е��ӹ��
        /// </summary>
        public class ChildTrack
        {
            private const string ChildTrackMenuAssetsPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackMenuItem.uxml";
            private const string ChildTrackContentAssetsPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackContentItem.uxml";
            private static Color selectColor= Color.green;
            private static Color normalColor = new Color(0, 0, 0, 0);
            private Action<ChildTrack> deleteAction;
            private Action<ChildTrack> destoryAction;
            private Action<ChildTrack, string> updateTrackNameAction;
            private VisualElement content;
            private TextField trackNameField;
            private string oldTrackNameFiledValue;
            private int index;
            public VisualElement menuRoot {  get; private set; }
            public VisualElement trackRoot { get; private set; }

            public VisualElement menuParent { get; private set; }
            public VisualElement trackParent { get; private set; }
            public int Index { get => index; }
            public void Init(VisualElement menuParent, VisualElement trackParent, int index ,Action<ChildTrack> deleteAction, Action<ChildTrack> destoryAction, Action<ChildTrack, string> updateTrackNameAction)
            {
                this.menuParent = menuParent;
                this.trackParent = trackParent;
                this.deleteAction = deleteAction;
                this.destoryAction = destoryAction;
                this.updateTrackNameAction = updateTrackNameAction;
                menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ChildTrackMenuAssetsPath).Instantiate().Query().ToList()[1];
                menuParent.Add(menuRoot);

                trackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ChildTrackContentAssetsPath).Instantiate().Query().ToList()[1];
                trackParent.Add(trackRoot);

                trackNameField = menuRoot.Q<TextField>("NameFiled");
                trackNameField.RegisterCallback<FocusInEvent>(TrackNameFieldFocusIn);
                trackNameField.RegisterCallback<FocusOutEvent>(TrackNameFieldFocusOut);


                Button deleteButton = menuRoot.Q<Button>("DeleteButton");
                deleteButton.clicked += () => deleteAction(this);

                SetIndex(index);
                UnSelect();
            }
            public void InitContent(VisualElement content)
            {
                this.content = content;
                trackRoot.Add(content);
            }
            public void SetIndex(int index)
            {
                this.index = index;
                //�����Ӳ˵�λ��
                Vector3 menuPos =menuRoot.transform.position;
                menuPos.y = index * ItemHeight;
                menuRoot.transform.position = menuPos;
                //������λ��
                Vector3 trackPos = trackRoot.transform.position;
                trackPos.y = index * ItemHeight+HeadHeight;
                trackRoot.transform.position = trackPos;
            }
            public void SetTrackName(string name)
            {
                trackNameField.value = name;
            }
            public void Select()
            {
                menuRoot.style.backgroundColor = selectColor;
            }

            public void UnSelect()
            {
                menuRoot.style.backgroundColor = normalColor;
            }
            private void TrackNameFieldFocusIn(FocusInEvent evt)
            {

                oldTrackNameFiledValue = trackNameField.value;
            }
            private void TrackNameFieldFocusOut(FocusOutEvent evt)
            {
                if (oldTrackNameFiledValue != trackNameField.value)
                {
                    updateTrackNameAction?.Invoke(this, trackNameField.value);
                }
            }
            public void DoDestory()
            {
                menuParent.Remove(menuRoot);
                trackParent.Remove(trackRoot);
            }
            public void Destory()
            {
                destoryAction(this);
            }

          
        }
    }
}