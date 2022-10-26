/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可用性判断
/// </summary>
public class UsableItem : ItemBaseModel
{

    public float cooldown; // potion usage interval, etc.
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

        return player.inventory.slots[inventoryIndex].item.CheckDurability();
    }

    // [Server] Use logic: make sure to call base.Use() in overrides too.
    public virtual void Use(ZVersePlayer player, int inventoryIndex)
    {
        // start cooldown (if any)
        // -> no need to set sync dict dirty if we have no cooldown
       // if (cooldown > 0)
       //     player.SetItemCooldown(cooldownCategory, cooldown);
    }

    // [Client] OnUse Rpc callback for effects, sounds, etc.
    // -> can't pass slotIndex because .Use might clear it before getting here already
    public virtual void OnUsed(ZVersePlayer player) { }


}
*/