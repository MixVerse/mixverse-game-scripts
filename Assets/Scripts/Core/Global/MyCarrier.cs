
using Config;
using System.Collections.Generic;
using Tools;

namespace Global
{
    public class MyCarrier : SingleMono<MyCarrier>
    {
        /// <summary>
        /// 字典 记录玩家资产
        /// key: 载具Id
        /// value：载具使用权：次数，-1为永久
        /// </summary>
        Dictionary<string, int> m_dicMyAsset;

        /// <summary>
        /// 添加资产
        /// </summary>
        /// <param name="carrierId"></param>
        public void AddAssets(string carrierId)
        {
            if (m_dicMyAsset == null)
            {
                m_dicMyAsset = new Dictionary<string, int>();
            }

            if (m_dicMyAsset.ContainsKey(carrierId))
            {
                m_dicMyAsset[carrierId] += CarrierConfig.GetDataSingle(carrierId).Ownership;
            }
            else
            {
                int Ownership = CarrierConfig.GetDataSingle(carrierId).Ownership;
                m_dicMyAsset.Add(carrierId, Ownership);
            }
        }

        /// <summary>
        /// 使用资产
        /// </summary>
        /// <param name="carrierId"></param>
        public void UseAssets(string carrierId)
        {
            if (m_dicMyAsset.ContainsKey(carrierId))
            {
                m_dicMyAsset[carrierId] -= 1;
                if (m_dicMyAsset[carrierId] <= 0)
                {
                    RemoveAssets(carrierId);
                }
            }
        }

        /// <summary>
        /// 移除资产
        /// </summary>
        /// <param name="carrierId"></param>
        public void RemoveAssets(string carrierId)
        {
            if (m_dicMyAsset.ContainsKey(carrierId))
            {
                m_dicMyAsset.Remove(carrierId);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            m_dicMyAsset = new Dictionary<string, int>();
        }
    } 
}
