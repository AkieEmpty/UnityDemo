using System;
using AkieEmpty.SkillRuntime;
using UnityEngine;

namespace AkieEmpty.Animations
{
    /// <summary>
    /// 技能播放器
    /// </summary>
    public class SkillPlayer : MonoBehaviour
    {
        [SerializeField] AnimationController animationController;
        private SkillConfig skillConfig;    // 当前播放的技能配置
        private Action skillEndAction;
        private Action<Vector3, Quaternion> rootMotionAction;
        private int currentFrameIndex;      // 当前是第几帧
        private float playTotalTime;        // 当前播放的总时间
        private int frameRote;              // 当前技能的帧率
        public bool IsPlaying { get; private set; } = false;
        private void OnEnable()
        {
        }
        private void OnDisable()
        {
            
        }  
        public void PlaySkill(SkillConfig skillConfig,Action skillEndAction=null,Action<Vector3,Quaternion> rootMotionAction = null)
        {
            this.skillConfig = skillConfig;
            this.skillEndAction = skillEndAction;
            this.rootMotionAction = rootMotionAction;
            currentFrameIndex = -1;
            playTotalTime = 0;
            frameRote = skillConfig.frameRote;
            IsPlaying=true;
            TickSkill();
        }
        private void TickSkill()
        {
            if (animationController == null) return;
            currentFrameIndex += 1;
            //驱动动画
            if (skillConfig.skillAnimationData.FrameDataDic.TryGetValue(currentFrameIndex, out SkillAnimationEvent animationData))
            {
                if(animationData.applyRootMotion) animationController.SetRootMotionAction(rootMotionAction);
                else animationController.ClearRootMotionAction();

                animationController.PlaySingleAniamtion(animationData.animationClip,1,0.25F,false,true);
            }
        }
        private void Update()
        {
            if(IsPlaying)
            {
                playTotalTime += Time.deltaTime;//播放总时长                     
                int targetFrameIndex = (int)(playTotalTime * frameRote);  // 根据总时间判断当前是第几帧
                                                                          
                while (currentFrameIndex < targetFrameIndex)// 防止一帧延迟过大，追帧
                {
                    // 驱动一次技能
                    TickSkill();
                }
                // 如果到达最后一帧，技能结束
                if (targetFrameIndex >= skillConfig.maxFrameCount)
                {
                    IsPlaying = false;
                    skillConfig = null;
                    if (rootMotionAction != null) animationController.ClearRootMotionAction();
                    rootMotionAction = null;
                    skillEndAction?.Invoke();
                }
            }
        }
    }
}
