using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AkieEmpty.CharacterSystem
{
    [CreateAssetMenu(menuName = "Config/Character/CharacterConfig", fileName = "CharacterConfig")]
    public class CharacterConfig : ConfigBase
    {
        [LabelText("移动速度")]public float moveSpeed;
        [LabelText("旋转速度")] public float rotateSpeed;
        [LabelText("启用根运动")] public bool applyRootMotionForMove;
        public Dictionary<string, AnimationClip> animationDic = new Dictionary<string, AnimationClip>();
    }
   
}
