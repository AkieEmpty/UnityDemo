using JKFrame;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AkieEmpty.Animations
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;

        private AnimationNodeBase previousNode; 
        private AnimationNodeBase currentNode;

        private Action<Vector3, Quaternion> rootMotionAction;
        private Coroutine transitionCoroutine;

        private int inputPort0 = 0;
        private int inputPort1 = 1;

        private float speed;
        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                currentNode.SetSpeed(speed);
            }
        } 

        public void Init()
        {
            animator = GetComponent<Animator>();
            graph = PlayableGraph.Create(nameof(AnimationController));//创建图
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);//设置图的时间模式
            mixer = AnimationMixerPlayable.Create(graph,3);//创建混合器
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "PlayableOutput", animator);//创建输出器
            playableOutput.SetSourcePlayable(mixer);//连接混合器
        }

        /// <summary>
        /// 播放单个动画
        /// </summary>
        public void PlaySingleAniamtion(AnimationClip clip, float speed = 1, float transitionFixedTime = 0.25f, bool applyFootIK = false, bool refreshAnimation = false)
        {
            SingleAnimationNode singleAnimationNode = null;
            if (currentNode == null)
            {
                singleAnimationNode = PoolSystem.GetObject<SingleAnimationNode>();
                if(singleAnimationNode==null) singleAnimationNode = new SingleAnimationNode();
                singleAnimationNode.Init(graph,mixer,clip,speed, inputPort0);
                mixer.SetInputWeight(inputPort0, 1);
            }
            else
            {
                SingleAnimationNode previousAnimationNode = currentNode as SingleAnimationNode; // 上一个节点
                //相同的动画并且不需要刷新时
                if (!refreshAnimation && previousAnimationNode != null && previousAnimationNode.GetAnimationClip() == clip) return;
                // 销毁掉当前可能被占用的Node
                DestoryNode(previousNode);
                singleAnimationNode = PoolSystem.GetObject<SingleAnimationNode>();
                if (singleAnimationNode == null) singleAnimationNode = new SingleAnimationNode();
                singleAnimationNode.Init(graph, mixer, clip, speed, inputPort1);
                previousNode = currentNode;
                StartTransitionAniamtion(transitionFixedTime);
            }
            this.speed = speed;
          
            currentNode = singleAnimationNode;
            currentNode.SetApplyFootIK(applyFootIK);
            if (!graph.IsPlaying())graph.Play();
        }
        /// <summary>
        /// 播放混合动画
        /// </summary>
        public void PlayBlendAnimation(List<AnimationClip> clips, float speed = 1, float transitionFixedTime = 0.25f, bool applyFootIK = false)
        {
            BlendAnimationNode blendAnimationNode = PoolSystem.GetObject<BlendAnimationNode>() ?? new BlendAnimationNode();
            if (currentNode == null)//首次播放
            {
                blendAnimationNode.Init(graph,mixer,clips,speed,inputPort0);
                mixer.SetInputWeight(inputPort0,1);
            }
            else
            {
                DestoryNode(previousNode);
                blendAnimationNode.Init(graph, mixer, clips, speed, inputPort1);
                previousNode= currentNode;
                StartTransitionAniamtion(transitionFixedTime);
            }
            this.speed = speed;
            currentNode = blendAnimationNode;
            currentNode.SetApplyFootIK(applyFootIK);
            if (!graph.IsPlaying()) graph.Play();
        }

        public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, float speed = 1, float transitionFixedTime = 0.25f, bool applyFootIK = false)
        {
            BlendAnimationNode blendAnimationNode = PoolSystem.GetObject<BlendAnimationNode>() ?? new BlendAnimationNode();
            if (currentNode == null)
            {
                blendAnimationNode.Init(graph, mixer, clip1,clip2, speed, inputPort0);
                mixer.SetInputWeight(inputPort0, 1);
            }
            else
            {
                DestoryNode(previousNode);
                blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort1);
                previousNode = currentNode;
                StartTransitionAniamtion(transitionFixedTime);
            }
            this.speed = speed;
           
            currentNode = blendAnimationNode;
            currentNode.SetApplyFootIK(applyFootIK);
            if (!graph.IsPlaying()) graph.Play();
        }

        private void StartTransitionAniamtion(float transitionFixedTime)
        {
           if (transitionCoroutine != null)this.StopCoroutine(transitionCoroutine);
            transitionCoroutine = this.StartCoroutine(TransitionAniamtion(transitionFixedTime));
        }

        private IEnumerator TransitionAniamtion(float transitionFixedTime)
        {
            // 交换端口号
            int temp = inputPort0;
            inputPort0 = inputPort1;
            inputPort1 = temp;

            // 硬切判断
            if (transitionFixedTime == 0)
            {
                mixer.SetInputWeight(inputPort0, 1);
                mixer.SetInputWeight(inputPort1, 0);
            }

            // 当前的权重
            float currentWeight = 1;
            float speed = 1 / transitionFixedTime;

            while (currentWeight > 0)
            {
                // 权重在减少
                currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
                mixer.SetInputWeight(inputPort1, currentWeight);  // 减少
                mixer.SetInputWeight(inputPort0, 1 - currentWeight); // 增加
                yield return null;
            }
            transitionCoroutine = null;
        }

        private void DestoryNode(AnimationNodeBase animationNode)
        {
            if (animationNode != null)
            {
                graph.Disconnect(mixer, animationNode.InputPort);
                animationNode.ObjectPushPool();
            }
        }

        public void SetBlendWeight(float clipWeight)
        {
            (currentNode as BlendAnimationNode).SetBlendWeight(clipWeight);
        }

        private void OnAnimatorMove()
        {
            rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
        }

        public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction = null)
        {
            this.rootMotionAction = rootMotionAction;
        }
        public void ClearRootMotionAction()
        {
            rootMotionAction = null;
        }
    }
}
