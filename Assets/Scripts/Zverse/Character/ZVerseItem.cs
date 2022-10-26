/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public partial struct ZVerseItem 
{
    public string itemId;   //��ʾ����Ʒ��ΨһID�����������ļ��������ݿⶼ��Ҫ��ID

    public int durability;    //��ǰ��Ʒ���;ö�

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
                throw new KeyNotFoundException("Ϊ�ҵ� itemId="+itemId+"����Ʒ����");
            return ItemBaseModel.AllItem[itemId];
        }
    }


    public string itemName => data.itemName;  //��Ʒ����
    public int maxStack => data.maxStack;    //��Ʒ�����
    public int maxDurability => data.maxDurability;   //��Ʒ����;ö�
    public float DurabilityPercent()
    {
        return (durability != 0 && maxDurability != 0) ? (float)durability / (float)maxDurability : 0;
    }
    public long buyPrice => data.buyPrice;  //��Ʒ����۸�
    public long sellPrice => data.sellPrice; //��Ʒ���ۼ۸�
    public long itemMallPrice => data.itemMallPrice;  //��Ʒ�̳ǽ��׼۸�
    public bool sellable => data.sellable;  //�Ƿ�ɳ���
    public bool tradable => data.tradable;   //�Ƿ�ɽ���   
    public bool destroyable => data.destroyable;   //�Ƿ����¼� ��ɾ��
    public string path => data.path;  //��Ʒͻ�����·��

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
