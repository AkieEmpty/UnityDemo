using JKFrame;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AkieEmpty.Animations
{
    /// <summary>
    /// 单个动画节点
    /// </summary>
    public class SingleAnimationNode : AnimationNodeBase
    {
        private AnimationClipPlayable clipPlayable;

        public void Init(PlayableGraph graph, AnimationMixerPlayable mixer, AnimationClip animationClip, float speed, int targetInputPort)
        {
            clipPlayable = AnimationClipPlayable.Create(graph, animationClip);
            clipPlayable.SetSpeed(speed);
           
            this.InputPort = targetInputPort;
            graph.Connect(clipPlayable, 0, mixer, targetInputPort);
        }

        public AnimationClip GetAnimationClip()
        {
            return clipPlayable.GetAnimationClip();
        }
        public override void SetApplyFootIK(bool value=false)
        {
            clipPlayable.SetApplyFootIK(false);
        }
        public override void SetSpeed(float speed)
        {
            clipPlayable.SetSpeed(speed);
        }
    }
}
