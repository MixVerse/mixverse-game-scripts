//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class UICostume : MonoBehaviour
//{
//    public KeyCode hotKey = KeyCode.U; // 'E' is already used for rotating
//    public GameObject panel;
//    public UICostumeSlot slotPrefab;
//    public Transform content;

//    [Header("Durability Colors")]
//    public Color brokenDurabilityColor = Color.red;
//    public Color lowDurabilityColor = Color.magenta;
//    [Range(0.01f, 0.99f)] public float lowDurabilityThreshold = 0.1f;

//    void Update()
//    {
//        ZVersePlayer player = ZVersePlayer.localPlayer;
//        if (player)
//        {
            
//            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
//                panel.SetActive(!panel.activeSelf);

//            //开启人物着装预览摄像机
//            player.costume.avatarCamera.enabled = panel.activeSelf;

//            if (panel.activeSelf)
//            {
        
//                UIUtils.BalancePrefabs(slotPrefab.gameObject, player.costume.slots.Count, content);

//                for (int i = 0; i < player.costume.slots.Count; ++i)
//                {
//                    UICostumeSlot slot = content.GetChild(i).GetComponent<UICostumeSlot>();
//                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
//                    ItemSlot itemSlot = player.costume.slots[i];

//                    //显示衣服显示部位说明
//                    CostumeInfo slotInfo = player.costume.slotInfo[i];
//                    slot.categoryOverlay.SetActive(slotInfo.requiredCategory != "");
//                    string overlay = Utils.ParseLastNoun(slotInfo.requiredCategory);
//                    slot.categoryText.text = overlay != "" ? overlay : "?";

//                    if (itemSlot.amount > 0)
//                    {
//                        //移动光标的详细信息说明
//                        slot.tooltip.enabled = true;
//                        if (slot.tooltip.IsVisible())
//                            slot.tooltip.text = itemSlot.ToolTip();
//                        slot.dragAndDropable.dragable = true;

//                        //耐久度颜色显示
//                        if (itemSlot.item.maxDurability > 0)
//                        {
//                            if (itemSlot.item.durability == 0)
//                                slot.image.color = brokenDurabilityColor;
//                            else if (itemSlot.item.DurabilityPercent() < lowDurabilityThreshold)
//                                slot.image.color = lowDurabilityColor;
//                            else
//                                slot.image.color = Color.white;
//                        }
//                        else slot.image.color = Color.white; 

//                        //衣服图片加载
//                        slot.image.sprite = Resources.Load<Sprite>(itemSlot.item.path);

//                        //冷却显示
//                        if (itemSlot.item.data is UsableItemModel usable)
//                        {
//                            float cooldown = player.GetItemCooldown(usable.cooldownCategory);
//                            slot.cooldownCircle.fillAmount = usable.cooldown > 0 ? cooldown / usable.cooldown : 0;
//                        }
//                        else slot.cooldownCircle.fillAmount = 0;

//                        // 数量显示
//                        slot.amountOverlay.SetActive(itemSlot.amount > 1);
//                        slot.amountText.text = itemSlot.amount.ToString();
//                    }
//                    else
//                    {
//                        //  空的衣服栏显示
//                        slot.tooltip.enabled = false;
//                        slot.dragAndDropable.dragable = false;
//                        slot.image.color = Color.clear;
//                        slot.image.sprite = null;
//                        slot.cooldownCircle.fillAmount = 0;
//                        slot.amountOverlay.SetActive(false);
//                    }
//                }
//            }
//        }
//        else panel.SetActive(false);
//    }
//}
