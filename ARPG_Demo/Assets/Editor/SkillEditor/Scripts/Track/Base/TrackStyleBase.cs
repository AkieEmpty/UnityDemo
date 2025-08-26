using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkieEmpty.SkillEditor
{
    /// <summary>
    /// ���ܹ����ʽ����
    /// </summary>
    public abstract class TrackStyleBase
    {
        public Label titleLabel;
        #region �����ڵ�
        public VisualElement trackMenuParent;
        public VisualElement trackContentParent;
        #endregion
        #region ������ڵ�
        public VisualElement trackMenuRoot;
        public VisualElement trackContentRoot;

        /// <summary>
        /// ���һ�����Item
        /// </summary>
        public virtual void AddItem(VisualElement ve)
        {
            trackContentRoot.Add(ve);
        }
        /// <summary>
        /// �Ƴ�һ�����Item
        /// </summary>
        public virtual void RemoveItem(VisualElement ve)
        {
            trackContentRoot.Remove(ve);        
        }
        /// <summary>
        /// �Ƴ����й��Item
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
