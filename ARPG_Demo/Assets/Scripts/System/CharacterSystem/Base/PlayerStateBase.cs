using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkieEmpty.Animations;
using AkieEmpty.InputSystem;
using JKFrame;
using UnityEngine;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerStateBase:StateBase
    {
        protected PlayerController PlayerController {  get; private set; }
        protected PlayerView PlayerView { get; private set; }
        protected AnimationController AnimationController { get; private set; }
      
        protected CharacterConfig CharacterConfig { get; private set; }
        public override void Init(IStateMachineOwner owner)
        {
            PlayerController = owner as PlayerController;
            PlayerView = PlayerController.View;
            AnimationController = PlayerController.AnimationController;
            CharacterConfig = PlayerController.CharacterConfig;
        }
        protected AnimationClip GetAnimationByName(string clipName)
        {
            if (CharacterConfig.animationDic.TryGetValue(clipName, out AnimationClip animationClip))
            {
                return animationClip;
            }
            return null;
        }
    }
}
