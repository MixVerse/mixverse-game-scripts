/* ========================================================
*      作 者：Lixi 
*      主 题：
*      主要功能：

*      详细描述：

*      创建时间：2022-03-14 16:47:07
*      修改记录：
*      版 本：1.0
 ========================================================*/
using LitJson;
using System.Collections.Generic;
using Tools;

namespace Config
{
    public class Config_MapData
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        public int MapId;

        /// <summary>
        /// 多语言名字Key
        /// </summary>
        public string NameKey;

        /// <summary>
        /// 简介key
        /// </summary>
        public string IntroduceKey;

        /// <summary>
        /// 是否开启传送
        /// </summary>
        public bool IsTransfer;

        /// <summary>
        /// 目标传送点
        /// </summary>
        public int TargetTransfer;

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon;

        public string Name;

        public bool IsOfficial;

        public string SceneName;

        public Config_MapData(
            int MapId, string NameKey,
            string IntroduceKey, bool IsTransfer, 
            int TargetTransfer, string Icon,string Name , bool IsOfficial,string SceneName
        ) {
            this.MapId = MapId;
            this.NameKey = NameKey;
            this.IntroduceKey = IntroduceKey;
            this.IsTransfer = IsTransfer;
            this.TargetTransfer = TargetTransfer;
            this.Icon = Icon;
            this.Name = Name;
            this.IsOfficial = IsOfficial;
            this.SceneName = SceneName;
        }
    }

    static class MapData
    {
        static Dictionary<int, Config_MapData> DicData;
        public static void StartLoading(string josnName)
        {
            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            if (DicData == null)
            {
                DicData = new Dictionary<int, Config_MapData>();
            }

            Config_MapData config;
            //
            foreach (JsonData json in jsonData)
            {
                string MapId = json["MapId"].ToString();
                string NameKey = json["NameKey"].ToString();
                string IntroduceKey = json["IntroduceKey"].ToString();
                string IsTransfer = json["IsTransfer"].ToString();
                string TargetTransfer = json["TargetTransfer"].ToString();
                string Icon = json["Icon"].ToString();
                string Name = json["Name"].ToString();
                string IsOfficial = json["IsOfficial"].ToString();
                string SceneName = json["SceneName"].ToString();

                // 数值处理
                InforValue.StrForInt(MapId, out int mapId);
                InforValue.IntStrForBool(IsTransfer, out bool isTransfer);
                InforValue.StrForInt(TargetTransfer, out int targetTransfer);
                InforValue.IntStrForBool(IsOfficial, out bool isOfficial);
                // 
                if (!DicData.ContainsKey(mapId))
                {
                    config = 
                        new Config_MapData(
                            mapId, NameKey, IntroduceKey, isTransfer, targetTransfer, Icon, Name, isOfficial,SceneName); 

                    DicData.Add(mapId, config);
                }
                System.Console.WriteLine("初始化" + DicData);
            }
        }

        public static Dictionary<int, Config_MapData> GetAllData()
        {
            if (DicData == null) return null;

            return DicData;
        }


        public static int GetMapCount()
        {
            if (DicData == null) return -1;

            return DicData.Count;
        }

        public static Config_MapData GetData(int mapId)
        {
            if (DicData == null) return null;

            if (DicData.ContainsKey(mapId))
            {
                return DicData[mapId];
            }
            return null;
        }
    }
}