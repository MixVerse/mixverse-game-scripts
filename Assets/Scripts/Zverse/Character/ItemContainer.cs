/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/// <summary>
/// 物品槽位父类  可以是 装备槽,道具槽位等等
/// </summary>
public class ItemContainer : NetworkBehaviour
{

    /// <summary>
    /// 同步槽位列表
    /// </summary>
    public SyncList<ItemSlot> slots = new SyncList<ItemSlot>();

   
    /// <summary>
    /// 获取此槽位所在的物品索引
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public int GetItemIndexByName(string itemName)
    {
        // (avoid FindIndex to minimize allocations)
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];
            if (slot.amount > 0 && slot.item.itemName == itemName)
                return i;
        }
        return -1;
        
    }

    /// <summary>
    /// 计算损失的所有耐久度
    /// </summary>
    /// <returns></returns>
    public int GetTotalMissingDurability()
    {
        int total = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.data.maxDurability > 0)
                total += slot.item.data.maxDurability - slot.item.durability;
        return total;
    }

    /// <summary>
    /// 修复所有物品
    /// </summary>
    [Server]
    public void RepairAllItems()
    {
        for (int i = 0; i < slots.Count; ++i)
        {
            if (slots[i].amount > 0)
            {
                ItemSlot slot = slots[i];
                slot.item.durability = slot.item.maxDurability;
                slots[i] = slot;
            }
        }
    }

}
*/
