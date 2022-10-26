using LitJson;
using myEnums;
using System.Collections.Generic;
using Tools;


namespace Config
{
    /// <summary>
    /// 远程武器配置
    /// </summary>
    public class WeaponsConfig
    {
        public struct Weapons
        {
            public string ID;

            /// <summary>
            /// 名称	
            /// </summary>
            public string Name;

            /// <summary>
            /// 名称key
            /// </summary>
            public string NameKey;

            /// <summary>
            /// 简介
            /// </summary>
            public string Describe;

            /// <summary>
            /// 简介key
            /// </summary>
            public string DescribeKey;

            /// <summary>
            /// 射程
            /// </summary>
            public float Range;

            /// <summary>
            /// 攻击分裂
            /// </summary>
            public int Split;

            /// <summary>
            /// 是否爆炸
            /// </summary>
            public bool Burst;

            /// <summary>
            /// 爆炸范围
            /// </summary>
            public float RangeBurst;

            /// <summary>
            /// 伤害
            /// </summary>
            public int Dmg;

            /// <summary>
            /// 攻击间隔
            /// </summary>
            public float CD;

            /// <summary>
            /// 附带能力
            /// </summary>
            public string Power;

            /// <summary>
            /// 可购买
            /// </summary>
            public bool isBuy;

            /// <summary>
            /// 购买货币类型
            /// </summary>
            public EExpendCurrency CurrencyType;

            /// <summary>
            /// 消耗货币
            /// </summary>
            public int Expend;

            public string Bullet;

            public Weapons(
                string ID, string Name, string NameKey,
                string Describe, string DescribeKey, float Range,
                int Split, bool Burst, float RangeBurst, int Dmg, float CD,
                string Power, bool isBuy, EExpendCurrency CurrencyType, int Expend, string Bullet)
            {
                this.ID = ID;
                this.Name = Name;
                this.NameKey = NameKey;
                this.Describe = Describe;
                this.DescribeKey = DescribeKey;
                this.Range = Range;
                this.Split = Split;
                this.Burst = Burst;
                this.RangeBurst = RangeBurst;
                this.Dmg = Dmg;
                this.CD = CD;
                this.Power = Power;
                this.isBuy = isBuy;
                this.CurrencyType = CurrencyType;
                this.Expend = Expend;
                this.Bullet = Bullet;
            }
        }

        static Dictionary<string, Weapons> DicConfig;

        public static void StartLoading(string josnName)
        {
            DicConfig = new Dictionary<string, Weapons>();

            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            Weapons config;
            //
            foreach (JsonData json in jsonData)
            {
                string ID = json["ID"].ToString();
                string Name = json["Name"].ToString();
                string NameKey = json["NameKey"].ToString();
                string Describe = json["Describe"].ToString();
                string DescribeKey = json["DescribeKey"].ToString();
                string Range = json["Range"].ToString();
                string Split = json["Split"].ToString();
                string Burst = json["Burst"].ToString();
                string RangeBurst = json["RangeBurst"].ToString();
                string Dmg = json["Dmg"].ToString();
                string CD = json["CD"].ToString();
                string Power = json["Power"].ToString();
                string IsBuy = json["IsBuy"].ToString();
                string CurrencyType = json["CurrencyType"].ToString();
                string Expend = json["Expend"].ToString();
                string Bullet = json["Bullet"].ToString();

                // 数值处理
                float.TryParse(Range, out float range);
                float.TryParse(RangeBurst, out float rangeBurst);
                float.TryParse(CD, out float cd);
                int.TryParse(Split, out int split);
                int.TryParse(Dmg, out int dmg);
                int.TryParse(Expend, out int expend);

                InforValue.IntStrForBool(IsBuy, out bool isBuy);
                InforValue.IntStrForBool(Burst, out bool burst);

                EExpendCurrency currencyType = InforValue.StrIntForEnum<EExpendCurrency>(CurrencyType);

                if (!DicConfig.ContainsKey(ID))
                {
                    config = new Weapons(ID, Name, NameKey, Describe, DescribeKey, range, split, burst, rangeBurst, dmg, cd, Power, isBuy, currencyType, expend, Bullet);

                    DicConfig.Add(ID, config);
                }
            }
        }

        public static Weapons GetData(string id)
        {
            //UnityEngine.Debug.LogError("调试：DicConfig = " + DicConfig.Count + "id = " + id);
            if (DicConfig.ContainsKey(id))
            {
                return DicConfig[id];
            }
            return default;
        }
    }
}