using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AkieEmpty.CharacterSystem
{
    [CreateAssetMenu(menuName = "Config/Character/CharacterConfig", fileName = "CharacterConfig")]
    public class CharacterConfig : ConfigBase
    {
        [LabelText("�ƶ��ٶ�")]public float moveSpeed;
        [LabelText("��ת�ٶ�")] public float rotateSpeed;
        [LabelText("���ø��˶�")] public bool applyRootMotionForMove;
        public Dictionary<string, AnimationClip> animationDic = new Dictionary<string, AnimationClip>();
    }
   
}
