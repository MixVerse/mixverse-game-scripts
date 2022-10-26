using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAddFriendCard : MonoBehaviour
{

    public Image icon;
    public Text userNameTex;
    public InputField applyReasonTex;

    public Button addBtn;


    private ZVersePlayer player;
    private void OnEnable()
    {
        player = GetComponentInParent<ZVersePlayer>();
    }


    public void SetInfo(ApplyInfo info)
    {

        userNameTex.text = info.userName;
        //applyReasonTex.text = info.applyMessage;

        addBtn.onClick.SetListener(delegate {
            player.friend.CmdAddFriendApplication(info.userId,info.userName, applyReasonTex.text);

        });
    }
}
