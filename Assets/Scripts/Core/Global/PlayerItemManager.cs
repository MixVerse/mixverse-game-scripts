using Config;
using System;
using System.Collections.Generic;
using Tools;
using static Config.ItemConfig;

namespace Global
{
    /// <summary>
    /// 玩家物品管理
    /// </summary>
    public class PlayerItemManager : SingleMono<PlayerItemManager>
    {
        #region Z币管理

        /// <summary>
        /// Z币
        /// </summary>
        int ZCurrency;
        public int GetZCurrency { get => ZCurrency; }

        /// <summary>
        /// 改变货币Z
        /// </summary>
        /// <param name="consume">传入正数为增加</param>
        /// <returns>改变是否成功</returns>
        public bool ChangeZCurrency(int consume)
        {
            if (consume < 0 && Math.Abs(consume) > ZCurrency)
            {
                return false;
            }
            ZCurrency += consume;
            return true;
        }

        #endregion

        #region 金币管理

        /// <summary>
        /// Z币
        /// </summary>
        int GoldCurrency;
        public int GetGoldCurrency { get => GoldCurrency; }

        /// <summary>
        /// 改变货币Z
        /// </summary>
        /// <param name="consume">传入正数为增加</param>
        /// <returns>改变是否成功</returns>
        public bool ChangeGoldCurrency(int consume)
        {
            if (consume < 0 && Math.Abs(consume) > GoldCurrency)
            {
                return false;
            }
            GoldCurrency += consume;
            return true;
        }

        #endregion

        #region 物品管理

        /// <summary>
        /// 物品 key 物品类型 value -> key 物品id value 物品数量
        /// </summary>
        Dictionary<EItemType, Dictionary<string, int>> m_dicMyItem;

        /// <summary>
        /// 获取指定的类型的物品列表
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetAllItemToType(EItemType itemType)
        {
            if (m_dicMyItem != null)
            {
                return m_dicMyItem[itemType];
            }
            return null;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="count">物品数量</param>
        public void AddItem(string id, int count)
        {
            if (m_dicMyItem == null)
            {
                m_dicMyItem = new Dictionary<EItemType, Dictionary<string, int>>();
            }
            //
            EItemType type = GetData(id).Type;

            if (m_dicMyItem.ContainsKey(type))
            {
                if (m_dicMyItem[type].ContainsKey(id))
                {
                    m_dicMyItem[type][id] += count;
                    return;
                }
                m_dicMyItem[type].Add(id, count);
                return;
            }
            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic.Add(id, count);
            m_dicMyItem.Add(type, dic);
        }

        /// <summary>
        /// 扣除物品数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ChangeItem(string id, int value)
        {
            if (HaveItemJude(id))
            {
                //
                EItemType type = GetData(id).Type;
                //
                int num = m_dicMyItem[type][id];
                if (num >= value)
                {
                    m_dicMyItem[type][id] -= value;
                    if (m_dicMyItem[type][id] <= 0)
                    {
                        m_dicMyItem[type].Remove(id);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItem(string id)
        {
            if (HaveItemJude(id))
            {
                EItemType type = GetData(id).Type;
                return m_dicMyItem[type][id];
            }
            return 0;
        }

        /// <summary>
        /// 判断是否拥有物品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HaveItemJude(string id)
        {
            if (m_dicMyItem == null)
            {
                return false;
            }
            EItemType type = GetData(id).Type;
            if (m_dicMyItem.ContainsKey(type))
            {
                if (m_dicMyItem[type].ContainsKey(id))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否满
        /// </summary>
        /// <param name="id">物品Id</param>
        /// <param name="maxStack">返回最大叠加数</param>
        /// <returns>仅当数量大于最大叠加返回true</returns>
        public bool MaxStackJudge(string id, out int maxStack)
        {
            maxStack = GetData(id).MaxStack;
            int have = GetItem(id);

            if (have > maxStack)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 武器管理

        List<string> m_weapoinLt;

        public void AddWeapoin(string id)
        {
            if (m_weapoinLt == null)
            {
                m_weapoinLt = new List<string>();
            }
            if (!m_weapoinLt.Contains(id))
            {
                m_weapoinLt.Add(id);
            }
        }

        public void RemoveWeapoin(string id)
        {
            m_weapoinLt.Remove(id);
        } 

        public bool HaveWeapoin(string id)
        {
            if (m_weapoinLt != null && m_weapoinLt.Contains(id))
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 初始化物品
        /// </summary>
        /// <param name="items"></param>
        public void InitItem(Dictionary<string, int> items)
        {
            foreach (var item in items)
            {
                string id = item.Key;
                int count = item.Value;
                AddItem(id, count);
            }
        }

        public void InitWeapoin(string[] weapoinIds)
        {

        }


        public void InitData()
        {

        }
    }
}