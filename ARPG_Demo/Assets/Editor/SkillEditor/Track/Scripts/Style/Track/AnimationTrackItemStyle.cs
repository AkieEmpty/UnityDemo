using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    public class AnimationTrackItemStyle : TrackItemStyleBase
    {
        private const string trackItemAssetPath = "Assets/Editor/SkillEditor/Track/Assets/AnimationTrack/AnimationTrackItem.uxml";
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
    }
}
