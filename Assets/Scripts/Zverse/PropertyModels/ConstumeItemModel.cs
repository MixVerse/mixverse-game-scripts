/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 穿着模型
/// </summary>
public class ConstumeItemModel : UsableItem
{
 
    public string category;  //服装部位
    public int healthBonus;   //血量增益
    public int manaBonus;     //蓝量增益
    public int damageBonus;    //伤害加成
    public int defenseBonus;   //伤害减免


    /// <summary>
    /// 判断是否可以着装
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    /// <returns></returns>
    public override bool CanUse(ZVersePlayer player, int inventoryIndex)
    {
        return FindEquipableSlotFor(player, inventoryIndex) != -1;
    }

    /// <summary>
    /// 判断是否可以着装
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    /// <param name="equipmentIndex"></param>
    /// <returns></returns>
    public bool CanCostume(ZVersePlayer player, int inventoryIndex, int equipmentIndex)
    {
        CostumeInfo slotInfo = (player.costume).slotInfo[equipmentIndex];
        string requiredCategory = slotInfo.requiredCategory;
        return base.CanUse(player, inventoryIndex) &&
               requiredCategory != "" &&
               category.StartsWith(requiredCategory);
    }

    /// <summary>
    /// 寻找是否有此类型着装的槽位
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    /// <returns></returns>
    int FindEquipableSlotFor(ZVersePlayer player, int inventoryIndex)
    {
        for (int i = 0; i < player.costume.slots.Count; ++i)
            if (CanCostume(player, inventoryIndex, i))
                return i;
        return -1;
    }

    /// <summary>
    /// 换装
    /// </summary>
    /// <param name="player"></param>
    /// <param name="inventoryIndex"></param>
    public override void Use(ZVersePlayer player, int inventoryIndex)
    {

        base.Use(player, inventoryIndex);


        int equipmentIndex = FindEquipableSlotFor(player, inventoryIndex);
        if (equipmentIndex != -1)
        {
            ItemSlot inventorySlot = player.inventory.slots[inventoryIndex];
            ItemSlot equipmentSlot = player.costume.slots[equipmentIndex];

            if (inventorySlot.amount > 0 && equipmentSlot.amount > 0 &&
                inventorySlot.item.Equals(equipmentSlot.item))
            {
                (player.costume).MergeInventoryCostume(inventoryIndex, equipmentIndex);
            }
            else
            {
                (player.costume).SwapInventoryCostume(inventoryIndex, equipmentIndex);
            }
        }
    }
}
*/