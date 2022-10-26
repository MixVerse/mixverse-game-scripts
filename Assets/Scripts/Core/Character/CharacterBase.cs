using UnityEngine;
using State;
using System.Collections;
using Factory;
using Mirror;

namespace Character
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum ECharacterType
    {
        none,
        player,
        npc,
        enemy,
        Character = player | npc | enemy
    }

    /// <summary>
    /// 角色立场
    /// </summary>
    public enum EStandpoint
    {
        /// <summary>
        /// 敌对
        /// </summary>
        hostility,

        /// <summary>
        /// 中立
        /// </summary>
        neutrality,

        /// <summary>
        /// 友善
        /// </summary>
        friendly
    }

    /// <summary>
    /// 角色基类
    /// </summary>
    public abstract class CharacterBase : NetworkBehaviour
    {
        [SerializeField] protected Animator m_animator;

        //
        protected BuffManager m_buffs;
        //
        protected SkillManager m_skills;

        #region 属性相关

        /// <summary>
        /// 人物Id
        /// </summary>
        [Header("人物Id")]
        public string m_Id;

        /// <summary>
        /// 人物类型
        /// </summary>
        [HideInInspector] public ECharacterType ECARType;

        /// <summary>
        /// 生命值
        /// </summary>
        [HideInInspector] public int Hp;

        /// <summary>
        ///  最大生命值
        /// </summary>
        protected int maxHp;

        /// <summary>
        /// 法力值
        /// </summary>
        [HideInInspector] public int Mp;

        /// <summary>
        /// 最大法力值
        /// </summary>
        protected int maxMp;

        protected int maxEnergy;

        public int Energy;


        /// <summary>
        /// 攻击力
        /// </summary>
        [HideInInspector]  public int attack;

        /// <summary>
        /// 攻击间隔
        /// </summary>
        protected float attackCD;

        /// <summary>
        /// 移动速度
        /// </summary>
        protected int speed;

        /// <summary>
        /// 检查范围
        /// </summary>
        protected float range;

        /// <summary>
        /// 名字
        /// </summary>
        [HideInInspector]  public string m_name;

        /// <summary>
        /// 名字多语言key
        /// </summary>
        protected string nameKey;

        /// <summary>
        /// 是否死亡
        /// </summary>
        protected bool m_bInDeath = false;

        #endregion

        /// <summary>
        /// 初始化角色
        /// </summary>
        protected abstract void InitData();

        #region 人物通用动作

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="target">攻击目标对象</param>
        public virtual void GetDmg(CharacterBase target)
        {
            //
            target.GetHurt(this, attack);
        }

        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="target">攻击者</param>
        /// <param name="dmg">收到的伤害</param>
        public virtual void GetHurt(CharacterBase target, int dmg, bool isDestroy = true)
        {
            if (m_bInDeath)
            {
                return;
            }
            
            if (Hp <= 0)
            {
                Death(target, isDestroy);
                return;
            }
            Hp -= dmg;
            string Hurt = AnimatorCharacterStateName.Hurt;
            m_animator.SetTrigger(Hurt);
        }

        /// <summary>
        /// 死亡
        /// </summary>
        /// <param name="isDestroy"> 死亡后是否销毁 默认：否</param>
        public virtual void Death(CharacterBase target, bool isDestroy)
        {
            m_bInDeath = true;
            string Death = AnimatorCharacterStateName.Death;
            m_animator.SetBool(Death, m_bInDeath);
            StartCoroutine(IDeathCount(isDestroy));
        }

        /// <summary>
        /// 移动
        /// </summary>
        protected abstract void Move(params object[] @params);

        #endregion

        #region 添加buff

        public void AddBuff(string id)
        {
            m_buffs.AddBuff(id, this);
        }

        #endregion

        protected virtual void Awake()
        {
            m_buffs = new BuffManager();
            m_skills = new SkillManager();
            InitData();
        }

        protected virtual void Update()
        {
            if (m_skills != null)
            {
                m_skills.Update(this);
            }
        }

        IEnumerator IDeathCount(bool isDestroy)
        {
            yield return new WaitForSeconds (1);
            if (isDestroy)
            {
                Destroy(gameObject);
                yield break;
            }
            gameObject.SetActive(false);
        }
    } 
}
