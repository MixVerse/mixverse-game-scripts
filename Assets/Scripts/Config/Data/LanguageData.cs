using LitJson;
using Main;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    /// <summary>
    /// 存储多语言类
    /// </summary>
    static class LanguageData
    {
        public class LanguageType
        {
            public LanguageType(string CN, string EN)
            {
                this.CN = CN;
                this.EN = EN;
            }

            public string CN;
            public string EN;
        }

        static Dictionary<string, LanguageType> DicData;

        public static void StartLoading(string josnName)
        {
            string jsonText = ConfigLoading.ReadFile(josnName);
            JsonReader reader = new JsonReader(jsonText);

            JsonData jsonData = JsonMapper.ToObject(reader);
            if (DicData == null)
            {
                DicData = new Dictionary<string, LanguageType>();
            }

            foreach (JsonData item in jsonData)
            {
                string key = item["LanguageKey"].ToString();
                string CN = item["CN"].ToString();
                string EN = item["EN"].ToString();
                if (!DicData.ContainsKey(key))
                {
                    LanguageType type = new LanguageType(CN, EN);
                    DicData.Add(key, type);
                }
            }
        }

        public static string GetText(string key)
        {
            if (DicData == null) return null;

            Main.ELanguage type = MainSetManager.EType;
            if (DicData.ContainsKey(key))
            {
                LanguageType language = DicData[key];
                switch (type)
                {
                    case Main.ELanguage.CN:
                        {
                            return language.CN;
                        }
                    case Main.ELanguage.EN:
                        {
                            return language.EN;
                        }
                    default:
                        break;
                }
            }
            return null;

        }

        public static string GetText(string key, params object[] @params)
        {
            if (DicData == null || string.IsNullOrEmpty(key)) return null;

            string txt = null;
            Main.ELanguage type = MainSetManager.EType;
            if (DicData.ContainsKey(key))
            {
                LanguageType language = DicData[key];
                switch (type)
                {
                    case Main.ELanguage.none:
                        break;
                    case Main.ELanguage.CN:
                        {
                            txt = language.CN;
                        }
                        break;
                    case Main.ELanguage.EN:
                        {
                            txt = language.EN;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(txt))
            {
                return string.Format(txt, @params);
            }

            return null;
        }
    } 
}
