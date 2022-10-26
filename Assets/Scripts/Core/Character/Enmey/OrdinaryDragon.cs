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
        /// 移动类型
        /// </summary>
        EMoveType moveType;

        /// <summary>
        /// 本地位置
        /// </summary>
        Vector3 localPosition;

        /// <summary>
        /// 浮动协程
        /// </summary>
        Coroutine cueMove;

        /// <summary>
        /// 完成移动
        /// </summary>
        bool m_finishMove = false;

        string m_powerId;

        #region 重写---通用动作

        public override void Death(CharacterBase target, bool isDestroy)
        {
            // 结束动画
            transform.DOKill();
            //
            base.Death(target, isDestroy);
            //关闭携程
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
            // buff 处理
            int speed = this.speed;
            //
            if (m_buffs != null)
            {
                speed = m_buffs.ValueDispose(EPart.DEX, this.speed);
            }
            // 下沉
            transform.DOLocalMoveY(infall, speed / 2f).OnComplete(() =>
            {
                // 结束后 Idle
                Tween tween = transform.DOLocalMoveY(infall - 0.03f, 0.25f);
                tween.SetLoops(2, LoopType.Yoyo);
                tween.OnComplete(() => { transform.DOKill(); m_finishMove = true; });

            });
        }

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="target">攻击目标对象</param>
        public override void GetDmg(CharacterBase target)
        {
        }

        #endregion

        #region 初始化

        protected override void InitData()
        {
            InitProperty();
            InitPower();
            //
            localPosition = transform.localPosition;
        }

        /// <summary>
        /// 初始化属性
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
        /// 初始化怪物能力
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
        /// 上下浮动携程
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
