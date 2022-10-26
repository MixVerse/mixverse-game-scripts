/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

/// <summary>
/// �û���Ʒ��
/// </summary>
public class ZVersePlayerInventory : InventoryBase
{

    public ZVersePlayer player;

    public int size = 30;
    public ItemBaseModelAndAmount[] defaultItems;
    public KeyCode[] splitKeys = { KeyCode.LeftShift, KeyCode.RightShift }; 


    [SyncVar] public ItemSlot trash;

    /// <summary>
    /// �Ƿ������������Ʒ��������������н���
    /// </summary>
    /// <returns></returns>
    public bool InventoryOperationsAllowed()
    {
        return true;
    }


    /// <summary>
    /// ɾ����Ʒ������Ʒ,,�������վ
    /// </summary>
    /// <param name="inventoryIndex"></param>
    [Command]
    public void CmdSwapInventoryTrash(int inventoryIndex)
    {
        
        if (InventoryOperationsAllowed() &&
            0 <= inventoryIndex && inventoryIndex < slots.Count)
        {
  
            ItemSlot slot = slots[inventoryIndex];
            if (slot.amount > 0 && slot.item.destroyable)
            {
                // overwrite trash
                trash = slot;

                // clear inventory slot
                slot.amount = 0;
                slots[inventoryIndex] = slot;
            }
        }
    }

    /// <summary>
    /// �ӻ���վ�ָ���Ʒ
    /// </summary>
    /// <param name="inventoryIndex"></param>
    [Command]
    public void CmdSwapTrashInventory(int inventoryIndex)
    {
        if (InventoryOperationsAllowed() &&
            0 <= inventoryIndex && inventoryIndex < slots.Count)
        {
            // inventory slot has to be empty or destroyable
            ItemSlot slot = slots[inventoryIndex];
            if (slot.amount == 0 || slot.item.destroyable)
            {
                // swap them
                slots[inventoryIndex] = trash;
                trash = slot;
            }
        }
    }

    /// <summary>
    /// ����Ʒ���е�����Ʒλ��
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    [Command]
    public void CmdSwapInventoryInventory(int fromIndex, int toIndex)
    {

        if (InventoryOperationsAllowed() &&
            0 <= fromIndex && fromIndex < slots.Count &&
            0 <= toIndex && toIndex < slots.Count &&
            fromIndex != toIndex)
        {
            // swap them
            ItemSlot temp = slots[fromIndex];
            slots[fromIndex] = slots[toIndex];
            slots[toIndex] = temp;
        }
    }

    /// <summary>
    /// ����Ʒ������Ʒ�ֳ�����
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    [Command]
    public void CmdInventorySplit(int fromIndex, int toIndex)
    {
      
        if (InventoryOperationsAllowed() &&
            0 <= fromIndex && fromIndex < slots.Count &&
            0 <= toIndex && toIndex < slots.Count &&
            fromIndex != toIndex)
        {
            // slotFrom needs at least two to split, slotTo has to be empty
            ItemSlot slotFrom = slots[fromIndex];
            ItemSlot slotTo = slots[toIndex];
            if (slotFrom.amount >= 2 && slotTo.amount == 0)
            {
                // split them serversided (has to work for even and odd)
                slotTo = slotFrom; // copy the value

                slotTo.amount = slotFrom.amount / 2;
                slotFrom.amount -= slotTo.amount; // works for odd too

                // put back into the list
                slots[fromIndex] = slotFrom;
                slots[toIndex] = slotTo;
            }
        }
    }

    /// <summary>
    /// ��Ʒ������Ʒ�ϲ�
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    [Command]
    public void CmdInventoryMerge(int fromIndex, int toIndex)
    {
        if (InventoryOperationsAllowed() &&
            0 <= fromIndex && fromIndex < slots.Count &&
            0 <= toIndex && toIndex < slots.Count &&
            fromIndex != toIndex)
        {
            // both items have to be valid
            ItemSlot slotFrom = slots[fromIndex];
            ItemSlot slotTo = slots[toIndex];
            if (slotFrom.amount > 0 && slotTo.amount > 0)
            {
                // make sure that items are the same type
                // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                if (slotFrom.item.Equals(slotTo.item))
                {
                    // merge from -> to
                    // put as many as possible into 'To' slot
                    int put = slotTo.IncreaseAmount(slotFrom.amount);
                    slotFrom.DecreaseAmount(put);

                    // put back into the list
                    slots[fromIndex] = slotFrom;
                    slots[toIndex] = slotTo;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcUsedItem(ZVerseItem item)
    {
        // validate
        if (item.data is UsableItemModel usable)
        {
            usable.OnUsed(player);
        }
    }

    /// <summary>
    /// ʹ��ĳ����Ʒ
    /// </summary>
    /// <param name="index"></param>
    [Command]
    public void CmdUseItem(int index)
    {
        // validate
        if (InventoryOperationsAllowed() &&
            0 <= index && index < slots.Count && slots[index].amount > 0 &&
            slots[index].item.data is UsableItemModel usable)
        {
           
            if (usable.CanUse(player, index))
            {
                
                ZVerseItem item = slots[index].item;
                usable.Use(player, index);
                RpcUsedItem(item);
            }
        }
    }

    /// <summary>
    /// �ϲ����ֿ���������Ʒ����Ʒ
    /// </summary>
    /// <param name="slotIndices"></param>
    void OnDragAndDrop_InventorySlot_InventorySlot(int[] slotIndices)
    {

        if (slots[slotIndices[0]].amount > 0 && slots[slotIndices[1]].amount > 0 &&
            slots[slotIndices[0]].item.Equals(slots[slotIndices[1]].item))
        {
            CmdInventoryMerge(slotIndices[0], slotIndices[1]);
        }
        else if (Utils.AnyKeyPressed(splitKeys))
        {
            CmdInventorySplit(slotIndices[0], slotIndices[1]);
        }
        else
        {
            CmdSwapInventoryInventory(slotIndices[0], slotIndices[1]);
        }
    }

    /// <summary>
    /// ��Ʒ�������վ
    /// </summary>
    /// <param name="slotIndices"></param>
    void OnDragAndDrop_InventorySlot_TrashSlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        CmdSwapInventoryTrash(slotIndices[0]);
    }

    /// <summary>
    /// ��Ʒ�ӻ���վ�ָ�
    /// </summary>
    /// <param name="slotIndices"></param>
    void OnDragAndDrop_TrashSlot_InventorySlot(int[] slotIndices)
    {
        // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
        CmdSwapTrashInventory(slotIndices[1]);
    }

    // validation
    void OnValidate()
    {
        // defaultItems is null when first adding the component. avoid error.
        if (defaultItems != null)
        {
            // it's easy to set a default item and forget to set amount from 0 to 1
            // -> let's do this automatically.
            for (int i = 0; i < defaultItems.Length; ++i)
                if (defaultItems[i].item != null && defaultItems[i].amount == 0)
                    defaultItems[i].amount = 1;
        }

        // force syncMode to observers for now.
        // otherwise trade offer items aren't shown when trading with someone
        // else, because we can't see the other person's inventory slots.
        if (syncMode != SyncMode.Observers)
        {
            syncMode = SyncMode.Observers;
#if UNITY_EDITOR
            Undo.RecordObject(this, name + " " + GetType() + " component syncMode changed to Observers.");
#endif
        }
    }
    /// <summary>
    /// ������Ʒ����Ʒ
    /// </summary>
    /// <param name="inventory"></param>
    [Server]
    public void LoadInventory()
    {

        for (int i = 0; i < size; ++i)
            slots.Add(new ItemSlot());

        List<zverse_inventory> inventoryList = zverse_inventory_dao.QueryUserInventory(player.user_id);  //��ȡ�û���װ����

        foreach (var inventory in inventoryList)
        {
            if (inventory.slot_index < size)  //λ�úϷ�
            {
                if (ItemBaseModel.AllItem.TryGetValue(inventory.item_id, out ItemBaseModel itemData))
                {
                    ZVerseItem item = new ZVerseItem(itemData);
                    item.durability = Mathf.Min(inventory.durability, item.maxDurability);
                    item.itemId = itemData.itemId;
                    slots[inventory.slot_index] = new ItemSlot(item, inventory.amount);
                }
                else
                {
                    Debug.LogError("LoadInventoryδ�ҵ�����Ʒ������Ϣ");
                }
            }
            else
            {
                Debug.LogError("LoadInventory����װ�޷�װ��");
            }
        }

    }
}
*/
