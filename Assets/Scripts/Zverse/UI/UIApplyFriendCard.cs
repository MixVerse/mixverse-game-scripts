using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIApplyFriendCard : MonoBehaviour
{

    public Image icon;
    public Text userNameTex;
    public Text applyReasonTex;

    public Button agreeBtn;
    public Button refuseBtn;


    private ZVersePlayer player;
    private void OnEnable()
    {
        player = GetComponentInParent<ZVersePlayer>();
    }


    public void SetInfo(ApplyInfo info)
    {

        userNameTex.text = info.userName;
        applyReasonTex.text = info.applyMessage;

        agreeBtn.onClick.SetListener(delegate {
            player.friend.CmdOpperatoinApply(info.userId, 0);
        
        });
        refuseBtn.onClick.SetListener(delegate {
            player.friend.CmdOpperatoinApply(info.userId, 1);

        });
    }
}
