using UnityEngine;

namespace Character
{
    /// <summary>
    /// ��������
    /// </summary>
    public class PlayerProperty : CharacterBase
    {
        [SerializeField] Transform m_transCanvas;

        [Header("����������ű�")]
        [SerializeField] Player m_player;
        public Player GetPlayer()
        {
            return m_player;
        }

        [Header("�����")]
        [SerializeField] Camera m_cameraTrans;
        /// <summary>
        /// �����
        /// </summary>
        public Camera GetCamera { get => m_cameraTrans; }

        [Header("��ʱ����")]
        public Transform CanvasTemporary;

        #region ����ͨ�ö�����д

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

        #region ����Ʒ�Ĳ���

        /// <summary>
        /// ������Ʒ������
        /// </summary>
        /// <param name="obj">Ҫ���ɵ���Ʒ</param>
        /// <param name="right">����������</param>
        public void CreateItemInHand(GameObject obj, bool right = true)
        {
            m_player.CreateItemInHand(obj, right);
        }

        /// <summary>
        /// ������Ʒ
        /// </summary>
        /// <param name="right">�Ƿ�����</param>
        /// <param name="isDestroy">�Ƿ�����</param>
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