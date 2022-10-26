//// Note: this script has to be on an always-active UI parent, so that we can
//// always react to the hotkey.
//using UnityEngine;
//using UnityEngine.UI;

//public partial class UIInventory : MonoBehaviour
//{
//    public static UIInventory singleton;
//    public KeyCode hotKey = KeyCode.I;
//    public GameObject panel;
//    public UIInventorySlot slotPrefab;
//    public Transform content;
//    public Text goldText;
//    public UIDragAndDropable trash;
//    public Image trashImage;
//    public GameObject trashOverlay;
//    public Text trashAmountText;

//    [Header("Durability Colors")]
//    public Color brokenDurabilityColor = Color.red;
//    public Color lowDurabilityColor = Color.magenta;
//    [Range(0.01f, 0.99f)] public float lowDurabilityThreshold = 0.1f;

//    public UIInventory()
//    {

//        if (singleton == null) singleton = this;
//    }

//    void Update()
//    {
//        ZVersePlayer player = ZVersePlayer.localPlayer;
//        if (player != null)
//        {
           
//            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())  //打开物品栏
//                panel.SetActive(!panel.activeSelf);

//            if (panel.activeSelf)
//            {
                
//                UIUtils.BalancePrefabs(slotPrefab.gameObject, player.inventory.slots.Count, content); //生成物品栏

//                //刷新物品栏中物品
//                for (int i = 0; i < player.inventory.slots.Count; ++i)
//                {
//                    UIInventorySlot slot = content.GetChild(i).GetComponent<UIInventorySlot>();
//                    slot.dragAndDropable.name = i.ToString(); 
//                    ItemSlot itemSlot = player.inventory.slots[i];

//                    if (itemSlot.amount > 0)     //每个格子中同类物品数量
//                    {
//                        //为使用物品增加监听
//                        int icopy = i; // lambdas直接用 i 索引会有问题
//                        slot.button.onClick.SetListener(() => {
//                            if (itemSlot.item.data is UsableItemModel usable &&
//                                usable.CanUse(player, icopy))
//                                player.inventory.CmdUseItem(icopy);
//                        });
                        
//                        //移动光标显示详细信息
//                        slot.tooltip.enabled = true;
//                        if (slot.tooltip.IsVisible())
//                            slot.tooltip.text = itemSlot.ToolTip();
//                        slot.dragAndDropable.dragable = true;

//                        //不同耐久度显示不同颜色
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

//                        //加载装备栏物品图片
//                        slot.image.sprite = Resources.Load<Sprite>(itemSlot.item.path);

//                        //物品使用后的冷却旋转进度
//                        if (itemSlot.item.data is UsableItemModel usable2)
//                        {
//                            float cooldown = player.GetItemCooldown(usable2.cooldownCategory);
//                            slot.cooldownCircle.fillAmount = usable2.cooldown > 0 ? cooldown / usable2.cooldown : 0;
//                        }
//                        else slot.cooldownCircle.fillAmount = 0;

//                        //显示该物品数量
//                        slot.amountOverlay.SetActive(itemSlot.amount > 1);
//                        slot.amountText.text = itemSlot.amount.ToString();
//                    }
//                    else
//                    {
//                        // 此物品格子为空时显示的状态
//                        slot.button.onClick.RemoveAllListeners();
//                        slot.tooltip.enabled = false;
//                        slot.dragAndDropable.dragable = false;
//                        slot.image.color = Color.clear;
//                        slot.image.sprite = null;
//                        slot.cooldownCircle.fillAmount = 0;
//                        slot.amountOverlay.SetActive(false);
//                    }
//                }

//                //用户金币数量
//                goldText.text = player.gold.ToString();

//                // 等待删除的物品
//                trash.dragable = player.inventory.trash.amount > 0;
//                if (player.inventory.trash.amount > 0)
//                {

//                    if (player.inventory.trash.item.maxDurability > 0)
//                    {
//                        if (player.inventory.trash.item.durability == 0)
//                            trashImage.color = brokenDurabilityColor;
//                        else if (player.inventory.trash.item.DurabilityPercent() < lowDurabilityThreshold)
//                            trashImage.color = lowDurabilityColor;
//                        else
//                            trashImage.color = Color.white;
//                    }
//                    else trashImage.color = Color.white; 
//                        trashImage.sprite = Resources.Load<Sprite>(player.inventory.trash.item.path);

//                    trashOverlay.SetActive(player.inventory.trash.amount > 1);
//                    trashAmountText.text = player.inventory.trash.amount.ToString();
//                }
//                else
//                {

//                    trashImage.color = Color.clear;
//                    trashImage.sprite = null;
//                    trashOverlay.SetActive(false);
//                }
//            }
//        }
//        else panel.SetActive(false);
//    }
//}
