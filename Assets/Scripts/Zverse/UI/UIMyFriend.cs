using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMyFriend : PlayerUIPanelBase
{

    public enum MyFriendChildPanel
    {
        FriendList,
        UnreadMessage,
        MineInfo
    }
    [Header("需要手动赋值")]
    public GameObject friendListPanel;
    public MeInfomationPanel meInfomationPanel;
    public GameObject massageListPanel;

    #region 状态按钮
    public Image friendButtonImage;
    public Image meButtonImage;
    public Image massageButtonImage;

    [Header("状态按钮正常状态")]
    public Color normalColor;
    [Header("状态按钮选中状态")]
    public Color selectColor;
    #endregion

    [Header("Player component")]
    public ZVersePlayer playerZ;


    [Header("朋友列表")]
    public GameObject friendListPrefab;
    public InputField searchField;
    public Transform friendContent;


    [Header("未读消息界面")]
    public GameObject unReadMessageCard;
    public Transform uiReadMessageContent;

    private void Awake()
    {
        InitBase();
    }

    private void Start()
    {
        InitBase();
        friendButtonImage.GetComponent<Button>().onClick.SetListener(delegate { ChangeState(MyFriendChildPanel.FriendList); });
        meButtonImage.GetComponent<Button>().onClick.SetListener(delegate { ChangeState(MyFriendChildPanel.MineInfo); });
        massageButtonImage.GetComponent<Button>().onClick.SetListener(delegate { ChangeState(MyFriendChildPanel.UnreadMessage); });
    }


    /// <summary>
    /// 点击切换页面
    /// </summary>
    /// <param name="panel"></param>
    public void ChangeState(MyFriendChildPanel panel)
    {
        friendButtonImage.color = normalColor;
        meButtonImage.color = normalColor;
        massageButtonImage.color = normalColor;

        friendListPanel.SetActive(false);
        meInfomationPanel.gameObject.SetActive(false);
        massageListPanel.SetActive(false);

        switch (panel)
        {
            case MyFriendChildPanel.UnreadMessage:
                massageButtonImage.color = selectColor;
                massageListPanel.SetActive(true);
                UpdateUnReadMessagePanel();
                break;
            case MyFriendChildPanel.FriendList:
                friendButtonImage.color = selectColor;
                friendListPanel.SetActive(true);
                playerZ.friend.CmdUpdateFriendList();  //刷新好友列表
                break;
            case MyFriendChildPanel.MineInfo:
                meButtonImage.color = selectColor;
                meInfomationPanel.gameObject.SetActive(true);
                meInfomationPanel.Init();
                UpdateMineInfoPanel();
                break;
        }
    }

    /// <summary>
    /// 打开页面
    /// </summary>
    public void ShowPanel()
    {
        ChangeState(MyFriendChildPanel.FriendList);
    }

    #region  朋友列表

    public void onInputValueChange()
    {
        if (string.IsNullOrEmpty(searchField.text))
            UpdateFriendListPanel();
    }

    public void onInputEnd()
    {
        if (string.IsNullOrEmpty(searchField.text))
            UpdateFriendListPanel();
        else
            FindFriends(searchField.text);    //查找用户

    }

    public void UpdateFriendListPanel()
    {
        if (!friendListPanel.activeSelf)
            return;
        //删除原先的项
        for (int i = friendContent.childCount - 1; i >= 0; i--)
            Destroy(friendContent.GetChild(i).gameObject);
        friendContent.DetachChildren();

        UIUtils.BalancePrefabs(friendListPrefab, playerZ.friend.friendList.Count, friendContent); //生成物品栏

        UIFriendListCard[] friendListCards = friendContent.GetComponentsInChildren<UIFriendListCard>(true);

        for (int i = 0; i < playerZ.friend.friendList.Count; i++)
        {
            friendListCards[i].SetInfo(playerZ.friend.friendList[i]);
        }



    }

    public void FindFriends(string name)
    {

        FriendListInfo info = playerZ.friend.friendList.Find(item => item.userName.Equals(name));

        if(info.userId!=0)
        {
            for (int i = friendContent.childCount - 1; i >= 0; i--)
                Destroy(friendContent.GetChild(i).gameObject);
            friendContent.DetachChildren();

            UIUtils.BalancePrefabs(friendListPrefab, 1, friendContent);

            UIFriendListCard friendListCard = friendContent.GetComponentInChildren<UIFriendListCard>();

            friendListCard.SetInfo(info);
        }

       
    }


    #endregion

    #region 未读消息
    public void UpdateUnReadMessagePanel()
    {

        if (!massageListPanel.activeSelf)
            return;
        //删除原先的项
        for (int i = uiReadMessageContent.childCount - 1; i >= 0; i--)
            Destroy(uiReadMessageContent.GetChild(i).gameObject);
        uiReadMessageContent.DetachChildren();


        List<FriendChatInfo> temp = playerZ.friend.chatList.FindAll(item => item.status == 1 && item.receiveId == playerZ.user_id);

        Dictionary<long, List<FriendChatInfo>> tempkv = new Dictionary<long, List<FriendChatInfo>>();

        foreach(var item in temp)
        {
            if (!tempkv.ContainsKey(item.sendId))
                tempkv.Add(item.sendId, new List<FriendChatInfo>());
            tempkv[item.sendId].Add(item);
        }


        UIUtils.BalancePrefabs(unReadMessageCard, tempkv.Keys.Count, uiReadMessageContent); //生成物品栏

        UIUnReadMessageCard[] unReadMessageCards = uiReadMessageContent.GetComponentsInChildren<UIUnReadMessageCard>(true);

        int index = 0;
        foreach (var item in tempkv.Keys)
        {   
            unReadMessageCards[index].SetInfo(tempkv[item]);
            index++;
        }


    }
    #endregion



    #region 设置界面
    public void UpdateMineInfoPanel()
    {

    }
    #endregion



}
