/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


[Serializable]
public partial struct ItemMallCategory
{
    public string category;
    public ItemBaseModel[] items;
}

[DisallowMultipleComponent]
public class ZVerseItemMall : NetworkBehaviour
{

    public ZVersePlayer player;
    public ZVerseChat chat;
    public ZVersePlayerInventory inventory;

    [SyncVar] public long coins = 0;
    public float couponWaitSeconds = 3;

    public override void OnStartServer()
    {
        InvokeRepeating(nameof(ProcessCoinOrders), 5, 5);
    }

    /// <summary>
    /// 输入优惠券
    /// </summary>
    /// <param name="coupon"></param>
    [Command]
    public void CmdEnterCoupon(string coupon)
    {
        // only allow entering one coupon every few seconds to avoid brute force
        if (NetworkTime.time >= player.nextRiskyActionTime)
        {
            // YOUR COUPON VALIDATION CODE HERE
            // coins += ParseCoupon(coupon);
            Debug.Log("coupon: " + coupon + " => " + name + "@" + NetworkTime.time);
            player.nextRiskyActionTime = NetworkTime.time + couponWaitSeconds;
        }
    }

    [Command]
    public void CmdUnlockItem(int categoryIndex, int itemIndex)
    {

        if (0 <= categoryIndex && categoryIndex <=ItemBaseModel.MallItems.Count &&
            0 <= itemIndex && itemIndex <= ItemBaseModel.MallItems[categoryIndex].items.Length)
        {
            ZVerseItem item = new ZVerseItem(ItemBaseModel.MallItems[categoryIndex].items[itemIndex]);
            if (0 < item.itemMallPrice && item.itemMallPrice <= coins)
            {

                if (inventory.Add(item, 1))
                {
                    coins -= item.itemMallPrice;
                    Debug.Log(item.itemName+"购买成功");

                    //这里无需保存玩家数据，因为在下次保存前客户端崩溃了，金币也没有减
                }
            }
        }
    }

    // coins can't be increased by an external application while the player is
    // ingame. we use an additional table to store new orders in and process
    // them every few seconds from here. this way we can even notify the player
    // after his order was processed successfully.
    //
    // note: the alternative is to keep player.coins in the database at all
    // times, but then we need RPCs and the client needs a .coins value anyway.
    [Server]
    void ProcessCoinOrders()
    {
      //  List<long> orders = Database.singleton.GrabCharacterOrders(name);
      //  foreach (long reward in orders)
      //  {
      //      coins += reward;
      //      Debug.Log("Processed order for: " + name + ";" + reward);
      //      chat.TargetMsgInfo("Processed order for: " + reward);
      //  }
    }
}
*/