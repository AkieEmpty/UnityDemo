using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AnimationTrackItemStyle : TrackItemStyleBase
    {
        private const string trackItemAssetPath = "Assets/Editor/SkillEditor/Assets/Track/TrackItem/AnimationTrackItem.uxml";
        private Label titleLabel;
        private VisualElement mainDragArea;
        private VisualElement animationOverLine;
        public void Init(TrackStyleBase trackStyle)
        {
            titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
            root = titleLabel;
            mainDragArea = root.Q<VisualElement>("Main");
            animationOverLine = root.Q<VisualElement>("OverLline");
            trackStyle.AddItem(root);
        }

        public void SetTitle(string title)
        {
            titleLabel.text = title;
        }

        public void RegisterMouseCallback<T>(EventCallback<T> action) where T : EventBase<T>, new()
        {
            mainDragArea.RegisterCallback<T>(action);
        }
        public void ToggleOverLine(bool isShow)
        {
            if(isShow) animationOverLine.style.display = DisplayStyle.Flex;
            else animationOverLine.style.display = DisplayStyle.None;

        }
        public void SetOverLinePos(int maxFrameCount,int frameUnitWidth)
        {
            Vector3 overLinePos = animationOverLine.transform.position;
            overLinePos.x = maxFrameCount * frameUnitWidth - 1;// 线条自身宽度为2
            animationOverLine.transform.position = overLinePos;
        }
  
    }
}
