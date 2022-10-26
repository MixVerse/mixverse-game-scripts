using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMyFriendInfo : PlayerUIPanelBase
{
    [Header("需要手动赋值")]
    public Image icon;
    public Text userId;
    public Text userName;
    public Button removeBtn;
    public Button copyId;


    [Header("Player component")]
    public ZVersePlayer playerZ;

    private void Awake()
    {
        InitBase();
    }

    /// <summary>
    /// 更新好友信息
    /// </summary>
    public void UpdateInfo(FriendListInfo info)
    {
        userId.text = info.userId.ToString();
        userName.text = info.userName.ToString();

        removeBtn.onClick.SetListener(delegate
        {
            //删除好友
            playerZ.friend.CmdRemoveFriend(playerZ.user_id, info.userId);

        });

        copyId.onClick.SetListener(delegate
        {
            playerZ.GetComponentInChildren<FrindPanel>().OnOffChatPanel();
            playerZ.friend.uiFriendChat.ShowPanel(info.userId);
        });
       
    }


}
