using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveLog : MonoBehaviour
{
#if !UNITY_EDITOR
    private float length;
    Queue queue;

    private void Awake()
    {
        if(CommonConstant.IS_SERVER)
        {
            LogToFile("Version of the runtime: " + Application.unityVersion, true, true);
            Application.logMessageReceivedThreaded += OnReceiveLogMsg;
            queue = new Queue();
        }
       
    }
    // Start is called before the first frame update
    void OnReceiveLogMsg(string condition, string stackTrace, LogType type)
    {

        if(type==LogType.Log||type==LogType.Error||type==LogType.Exception)
        {
            string msg = "";
            string _type = Enum.GetName(typeof(LogType), type);
            if (type == LogType.Log)
                msg = "[LogType]:" + _type + "--[MSG]:" + condition;
            else
                msg = "[LogType]:" + _type + "--[MSG]:" + condition + "--" + "[station]:" + stackTrace;
            queue.Enqueue(msg);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        if(CommonConstant.IS_SERVER)
          CheckLogs();
    }
    void CheckLogs()
    {
        if (queue.Count != 0)
        {
            LogToFile(queue.Peek().ToString(), true, true, () => { queue.Dequeue(); });
        }
    }
    public void LogToFile(string str, bool bwithTime, bool bAppendLineFeed,System.Action callback = null)
    {

        if (str == null) return;
        try
        {

            string fname =Application.dataPath + "/../"+DateTime.Now.ToString("yyyyMMdd")+"_Unitylog.txt";

            if (fname == "" || fname == null) return;

            if(!File.Exists(fname))
            {
                FileStream fs = File.Create(fname);
                fs.Close();
            }
              
            StreamWriter writer = new StreamWriter(fname, true, System.Text.Encoding.Default);

            if (bwithTime) writer.Write(System.DateTime.Now.ToString());
            if (bAppendLineFeed) writer.WriteLine(str);
            else writer.Write(str);
            writer.Close();
            callback?.Invoke();
        }
        catch
        {
            throw;
        }
    }
#endif
}
