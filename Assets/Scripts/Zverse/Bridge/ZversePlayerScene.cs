using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public partial struct ChooseSceneInfo
{
    public int id;
    public int maxNum;
    public int curNum;
    public string icon;
    public ScenePermission permission; //房间权限(如果有的话)
    public string owner;  //房主(如果有的话)
}
public class ZversePlayerScene : NetworkBehaviour
{

    public readonly SyncDictionary<string, List<ChooseSceneInfo>> publicSceneInfo = new SyncDictionary<string, List<ChooseSceneInfo>>();

    public readonly SyncDictionary<long, ChooseSceneInfo> roomSceneInfo = new SyncDictionary<long, ChooseSceneInfo>();


    public ZVersePlayer player;

    public override void OnStartServer()
    {
        //加载公共场景信息
        var list = ZVerseNetworkManager.Instance.publicScenesDic;
        foreach (var kv in list)
        {
            publicSceneInfo.Add(kv.Key, new List<ChooseSceneInfo>());
            foreach (var v in kv.Value)
            {
                ChooseSceneInfo temp = new ChooseSceneInfo();
                temp.id = v.id;
                temp.curNum = v.players.Count;
                temp.maxNum = v.maxNumber;
                temp.icon = "";
                publicSceneInfo[kv.Key].Add(temp);
            }
        }

        //加载私有场景信息
        UpdateRoomSceneInfo();


    }



    /// <summary>
    /// 更新公共场景信息
    /// </summary>
    /// <param name="type"></param>
    [Command]
    public void CmdUpdatePublicSceneInfo(string type)
    {
        var list = ZVerseNetworkManager.Instance.publicScenesDic[type];
        publicSceneInfo[type].Clear();
        foreach (var item in list)
        {
            ChooseSceneInfo temp = new ChooseSceneInfo();
            temp.id = item.id;
            temp.curNum = item.players.Count;
            temp.maxNum = item.maxNumber;
            temp.icon = type.ToString();
            publicSceneInfo[type].Add(temp);
        }
    }


    /// <summary>
    /// 更新自建场景信息
    /// </summary>
    [Command]
    public void CmdUpdateRoomSceneInfo()
    {
        UpdateRoomSceneInfo();
    }

    /// <summary>
    /// 加载自建场景
    /// </summary>
    private void UpdateRoomSceneInfo()
    {
        roomSceneInfo.Clear();
        var scenes = ZVerseNetworkManager.Instance.ownerScenes;
        foreach (var item in scenes)
        {
            ChooseSceneInfo temp = new ChooseSceneInfo();
            temp.id = ((int)item.Key);
            temp.curNum = item.Value.currentNumber;
            temp.maxNum = item.Value.maxNumber;
            temp.icon = "";
            temp.permission = (ScenePermission)item.Value.roomPermission;
            temp.owner = item.Value.players[0].identity.gameObject.name;
            roomSceneInfo.Add(item.Key, temp);
        }
    }


    /// <summary>
    /// 加入公共场景
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    [Client]
    public void EnterPublicScene(int id, string type)
    {
        NetworkClient.connection.Send(new PlayerInPublicSceneMsg()
        {
            id = id,
            user_id = player.user_id,
            sceneType = type
        });
    }


    /// <summary>
    /// 创建自建场景
    /// </summary>
    [Client]
    public void CreateRoomScene(int maxNum,ScenePermission p)
    {
        NetworkClient.connection.Send(new PlayerCreateRoomSceneMsg()
        {

            user_id = player.user_id,
            maxNumber = maxNum,
            roomPermission=(int)p

        }) ;
        
        
    }


    /// <summary>
    /// 加入自建场景
    /// </summary>
    [Client]
    public void EnterRoomScene(long roomId)
    {
        NetworkClient.connection.Send(new PlayerEnterRoomSceneMsg()
        {

            user_id = player.user_id,
            roomId = roomId

        }); 
    }

    /// <summary>
    /// 返回主场景
    /// </summary>
    [Client]
    public void BackToMainScene()
    {
        NetworkClient.connection.Send(new PlayerBackToMainSceneMsg()
        {

            user_id = player.user_id

        });
    }

    

    
   
}
