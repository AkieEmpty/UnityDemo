using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKFrame;

namespace AkieEmpty.CharacterSystem
{
    public class PlayerStateBase:StateBase
    {
        protected PlayerController playerController;

        public override void Init(IStateMachineOwner owner)
        {
            playerController = owner as PlayerController;
        }
    }
}
