using System.Collections.Generic;
using AkieEmpty.Animations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public abstract class CharacterViewBase : SerializedMonoBehaviour
    {
        [SerializeField] protected AnimatonController animatonController;
        public virtual void Init()
        {
            animatonController.Init();
        }

        public void PlayAnimation(AnimationClip clip)
        {
            animatonController.PlaySingleAniamtion(clip);
        }
    }
}
