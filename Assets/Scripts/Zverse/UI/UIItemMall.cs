//using UnityEngine;
//using UnityEngine.UI;
//using Mirror;

//public partial class UIItemMall : MonoBehaviour
//{
//    public KeyCode hotKey = KeyCode.X;
//    public GameObject panel;
//    public Button categorySlotPrefab;
//    public Transform categoryContent;
//    public ScrollRect scrollRect;
//    public UIItemMallSlot itemSlotPrefab;
//    public Transform itemContent;
//    public string buyUrl = "http://unity3d.com/";
//    int currentCategory = 0;
//    public Text nameText;
//    public Text levelText;
//    public Text currencyAmountText;
//    public Button buyButton;
//    public InputField couponInput;
//    public Button couponButton;
//    public GameObject inventoryPanel;

//    void ScrollToBeginning()
//    {
//        // update first so we don't ignore recently added messages, then scroll
//        Canvas.ForceUpdateCanvases();
//        scrollRect.verticalNormalizedPosition = 1;
//    }

//    void Update()
//    {
//        ZVersePlayer player = ZVersePlayer.localPlayer;
//        if (player != null)
//        {
            
//            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
//                panel.SetActive(!panel.activeSelf);

        
//            if (panel.activeSelf)
//            {

//                UIUtils.BalancePrefabs(categorySlotPrefab.gameObject, ItemBaseModel.MallItems.Count, categoryContent);

//                // refresh all category buttons
//                for (int i = 0; i < ItemBaseModel.MallItems.Count; ++i)
//                {
//                    Button button = categoryContent.GetChild(i).GetComponent<Button>();
//                    button.interactable = i != currentCategory;
//                    button.GetComponentInChildren<Text>().text = ItemBaseModel.MallItems[i].category;
//                    int icopy = i; // needed for lambdas, otherwise i is Count
//                    button.onClick.SetListener(() => {
//                        // set new category and then scroll to the top again
//                        currentCategory = icopy;
//                        ScrollToBeginning();
//                    });
//                }

//                if (ItemBaseModel.MallItems.Count > 0)
//                {
//                    // instantiate/destroy enough item slots for that category
//                    ItemBaseModel[] items = ItemBaseModel.MallItems[currentCategory].items;
//                    UIUtils.BalancePrefabs(itemSlotPrefab.gameObject, items.Length, itemContent);

//                    // refresh all items in that category
//                    for (int i = 0; i < items.Length; ++i)
//                    {
//                        UIItemMallSlot slot = itemContent.GetChild(i).GetComponent<UIItemMallSlot>();
//                        ItemBaseModel itemData = items[i];

//                        // refresh item

//                        // only build tooltip while it's actually shown. this
//                        // avoids MASSIVE amounts of StringBuilder allocations.
//                        if (slot.tooltip.IsVisible())
//                            slot.tooltip.text = new ZVerseItem(itemData).ToolTip();
//                        slot.image.color = Color.white;
//                        slot.image.sprite = Resources.Load<Sprite>(itemData.path);
//                        slot.nameText.text = itemData.itemName;
//                        slot.priceText.text = itemData.itemMallPrice.ToString();
//                        slot.unlockButton.interactable =  player.mall.coins >= itemData.itemMallPrice;
//                        int icopy = i; // needed for lambdas, otherwise i is Count
//                        slot.unlockButton.onClick.SetListener(() => {
//                            player.mall.CmdUnlockItem(currentCategory, icopy);
//                            inventoryPanel.SetActive(true); // better feedback
//                        });
//                    }
//                }

//                // overview
//                nameText.text = player.name;
//                levelText.text = "Lv.0";
//                currencyAmountText.text = player.mall.coins.ToString();
//                buyButton.onClick.SetListener(() => { Application.OpenURL(buyUrl); });
//                couponInput.interactable = NetworkTime.time >= player.nextRiskyActionTime;
//                couponButton.interactable = NetworkTime.time >= player.nextRiskyActionTime;
//                couponButton.onClick.SetListener(() => {
//                    if (!string.IsNullOrWhiteSpace(couponInput.text))
//                        player.mall.CmdEnterCoupon(couponInput.text);
//                    couponInput.text = "";
//                });
//            }
//        }
//        else panel.SetActive(false);
//    }
//}
