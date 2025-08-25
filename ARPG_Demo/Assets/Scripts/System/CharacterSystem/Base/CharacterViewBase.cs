using System;
using System.Collections.Generic;
using AkieEmpty.Animations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public abstract class CharacterViewBase : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationController animatonController;
        public AnimationController AnimationController => animatonController;
        private CharacterConfig characterConfig;
    


        
        public virtual void Init(CharacterConfig characterConfig)
        {
            this.characterConfig = characterConfig;
            animatonController.Init();
            
        }

       
       
       
        
    }
}
