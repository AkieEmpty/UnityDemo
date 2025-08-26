using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class MultilineTrackStyle : TrackStyleBase
    {
        private const string MenuAssetPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackMenu.uxml";
        //private const string TrackAssetPath = "Assets/Editor/SkillEditor/Assets/Track/SinglineTrackStyle/SingleLineTrackContent.uxml";
        private readonly List<ChildTrack> childTrackList = new List<ChildTrack>();

        private VisualElement menuItemParent;//子轨道的菜单父物体

        private Func<bool> addChildTrackFunc;
        private Func<int,bool> deleteChildTrackFunc;

        public void Init(VisualElement trackMenuParent, VisualElement trackContentParent, string title, Func<bool> addChildTrackFunc, Func<int, bool> deleteChildTrackFunc)
        {
            this.trackMenuParent = trackMenuParent;
            this.trackContentParent = trackContentParent;
            this.addChildTrackFunc = addChildTrackFunc;
            this.deleteChildTrackFunc = deleteChildTrackFunc;
            trackMenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
            //trackContentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];

            trackMenuRoot.Q<Label>().text = title;

            menuItemParent = trackMenuRoot.Q<VisualElement>("TrackMenuList");

            trackMenuParent.Add(trackMenuRoot);
            // trackContentParent.Add(trackContentRoot);

            Button addButton = trackMenuRoot.Q<Button>("AddButton");
            addButton.clicked += OnAddButtonClick;
        }

        //添加子轨道
        private void OnAddButtonClick()
        {
            if (addChildTrackFunc == null) return;
            //由上级具体轨道类来判断能不能添加
            if (addChildTrackFunc())
            {
                ChildTrack childTrack = new ChildTrack();
                childTrack.Init(menuItemParent, null,childTrackList.Count, DeleteChildTrack);
                childTrackList.Add(childTrack);
            }
        }
        //删除子轨道
        private void DeleteChildTrack(ChildTrack childTrack)
        {
            if (deleteChildTrackFunc == null) return;
            //由上级具体轨道类来判断能不能删除
            if (deleteChildTrackFunc(childTrack.Index))
            {
                childTrack.Destory();
                childTrackList.RemoveAt(childTrack.Index);
                //所有的子轨道都需要更新一下索引
                UpdateChildTrackIndex(childTrack.Index);
            }
          
        }
        //更新子轨道索引
        private void UpdateChildTrackIndex(int startIndex = 0)
        {
            for (int i = startIndex; i < childTrackList.Count; i++)
            {
                childTrackList[i].SetIndex(i);
            }
        }


        /// <summary>
        /// 多行轨道中的子轨道
        /// </summary>
        public class ChildTrack
        {
            private const string MenuItemAssetsPath = "Assets/Editor/SkillEditor/Assets/Track/MultilineTrackStyle/MultilineTrackMenuItem.uxml";
            private int index;
            public VisualElement menuRoot {  get; private set; }
            public VisualElement trackRoot { get; private set; }

            public VisualElement menuParent { get; private set; }
            public VisualElement trackParent { get; private set; }
            public int Index { get => index; }
            public void Init(VisualElement menuParent, VisualElement trackParent, int index ,Action<ChildTrack> deleteAction)
            {
                this.menuParent = menuParent;
                this.trackParent = trackParent;
                this.index = index;
                menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuItemAssetsPath).Instantiate().Query().ToList()[1];
                menuParent.Add(menuRoot);

                Button deleteButton = menuRoot.Q<Button>("DeleteButton");
                deleteButton.clicked += () => deleteAction(this);
            }

            public void SetIndex(int index)
            {
                this.index = index;
            }

            public void Destory()
            {
                menuParent.Remove(menuRoot);
            }
        }
    }
}