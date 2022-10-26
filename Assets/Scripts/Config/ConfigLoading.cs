using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigLoading
{
    bool m_openEditorLog = false;
    //
    static Dictionary<string, string> configPaths;

    /// <summary>
    /// 配置文件数量 
    /// </summary>
    static int m_configAm;

    /// <summary>
    /// 记录加载完成的配置文件数量
    /// </summary>
    static int m_configCount;

    public static bool FinishLoading;

    public void StartLoad(MonoBehaviour mono, ref bool openEditorLog)
    {
        //
        m_openEditorLog = openEditorLog;
        //
        configPaths = new 
            Dictionary<string, string>();
        //
        mono.StartCoroutine(IJsonLoading("地图配置表"));
        mono.StartCoroutine(IJsonLoading("房间配置表"));
        mono.StartCoroutine(IJsonLoading("传送点配置表"));
        mono.StartCoroutine(IJsonLoading("物品配置"));

        mono.StartCoroutine(IJsonLoading("载具配置"));
        mono.StartCoroutine(IJsonLoading("敌人配置"));
        //
        mono.StartCoroutine(IJsonLoading("远程武器配置"));
        //
        mono.StartCoroutine(IJsonLoading("水枪游戏配置", "水枪游戏奖励配置", "水枪游戏道具配置", "水枪游戏敌人配置", "水枪游戏区域配置"));
        //
        mono.StartCoroutine(ILoadConfig());
    }

    private void LoadConfig()
    {
        MapData.StartLoading("地图配置表");
        RoomData.StartLoading("房间配置表");
        TransferData.StartLoading("传送点配置表");
        ItemConfig.StartLoading("物品配置");
        //
        CarrierConfig.StartLoading("载具配置");
        EnemyConfig.StartLoading("敌人配置");
        //
        WeaponsConfig.StartLoading("远程武器配置");
        //
        WaterGunFightConfig.StartLoading("水枪游戏配置", "水枪游戏奖励配置", "水枪游戏道具配置", "水枪游戏敌人配置", "水枪游戏区域配置");

        Debug.Log(" --------- 全部配置文件已经加载 -------- ");
        FinishLoading = true;
    }

    public static string ReadFile(string fileName)
    {
        //
        if (configPaths.ContainsKey(fileName))
        {
            //Debug.Log($"读取数据: {fileName}");
            return configPaths[fileName];
        }
        //
        //Debug.Log($"configPaths 没有装载数据: {fileName}");
        return null;
    }

    /// <summary>
    /// 加载json文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnumerator IJsonLoading(string name)
    {
        // 没启动一个携程 读取的配置文件加1
        m_configAm += 1;
        string localPath;
        localPath = DataPaths.JsonPath + $"{name}.json";

        UnityWebRequest webRequest = UnityWebRequest.Get(localPath);

        yield return webRequest.SendWebRequest();

        if (webRequest.error != null)
        {
            // 读取失败
            m_configAm -= 1;
            Debug.LogError("读取文件出错 : path: " + localPath + "Name: " + name);
        }
        else
        {
            m_configCount += 1;
            string jsonText = webRequest.downloadHandler.text;
            if (!configPaths.ContainsKey(name))
            {
                configPaths.Add(name, jsonText);
#if UNITY_EDITOR
                if (m_openEditorLog)
                {
                    Debug.Log($" --------- 装载成功 {name} -------- ");
                }
#endif
                yield break;
            }
#if UNITY_EDITOR
            if (m_openEditorLog)
            {
                Debug.Log($"重复加载的配置: {name}");
            }
#endif  
        }
    }

    IEnumerator IJsonLoading(params string[] names)
    {
        UnityWebRequest webRequest;
        int length = names.Length;
        for (int i = 0; i < length; i++)
        {
            string name = names[i];
            // 没启动一个携程 读取的配置文件加1
            m_configAm += 1;
            string localPath;
            localPath = DataPaths.JsonPath + $"{name}.json";

            webRequest = UnityWebRequest.Get(localPath);

            yield return webRequest.SendWebRequest();

            if (webRequest.error != null)
            {
                // 读取失败
                m_configAm -= 1;
#if UNITY_EDITOR
                if (m_openEditorLog)
                {
                    Debug.LogError("读取文件出错 : " + localPath);
                }
#endif
            }
            else
            {
                m_configCount += 1;
                string jsonText = webRequest.downloadHandler.text;
                if (!configPaths.ContainsKey(name))
                {
                    configPaths.Add(name, jsonText);
#if UNITY_EDITOR
                    if (m_openEditorLog)
                    {
                        Debug.Log($" --------- 装载成功 {name} -------- ");
                    }
#endif
                    continue;
                }
#if UNITY_EDITOR
                if (m_openEditorLog)
                {
                    Debug.Log($"重复加载的配置: {name}");
                }
#endif  
            }
        }
    }

    /// <summary>
    /// 实装配置文件数据
    /// </summary>
    /// <returns></returns>
    IEnumerator ILoadConfig()
    {
        yield return new WaitUntil(() => JudgeLoading());

#if UNITY_EDITOR
        Debug.Log(" --------- 全部配置文件已经装载 -------- ");
#endif        
        LoadConfig();
        if (configPaths != null)
        {
            configPaths.Clear();
        }
    }

    private bool JudgeLoading()
    {
        if (m_configCount >= m_configAm)
        {
            return true;
        }
        return false;
    }
}
