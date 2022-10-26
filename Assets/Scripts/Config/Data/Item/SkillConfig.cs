using LitJson;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Config
{

    /// <summary>
    /// 技能被动触发类型
    /// </summary>
    public enum ETrigger_SkillV2
    {
        none,

        /// <summary>
        /// 永恒的
        /// </summary>
        eternity,

        /// <summary>
        /// 周期
        /// </summary>
        period,

        /// <summary>
        /// 概率
        /// </summary>
        probability
    }

    /// <summary>
    /// 技能消耗类型
    /// </summary>
    public enum EExpend_Skill
    {
        /// <summary>
        /// 无
        /// </summary>
        none,

        /// <summary>
        /// 生命值
        /// </summary>
        Hp,

        /// <summary>
        /// 法力值
        /// </summary>
        Mp
    }

    public static class SkillConfig
    {
        public struct Skill_Config 
        {
            /// <summary>
            /// Id
            /// </summary>
            public string SkillId;

            public string Name;

            public string NameKey;

            public string Explain;

            public string ExplainKey;

            /// <summary>
            /// 主动 / 被动
            /// </summary>
            public bool Active;

            /// <summary>
            /// 被动触发类型
            /// </summary>
            public ETrigger_SkillV2 ETrigger;

            /// <summary>
            /// 触发数值
            /// </summary>
            public int TriggerValue;

            /// <summary>
            /// 消耗类型
            /// </summary>
            public EExpend_Skill EExpend;

            /// <summary>
            /// 消耗
            /// </summary>
            public int Expend;

            /// <summary>
            /// 技能数值
            /// </summary>
            public int Value;

            /// <summary>
            /// 冷却
            /// </summary>
            public float CD;

            /// <summary>
            /// 附加buffId
            /// </summary>
            public string BuffId;

            public Skill_Config (                              
                string SkillId, string Name, string NameKey,
                string Explain, string ExplainKey, bool Active,
                ETrigger_SkillV2 ETrigger, int TriggerValue,
                EExpend_Skill EExpend, int Expend,
                int Value, float CD, string BuffId
            ) {
                this.SkillId = SkillId;
                this.Name = Name;
                this.NameKey = NameKey;
                this.Explain = Explain;
                this.ExplainKey = ExplainKey;
                this.Active = Active;
                this.ETrigger = ETrigger;
                this.TriggerValue = TriggerValue;
                this.EExpend = EExpend;
                this.Expend = Expend;
                this.Value = Value;
                this.CD = CD;
                this.BuffId = BuffId;
            }
        }

        static Dictionary<string, Skill_Config> m_dicConfig;

        public static void StartLoading(string josnName)
        {
            m_dicConfig = new Dictionary<string, Skill_Config>();

            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            Skill_Config config;
            //
            foreach (JsonData json in jsonData)
            {
                string SkillId = json["SkillId"].ToString();
                string Name = json["Name"].ToString();
                string NameKey = json["NameKey"].ToString();
                string Explain = json["Explain"].ToString();
                string ExplainKey = json["ExplainKey"].ToString();
                string Active = json["Active"].ToString();
                string ETrigger = json["ETrigger"].ToString();
                string TriggerValue = json["TriggerValue"].ToString();
                string EExpend = json["EExpend"].ToString();
                string Expend = json["Expend"].ToString();
                string Value = json["Value"].ToString();
                string CD = json["CD"].ToString();
                string BuffId = json["BuffId"].ToString();

                // 数值处理
                int.TryParse(Value, out int value);
                int.TryParse(TriggerValue, out int triggerValue);
                int.TryParse(Expend, out int expend);
                //
                float.TryParse(CD, out float cd);
                //
                bool active = InforValue.IntStrForBool(Active);

                ETrigger_SkillV2 eTrigger = 
                    InforValue.StrIntForEnum<ETrigger_SkillV2>(ETrigger);

                EExpend_Skill eExpend = 
                    InforValue.StrIntForEnum<EExpend_Skill>(EExpend);

                if (!m_dicConfig.ContainsKey(SkillId))
                {
                    config = 
                        new Skill_Config (SkillId, Name, NameKey, Explain, ExplainKey, active, eTrigger, triggerValue, eExpend, expend, value, cd, BuffId);
                    m_dicConfig.Add(SkillId, config);
                }
            }
        }

        public static Skill_Config GetData(string id)
        {
            if (m_dicConfig.ContainsKey(id))
            {
                return m_dicConfig[id];
            }
            return default;
        }

        private static bool GetRand(int prob)
        {
            int random = Random.Range(1, 11);
            if (random <= prob)
            {
                return true;
            }
            return false;
        }

        public static bool GetProbability(string skilldId)
        {
            bool succeed = false;
            if (m_dicConfig.ContainsKey(skilldId))
            {
                Skill_Config config = m_dicConfig[skilldId];
                if (config.ETrigger == ETrigger_SkillV2.probability)
                {
                    return GetRand(config.TriggerValue);
                }
            }
            return succeed;
        }
    }
}