using Global;
using LitJson;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Config
{
    public class CarrierConfig
    {
        public struct Carrier_config
        {
            /// <summary>
            /// 载具Id		
            /// </summary>
            public string Id;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 简介
            /// </summary>
            public string Introduce;

            /// <summary>
            /// 名称多语言
            /// </summary>
            public string NameKey;

            /// <summary>
            /// 简介多语言
            /// </summary>
            public string IntroduceKey;

            /// <summary>
            /// 购买
            /// </summary>
            public bool IsBuy;

            /// <summary>
            /// 售价
            /// </summary>
            public int Price;

            /// <summary>
            /// 使用次数
            /// </summary>
            public int Ownership;

            /// <summary>
            /// 最大乘坐人数
            /// </summary>
            public int MaxMember;

            public Carrier_config (
                string Id, string Name, string Introduce,
                string NameKey, string IntroduceKey,
                bool IsBuy, int Price, int Ownership, int MaxMember
            ) {
                this.Id = Id;
                this.Name = Name;
                this.Introduce = Introduce;
                this.NameKey = NameKey;
                this.IntroduceKey = IntroduceKey;
                this.IsBuy = IsBuy;
                this.Price = Price;
                this.Ownership = Ownership;
                this.MaxMember = MaxMember;
            }
        }

        static Dictionary<string, Carrier_config> ConfigDic;

        public static void StartLoading(string josnName)
        {
            ConfigDic = new Dictionary<string,Carrier_config>();

            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            Carrier_config config;
            //
            foreach (JsonData json in jsonData)
            {
                string Id = json["Id"].ToString();
                string Name = json["Name"].ToString();
                string Introduce = json["Introduce"].ToString();
                string NameKey = json["NameKey"].ToString();
                string IntroduceKey = json["IntroduceKey"].ToString();
                string IsBuy = json["IsBuy"].ToString();
                string Price = json["Pice"].ToString();
                string Ownership = json["Ownership"].ToString();
                string MaxMember = json["MaxMember"].ToString();

                // 数值处理
                InforValue.IntStrForBool(IsBuy, out bool isBuy);
                int.TryParse(Price, out int price);
                int.TryParse(Ownership, out int ownership);
                int.TryParse(MaxMember, out int maxMember);

                if (!ConfigDic.ContainsKey(Id))
                {
                    config = 
                        new Carrier_config (Id, Name, Introduce, NameKey, IntroduceKey, isBuy, price, ownership, maxMember);
                    ConfigDic.Add(Id, config);
                }
            }
        }

        /// <summary>
        /// 获取单个所有数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Carrier_config GetDataSingle(string id)
        {
            if (ConfigDic.ContainsKey(id))
            {
                return ConfigDic[id];
            }

            return default;
        }

        /// <summary>
        /// 获取最大人数
        /// </summary>
        /// <param name="id"></param>
        /// <returns>失败返回 -1 </returns>
        public static int GetMaxMember(string id)
        {
            if (ConfigDic.ContainsKey(id))
            {
                return ConfigDic[id].MaxMember;
            }
            return -1;
        }

        /// <summary>
        /// 消耗
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true 消耗成功</returns>
        public static bool StatrConsume(string id)
        {
            if (ConfigDic.ContainsKey(id))
            {
                int consume = ConfigDic[id].Price;
                // 全局扣钱处理
                bool stateConsume = PlayerItemManager.Instance.ChangeZCurrency(consume);

                return stateConsume;
            }
            Debug.Log("扣取错误");
            return false;
        }
    } 
}
