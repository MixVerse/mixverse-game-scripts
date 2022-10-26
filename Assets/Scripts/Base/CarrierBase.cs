using Config;
using Mirror;
using UnityEngine;

namespace Base
{
    public abstract class CarrierBase : NetworkBehaviour
    {
        [Header("载具ID")]
        [SerializeField] protected string m_carrierId;

        [Header("位置")]
        [SerializeField] protected Transform[] m_seatPos;
       
        [Header("声音控制")]
        [SerializeField] protected AudioSource m_source;

        /// <summary>
        /// 是否可以进入
        /// </summary>
        public bool IsEnter { get; protected set; }

        /// <summary>
        /// 花费
        /// </summary>
        public int Price {
            get {
                return CarrierConfig.GetDataSingle(m_carrierId).Price;
            }
        }

        /// <summary>
        /// 运作
        /// </summary>
        public bool IsOperation { get; protected set; }

        /// <summary>
        /// 开始运作
        /// </summary>
        public abstract void StartOperation();

        #region 进入/离开

        /// <summary>
        /// 进入事件
        /// </summary>
        public abstract bool EnterEvent(Transform playerPos);

        /// <summary>
        /// 离开事件
        /// </summary>
        public abstract void LeaveEvent(); 

        #endregion

        #region 动画

        /// <summary>
        /// 动画管理
        /// </summary>
        protected abstract void AnimatorManager();

        /// <summary>
        /// 动画事件管理
        /// </summary>
        protected abstract void AnimationEventManager();

        #endregion
    } 
}
