using UnityEngine;

namespace PvrEvent
{
    public class Pvr_UEvent : MonoBehaviour
    {
        public EHandEvent handEvent;


        /// <summary>
        /// true: ��ǰָ��UI 
        /// </summary>
        bool bo_forUI;

        /// <summary>
        /// true: ��ǰָ��UI 
        /// </summary>
        public bool BForUI { get => bo_forUI; }

        /// <summary>
        /// ��ǰָ��UI��Ԥ�Ƽ�
        /// </summary>
        GameObject m_current;

        /// <summary>
        /// ��ǰָ��UI��Ԥ�Ƽ�
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
