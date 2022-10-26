using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINewFriend : PlayerUIPanelBase
{
    [Header("需要手动赋值")]
    [Header("Prefab")]
    public GameObject applyFriendCard;
    public GameObject addFriendCard;
    public InputField searchField;

    public Transform applyContent;

    public GameObject childPanel;


    [Header("Player component")]
    public ZVersePlayer player1;


    private void Awake()
    {
        InitBase(); 
    }

    public void ShowPanel()
    {
        searchField.text = "";
        UpdateApplyInfoCard();
    }

    public void onInputValueChange()
    {
        if (string.IsNullOrEmpty(searchField.text))
            UpdateApplyInfoCard();
    }

    public void onInputEnd()
    {
        if (string.IsNullOrEmpty(searchField.text))
            UpdateApplyInfoCard();
        else
            player1.friend.CmdSearchPlayerByName(searchField.text);    //查找用户

    }

    /// <summary>
    /// 更新申请好友列表
    /// </summary>
    public void UpdateApplyInfoCard()
    {
        //删除原先的项
        for (int i=applyContent.childCount-1;i>=0; i--)
            Destroy(applyContent.GetChild(i).gameObject);
        applyContent.DetachChildren();

        UIUtils.BalancePrefabs(applyFriendCard, player1.friend.applyList.Count, applyContent); //生成物品栏

        UIApplyFriendCard[] applyFriendCards = applyContent.GetComponentsInChildren<UIApplyFriendCard>(true);

        for(int i=0;i<player1.friend.applyList.Count;i++)
        {
            applyFriendCards[i].SetInfo(player1.friend.applyList[i]);
        }
    }



    /// <summary>
    /// 更新查找用户结果列表
    /// </summary>
    /// <param name="infoList"></param>
    public void UpdateAddInfoCard(List<ApplyInfo> infoList)
    {

        //删除原先的项
        for (int i = applyContent.childCount - 1; i >= 0; i--)
            Destroy(applyContent.GetChild(i).gameObject);
        applyContent.DetachChildren();

        UIUtils.BalancePrefabs(addFriendCard, infoList.Count, applyContent); //生成物品栏

        UIAddFriendCard[] addFriendCards = applyContent.GetComponentsInChildren<UIAddFriendCard>(true);

        for (int i = 0; i < infoList.Count; i++)
        {
            addFriendCards[i].SetInfo(infoList[i]);
        }
    }



}
