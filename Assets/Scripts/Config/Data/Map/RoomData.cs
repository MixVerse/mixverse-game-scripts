using LitJson;
using System.Collections.Generic;
using Tools;

namespace Config
{
    public class Config_RoomData
    {
        /// <summary>
        /// 房间Id
        /// </summary>
        public int RoomId;

        /// <summary>
        /// 地图id
        /// </summary>
        public int MapId;

        /// <summary>
        /// 多语言Key
        /// </summary>
        public string NameKey;

        /// <summary>
        /// 简介Key
        /// </summary>
        public string IntroduceKey;

        /// <summary>
        /// 房间编号
        /// </summary>
        public int RoomNumber;

        /// <summary>
        /// 是否开启菜单传送
        /// </summary>
        public bool MenuTransfer;

        /// <summary>
        /// 最大成员数量
        /// </summary>
        public int Member;

        /// <summary>
        /// 图标ID
        /// </summary>
        public string Icon;

        public Config_RoomData (
            int RoomId, int MapId, string NameKey, 
            string IntroduceKey, int RoomNumber, 
            bool MenuTransfer, int Member, string Icon
        ) {
            this.RoomId = RoomId;
            this.MapId = MapId;
            this.NameKey = NameKey;
            this.IntroduceKey = IntroduceKey;
            this.RoomNumber = RoomNumber;
            this.MenuTransfer = MenuTransfer;
            this.Member = Member;
            this.Icon = Icon;
        }
    }

    public static class RoomData
    {
        static Dictionary<int, Config_RoomData> DicData;

        public static void StartLoading(string jsonName)
        {
            string jsonText = ConfigLoading.ReadFile(jsonName);
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            if (DicData == null)
            {
                DicData = new Dictionary<int, Config_RoomData> ();
            }
            Config_RoomData config;
            foreach (JsonData json in jsonData)
            {
                string RoomId = json["RoomId"].ToString();
                string MapId = json["MapId"].ToString();
                string NameKey = json["NameKey"].ToString();
                string IntroduceKey = json["IntroduceKey"].ToString();
                string RoomNumber = json["RoomNumber"].ToString();
                string MenuTransfer = json["MenuTransfer"].ToString();
                string Member = json["Member"].ToString();
                string Icon = json["Icon"].ToString();

                InforValue.StrForInt(RoomId, out int roomId);
                InforValue.StrForInt(MapId, out int mapId);
                InforValue.StrForInt(RoomNumber, out int roomNumber);
                InforValue.IntStrForBool(MenuTransfer, out bool menuTransfer);
                InforValue.StrForInt(Member, out int member);

                if (!DicData.ContainsKey(roomId))
                {
                    config = new Config_RoomData(roomId, mapId, NameKey, IntroduceKey, roomNumber, menuTransfer, member, Icon);
                    DicData.Add(mapId, config);
                }
            }
        }

        public static Config_RoomData GetData(int roomId)
        {
            if (DicData.ContainsKey(roomId))
            {
                return DicData[roomId];
            }
            return null;
        }

        /// <summary>
        /// 判断是否开始菜单传送
        /// </summary>
        /// <returns></returns>
        public static bool IsOpenTransfer(int roomId)
        {
            if (DicData == null) return false;

            if (DicData.ContainsKey(roomId))
            {
                return DicData[roomId].MenuTransfer;
            }
            return false;
        }
    } 
}
