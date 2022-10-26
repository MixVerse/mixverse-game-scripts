/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/// <summary>
/// ��Ʒ��λ����  ������ װ����,���߲�λ�ȵ�
/// </summary>
public class ItemContainer : NetworkBehaviour
{

    /// <summary>
    /// ͬ����λ�б�
    /// </summary>
    public SyncList<ItemSlot> slots = new SyncList<ItemSlot>();

   
    /// <summary>
    /// ��ȡ�˲�λ���ڵ���Ʒ����
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
    /// ������ʧ�������;ö�
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
    /// �޸�������Ʒ
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
