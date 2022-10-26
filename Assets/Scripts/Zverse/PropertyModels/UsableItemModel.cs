/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可用性判断
/// </summary>
public  class UsableItemModel : ItemBaseModel
{

    public float cooldown;  //冷却时间

    public string _cooldownCategory;  //同类型共享的冷却时间，如果只有一个物品，则使用 item_id

    public string cooldownCategory =>

       string.IsNullOrWhiteSpace(_cooldownCategory) ? itemId.ToString() : _cooldownCategory;

    /// <summary>
    /// 判断是否可用
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    /// <returns></returns>
    public virtual bool CanUse(ZVersePlayer player, int inventoryIndex)
    {

        //return player.level.current >= minLevel &&
        //       player.GetItemCooldown(cooldownCategory) == 0 &&
        //       player.inventory.slots[inventoryIndex].item.CheckDurability();

        return player.GetItemCooldown(cooldownCategory)==0&& 
            player.inventory.slots[inventoryIndex].item.CheckDurability();
    }

    /// <summary>
    /// 使用物品，子类调用 需加上 base.Use()
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    /// [Server]
    public virtual void Use(ZVersePlayer player, int inventoryIndex)
    {

        if (cooldown > 0)
            player.SetItemCooldown(cooldownCategory, cooldown);
    }

    /// <summary>
    /// [Client] 客户端使用物品后回调
    /// </summary>
    /// <param name="player"></param>
    public virtual void OnUsed(ZVersePlayer player) { }


}
*/