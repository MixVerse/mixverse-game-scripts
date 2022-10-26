using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Config;

public class UIWeaponPack : PlayerUIPanelBase
{
    public RectTransform content;//物品格子
    public GameObject selectButtonPanel;//选择按钮面板
    public GameObject selectButton;//选择按钮面板
    public GameObject itemGrldObj;
    public ItemButton[] itemButton;


    // Start is called before the first frame update
    void Awake()
    {
        InitBase();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !UIUtils.AnyInputActive())
            OnOff();                                     

        ZVersePlayer player = ZVersePlayer.localPlayer;
        if(player!=null && panel.activeSelf)
        {
           
            UpdateGridNumber(player);
           // ContentMove();

        }

    }


    /// <summary>
    /// 删除选择的物品
    /// </summary>
    public void DeletePitchOnItem()
    {

        if (ZVersePlayer.localPlayer != null)
        {
            List<string> itemids = new List<string>();
            for (int i = 0; i < content.childCount; i++)
            {
                if (itemButton[i].selectedToggle.gameObject.activeSelf && itemButton[i].selectedToggle.isOn)
                {
                    itemids.Add(itemButton[i].itemID);
                    //Destroy(itemButton[i]);
                }
            }
            ZVersePlayer.localPlayer.weapon.CmdDeleteItems(itemids);
        }

      
    }


    /// <summary>
    /// 更新格子数量
    /// </summary>
    public void UpdateGridNumber(ZVersePlayer player)
    {

        UIUtils.BalancePrefabs(itemGrldObj, player.weapon.slots.Count, content); //生成物品栏


        itemButton = null;
        itemButton = content.GetComponentsInChildren<ItemButton>();

        for (int i = 0; i < player.weapon.slots.Count; ++i)
        {
   
            ItemSlot itemSlot = player.weapon.slots[i];

            if (itemSlot.amount > 0)     //每个格子中同类物品数量
            {
                itemButton[i].ChangeShow(itemSlot);

            }
            else
            {

                itemButton[i].Close();
            }
        }

        OnOffItemToggle();

    }

    public void OnOffItemToggle()
    {

        if(ZVersePlayer.localPlayer!=null)
        {
            for (int i = 0; i < itemButton.Length; i++)
            {
                if (string.IsNullOrEmpty(itemButton[i].itemID) || !selectButtonPanel.activeSelf)
                {
                    itemButton[i].selectedToggle.gameObject.SetActive(false);
                    itemButton[i].selectedToggle.isOn = false;
                }else if (selectButtonPanel.activeSelf)
                {
                    itemButton[i].selectedToggle.gameObject.SetActive(true);
                }
                
            }
        }
       
    }

    public void Close()
    {
        selectButton.SetActive(true);
        selectButtonPanel.SetActive(false);
        content.localPosition = Vector3.zero;
        OnOffItemToggle();
    }





    [SerializeField] private float contentMoveSpeed = 10;
    void ContentMove()
    {
        if (!panel.activeSelf)
        {
            return;
        }
        if (player.GetPlayer().inputManager.GetLRockerV2().y > 0.1f || player.GetPlayer().inputManager.GetRRockerV2().y > 0.1f)
        {
            Debug.LogError("调试：向上");
            content.localPosition += new Vector3(0, Time.deltaTime * contentMoveSpeed, 0);
        }
        else
        {
            if (player.GetPlayer().inputManager.GetLRockerV2().y < -0.1f || player.GetPlayer().inputManager.GetRRockerV2().y < -0.1f)
            {
                Debug.LogError("调试：向下");
                content.localPosition -= new Vector3(0, Time.deltaTime * contentMoveSpeed, 0);
            }
        }
    }
}
