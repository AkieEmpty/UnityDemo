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
            graph = PlayableGraph.Create(nameof(AnimationController));//����ͼ
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);//����ͼ��ʱ��ģʽ
            mixer = AnimationMixerPlayable.Create(graph,3);//���������
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "PlayableOutput", animator);//���������
            playableOutput.SetSourcePlayable(mixer);//���ӻ����
        }

        /// <summary>
        /// ���ŵ�������
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
                SingleAnimationNode previousAnimationNode = currentNode as SingleAnimationNode; // ��һ���ڵ�
                //��ͬ�Ķ������Ҳ���Ҫˢ��ʱ
                if (!refreshAnimation && previousAnimationNode != null && previousAnimationNode.GetAnimationClip() == clip) return;
                // ���ٵ���ǰ���ܱ�ռ�õ�Node
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
        /// ���Ż�϶���
        /// </summary>
        public void PlayBlendAnimation(List<AnimationClip> clips, float speed = 1, float transitionFixedTime = 0.25f, bool applyFootIK = false)
        {
            BlendAnimationNode blendAnimationNode = PoolSystem.GetObject<BlendAnimationNode>() ?? new BlendAnimationNode();
            if (currentNode == null)//�״β���
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
            // �����˿ں�
            int temp = inputPort0;
            inputPort0 = inputPort1;
            inputPort1 = temp;

            // Ӳ���ж�
            if (transitionFixedTime == 0)
            {
                mixer.SetInputWeight(inputPort0, 1);
                mixer.SetInputWeight(inputPort1, 0);
            }

            // ��ǰ��Ȩ��
            float currentWeight = 1;
            float speed = 1 / transitionFixedTime;

            while (currentWeight > 0)
            {
                // Ȩ���ڼ���
                currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
                mixer.SetInputWeight(inputPort1, currentWeight);  // ����
                mixer.SetInputWeight(inputPort0, 1 - currentWeight); // ����
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
