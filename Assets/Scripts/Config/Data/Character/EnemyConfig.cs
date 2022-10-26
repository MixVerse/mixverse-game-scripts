using Character;
using LitJson;
using System.Collections.Generic;
using Tools;

namespace Config
{
    /// <summary>
    /// 掉落类型
    /// </summary>
    public enum EDropType
    {
        none,
        item,
        buff
    }

    /// <summary>
    /// 移动类型
    /// </summary>
    public enum EMoveType 
    { 
        /// <summary>
        /// 正常类型
        /// </summary>
        none,
        
        /// <summary>
        /// 浮动
        /// </summary>
        inFloat,

        /// <summary>
        /// 无法移动
        /// </summary>
        dont
    }

    public class EnemyConfig
    {
        public struct Enemy
        {
            /// <summary>
            /// ID
            /// </summary>
            public string Id;

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
            /// 血量
            /// </summary>
            public int MaxHp;

            /// <summary>
            /// 攻击
            /// </summary>
            public int ATK;

            /// <summary>
            /// 移动速度
            /// </summary>
            public int SPD;

            public EMoveType MoveType;

            /// <summary>
            /// 敌对立场
            /// </summary>
            public EStandpoint Standpoint;

            /// <summary>
            /// 攻击间隔
            /// </summary>
            public float ATKCD;

            /// <summary>
            /// 检测范围
            /// </summary>
            public float Range;

            /// <summary>
            /// 能力	
            /// </summary>
            public string Power;

            /// <summary>
            /// 击杀掉落类型
            /// </summary>
            public EDropType DropType;

            /// <summary>
            /// 击杀掉落
            /// </summary>
            public string Drop;

            public int DropNum;
    

        public Enemy(
            string Id, string Name, string NameKey,
            string Describe, string DescribeKey,
            int MaxHp, int ATK, int SPD, EMoveType MoveType,
            EStandpoint Standpoint, float ATKCD, float Range,
            string Power, EDropType DropType, string Drop, int DropNum
        ) {
                this.Id = Id;
                this.Name = Name;
                this.NameKey = NameKey;
                this.Describe = Describe;
                this.DescribeKey = DescribeKey;
                this.MaxHp = MaxHp;
                this.ATK = ATK;
                this.SPD = SPD;
                this.MoveType = MoveType;
                this.Standpoint = Standpoint;
                this.ATKCD = ATKCD;
                this.Range = Range;
                this.Power = Power;
                this.DropType = DropType;
                this.Drop = Drop;
                this.DropNum = DropNum;
            }
        }

        static Dictionary<string, Enemy> ConfigDic;

        public static void StartLoading(string jsonName)
        {
            ConfigDic = new Dictionary<string, Enemy>();

            string jsonText = ConfigLoading.ReadFile(jsonName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);
            //
            foreach (JsonData item in jsonData)
            {
                string Id = item["Id"].ToString();
                string Name = item["Name"].ToString();
                string NameKey = item["NameKey"].ToString();
                string Describe = item["Describe"].ToString();
                string DescribeKey = item["DescribeKey"].ToString();
                string MaxHp = item["MaxHp"].ToString();
                string ATK = item["ATK"].ToString();
                string SPD = item["SPD"].ToString();
                string MoveType = item["MoveType"].ToString();
                string Standpoint = item["Standpoint"].ToString();
                string ATKCD = item["ATKCD"].ToString();
                string Range = item["Range"].ToString();
                string Power = item["Power"].ToString();
                string DropType = item["DropType"].ToString();
                string Drop = item["Drop"].ToString();
                string DropNum = item["DropNum"].ToString();

                int.TryParse(MaxHp, out int maxHp);
                int.TryParse(ATK, out int atk);
                int.TryParse(SPD, out int spd);
                int.TryParse(DropNum, out int dropNum);
                float.TryParse(ATKCD, out float atkCD);
                float.TryParse(Range, out float range);

                EStandpoint standpoint = InforValue.StrIntForEnum<EStandpoint>(Standpoint);
                EDropType dropType = EDropType.none;
                if (!DropType.Equals("null"))
                {
                    dropType = InforValue.StrIntForEnum<EDropType>(DropType);
                }
                EMoveType moveType = InforValue.StrIntForEnum<EMoveType>(MoveType);
                moveType = moveType == default ? EMoveType.none : moveType;

                if (!ConfigDic.ContainsKey(Id))
                {
                    Enemy enemy =
                        new Enemy(Id, Name, NameKey, Describe, DescribeKey, maxHp, atk, spd, moveType, standpoint, atkCD, range, Power, dropType, Drop, dropNum);

                    ConfigDic.Add(Id, enemy);
                }
            }
        }

        public static Enemy GetAll(string id)
        {
            if (ConfigDic.ContainsKey(id))
            {
                return ConfigDic[id];
            }
            return default;
        }
    } 
}
