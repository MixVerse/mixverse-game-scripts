using LitJson;
using System;
using System.Collections.Generic;
using Tools;

namespace Config
{
    public enum ETransferType
    {
        none = -1,
        map = 0,
        room = 1,
        transfer = 2
    }
    public class Config_TransferData
    {
        public int TransferId;
        public ETransferType TransferType;
        public int TergetId;

        public Config_TransferData (
            int TransferId, ETransferType TransferType, int TergetId
        ) {
            this.TransferId = TransferId;
            this.TransferType = TransferType;
            this.TergetId = TergetId;
        }
    }

    public class TransferData
    {
        static Dictionary<int, Config_TransferData> DicData;

        public static void StartLoading(string jsonName)
        {
            string jsonText = ConfigLoading.ReadFile(jsonName);

            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);

            if (DicData == null)
            {
                DicData = new Dictionary<int, Config_TransferData>();
            }
            Config_TransferData config;

            foreach (JsonData json in jsonData)
            {
                string TransferId = json["TransferId"].ToString();
                string TransferType = json["TransferType"].ToString();
                string TergetId = json["TergetId"].ToString();

                InforValue.StrForInt(TransferId, out int transferId);
                InforValue.StrForInt(TransferType, out int transferType);
                InforValue.StrForInt(TergetId, out int tergetId);

                ETransferType etype = (ETransferType)Enum.ToObject(typeof(ETransferType), transferType);

                if (!DicData.ContainsKey(transferId))
                {
                    config = new Config_TransferData(transferId, etype, tergetId);
                    DicData.Add(transferId, config);
                }
            }
        }
    } 
}
