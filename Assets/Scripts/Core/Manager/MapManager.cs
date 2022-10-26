using Config;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// µØÍ¼×Öµä key-> mapId value -> mapName
        /// </summary>
        public static Dictionary<int, string> Maps;

        public void InitData()
        {
            Maps = new Dictionary<int, string>();
            Dictionary<int, Config_MapData> config = MapData.GetAllData();

            foreach (var data in config.Values)
            {
                Maps.Add(data.MapId, data.SceneName);
            }
        }
    }
}