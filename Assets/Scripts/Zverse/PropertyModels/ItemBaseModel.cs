/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

/// <summary>
/// 物品父类模型
/// </summary>
[Serializable]
public partial class ItemBaseModel 
{

    public string itemId;   //物品唯一id
    public string itemName;  //物品名称
    public int maxStack;   //最大库存
    public int maxDurability = 0; //最大耐久度
    public long buyPrice;     //购买价格
    public long sellPrice;    //出售价格
    public long itemMallPrice;   //商城交易价格
    public bool sellable;      //是否可出售
    public bool tradable;       //是否可交易
    public bool destroyable;    //删除或下架
    public string toolTip;  //移动到物品上面显示的信息
    public string path;    //图片或模型路径

    static Dictionary<string, ItemBaseModel> itemCache;
    public static Dictionary<string, ItemBaseModel> AllItem   //所有物品缓存,KEY为 itemid
    {
        get
        {
            // not loaded yet?
            if (itemCache == null)
            {
                // get all ScriptableItems in resources
                ItemBaseModel[] items = SettingLoading.Instance.LoadAllItem(); //正常使用需读取文件或数据库方法

                List<string> duplicates = items.ToList().FindDuplicates(item => item.itemId);
                if (duplicates.Count == 0)  //筛选重复的物品
                {
                    itemCache = items.ToDictionary(item => item.itemId, item => item);
                }
                else
                {
                    foreach (var duplicate in duplicates)
                        Debug.LogError("未找到配置物品itemid:"+duplicate);
                }
            }
            return itemCache;
        }
    }


    static List<ItemMallCategory> itemMallCache;
    public static List<ItemMallCategory> MallItems   //商店物品缓存,Key为物品分类
    {
        get
        {
            // not loaded yet?
            if (itemMallCache == null)
            {
                itemMallCache = SettingLoading.Instance.LoadMallItems();
            }
            return itemMallCache;
        }
    }




    /// <summary>
    /// 射线移动到物品上面显示的信息
    /// </summary>
    /// <returns></returns>
    public virtual string ToolTip()
    {
        StringBuilder tip = new StringBuilder(toolTip);
        tip.Replace("{NAME}", itemName);
        tip.Replace("{DESTROYABLE}", (destroyable ? "是" : "否"));
        tip.Replace("{SELLABLE}", (sellable ? "是" : "否"));
        tip.Replace("{TRADABLE}", (tradable ? "是" : "否"));
        tip.Replace("{BUYPRICE}", buyPrice.ToString());
        tip.Replace("{SELLPRICE}", sellPrice.ToString());
        return tip.ToString();
        
    }



}

[Serializable]
public struct ItemBaseModelAndAmount
{
    public ItemBaseModel item;
    public int amount;
}
*/