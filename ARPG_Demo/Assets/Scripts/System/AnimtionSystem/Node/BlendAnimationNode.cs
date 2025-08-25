using System;
using System.Collections.Generic;
using Codice.CM.Common;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AkieEmpty.Animations
{
    /// <summary>
    /// 混合动画节点
    /// </summary>
    public class BlendAnimationNode : AnimationNodeBase
    {
        private readonly List<AnimationClipPlayable> blendClipPlayableList = new List<AnimationClipPlayable>(10);
        private AnimationMixerPlayable blendMixer;
        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, List<AnimationClip> clips, float speed, int inputPort)
        {
            blendMixer = AnimationMixerPlayable.Create(graph, clips.Count);
            graph.Connect(blendMixer, 0, outputMixer, inputPort);
            this.InputPort = inputPort;
            for (int i = 0; i < clips.Count; i++)
            {
                CreateAndConnectBlendPlayable(graph, clips[i], i, speed);
            }
        }
        
        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip clip1, AnimationClip clip2, float speed, int inputPort)
        {
            blendMixer = AnimationMixerPlayable.Create(graph, 2);
            graph.Connect(blendMixer, 0, outputMixer, inputPort);
            this.InputPort = inputPort;
            CreateAndConnectBlendPlayable(graph, clip1, 0, speed);
            CreateAndConnectBlendPlayable(graph, clip2, 1, speed);
        }

       

        private void CreateAndConnectBlendPlayable(PlayableGraph graph, AnimationClip clip, int inputPort, float speed)
        {
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph,clip);
            clipPlayable.SetSpeed(speed);
            blendClipPlayableList.Add(clipPlayable);
            graph.Connect(clipPlayable, 0, blendMixer, inputPort);
        }

        public override void Destory()
        {
            blendClipPlayableList.Clear();
            base.Destory();
        }

        public void SetBlendWeight(float clipWeight)
        {
            blendMixer.SetInputWeight(0, clipWeight);
            blendMixer.SetInputWeight(1, 1 - clipWeight);
        }

        public override void SetSpeed(float speed)
        {
            for (int i = 0; i < blendClipPlayableList.Count; i++)
            {
                blendClipPlayableList[i].SetSpeed(speed);
            }
        }

        public override void SetApplyFootIK(bool value=false)
        {
            for (int i = 0; i < blendClipPlayableList.Count; i++)
            {
                blendClipPlayableList[i].SetApplyFootIK(value);
            }
        }
    }
}
