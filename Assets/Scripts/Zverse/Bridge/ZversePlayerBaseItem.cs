using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/// <summary>
/// 玩家背包父类
/// </summary>
public class ZversePlayerBaseItem : NetworkBehaviour
{

    /// <summary>
    /// 同步槽位列表
    /// </summary>
    public SyncList<ItemSlot> slots = new SyncList<ItemSlot>();




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
    /// 计算被指定类型物品占用槽位
    /// </summary>
    /// <returns></returns>
    public int SlotsOccupied(Config.EItemType type)
    {
        int occupied = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.Type == type)
                ++occupied;
        return occupied;
    }

    /// <summary>
    /// 计算某个物品数量
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Count(ZverseItem item)
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
    public bool Remove(ZverseItem item, int amount)
    {
        Debug.LogError("删除物品：" + item.Id);
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];

            if (slot.amount > 0 && slot.item.Equals(item))
            {
     
                amount -= slot.DecreaseAmount(amount);
                slots[i] = slot;

                if (amount == 0) return true;
            }
        }


        return false;
    }

    /// <summary>
    /// 添加物品到库存槽(检测是否可以添加)
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool CanAdd(ZverseItem item, int amount)
    {

        for (int i = 0; i < slots.Count; ++i)//循环以0开始，运行次数与库存槽数量相同(当前槽位数量为0时?)
        {

            if (slots[i].amount == 0)//如果当前槽位的物品数量为0
                amount -= item.MaxStack;//添加数量为参数值 - 参数物品的最大叠加值（负数）
            else if (slots[i].item.Equals(item))//如果当前位置物品是要添加的物品
                amount -= (slots[i].item.MaxStack - slots[i].amount);//添加的数量为 参数值 - （当前物品最大叠加值 - 当前物品拥有数量）
            if (amount <= 0) return true;//如果得到的添加数量<=0则返回true
        }

        return false;
    }

    /// <summary>
    /// 添加物品到库存槽
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool Add(ZverseItem item, int amount)
    {

        if (CanAdd(item, amount))//检测是否可以添加
        {
            Debug.LogError("调试：已存在物品添加数量");
            //已存在物品添加数量
            for (int i = 0; i < slots.Count; ++i)
            {

                if (slots[i].amount > 0 && slots[i].item.Equals(item))
                {
                    ItemSlot temp = slots[i];
                    amount -= temp.IncreaseAmount(amount);
                    slots[i] = temp;
                }

                if (amount <= 0) return true;
            }

            Debug.LogError("调试：不存在物品添加新的槽位");
            //不存在物品添加新的槽位
            for (int i = 0; i < slots.Count; ++i)
            {

                if (slots[i].amount == 0)
                {
                    int add = Mathf.Min(amount, item.MaxStack);
                    slots[i] = new ItemSlot(item, add);
                    amount -= add;
                }

                if (amount <= 0) return true;
            }

            if (amount != 0) Debug.LogError("inventory add failed: " + item.MaxStack + " " + amount);
        }
        Debug.LogError("调试：添加失败");
        return false;
    }

    /// <summary>
    /// 获取此槽位所在的物品索引
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public int GetItemIndexByName(string itemId)
    {
        // (avoid FindIndex to minimize allocations)
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];
            if (slot.amount > 0 && slot.item.Id == itemId)
                return i;
        }
        return -1;

    }

}
