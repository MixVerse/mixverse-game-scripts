using LitJson;
using System.Collections.Generic;
using Tools;

namespace Config
{
    /// <summary>
    /// buff增益类型
    /// </summary>
    public enum EBuffType
    {
        /// <summary>
        /// 生命
        /// </summary>
        Hp,

        /// <summary>
        /// 法力
        /// </summary>
        Mp,

        /// <summary>
        /// 能量
        /// </summary>
        energy,

        /// <summary>
        /// 攻击
        /// </summary>
        ATK,

        /// <summary>
        /// 防御
        /// </summary>
        DEF,

        /// <summary>
        /// 移动速度
        /// </summary>
        DEX
    }

    /// <summary>
    /// 持续类型
    /// </summary>
    public enum ESustain
    {
        none,

        /// <summary>
        /// 时间
        /// </summary>
        time,

        /// <summary>
        /// 计次
        /// </summary>
        count
    }

    public enum EBuffValue
    {
        /// <summary>
        /// 正常
        /// </summary>
        none,

        /// <summary>
        /// 翻倍
        /// </summary>
        Double,

        /// <summary>
        /// 百分比
        /// </summary>
        Percentage,

        /// <summary>
        /// 百分比增加
        /// </summary>
        AddPercentage
    }

    public static class BuffConfig
    {
        public struct BuffData
        {
            public string Id;

            public string Name;

            public string NameKey;

            public string Introduce;

            public string IntroduceKey;

            public EBuffType Type;

            public ESustain ESustain;

            /// <summary>
            /// 持续数值
            /// </summary>
            public float Sustain;

            /// <summary>
            /// buff数值类型
            /// </summary>
            public EBuffValue TypeValue;

            /// <summary>
            /// buff数值
            /// </summary>
            public int BuffValue;

            public BuffData (
                string Id, string Name, string NameKey, string Introduce, string IntroduceKey,
                EBuffType Type, ESustain ESustain,
                float Sustain, EBuffValue TypeValue, int BuffValue
            ) {
                this.Id = Id;
                this.Name = Name;
                this.NameKey = NameKey;
                this.Introduce = Introduce;
                this.IntroduceKey = IntroduceKey;
                this.Type = Type;
                this.ESustain = ESustain;
                this.Sustain = Sustain;
                this.TypeValue = TypeValue;
                this.BuffValue = BuffValue;
            }
        }

        static Dictionary<string, BuffData> m_dicConfig;

        public static void StartLoading(string josnName)
        {
            m_dicConfig = new Dictionary<string, BuffData>();

            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            BuffData config;
            //
            foreach (JsonData json in jsonData)
            {
                string Id = json["Id"].ToString();
                string Name = json["Name"].ToString();
                string NameKey = json["NameKey"].ToString();
                string Introduce = json["Introduce"].ToString();
                string IntroduceKey = json["IntroduceKey"].ToString();
                string Type = json["Type"].ToString();
                string ESustain = json["ESustain"].ToString();
                string Sustain = json["Sustain"].ToString();
                string TypeValue = json["TypeValue"].ToString();
                string BuffValue = json["BuffValue"].ToString();

                // 数值处理
                int.TryParse(BuffValue, out int buffValue);
                //
                float.TryParse(Sustain, out float sustain);

                EBuffType type = InforValue.StrIntForEnum<EBuffType>(Type);
                ESustain eSustain = InforValue.StrIntForEnum<ESustain>(ESustain);
                EBuffValue typeValue = InforValue.StrIntForEnum<EBuffValue>(TypeValue);

                if (!m_dicConfig.ContainsKey(Id))
                {
                    config = 
                        new BuffData(Id, Name, NameKey, Introduce, IntroduceKey, type, eSustain, sustain, typeValue, buffValue);

                    m_dicConfig.Add(Id, config);
                }
            }
        }

        public static BuffData GetData(string buffId)
        {
            if (m_dicConfig.ContainsKey(buffId))
            {
                return m_dicConfig[buffId];
            }

            return default;
        }
    }
}
