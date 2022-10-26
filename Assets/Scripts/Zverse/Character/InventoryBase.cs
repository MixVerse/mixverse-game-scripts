/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 库存父类
/// </summary>
[DisallowMultipleComponent]
public class InventoryBase : ItemContainer
{

    /// <summary>
    /// 计算空闲槽位
    /// </summary>
    /// <returns></returns>
    public int SlotsFree()
    {
        int free = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount == 0)
                ++free;
        return free;
    }

    /// <summary>
    /// 计算被占用槽位
    /// </summary>
    /// <returns></returns>
    public int SlotsOccupied()
    {
        int occupied = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0)
                ++occupied;
        return occupied;
    }

    /// <summary>
    /// 计算某个物品数量
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Count(ZVerseItem item)
    {
        int amount = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.Equals(item))
                amount += slot.amount;
        return amount;
    }

    /// <summary>
    /// 从库存中删除某个物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Remove(ZVerseItem item, int amount)
    {
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];
      
            if (slot.amount > 0 && slot.item.Equals(item))
            {
                // take as many as possible
                amount -= slot.DecreaseAmount(amount);
                slots[i] = slot;

                // are we done?
                if (amount == 0) return true;
            }
        }


        return false;
    }

    /// <summary>
    /// 添加物品到库存槽
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CanAdd(ZVerseItem item, int amount)
    {
        // go through each slot
        for (int i = 0; i < slots.Count; ++i)
        {
            // empty? then subtract maxstack
            if (slots[i].amount == 0)
                amount -= item.maxStack;
            // not empty. same type too? then subtract free amount (max-amount)
            // note: .Equals because name AND dynamic variables matter (petLevel etc.)
            else if (slots[i].item.Equals(item))
                amount -= (slots[i].item.maxStack - slots[i].amount);

            // were we able to fit the whole amount already?
            if (amount <= 0) return true;
        }

        // if we got here than amount was never <= 0
        return false;
    }

    /// <summary>
    /// 添加物品到库存槽
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Add(ZVerseItem item, int amount)
    {

        if (CanAdd(item, amount))
        {
            //已存在物品添加数量
            for (int i = 0; i < slots.Count; ++i)
            {
                // not empty and same type? then add free amount (max-amount)
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                if (slots[i].amount > 0 && slots[i].item.Equals(item))
                {
                    ItemSlot temp = slots[i];
                    amount -= temp.IncreaseAmount(amount);
                    slots[i] = temp;
                }

                // were we able to fit the whole amount already? then stop loop
                if (amount <= 0) return true;
            }

            //不存在物品添加新的槽位
            for (int i = 0; i < slots.Count; ++i)
            {
                // empty? then fill slot with as many as possible
                if (slots[i].amount == 0)
                {
                    int add = Mathf.Min(amount, item.maxStack);
                    slots[i] = new ItemSlot(item, add);
                    amount -= add;
                }

                // were we able to fit the whole amount already? then stop loop
                if (amount <= 0) return true;
            }
            // we should have been able to add all of them
            if (amount != 0) Debug.LogError("inventory add failed: " + item.itemName + " " + amount);
        }
        return false;
    }
}
*/