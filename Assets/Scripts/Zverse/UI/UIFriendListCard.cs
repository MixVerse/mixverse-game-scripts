using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendListCard : MonoBehaviour
{


    public Button friendDetailsBtn;
    public Image friendIcon;
    public Text friendName;
    public Text friendStatus;
    public GameObject noLinkMask;



    private ZVersePlayer player;
    private void OnEnable()
    {
        player = GetComponentInParent<ZVersePlayer>();
    }


    public void SetInfo(FriendListInfo info)
    {
        friendName.text = info.userName;
        friendStatus.text = info.status == 1 ? "‘⁄œﬂ" : "¿Îœﬂ";
        noLinkMask.SetActive(info.status != 1);

        //friendDetailsBtn.onClick.SetListener(delegate {
        //    player.GetComponentInChildren<FrindPanel>().OnOffFriendInfomationPanel();
        //    player.friend.myFriendInfo.UpdateInfo(info);
        //});

    }

    public void OnClick()
    {
        FrindPanel frindPanel = GetComponentInParent<FrindPanel>();
        frindPanel.ClickAndOffAllPanel();
        frindPanel.OnOffFriendInfomationPanel();
    }
}
