using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AkieEmpty.CharacterSystem
{
    [CreateAssetMenu(menuName = "Config/Character/CharacterConfig", fileName = "CharacterConfig")]
    public class CharacterConfig : ConfigBase
    {
        [LabelText("�����ٶ�")]public float walkSpeed;
        [LabelText("�����ٶ�")]public float runSpeed;
        [LabelText("��ת�ٶ�")] public float rotateSpeed;
        [LabelText("����")] public float gravity;
        [LabelText("��·�뱼�ܵĹ����ٶ�")] public float walkAndRunTransitionSpeed;
        [LabelText("���ø��˶�")] public bool applyRootMotionForMove;
        [LabelText("����")] public Dictionary<string, AnimationClip> animationDic = new Dictionary<string, AnimationClip>();
       

       
    }
   
}
