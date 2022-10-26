/* ========================================================
*      作 者：Lixi 
*      主 题：
*      主要功能：

*      详细描述：

*      创建时间：2022-03-28 16:58:28
*      修改记录：
*      版 本：1.0
 ========================================================*/
using LitJson;
using myEnums;
using System;
using System.Collections.Generic;
using Tools;
using System.Linq;

namespace Config
{
    public enum EWaterGunFightItemType 
    { 
        none,
        buff,
        Monster  
    }

    /// <summary>
    /// 水枪游戏副本配置
    /// </summary>
    public static class WaterGunFightConfig
    {
        #region Struct

        public struct MainConfig
        {
            public int Id;
            public string Name;
            public string Namekey;
            public string Introduce;
            public string IntroduceKey;
            public string Rule;
            public string RuleKey;
            public Dictionary<int, int> NumberDouble;
            public string[] weaponIds;

            public MainConfig(
                int Id, string Name, string Namekey, string Introduce,
                string IntroduceKey, string Rule, string RuleKey,
                Dictionary<int, int> NumberDouble, string[] weaponIds
            )
            {
                this.Id = Id;
                this.Name = Name;
                this.Namekey = Namekey;
                this.Introduce = Introduce;
                this.IntroduceKey = IntroduceKey;
                this.Rule = Rule;
                this.RuleKey = RuleKey;
                this.NumberDouble = NumberDouble;
                this.weaponIds = weaponIds;
            }
        }

        public struct Award
        {
            public EAwardType type;
            public int score;

            public Award(
                EAwardType type, int score
            ) {
                this.type = type;
                this.score = score;
            }
        }

        public struct AwardConfig
        {

            public int MaxIntegral;
            public bool formula;
            public float[] XY;
            public Award[] Awards;

            public AwardConfig(
                int MaxIntegral, bool formula,
                float[] XY, Award[] Awards
            )
            {
                this.MaxIntegral = MaxIntegral;
                this.formula = formula;
                this.XY = XY;
                this.Awards = Awards;
            }
        }

        /// <summary>
        /// 道具配置
        /// </summary>
        public struct Item_WG
        {
            /// <summary>
            /// 道具ID			
            /// </summary>
            public string Id;

            /// <summary>
            /// 总数
            /// </summary>
            public int Max;

            /// <summary>
            /// 出现位置
            /// </summary>
            public string[] Site;

            /// <summary>
            /// 击杀分数
            /// </summary>
            public int Score;

            /// <summary>
            /// 附带buff
            /// </summary>
            public string Buff;

            /// <summary>
            /// 触发概率
            /// </summary>
            public float Probability;

            /// <summary>
            /// 组Id
            /// </summary>
            public string Group;

            public Item_WG(
                string Id, int Max, string[] Site, 
                int Score, string Buff, float Probability,
                string Group
            ) {
                this.Id = Id;
                this.Max = Max;
                this.Site = Site;
                this.Score = Score;
                this.Buff = Buff;
                this.Probability = Probability;
                this.Group = Group;
            }
        }

        /// <summary>
        /// 怪物配置
        /// </summary>
        public struct EnemyConfig
        {
            /// <summary>
            /// 怪物Id
            /// </summary>
            public string Id;

            /// <summary>
            /// 出现总数
            /// </summary>
            public int Max;

            /// <summary>
            /// 击杀分数
            /// </summary>
            public int Score;

            /// <summary>
            /// 出现位置
            /// </summary>
            public string[] Site; 
            
            public EnemyConfig (
                string Id, int Max, int Score,
                string[] Site
            ) {
                this.Id=Id;
                this.Max = Max;
                this.Score =Score;
                this.Site = Site;
            }
        }

        /// <summary>
        /// 区域位置
        /// </summary>
        public struct Pos
        {
            public string Id;
            public float PosX1;
            public float PosX2;
            public float PosY1;
            public float PosY2;
            public float PosZ1;
            public float PosZ2;

            public Pos (
                string Id, float PosX1, float PosX2, float PosY1, float PosY2,
                float PosZ1, float PosZ2
            ) {
                this.Id = Id;
                this.PosX1 = PosX1;
                this.PosX2 = PosX2;
                this.PosY1 = PosY1;
                this.PosY2 = PosY2;
                this.PosZ1 = PosZ1;
                this.PosZ2 = PosZ2;
            }
        }

        #endregion

        static Dictionary<int, MainConfig> dicMain;
        static Dictionary<int, AwardConfig> dicAward;
        static Dictionary<string, Dictionary<string, Item_WG>> dicItem;
        static Dictionary<string, EnemyConfig> dicEnemy;
        static Dictionary<string, Pos> dicAreaPos;

        static List<int> m_probabilityLt;

        public static void StartLoading (
            string jsonName1, string jsonName2, 
            string jsonName3, string jsonName4, string jsonName5
        ) {
            #region 主配置

            string jsonText = ConfigLoading.ReadFile(jsonName1);
            JsonReader reader = new JsonReader(jsonText);

            JsonData jsonData = JsonMapper.ToObject(reader);
            if (dicMain == null)
            {
                dicMain = new Dictionary<int, MainConfig>();
            }

            foreach (JsonData item in jsonData)
            {
                string Id = item["Id"].ToString();
                string Name = item["Name"].ToString();
                string Namekey = item["Namekey"].ToString();
                string Introduce = item["Introduce"].ToString();
                string IntroduceKey = item["IntroduceKey"].ToString();
                string Rule = item["Rule"].ToString();
                string RuleKey = item["RuleKey"].ToString();
                string NumberDouble = item["NumberDouble"].ToString();
                string WeaponIds = item["weaponIds"].ToString();

                int.TryParse(Id, out int id);

                Dictionary<int, int> numberDouble = new Dictionary<int, int>();
                string[] m_numberArry = NumberDouble.Split('#');
                for (int i = 0; i < m_numberArry.Length; i++)
                {
                    string[] arry = m_numberArry[i].Split('-');
                    if (arry.Length == 2)
                    {
                        int.TryParse(arry[0], out int count);
                        int.TryParse(arry[1], out int multiple);

                        if (!numberDouble.ContainsKey(count))
                        {
                            numberDouble.Add(count, multiple);
                        }
                    }
                }

                string[] weaponIds = WeaponIds.Split('#');


                if (!dicMain.ContainsKey(id))
                {
                    MainConfig mainConfig = new MainConfig(id,
                        Name, Namekey, Introduce, IntroduceKey, Rule, RuleKey, numberDouble, weaponIds);

                    dicMain.Add(id, mainConfig);
                }
            }

            #endregion

            #region 奖励配置

            jsonText = ConfigLoading.ReadFile(jsonName2);
            reader = new JsonReader(jsonText);

            jsonData = JsonMapper.ToObject(reader);

            dicAward = new Dictionary<int, AwardConfig>();
            m_probabilityLt = new List<int>();

            foreach (JsonData item in jsonData)
            {
                string MaxIntegral = item["MaxIntegral"].ToString();
                string Formula = item["Formula"].ToString();
                string XY = item["XY"].ToString();
                string Award1 = item["Award1"].ToString();
                string Award2 = item["Award2"].ToString();

                int.TryParse(MaxIntegral, out int maxIntegral);
                InforValue.IntStrForBool(Formula, out bool formula);
                string[] arry = XY.Split('#');
                List<float> list = new List<float>();
                for (int i = 0; i < arry.Length; i++)
                {
                    float.TryParse(arry[i], out float @float);
                    list.Add(@float);
                }
                float[] xy = list.ToArray();
                Award award1 = AnalysisAwardData(Award1);
                Award award2 = AnalysisAwardData(Award2);
                Award[] awards = new Award[] { award1, award2 };

                if (!dicAward.ContainsKey(maxIntegral))
                {
                    AwardConfig config = new
                        AwardConfig(maxIntegral, formula, xy, awards);

                    dicAward.Add(maxIntegral, config);
                    m_probabilityLt.Add(maxIntegral);
                }
            }

            for (int i = 0;i < m_probabilityLt.Count; i++)
            {
                for (int j = 0; j < m_probabilityLt.Count - 1 - i; j++)
                {
                    int min = m_probabilityLt[j];
                    int max = m_probabilityLt[j + 1];
                    if (min > max)
                    {
                        int value = m_probabilityLt[j];
                        m_probabilityLt[j] = m_probabilityLt[j + 1];
                        m_probabilityLt[j + 1] = value;
                    }
                }
            }

            #endregion

            #region 物品配置
            //
            dicItem = new Dictionary<string, Dictionary<string, Item_WG>>();
            //
            jsonText = ConfigLoading.ReadFile(jsonName3);
            reader = new JsonReader(jsonText);
            jsonData = JsonMapper.ToObject(reader);

            foreach (JsonData item in jsonData)
            {
                string Id = item["Id"].ToString();
                string Max = item["Max"].ToString();
                string Site = item["Site"].ToString();
                string Score = item["Score"].ToString();
                string Buff = item["Buff"].ToString();
                string Probability = item["Probability"].ToString();
                string Group = item["Group"].ToString();

                int.TryParse(Max, out int max);
                int.TryParse(Score, out int score);
                float probability = InforValue.InforProbability(Probability);

                string[] site = Site.Split('#');
                Item_WG config =
                    new Item_WG(Id, max, site, score, Buff, probability, Group);
                if (!Group.Equals("null") && !dicItem.ContainsKey(Group))
                {
                    Dictionary<string, Item_WG> dic = new Dictionary<string, Item_WG>();
                    dic.Add(Id, config);
                    dicItem.Add(Group, dic);
                }
                else if (!Group.Equals("null") && dicItem.ContainsKey(Group))
                {
                    if (!dicItem[Group].ContainsKey(Id))
                    {
                        dicItem[Group].Add(Id, config);
                    }
                }
                else if (Group.Equals("null"))
                {
                    Dictionary<string, Item_WG> dic = new Dictionary<string, Item_WG>();
                    dic.Add(Id, config);
                    dicItem.Add(Id, dic);
                }
            }

            #endregion

            #region 敌人配置

            dicEnemy = new Dictionary<string, EnemyConfig>();

            jsonText = ConfigLoading.ReadFile(jsonName4);
            reader = new JsonReader(jsonText);
            jsonData = JsonMapper.ToObject(reader);

            foreach (JsonData item in jsonData)
            {
                string Id = item["Id"].ToString();
                string Max = item["Max"].ToString();
                string Score = item["Score"].ToString();
                string Site = item["Site"].ToString();

                int max = InforValue.StrForInt(Max);
                int score = InforValue.StrForInt(Score);
                string[] site = Site.Split('#');

                if (!dicEnemy.ContainsKey(Id))
                {
                    EnemyConfig config = new EnemyConfig(Id, max, score, site);
                    dicEnemy.Add(Id, config);
                }
            }

            #endregion

            #region 位置配置

            dicAreaPos = new Dictionary<string, Pos>();

            jsonText = ConfigLoading.ReadFile(jsonName5);
            reader = new JsonReader(jsonText);
            jsonData = JsonMapper.ToObject(reader);

            foreach (JsonData item in jsonData)
            {
                string Id = item["Id"].ToString();
                string PosX1 = item["PosX1"].ToString();
                string PosX2 = item["PosX2"].ToString();
                string PosY1 = item["PosY1"].ToString();
                string PosY2 = item["PosY2"].ToString();
                string PosZ1 = item["PosZ1"].ToString();
                string PosZ2 = item["PosZ2"].ToString();

                float.TryParse(PosX1, out float posX1);
                float.TryParse(PosX2, out float posX2);
                float.TryParse(PosY1, out float posY1);
                float.TryParse(PosY2, out float posY2);
                float.TryParse(PosZ1, out float posZ1);
                float.TryParse(PosZ2, out float posZ2);

                if (!dicAreaPos.ContainsKey(Id))
                {
                    Pos config = new Pos(Id, posX1, posX2, posY1, posY2, posZ1, posZ2);
                    dicAreaPos.Add(Id, config);
                }
            } 

            #endregion
        }

        /// <summary>
        /// 解析奖励数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Award AnalysisAwardData(string data)
        {
            string[] awardArry = data.Split('#');
            if (awardArry.Length == 2)
            {
                //
                int.TryParse(awardArry[0], out int intType);
                //
                int.TryParse(awardArry[1], out int count);
                EAwardType type =
                        (EAwardType)Enum.ToObject(typeof(EAwardType), intType);

                Award award = new Award(type, count);
                return award;
            }
            return default;
        }

        public static EnemyConfig GetEnemyConfig(string enmeyId)
        {
            if (dicEnemy.ContainsKey(enmeyId))
            {
                return dicEnemy[enmeyId];
            }
            return default;
        }

        public static Pos GetPosConfig(string areaKey)
        {
            if (dicAreaPos.ContainsKey(areaKey))
            {
                return dicAreaPos[areaKey];
            }
            return default;
        }

        public static Item_WG GetItemData(string groupId)
        {
            if (dicItem.ContainsKey(groupId))
            {
                if (dicItem[groupId].Count == 1)
                {
                    return dicItem[groupId][groupId];
                }

                List<int> pros = new List<int>();
                // 存放
                List<string> ids = new List<string>();
                foreach (var a in dicItem[groupId].Values)
                {
                    pros.Add((int)(a.Probability * 0.1f));
                    ids.Add(a.Id);
                }

                // 对概率进行排序
                for (int i = 0; i < pros.Count; i++)
                {
                    for (int j = 0; j < pros.Count - 1 - i; j++)
                    {
                        if (pros[j] > pros[j + 1])
                        {
                            int newMin = pros[j];
                            pros[j] = pros[j + 1];
                            pros[j + 1] = newMin;

                            string newMin2 = ids[j];
                            ids[j] = ids[j + 1];
                            ids[j + 1] = newMin2;
                        }
                    }
                }

                // 通过概率 计算区间
                int value = 0;
                for (int i = 0;i < pros.Count; i++)
                {
                    value += pros[i];
                    pros[i] = value;
                }

                int section = 0; // 返回的概率区间
                int random = UnityEngine.Random.Range(1, 11); // 随机到的概率
                // 进行判断 随机数在那个区间
                for (int i = 0; i < pros.Count; i++)
                {
                    int pass = pros[i];
                    if (random <= pass)
                    {
                        section = i;
                        break;
                    }
                }

                string id = ids[section];
                return dicItem[groupId][id];
            }
            return default;
        }

        public static void GetAward(int score, out int z, out int s)
        {
            AwardConfig config = default;
            for (int i = 0; i < m_probabilityLt.Count; i++)
            {
                int pass = m_probabilityLt[i];
                if (score <= pass)
                {
                    config = dicAward[pass];
                    break;
                }
            }

            z = config.Awards[0].score;
            s = config.Awards[1].score;
            if (config.formula)
            {
                float X = config.XY[0];
                float Y = config.XY[1];

                //80 + 分数 * 0.1
                if (z == 0)
                {
                    z = (int)Math.Round(X + (score * Y));
                }
                if (s == 0)
                {
                    s = (int)Math.Round(X + (score * Y));
                }
            }
        }
    } 
}
