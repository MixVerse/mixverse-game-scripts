using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
/// <summary>
/// 玩家好友类
/// </summary>
[SerializeField]
public struct ApplyInfo
{
    public long userId;
    public string userName;
    public string applyMessage;
    public string icon;
}
[SerializeField]
public struct FriendListInfo
{
    public long userId;
    public string userName;
    public int status;  //在线状态 1 在线 0 离线
    public string icon;
}
public struct FriendChatInfo
{
    public long sendId;
    public long receiveId;
    public string message;
    public int status; //消息是否已读 0 已读 1 未读
    public DateTime time;  //消息发送时间
}

public class ZversePlayerFriend : NetworkBehaviour
{

    public ZVersePlayer player;
    public UINewFriend newFriendPanel;
    public UIMyFriend myFriend;
    public UIMyFriendInfo myFriendInfo;
    public UIFriendChat uiFriendChat;

    public readonly SyncList<ApplyInfo> applyList = new SyncList<ApplyInfo>();   //好友申请列表

    public readonly SyncList<FriendListInfo> friendList = new SyncList<FriendListInfo>();   //好友信息列表

    public readonly SyncList<FriendChatInfo> chatList = new SyncList<FriendChatInfo>();  //消息发送列表
    public override void OnStartServer()
    {
        //加载好友申请消息
        LoadApplyList();
        LoadFriendList();
        LoadAllUnReadMessage();  //玩家登陆时，加载所有未读消息


    }


    public override void OnStartClient()
    {
        //创建角色后调用同步列表回调事件
        applyList.Callback += onApplyListChange;
        friendList.Callback += onFriendListChange;
        chatList.Callback += onChatListChange;
    }


    private void onApplyListChange(SyncList<ApplyInfo>.Operation op,int index,ApplyInfo oldValue,ApplyInfo newValue)
    {
        if(ZVersePlayer.localPlayer && newFriendPanel.childPanel.activeSelf)
        {
            newFriendPanel.UpdateApplyInfoCard();
        }
    }

    private void onFriendListChange(SyncList<FriendListInfo>.Operation op, int index, FriendListInfo oldValue, FriendListInfo newValue)
    {
        if (ZVersePlayer.localPlayer)
        {
            myFriend.UpdateFriendListPanel();
        }
    }

    private void onChatListChange(SyncList<FriendChatInfo>.Operation op, int index, FriendChatInfo oldValue, FriendChatInfo newValue)
    {
        if(ZVersePlayer.localPlayer&& uiFriendChat.childPanel.activeSelf)
        {
            if(op==SyncList<FriendChatInfo>.Operation.OP_ADD)
            {
                uiFriendChat.UpdateChatCard();
            }

            myFriend.UpdateUnReadMessagePanel();
        }

        
    }




    /// <summary>
    /// 处理添加好友申请
    /// </summary>
    /// <param name="applyId">申请者user_id</param>
    /// <param name="status">0 表示同意 1 表示拒绝</param>
    [Command]
    public void CmdOpperatoinApply(long applyId,int status)
    {
        zverse_friend friend= zverse_friend_dao.QueryFriendApply(player.user_id, applyId);
        if(friend==null)
        {
            RpcError(MessageConstant.OPERATOIN_FRIEND_APPLY_ERROR);
            Debug.LogError(MessageConstant.OPERATOIN_FRIEND_APPLY_ERROR);
            return;
        }
        if (status==0)
        {
            friend.status = 0;
            friend.add_time_at = DateTime.Now;

            //增加对方好友记录
            zverse_friend user = new zverse_friend();
            user.user_id = applyId;
            user.friend_id = player.user_id;
            user.friend_name = player.user_name;
            user.status = 0;
            user.add_time_at = DateTime.Now;

            zverse_friend_dao.UpdateInfo(friend);
            zverse_friend_dao.Insert(user);

        }
        else if(status==1)
        {
            friend.status = 2;
            zverse_friend_dao.UpdateInfo(friend);
        }
        ApplyInfo info = applyList.Find(item => item.userId == applyId);
        if (info.userId != 0)
            applyList.Remove(info);
    }

    /// <summary>
    /// 好友信息处理失败回调
    /// </summary>
    /// <param name="error"></param>
    [TargetRpc]
    public void RpcError(string error)
    {
        Debug.LogError(error);
    }


    /// <summary>
    /// 加载申请好友列表
    /// </summary>
    [Server]
    private void LoadApplyList()
    {
        List<zverse_friend> list = zverse_friend_dao.QueryAllFriendApply(player.user_id);
        if(list!=null)
        {
            foreach(var item in list)
            {
                ApplyInfo info = new ApplyInfo();
                info.userId = item.friend_id;
                info.userName = item.friend_name;
                info.applyMessage = item.apply_message;
                applyList.Add(info);
            }

        }
    }

    /// <summary>
    /// 查询玩家
    /// </summary>
    /// <param name="userName"></param>
    [Command]
    public void CmdSearchPlayerByName(string userName)
    {
        List<zverse_player> players=zverse_player_dao.QueryPlayerByLikeName(userName);
        List<ApplyInfo> infoList = new List<ApplyInfo>();
        if(players!=null)
        {
            foreach(var item in players)
            {
                ApplyInfo info = new ApplyInfo();
                info.userId = item.user_id;
                info.userName = item.user_name;
                infoList.Add(info);
            }

        }
        //返回搜索结果
        RpcReturnSearchPlayers(infoList);

    }

    /// <summary>
    /// 返回查询结果
    /// </summary>
    /// <param name="infoList"></param>
    [TargetRpc]
    public void RpcReturnSearchPlayers(List<ApplyInfo> infoList)
    {
        //更新搜索结果
        newFriendPanel.UpdateAddInfoCard(infoList);
    }

    /// <summary>
    /// 发送添加好友申请
    /// </summary>
    /// <param name="user_id"></param>
    [Command]
    public void CmdAddFriendApplication(long userId,string userName,string applyMessage)
    {

        if(zverse_friend_dao.QueryFriendApply(userId, player.user_id)!=null)
        {
            RpcError(MessageConstant.APPLICATION_EXIST);
            Debug.LogError(MessageConstant.APPLICATION_EXIST);
            return;
        }

        zverse_friend friend = new zverse_friend();
        friend.user_id = userId;
        friend.friend_id = player.user_id;
        friend.friend_name = player.user_name;
        friend.status = 1;
        friend.apply_message = applyMessage;
        zverse_friend_dao.Insert(friend);

        //如果对方在线，刷新对方消息列表
        if(ZVersePlayer.onlinePlayers.ContainsKey(userName))
        {
            ApplyInfo info = new ApplyInfo();
            info.userId = player.user_id;
            info.userName = player.user_name;
            info.applyMessage = applyMessage;
            ZVersePlayer.onlinePlayers[userName].friend.applyList.Add(info);
        }

    }

    [Server]
    public void LoadFriendList()
    {
        List<zverse_friend> list = zverse_friend_dao.QueryAllFriend(player.user_id);
        if (list != null)
        {
            foreach (var item in list)
            {
                FriendListInfo info = new FriendListInfo();
                info.userId = item.friend_id;
                info.userName = item.friend_name;
                info.status = ZVersePlayer.onlinePlayers.ContainsKey(info.userName) ? 1 : 0;
                friendList.Add(info);
            }

        }
    }

    /// <summary>
    /// 刷新好友列表
    /// </summary>
    [Command]
    public void CmdUpdateFriendList()
    {
        friendList.Clear();
        LoadFriendList();
    }


    /// <summary>
    /// 删除好友
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="friendId"></param>
    [Command]
    public void CmdRemoveFriend(long userId,long friendId)
    {
        zverse_friend record_1 = zverse_friend_dao.QueryFriend(userId, friendId);
        zverse_friend record_2 = zverse_friend_dao.QueryFriend(friendId, userId);
        if (record_1 == null|| record_2==null)
        {
            RpcError(MessageConstant.FRIEND_NOT_EXIST);
            Debug.LogError(MessageConstant.FRIEND_NOT_EXIST);
            return;
        }

        record_1.status = 3;
        record_2.status = 3;

        //更新记录
        zverse_friend_dao.UpdateBatch(new List<zverse_friend>() { record_1, record_2 });

        FriendListInfo info = friendList.Find(item => item.userId == friendId);
        if (info.userId != 0)
            friendList.Remove(info);

        //如果对方在线，刷新对方消息列表
        string fName = record_1.friend_name;
        if (ZVersePlayer.onlinePlayers.ContainsKey(fName))
        {
            FriendListInfo otherInfo = ZVersePlayer.onlinePlayers[fName].friend.friendList.Find(item => item.userId == userId);
            if(otherInfo.userId!=0)
               ZVersePlayer.onlinePlayers[fName].friend.friendList.Remove(otherInfo);
        }
    }

    /// <summary>
    /// 玩家登陆后加载所有未读的消息
    /// </summary>
    [Server]
    public void LoadAllUnReadMessage()
    {
        List<zverse_friend_message> list = zverse_friend_message_dao.QueryAllUnReadMessage(player.user_id);
        if(list!=null)
        {
            foreach (var item in list)
            {
                FriendChatInfo info = new FriendChatInfo();
                info.receiveId = item.receive_id;
                info.sendId = item.send_id;
                info.status = item.status;
                info.message = item.message;
                info.time = item.send_time_at;
                chatList.Add(info);
            }

        }
    }

    /// <summary>
    /// 把所有为读消息标记为已读
    /// </summary>
    /// <param name="receiveId"></param>
    [Command]
    public void CmdReadAllMessage(long receiveId,long sendId)
    {

        //设置所有未读消息为已读
        for(int i= 0; i < chatList.Count;i++)
        {
            if(chatList[i].receiveId==receiveId&& chatList[i].sendId==sendId)
            {
                FriendChatInfo info = chatList[i];
                info.status = 0;
                chatList[i] = info;
            }
        }

        //删除所有未读消息
        zverse_friend_message_dao.DeleteAllUnReadMessage(receiveId,sendId);
    }


    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="sendId"></param>
    /// <param name="receiveId"></param>
    /// <param name="message"></param>
    [Command]
    public void CmdSendMessage(long sendId,long receiveId,string receiveName,string message)
    {

        FriendChatInfo info = new FriendChatInfo();
        info.receiveId = receiveId;
        info.sendId = sendId;
        info.status = 1;
        info.message = message;
        info.time = DateTime.Now;
        chatList.Add(info);
        //如果对方在线，直接加入对方消息队列,不在线则暂存于数据库
        if (ZVersePlayer.onlinePlayers.TryGetValue(receiveName,out ZVersePlayer _player))
        {
            _player.friend.chatList.Add(info);
        }
        else
        {
            zverse_friend_message msg = new zverse_friend_message();
            msg.send_id = sendId;
            msg.receive_id = receiveId;
            msg.message = message;
            msg.status = 1;
            msg.send_time_at = info.time;
            zverse_friend_message_dao.Insert(msg);
        }
       
    }


}
