/* ========================================================
*      作 者：Lixi 
*      主 题：
*      主要功能：

*      详细描述：

*      创建时间：2022-03-23 12:10:05
*      修改记录：
*      版 本：1.0
 ========================================================*/
using Character;
using Common.UI;
using Tools;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// 进入基类
    /// </summary>
    public abstract class EnterBase : MonoBehaviour
    {
        [Header("关联的载具")]
        [SerializeField] protected CarrierBase m_carrier;

        [Header("附带的提示画布")]
        [SerializeField] CommonUIBase m_canvas;

        [Header("提示画布距离")]
        [SerializeField] float m_distance = 4;

        [Header("提示画布高度")]
        [SerializeField] float m_yPos = 2;

        /// <summary>
        /// 批准进入
        /// </summary>
        /// <param name="playerPos">玩家预制体</param>
        protected virtual void EnterPermit(params object[] @params)
        {
            Transform playerPos = (Transform)@params[0];
            m_carrier.EnterEvent(playerPos);
        }

        /// <summary>
        /// 进入管理
        /// </summary>
        /// <param name="playerTrans"></param>
        protected virtual void EnterManager(PlayerProperty playerTrans)
        {
            // 获取关联信息
            if (m_carrier.IsEnter && !m_carrier.IsOperation) // 2. 可进入
            {
                // 生成提示画布
                Transform cameraTrans = playerTrans.GetCamera.transform;
                //
                GameObject canvas = 
                    UICreate.CanvasCreate(cameraTrans, m_canvas.gameObject, m_distance, m_yPos);
                //
                canvas.GetComponent<Canvas>().worldCamera = playerTrans.GetCamera; 
                //
                if (canvas.TryGetComponent(out CommonUIBase common))
                {
                    Transform trans = playerTrans.transform;
                    DataCanvasRegister(common, trans);
                }
                return;
            }
            // 1. 不可进入
        }

        /// <summary>
        /// 请求进入
        /// </summary>
        /// <param name="playerTrans">玩家预制体</param>
        public void EnterResponse(Transform playerTrans)
        {
            if (playerTrans.TryGetComponent(out PlayerProperty player))
            {
                EnterManager(player);
            }
        }

        #region 画布

        ///// <summary>
        ///// 生成提示画布
        ///// </summary>
        ///// <param name="cameraTrans">玩家摄像机</param>
        ///// <returns></returns>
        //protected virtual Transform CanvasCreate(Transform cameraTrans)
        //{
        //    // 生成画布
        //    Transform hintCanvas = Instantiate(m_canvas.transform);
        //    hintCanvas.gameObject.SetActive(true);
        //    hintCanvas.position = Vector3.zero;
        //    hintCanvas.rotation = Quaternion.identity;
        //    //
        //    Vector3 pos =
        //        cameraTrans.transform.position +
        //        cameraTrans.transform.forward * m_distance;
        //    //
        //    Quaternion q2 = Quaternion.LookRotation(cameraTrans.transform.forward);

        //    hintCanvas.transform.position = new Vector3(pos.x, m_yPos, pos.z);
        //    hintCanvas.transform.rotation = new Quaternion(0, q2.y, 0, q2.w);

        //    return hintCanvas;
        //}

        /// <summary>
        /// 注册画布信息
        /// </summary>
        /// <param name="canvas"></param>
        protected abstract void DataCanvasRegister(params object[] @params); 

        #endregion
    }
}
