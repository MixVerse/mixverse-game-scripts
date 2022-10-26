using LitJson;
using System.Collections.Generic;
using Tools;

namespace Config
{
    public enum EItemType 
    {
        /// <summary>
        /// 货币
        /// </summary>
        currency,

        /// <summary>
        /// 家具
        /// </summary>
        furniture,

        /// <summary>
        /// 房屋
        /// </summary>
        house,

        /// <summary>
        /// 装饰物
        /// </summary>
        decorate,

        /// <summary>
        /// 武器
        /// </summary>
        Weapon,

        /// <summary>
        /// 地面
        /// </summary>
        Ground,

        /// <summary>
        /// 外观服装
        /// </summary>
        Costume
    }

    public static class ItemConfig
    {
        public struct Item_Config
        {
            /// <summary>
            /// 物品Id
            /// </summary>
            public string Id;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 介绍
            /// </summary>
            public string Introduce;

            /// <summary>
            /// 名称多语言
            /// </summary>
            public string NameKey;

            /// <summary>
            /// 介绍多语言
            /// </summary>
            public string IntroduceKey;

            /// <summary>
            /// 物品类型
            /// </summary>
            public EItemType Type;

            /// <summary>
            /// 是否出售
            /// </summary>
            public bool Sell;

            /// <summary>
            /// 出售金额
            /// </summary>
            public int Price;

            /// <summary>
            /// 最大叠加
            /// </summary>
            public int MaxStack;

            /// <summary>
            /// 是否挂壁
            /// </summary>
            public bool IsHang;

            public string WeaponID;

            public bool IsGreen;

            public bool IsChangeColor;

            public Item_Config(
                string Id, string Name, string Introduce,
                string NameKey, string IntroduceKey,
                EItemType Type, bool Sell, int Price,
                int MaxStack,bool IsHang, string WeaponID, bool IsGreen, bool IsChangeColor
            ) {
                this.Id = Id;
                this.Name = Name;
                this.Introduce = Introduce;
                this.NameKey = NameKey;
                this.IntroduceKey = IntroduceKey;
                this.Type = Type;
                this.Sell = Sell;
                this.Price = Price;
                this.MaxStack = MaxStack;
                this.IsHang = IsHang;
                this.WeaponID = WeaponID;
                this.IsGreen = IsGreen;
                this.IsChangeColor = IsChangeColor;
            }
        }

        //
        static Dictionary<string, Item_Config> m_configItem; 

        //
        public static void StartLoading(string josnName)
        {
            m_configItem = new Dictionary<string, Item_Config>();

            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            Item_Config config;
            //
            foreach (JsonData json in jsonData)
            {
                string Id = json["Id"].ToString();
                string Name = json["Name"].ToString();
                string Introduce = json["NameKey"].ToString();
                string NameKey = json["Introduce"].ToString();
                string IntroduceKey = json["IntroduceKey"].ToString();
                string Type = json["Type"].ToString();
                string Sell = json["Sell"].ToString();
                string Price = json["Price"].ToString();
                string MaxStack = json["MaxStack"].ToString();
                string IsHang = json["IsHang"].ToString();
                string WeaponID = json["WeaponID"].ToString();
                string IsGreen = json["IsGreen"].ToString();
                string IsChangeColor = json["IsChangeColor"].ToString();

                // 数值处理
                bool sell = InforValue.IntStrForBool(Sell);
                int.TryParse(Price, out int price);
                int.TryParse(MaxStack, out int maxStack);
                bool isHang = InforValue.IntStrForBool(IsHang);
                bool isGreen = InforValue.IntStrForBool(IsGreen);
                bool isChangeColor = InforValue.IntStrForBool(IsChangeColor);
                //
                EItemType type = InforValue.StrIntForEnum<EItemType>(Type);

                if (!m_configItem.ContainsKey(Id))
                {
                    config = new Item_Config(Id, Name, Introduce, NameKey, IntroduceKey, type, sell, price, maxStack, isHang, WeaponID, isGreen, isChangeColor);
                    m_configItem.Add(Id, config);
                }
            }
        }

        public static Item_Config GetData(string id)
        {
            if (m_configItem.ContainsKey(id))
            {
                return m_configItem[id];
            }
            return default;
        }

        public static Dictionary<string,Item_Config> GetAllItem()
        {
            return m_configItem;
        }
    }
}