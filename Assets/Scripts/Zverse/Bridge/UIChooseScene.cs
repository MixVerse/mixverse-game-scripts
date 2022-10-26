using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIChooseScene : PlayerUIPanelBase
{

    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text infomationText;

    public Button[] buttonItems;

    public Button enterScene;

    private int enterId;

    private void Awake()
    {
        InitBase();
    }


    private void Update()
    {
        ZVersePlayer player = ZVersePlayer.localPlayer;
        if(player!=null)
        {
            var list= player.sceneInfo.publicSceneInfo[ZVerseNetworkManager.Instance.publicScenes[0]];
            for(int i=0;i< list.Count;i++)
            {
                buttonItems[i].transform.GetChild(0).GetComponent<Text>().text = list[i].id.ToString();
                buttonItems[i].transform.GetChild(1).GetComponent<Text>().text = list[i].curNum.ToString() + "/" + list[i].maxNum.ToString();
                int copyi = i;
                buttonItems[i].onClick.SetListener(delegate {
                    enterId = list[copyi].id;               
                });
            }

            enterScene.onClick.SetListener(delegate { player.sceneInfo.EnterPublicScene(enterId, ZVerseNetworkManager.Instance.publicScenes[0]); });
        }
    }

    public void GoToScene()
    {
        if(enterId == 0)
        {
            var list = ZVersePlayer.localPlayer.sceneInfo.publicSceneInfo[ZVerseNetworkManager.Instance.publicScenes[0]];
            enterId = list[Random.Range(0, list.Count)].id;
        }
        ZVersePlayer.localPlayer.sceneInfo.EnterPublicScene(enterId, ZVerseNetworkManager.Instance.publicScenes[0]);
    }

    private void OnEnable()
    {
        ZVersePlayer player = ZVersePlayer.localPlayer;
        if (player != null)
        {
            player.sceneInfo.CmdUpdatePublicSceneInfo(ZVerseNetworkManager.Instance.publicScenes[0]);
        }
    }
}
