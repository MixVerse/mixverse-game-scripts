using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Config;

public class ZversePlayerWeapon : ZversePlayerBaseItem
{

    public ZVersePlayer player;

    public int size = 90;


    /// <summary>
    /// 是否允许玩家与物品栏（库存栏）进行交互
    /// </summary>
    /// <returns></returns>
    public bool InventoryOperationsAllowed()
    {
        return true;
    }
    public override void OnStartServer()
    {
        LoadWeapon();
    }


    public override void OnStartClient()
    {
        slots.Callback += SyncListChanged;
    }
    public void SyncListChanged(SyncList<ItemSlot>.Operation op, int itemIndex, ItemSlot oldItem, ItemSlot newItem)
    {
        ZVersePlayer.localPlayer.player.leftController.leftHandUI.class_Packsack.UpdateGrid();
        ZVersePlayer.localPlayer.player.leftController.leftHandUI.class_DecoratePut.ShowCurrectTypeItem();
    }

    public List<ItemSlot> GetAllItemToType(EItemType type)
    {
        List<ItemSlot> itemSlots = new List<ItemSlot>();
        foreach (ItemSlot slot in slots)
        {
            if (slot.amount > 0 && slot.item.Type == type)
            {
                itemSlots.Add(slot);
            }
        }
        return itemSlots;
    }
    /// <summary>
    /// 在物品栏中调整物品位置
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
    /// 将物品栏中物品分成两格
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
    /// 物品栏中物品合并
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

    /// <summary>
    /// 删除物品
    /// </summary>
    /// <param name="itemids"></param>
    [Command]
    public void CmdDeleteItems(List<string> itemids)
    {

         foreach(var id in itemids)
         {
            ItemSlot temp = slots.Find(slot => slot.amount>0 && slot.item.itemId.Equals(id));
            Remove(temp.item, 1);
         }
    }

    [Command]
    public void CmdDeleteItemsSingle(string itemid)
    {
        ItemSlot temp = slots.Find(slot => slot.amount > 0 && slot.item.itemId.Equals(itemid));
        Remove(temp.item, 1);
    }

    [ClientRpc]
    public void RpcUsedItem(ZverseItem item)
    {

    }

    /// <summary>
    /// 使用某个物品
    /// </summary>
    /// <param name="index"></param>
    [Command]
    public void CmdUseItem(int index)
    {


        ZverseItem item = slots[index].item;

        RpcUsedItem(item);


    }
    [Command]
    public void AddItem(ZverseItem item, int count)
    {
        Add(item, count);
    }

    [ClientRpc]
    public void lscAdd(bool isOk)
    {
        Debug.LogError(isOk);
    }

    /// <summary>
    /// 加载物品栏物品
    /// </summary>
    /// <param name="inventory"></param>
    [Server]
    public void LoadWeapon()
    {

        
        for (int i = 0; i < size; ++i)
            slots.Add(new ItemSlot());

        Debug.LogError("调试：当前格子数量为：" + slots.Count);

        List<zverse_weapon> weaponList = zverse_weapon_dao.QueryUserWeapon(player.user_id);  //读取用户着装数据
        if (weaponList == null)
        {
            return;
        }
        foreach (var weapon in weaponList)
        {
            if (weapon.slot_index < size)  //位置合法
            {
                //ItemConfig.Item_Config config = ItemConfig.GetData(weapon.item_id);

                ZverseItem item = new ZverseItem(weapon.item_id);
                item.itemId = weapon.item_id;
                slots[weapon.slot_index] = new ItemSlot(item, weapon.amount);
            }
            else
            {
                Debug.LogError("LoadInventory此着装无法装备");
            }
        }

    }

    [Server]
    public void UpdateItems()
    {
        List<zverse_weapon> list = new List<zverse_weapon>();
        for (int i = 0; i < player.weapon.slots.Count; i++)
        {
            if (player.weapon.slots[i].amount > 0)
            {
                zverse_weapon w = new zverse_weapon();
                w.user_id = player.user_id;
                w.item_id = player.weapon.slots[i].item.itemId;
                w.item_name = player.weapon.slots[i].item.Name;
                w.slot_index = i;
                w.amount = player.weapon.slots[i].amount;
                list.Add(w);
            }

        }
        zverse_weapon_dao.UpdateInfo(player.user_id, list);
    }
}
