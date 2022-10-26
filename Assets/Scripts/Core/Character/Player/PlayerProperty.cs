using UnityEngine;

namespace Character
{
    /// <summary>
    /// 人物属性
    /// </summary>
    public class PlayerProperty : CharacterBase
    {
        [SerializeField] Transform m_transCanvas;

        [Header("关联的人物脚本")]
        [SerializeField] Player m_player;
        public Player GetPlayer()
        {
            return m_player;
        }

        [Header("摄像机")]
        [SerializeField] Camera m_cameraTrans;
        /// <summary>
        /// 摄像机
        /// </summary>
        public Camera GetCamera { get => m_cameraTrans; }

        [Header("临时画布")]
        public Transform CanvasTemporary;

        #region 人物通用动作重写

        public override void GetDmg(CharacterBase target)
        {
        }

        protected override void Move(params object[] @params)
        {
            StartCoroutine(m_player.IE_Move());
        }

        public void StopMove(bool stop = true)
        {
            m_player.whetherOpenMove = !stop;
        } 

        #endregion

        protected override void InitData()
        {
        }

        #region 对物品的操作

        /// <summary>
        /// 生成物品在手上
        /// </summary>
        /// <param name="obj">要生成的物品</param>
        /// <param name="right">生成在左手</param>
        public void CreateItemInHand(GameObject obj, bool right = true)
        {
            m_player.CreateItemInHand(obj, right);
        }

        /// <summary>
        /// 放下物品
        /// </summary>
        /// <param name="right">是否右手</param>
        /// <param name="isDestroy">是否销毁</param>
        public void ItemInHandDestroy()
        {
            m_player.ItemInHandDestroy();
        } 

        #endregion

        protected override void Awake()
        {
            Move();
            base.Awake();
        }
    }
}