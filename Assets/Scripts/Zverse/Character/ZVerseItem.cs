/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public partial struct ZVerseItem 
{
    public string itemId;   //表示此物品的唯一ID，无论配置文件还是数据库都需要此ID

    public int durability;    //当前物品的耐久度

    public ZVerseItem(ItemBaseModel data)
    {
        itemId = data.itemId;
        durability = data.maxDurability;
    }

    public ItemBaseModel data
    {
        get
        {

            if (!ItemBaseModel.AllItem.ContainsKey(itemId))
                throw new KeyNotFoundException("为找到 itemId="+itemId+"的物品缓存");
            return ItemBaseModel.AllItem[itemId];
        }
    }


    public string itemName => data.itemName;  //物品名称
    public int maxStack => data.maxStack;    //物品最大库存
    public int maxDurability => data.maxDurability;   //物品最大耐久度
    public float DurabilityPercent()
    {
        return (durability != 0 && maxDurability != 0) ? (float)durability / (float)maxDurability : 0;
    }
    public long buyPrice => data.buyPrice;  //物品购买价格
    public long sellPrice => data.sellPrice; //物品出售价格
    public long itemMallPrice => data.itemMallPrice;  //物品商城交易价格
    public bool sellable => data.sellable;  //是否可出售
    public bool tradable => data.tradable;   //是否可交易   
    public bool destroyable => data.destroyable;   //是否已下架 或删除
    public string path => data.path;  //物品突变加载路径

    // helper function to check for valid durability if a durability item
    public bool CheckDurability() =>
        maxDurability == 0 || durability > 0;

    public string ToolTip()
    {

        StringBuilder tip = new StringBuilder(data.ToolTip());

        if (maxDurability > 0)
            tip.Replace("{DURABILITY}", (DurabilityPercent() * 100).ToString("F0"));


        Utils.InvokeMany(typeof(ZVerseItem), this, "ToolTip_", tip);

        return tip.ToString();
    }

}
*/
