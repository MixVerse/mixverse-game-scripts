/* ========================================================
*      作 者：Lixi 
*      主 题：
*      主要功能：

*      详细描述：

*      创建时间：2022-03-23 12:10:05
*      修改记录：
*      版 本：1.0
 ========================================================*/
using Global;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public delegate void DGECommonUIClick(params object[] @params);

    /// <summary>
    /// 通用UI界面 生成带canvas
    /// </summary>
    public class CommonUIBase : MonoBehaviour
    {
        #region SerializeField

        [Header("货币文本")]
        [SerializeField] protected Text m_currencyText;

        [Header("内容文本")]
        [SerializeField] protected LanguageTool m_contentTxt;

        [Header("确定按钮")]
        [SerializeField] protected Button m_YBtn;

        [Header("取消按钮")]
        [SerializeField] protected Button m_NBtn;

        #endregion

        AudioSource m_soure;
        protected AudioSource Source { 
            get {
                if (m_soure == null)
                {
                    m_soure = gameObject.AddComponent<AudioSource>();
                    m_soure.playOnAwake = false;
                    m_soure.Stop();
                }
                return m_soure;
            }
        
        }

        List<object> @params;
        public void AddParams(params object[] @params)
        {
            if (this.@params == null)
            {
                this.@params = new List<object>();
            }
            for (int i = 0; i < @params.Length; i++)
            {
                this.@params.Add(@params[i]);
            }
        }

        /// <summary>
        /// 点击事件委托事件
        /// </summary>
        public event DGECommonUIClick OClick;

        #region 初始化

        /// <summary>
        /// 初始化UI
        /// </summary>
        public virtual void InitUI()
        {
            if (m_currencyText != null)
            {
                // 获取货币
                int zb = PlayerItemManager.Instance.GetZCurrency;
                //
                m_currencyText.text = zb.ToString();
            }
        }

        /// <summary>
        /// 初始化文本
        /// </summary>
        /// <param name="params"></param>
        public virtual void InitConteneText(params object[] @params)
        {
            if (m_contentTxt != null)
            {
                m_contentTxt.ChangeText(@params);
            }
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        public virtual void Init(params object[] @params)
        {
            InitUI();
            AddListener();
        } 

        #endregion

        #region 监听管理

        protected virtual void RemoveListener()
        {
            m_YBtn.onClick.RemoveAllListeners();
            m_NBtn.onClick.RemoveAllListeners();
        }

        protected virtual void AddListener()
        {
            m_YBtn.onClick.AddListener(ClickYEvent);
            m_NBtn.onClick.AddListener(ClickNEvent);
        }

        protected virtual void ClickNEvent()
        {
            //
            Source.Play();
            //
            Destroy(gameObject);
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        protected virtual void ClickYEvent()
        {
            //
            Source.Play();
            //
            if (OClick != null)
            {
                OClick(@params.ToArray());
                OClick = null;
            }
            //
            ClickNEvent();
        }

        #endregion

        #region 生命周期

        protected virtual void OnDestroy()
        {
            RemoveListener();
        } 

        #endregion
    } 
}
