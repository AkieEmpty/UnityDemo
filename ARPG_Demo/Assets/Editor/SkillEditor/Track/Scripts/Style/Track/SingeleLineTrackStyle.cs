using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// 技能单行轨道样式
    /// </summary>
    public class SingeleLineTrackStyle : TrackStyleBase
    {
        private const string MenuAssetPath = "Assets/Editor/SkillEditor/Track/Assets/SinglineTrackStyle/SingleLineTrackMenu.uxml";
        private const string TrackAssetPath = "Assets/Editor/SkillEditor/Track/Assets/SinglineTrackStyle/SingleLineTrackContent.uxml";
        public void Init(VisualElement trackMenuParent, VisualElement trackContentParent, string title)
        {
            this.trackMenuParent = trackMenuParent;
            this.trackContentParent = trackContentParent;

            trackMenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
            trackContentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];

            ((Label)trackMenuRoot).text = title;

            trackMenuParent.Add(trackMenuRoot);
            trackContentParent.Add(trackContentRoot);
        }

        public void RegisterDragUpdatedCallback(EventCallback<DragUpdatedEvent> action)
        {
            trackContentRoot.RegisterCallback<DragUpdatedEvent>(action);
        }
        public void RegisterDragExitedCallback(EventCallback<DragExitedEvent> action)
        {
            trackContentRoot.RegisterCallback<DragExitedEvent>(action);
        }

    }
}

