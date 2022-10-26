using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnReadMessageCard : MonoBehaviour
{

    public Image icon;
    public Text sendNameTex;
    public Text messageTex;
    public Text msgCountTex;

    public Button openBtn;

    public void SetInfo(List<FriendChatInfo> infos)
    {
        FriendListInfo info = ZVersePlayer.localPlayer.friend.friendList.Find(item => item.userId == infos[0].sendId);
        if (info.userId != 0)
            sendNameTex.text = info.userName;
        messageTex.text = infos[infos.Count - 1].message;
        msgCountTex.text = infos.Count.ToString();

        openBtn.onClick.SetListener(delegate {

            ZVersePlayer.localPlayer.GetComponentInChildren<FrindPanel>().OnOffChatPanel();
            ZVersePlayer.localPlayer.friend.uiFriendChat.ShowPanel(infos[0].sendId);
        });
    }
}
