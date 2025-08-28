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
        private const float HeadHeight = 35;//5是间距
        private const float ItemHeight = 32;//2是底部外边距

        private readonly List<ChildTrack> childTrackList = new List<ChildTrack>();

        private VisualElement menuItemParent;//子轨道的菜单父物体

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
            //轨道内容   
            trackContentRoot.style.height = height;
            //轨道菜单
            trackMenuRoot.style.height = height;
            //子轨道的菜单父物体
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
      
        //删除子轨道
        private void DeleteChildTrack(ChildTrack childTrack)
        {
            if (deleteChildTrackFunc == null) return;
            //由上级具体轨道类来判断能不能删除
            if (deleteChildTrackFunc(childTrack.Index))
            {
                childTrack.DoDestory();
                childTrackList.RemoveAt(childTrack.Index);
                //所有的子轨道都需要更新一下索引
                UpdateChildTrackIndex(childTrack.Index);
                UpdateSize();
            }
          
        }
        private void DeleteChildTrackAndData(ChildTrack childTrack)
        {
            childTrack.DoDestory();
            childTrackList.RemoveAt(childTrack.Index);
            //所有的子轨道都需要更新一下索引
            UpdateChildTrackIndex(childTrack.Index);
            UpdateSize();
        }
        //更新子轨道索引
        private void UpdateChildTrackIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < childTrackList.Count; i++)
            {
                childTrackList[i].SetIndex(i);
            }
        }
        //交换子轨道
        private void SpawnChildTrack(int index1,int index2)
        {
            if(index1!=index2)
            {
                ChildTrack childTemp = childTrackList[index1];
                childTrackList[index1] = childTrackList[index2];
                childTrackList[index2] = childTemp;
                UpdateChildTrackIndex();
                //通知上级处理实际的数据变更
                spawnChildTrackAction?.Invoke(index1,index2);
            }
        }

        #region 子轨道交互
        private bool isDragging =false;
        private int selectTrackIndex = -1;
        private void ItemParentMosueDown(MouseDownEvent evt)
        {

            //关闭旧的
            if (selectTrackIndex != -1)
            {
                childTrackList[selectTrackIndex].UnSelect();
            }
            //通过高度推导出当前交互的是第几个子菜单
            float mousePosition = evt.localMousePosition.y ;
            selectTrackIndex = GetChildIndexByMousePosition(mousePosition);
            childTrackList[selectTrackIndex].Select();
            //TODO:拖拽
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
                selectTrackIndex = mouseTrackIndex;//把选中的轨道更新为选中的轨道
            }
        }

        private void ItemParentMosueUp(MouseUpEvent evt)
        {
            isDragging = false;
        }

        private void ItemParentMosueOut(MouseOutEvent evt)
        {
            //ItemParentMouseOut这个函数经常互无意义调用,以为子物体和我们产生遮挡关系
            //检测鼠标是否真的离开了我们选中的区域
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
        /// 多行轨道中的子轨道
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
                //计算子菜单位置
                Vector3 menuPos =menuRoot.transform.position;
                menuPos.y = index * ItemHeight;
                menuRoot.transform.position = menuPos;
                //计算轨道位置
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