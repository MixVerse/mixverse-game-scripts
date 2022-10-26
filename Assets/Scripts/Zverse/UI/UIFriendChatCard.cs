using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendChatCard : MonoBehaviour
{

    public Image icon;

    public Text messageTex;


    public void  SetInfo(FriendChatInfo info)
    {
        messageTex.text = info.message;
    }
}
