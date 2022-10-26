using AmusementPark;
using Config;
using DG.Tweening;
using Factory;
using System.Collections;
using UnityEngine;
using static Config.EnemyConfig;

namespace Character
{
    public class OrdinaryDragon : CharacterBase
    {
        Enemy m_configEnemy;

        /// <summary>
        /// �ƶ�����
        /// </summary>
        EMoveType moveType;

        /// <summary>
        /// ����λ��
        /// </summary>
        Vector3 localPosition;

        /// <summary>
        /// ����Э��
        /// </summary>
        Coroutine cueMove;

        /// <summary>
        /// ����ƶ�
        /// </summary>
        bool m_finishMove = false;

        string m_powerId;

        #region ��д---ͨ�ö���

        public override void Death(CharacterBase target, bool isDestroy)
        {
            // ��������
            transform.DOKill();
            //
            base.Death(target, isDestroy);
            //�ر�Я��
            if (cueMove != null)
            {
                StopCoroutine(cueMove);
                cueMove = null;
            }
            int score = WaterGunFightConfig.GetEnemyConfig(m_configEnemy.Id).Score;
            WaterGameManager.Instance.GetScore(score);
        }

        protected override void Move(params object[] @params)
        {
            float infall = (float)@params[0];
            m_finishMove = false;
            // buff ����
            int speed = this.speed;
            //
            if (m_buffs != null)
            {
                speed = m_buffs.ValueDispose(EPart.DEX, this.speed);
            }
            // �³�
            transform.DOLocalMoveY(infall, speed / 2f).OnComplete(() =>
            {
                // ������ Idle
                Tween tween = transform.DOLocalMoveY(infall - 0.03f, 0.25f);
                tween.SetLoops(2, LoopType.Yoyo);
                tween.OnComplete(() => { transform.DOKill(); m_finishMove = true; });

            });
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="target">����Ŀ�����</param>
        public override void GetDmg(CharacterBase target)
        {
        }

        #endregion

        #region ��ʼ��

        protected override void InitData()
        {
            InitProperty();
            InitPower();
            //
            localPosition = transform.localPosition;
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        private void InitProperty()
        {
            m_configEnemy = GetAll(m_Id);
            //
            ECARType = ECharacterType.enemy;
            //
            maxHp = m_configEnemy.MaxHp;
            Hp = maxHp;
            //
            attack = m_configEnemy.ATK;
            attackCD = m_configEnemy.ATKCD;
            speed = m_configEnemy.SPD;
            moveType = m_configEnemy.MoveType;
            range = m_configEnemy.Range;
            //
            m_name = m_configEnemy.Name;
            nameKey = m_configEnemy.NameKey;
            //
            m_powerId = m_configEnemy.Power;
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        private void InitPower()
        {
            if (!string.IsNullOrEmpty(m_powerId) &&
                !m_powerId.Equals("null")
            ) {
                m_skills.AddSkill(m_powerId);
            }
        } 

        #endregion

        private void OnEnable()
        {
            Hp = maxHp;
            transform.localPosition = localPosition;
            if (moveType == EMoveType.inFloat)
            {
                cueMove = StartCoroutine(IStartMove());
            }
        }

        protected override void Update()
        {
            m_skills.USESKill(m_powerId, this);
            base.Update();
        }

        /// <summary>
        /// ���¸���Я��
        /// </summary>
        /// <returns></returns>
        IEnumerator IStartMove()
        {
            while (!m_bInDeath)
            {
                //
                float infall = localPosition.y - 0.15f;
                Move(infall);
                yield return new WaitUntil(() => m_finishMove);
                //
                infall = localPosition.y + 0.15f;
                Move(infall);
                yield return new WaitUntil(() => m_finishMove);
            }
        }
    }
}
