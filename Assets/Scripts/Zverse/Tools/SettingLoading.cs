//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Reflection;
///// <summary>
///// 记载配置文件
///// </summary>
//public class SettingLoading : MonoBehaviour
//{
//    public static SettingLoading Instance;

//    private Dictionary<string, string> configJsons = new Dictionary<string, string>();

//    private List<ItemBaseModel> dataList = new List<ItemBaseModel>();

//    private static int m_configAm;

//    private static int m_configCount;

//    public static bool FinishLoading;

//    private void Awake()
//    {
//        Instance = this;
//    }
//    void Start()
//    {
//        StartCoroutine(IJsonLoading("UsableItemModel")); //加载物品栏
//        StartCoroutine(IJsonLoading("MallItems"));  //加载商店栏
//    }


//    IEnumerator IJsonLoading(string name)
//    {
//        m_configAm += 1;
//        string localPath;
//        localPath = DataPaths.JsonPath + $"{name}.json";

//        UnityWebRequest webRequest = UnityWebRequest.Get(localPath);

//        yield return webRequest.SendWebRequest();

//        if (webRequest.error != null)
//        {
//            // 读取失败
//            m_configAm -= 1;
//            Debug.LogError("读取文件出错 : " + localPath);
//        }
//        else
//        {
//            string jsonText = webRequest.downloadHandler.text;
//            if (!configJsons.ContainsKey(name))
//            {
//                m_configCount += 1;
//                configJsons.Add(name, jsonText);
//            }
//        }
//    }

//    /// <summary>
//    /// 加载商城物品
//    /// </summary>
//    /// <returns></returns>
//    public List<ItemMallCategory> LoadMallItems() 
//    {
//        List<ItemMallCategory> list = new List<ItemMallCategory>();

//        JObject jsonObject = JObject.Parse(configJsons["MallItems"]);

//        JArray categories = (JArray)jsonObject["Categories"];

//        for(int i=0;i<categories.Count;i++)
//        {
//            /*商店商品分类
//            * costume 衣服
//            * furniture 家具
//            * potion  消耗品
//            * .......
//            */
//            string jsons = jsonObject[categories[i].ToString()].ToString();
//            list.Add(new ItemMallCategory() { category = categories[i].ToString(), items = LoadMallItemsFromJson(categories[i].ToString(), jsons).ToArray() });

//        }


//        return list;

//    }
//    public List<ItemBaseModel> LoadMallItemsFromJson(string type,string jsons) 
//    {
//        List<ItemBaseModel> temp = new List<ItemBaseModel>();
//        if(type.Equals("costume"))
//        {
//           temp.AddRange(JsonConvert.DeserializeObject<List<CostumeItemModel>>(jsons));
//        }else if(type.Equals("furniture"))
//        {

//        }
//        else if(type.Equals("potion"))
//        {

//        }


//        return temp;
//    }




//    public ItemBaseModel[] LoadAllItem()
//    {

//        LoadValueFromJson<UsableItemModel>("UsableItemModel");
//        /**
//         * ....增加更多配置
//         */
//        return dataList.ToArray();
      
//    }

//    public void LoadValueFromJson<T>(string name) where T : ItemBaseModel,new()
//    {

//        if (!configJsons.ContainsKey(name))
//            return;
//        List<T> itemList = JsonConvert.DeserializeObject<List<T>>(configJsons[name]);


//        // foreach(var item in itemList)
//        // {
//        //
//        //     T model = new T();
//        //     FieldInfo[] infos= model.GetType().GetFields(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
//        //     foreach(var info in infos)
//        //     {
//        //         info.SetValue(model, json[info.Name].ToString());
//        //     }
//        //
//        //     dataList.Add(model);
//        // }
//        dataList.AddRange(itemList);

       

//    }
//}



