using UnityEngine;
using System;
using System.Collections.Generic;
using LitJson;
using Main;

namespace Config
{
    [Serializable]
    public class Language
    {
        public Language(string Key, string CN, string EN)
        {
            this.Key = Key;
            this.CN = CN;
            this.EN = EN;
        }
        public string Key;
        public string CN;
        public string EN;
    }

    public class LanguageConfig : ScriptableObject
    {
        [SerializeField]
        List<Language> m_language;

        public void StartLoading(string jsonText)
        {
            JsonReader reader = new JsonReader(jsonText);
            JsonData jsonData = JsonMapper.ToObject(reader);
            //
            if (m_language == null)
            {
                m_language = new List<Language>();
            }
            //
            foreach (JsonData item in jsonData)
            {
                string key = item["LanguageKey"].ToString();
                string CN = item["CN"].ToString();
                string EN = item["EN"].ToString();
                Language type = new Language(key, CN, EN);
                m_language.Add(type);
            }
        }

        public string GetText(string key)
        {
            if (m_language == null || string.IsNullOrEmpty(key)) return null;

            Main.ELanguage type = MainSetManager.EType;
            for (int i = 0; i < m_language.Count; i++)
            {
                Language language = m_language[i];
                if (language.Key.Equals(key))
                {
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
            }
            return null;

        }

        public string GetText(string key, params object[] @params)
        {
            if (m_language == null || string.IsNullOrEmpty(key)) return null;

            string txt = null;
            Main.ELanguage type = MainSetManager.EType;
            for (int i = 0; i < m_language.Count; i++)
            {
                Language language = m_language[i];
                if (language.Key.Equals(key))
                {
                    switch (type)
                    {
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
            }
            if (!string.IsNullOrEmpty(txt))
            {
                return string.Format(txt, @params);
            }

            return null;
        }
    } 
}
