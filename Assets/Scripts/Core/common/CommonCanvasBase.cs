using Global;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public delegate void DTECommonCanvas(params object[] @params);

    public class CommonCanvasBase : MonoBehaviour
    {
        #region SerializeField

        [Header("确定按钮")]
        [SerializeField] Button m_btnY;

        [Header("取消按钮")]
        [SerializeField] Button m_btnN;

        [Header("拥有货币")]
        [SerializeField] Text m_currencyHave;

        [Header("提示文字")]
        [SerializeField] LanguageTool m_txtHint; 

        #endregion

        /// <summary>
        /// 委托事件
        /// </summary>
        public event DTECommonCanvas ODte;

        object[] @params;

        #region 生命周期

        protected virtual void Awake()
        {
            InitData();
        }

        protected virtual void OnDestroy()
        {
            RemoveListener();
        }

        #endregion

        #region 按钮事件

        public virtual void AddListener()
        {
            m_btnN.onClick.AddListener(ClickNEvent);
            m_btnY.onClick.AddListener(ClickYEvent);
        }

        public virtual void RemoveListener()
        {
            m_btnN.onClick.RemoveAllListeners();
            m_btnY.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        public virtual void ClickNEvent()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        public virtual void ClickYEvent()
        {
            if (ODte != null)
            {
                ODte(@params);
                ODte = null;
                @params = null;
            }

            Destroy(gameObject);
        }

        #endregion

        /// <summary>
        /// 处理提示文本
        /// </summary>
        /// <param name="params"></param>
        public virtual void InitHintText(params object[] @params)
        {
            m_txtHint.ChangeText(@params);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        protected virtual void InitData()
        {
            m_currencyHave.text = PlayerItemManager.Instance.GetZCurrency.ToString();
            AddListener();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="params"></param>
        public void GetParams(params object[] @params)
        {
            this.@params = @params;
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ClickYEvent();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                ClickNEvent();
            }
        }

#endif
    } 
}
