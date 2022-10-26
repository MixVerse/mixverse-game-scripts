using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendChat : PlayerUIPanelBase
{
    [Header("需要手动赋值")]
    public GameObject childPanel;

    [Header("prefab")]
    public GameObject sendMsgCard;
    public GameObject receiveMsgCard;

    [Header("Player component")]
    public ZVersePlayer playerZ;

    [Header("UI")]
    public Transform chatContent;
    public InputField input;
    public Button sendBtn;

    private long otherId;

    private void Awake()
    {
        InitBase();
    }

    /// <summary>
    /// 打开和对应玩家的聊天记录
    /// </summary>
    /// <param name="otherId"></param>
    /// <param name="otherName"></param>
    public void ShowPanel(long otherId)
    {
        this.otherId = otherId;
        //打开聊天框时，设置所有未读消息为已读
        playerZ.friend.CmdReadAllMessage(playerZ.user_id,otherId);
        UpdateChatCard();
    }


    /// <summary>
    /// 更新聊天消息
    /// </summary>
    public void UpdateChatCard()
    {

        //删除原先的项
        for (int i = chatContent.childCount - 1; i >= 0; i--)
            Destroy(chatContent.GetChild(i).gameObject);

        chatContent.DetachChildren();

        List<FriendChatInfo> temp = new List<FriendChatInfo>();
        for (int i = 0; i < playerZ.friend.chatList.Count; i++)
        {
            if ((playerZ.friend.chatList[i].sendId == playerZ.user_id && playerZ.friend.chatList[i].receiveId == otherId )
                || (playerZ.friend.chatList[i].sendId == otherId && playerZ.friend.chatList[i].receiveId == playerZ.user_id))
            {
                temp.Add(playerZ.friend.chatList[i]);
            }

        }


        for (int i =0;i< temp.Count; i++)
        {
            if (temp[i].sendId == playerZ.user_id )
                GameObject.Instantiate(sendMsgCard, chatContent, false);
            else 
                GameObject.Instantiate(receiveMsgCard, chatContent, false);

        }

        UIFriendChatCard[] friendChatCards = chatContent.GetComponentsInChildren<UIFriendChatCard>(true);

        for (int i = 0; i < temp.Count; i++)
        {

            friendChatCards[i].SetInfo(temp[i]);
        }



    }

    /// <summary>
    /// 点击发送消息按钮
    /// </summary>
    public void OnClickSend()
    {
        if(string.IsNullOrEmpty(input.text))
        {
            Debug.LogError("消息不能为空");
            return;
        }

        FriendListInfo info = ZVersePlayer.localPlayer.friend.friendList.Find(item => item.userId == otherId);
        playerZ.friend.CmdSendMessage(playerZ.user_id, otherId,info.userName, input.text);

    }


}
