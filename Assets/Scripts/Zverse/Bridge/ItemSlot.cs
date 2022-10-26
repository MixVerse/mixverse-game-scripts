using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// 每个物品槽位信息，包括物品信息和物品数量
/// </summary>
[Serializable]
public partial struct ItemSlot
{
    public ZverseItem item;   //物品信息
    public int amount;   //物品当前数量

    // constructors
    public ItemSlot(ZverseItem item, int amount = 1)
    {
        this.item = item;
        this.amount = amount;
    }

    /// <summary>
    /// 减少物品数量
    /// </summary>
    /// <param name="reduceBy"></param>
    /// <returns></returns>
    public int DecreaseAmount(int reduceBy)
    {
        // as many as possible
        int limit = Mathf.Clamp(reduceBy, 0, amount);
        amount -= limit;
        return limit;
    }


    /// <summary>
    /// 增加物品数量
    /// </summary>
    /// <param name="increaseBy"></param>
    /// <returns></returns>
    public int IncreaseAmount(int increaseBy)
    {
        // as many as possible
        int limit = Mathf.Clamp(increaseBy, 0, item.MaxStack - amount);
        amount += limit;
        return limit;
    }
}
