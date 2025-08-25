using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AkieEmpty.CharacterSystem
{
    [CreateAssetMenu(menuName = "Config/Character/CharacterConfig", fileName = "CharacterConfig")]
    public class CharacterConfig : ConfigBase
    {
        [LabelText("行走速度")]public float walkSpeed;
        [LabelText("奔跑速度")]public float runSpeed;
        [LabelText("旋转速度")] public float rotateSpeed;
        [LabelText("重力")] public float gravity;
        [LabelText("走路与奔跑的过渡速度")] public float walkAndRunTransitionSpeed;
        [LabelText("启用根运动")] public bool applyRootMotionForMove;
        [LabelText("动画")] public Dictionary<string, AnimationClip> animationDic = new Dictionary<string, AnimationClip>();
       

       
    }
   
}
