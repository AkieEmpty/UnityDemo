using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKFrame;

namespace AkieEmpty.Animations
{
    /// <summary>
    /// 动画节点基类
    /// </summary>
    public abstract class AnimationNodeBase
    {
        public int InputPort { get; protected set; }
        public abstract void SetSpeed(float speed);
        public abstract void SetApplyFootIK(bool value = false);
        public virtual void Destory()
        {
            this.ObjectPushPool();
        }
    }
}
