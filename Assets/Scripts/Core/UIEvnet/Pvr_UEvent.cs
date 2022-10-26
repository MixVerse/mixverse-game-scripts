using UnityEngine;

namespace PvrEvent
{
    public class Pvr_UEvent : MonoBehaviour
    {
        public EHandEvent handEvent;


        /// <summary>
        /// true: 当前指向UI 
        /// </summary>
        bool bo_forUI;

        /// <summary>
        /// true: 当前指向UI 
        /// </summary>
        public bool BForUI { get => bo_forUI; }

        /// <summary>
        /// 当前指向UI的预制件
        /// </summary>
        GameObject m_current;

        /// <summary>
        /// 当前指向UI的预制件
        /// </summary>
        public GameObject Current
        {
            get
            {
                if (bo_forUI)
                {
                    return m_current;
                }
                return null;
            }
        }

        //private void Awake()
        //{
        //    if (TryGetComponent(out Pvr_UIPointer pointer))
        //    {
        //        pointer.UIPointerElementEnter += UIPointerElementEnter;
        //        pointer.UIPointerElementExit += UIPointerElementExit;
        //    }
        //}

        //private void UIPointerElementExit(object sender, UIPointerEventArgs e)
        //{
        //    bo_forUI = false;
        //    m_current = null;
        //}

        //private void UIPointerElementEnter(object sender, UIPointerEventArgs e)
        //{
        //    bo_forUI = true;
        //    if (m_current != e.currentTarget)
        //    {
        //        m_current = e.currentTarget;
        //    }
        //    Debug.Log(e.currentTarget.name);
        //}
    } 
}
