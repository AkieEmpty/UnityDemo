using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// 技能轨道样式基类
    /// </summary>
    public abstract class TrackStyleBase
    {
        public Label titleLabel;
        #region 自身父节点
        public VisualElement trackMenuParent;
        public VisualElement trackContentParent;
        #endregion
        #region 自身根节点
        public VisualElement trackMenuRoot;
        public VisualElement trackContentRoot;

        /// <summary>
        /// 添加一个轨道Item
        /// </summary>
        public virtual void AddItem(VisualElement ve)
        {
            trackContentRoot.Add(ve);
        }
        /// <summary>
        /// 移除一个轨道Item
        /// </summary>
        public virtual void RemoveItem(VisualElement ve)
        {
            trackContentRoot.Remove(ve);        
        }
        /// <summary>
        /// 移除所有轨道Item
        /// </summary>
        public virtual void RemoveAllItem()
        {
            trackContentRoot.Clear();
        }

        #endregion

        public virtual void Destory()
        {
            if(trackMenuRoot!=null)trackMenuParent.Remove(trackMenuRoot);
            if(trackContentRoot!=null)trackContentParent.Remove(trackContentRoot);
        }

    }
}
