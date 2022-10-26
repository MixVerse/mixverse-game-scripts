using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using BestHTTP;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ZVerseGameManager : MonoBehaviour
{

    private static ZVerseGameManager instance;
    public static ZVerseGameManager Instance { get { return instance; } }

    public bool server=false;
    public string serverAddress = "localhost";

    private static  bool firstOpenApp=true;
    public static bool FirstOpenApp { get { return firstOpenApp; } }

    private void Awake()
    {
        if(instance==null)
          instance = this;
        //ZVerseMysqlConnect.Init();
        //zverse_users_dao.CreateTB();
        //zverse_users_dao.Insert(new zverse_users());
        // zverse_users user= zverse_users_dao.QueryUser(1);
        //Debug.Log(user.ToString());
        //zverse_users_dao.Test();
        // zverse_users_dao.InsertBatch(new List<zverse_users>());
        //zverse_users_dao.UpdateInfo(user);
        //List<zverse_users> users = zverse_users_dao.QueryUserByName("cqj");
        //zverse_users_dao.UpdateBatch(users);
        //Debug.Log(DateTime.Today.AddDays(-4));
        //zverse_users_dao.QueryUserByLastLoginAt(DateTime.Today.AddDays(-4), DateTime.Today.AddDays(-3));
        // zverse_users_dao.QueryUserByIds(new List<string>() { "1", "2", "3" });
       
    }
    private void Start()
    {

        if (server)
        {
            Debug.Log("start server ..........");
            ZVerseNetworkManager.Instance.networkAddress = serverAddress;
            ZVerseNetworkManager.Instance.StartServer();
        }
        else
        {
            if (firstOpenApp)
            {
                Debug.Log("start client ..........");
                firstOpenApp = false;
                ZVerseNetworkManager.Instance.StartClient();
            }
               
        }
            

    }


    

}
