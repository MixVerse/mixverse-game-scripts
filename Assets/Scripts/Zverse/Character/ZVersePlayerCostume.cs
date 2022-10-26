using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;


[Serializable]
public partial struct CostumeInfo
{
    public string category;    //着装部位
    public Transform location;
}
public class ZVersePlayerCostume : NetworkBehaviour
{
    /*
    [Header("Components")]
    public ZVersePlayer player;
    public Animator animator;

    //服饰物品栏
    public SyncList<ItemSlot> costumeSlot = new SyncList<ItemSlot>();

    //正在着装的服饰
    public readonly SyncDictionary<string, ItemSlot> costumePutOn = new SyncDictionary<string, ItemSlot>();


    [Header("Avatar")]
    public Camera avatarCamera;

    [Header("Equipment Info")]
    public List<CostumeInfo> costumeInfos = new List<CostumeInfo>() {
        new CostumeInfo { category = "Weapon", location = null },   //武器
        new CostumeInfo { category = "Head", location = null },  //头饰
        new CostumeInfo { category = "Chest", location = null },   //胸
        new CostumeInfo { category = "Legs", location = null },    //腿
        new CostumeInfo { category = "Shield", location = null },   //盾
        new CostumeInfo { category = "Shoulders", location = null }, //肩膀
        new CostumeInfo { category = "Hands", location = null },  //手
        new CostumeInfo { category = "Feet", location = null },  //脚
    };

    //服饰的位置
   // private Dictionary<string, Transform> skinBones = new Dictionary<string, Transform>();

    void Awake()
    {
        //加载所有着装节点
        //foreach (SkinnedMeshRenderer skin in GetComponentsInChildren<SkinnedMeshRenderer>())
        //    foreach (Transform bone in skin.bones)
        //        skinBones[bone.name] = bone;
    }

    public override void OnStartClient()
    {
        //注册客户端着装改变回调
        costumeSlot.Callback += OnCostumeSlotChanged;

        costumePutOn.Callback += OnCostumePutOnChanged;

        //初始化着装
        foreach (var kv in costumePutOn)
            RefreshLocation(kv);
    }

    /// <summary>
    ///  着装栏发生变化
    /// </summary>
    /// <param name="op"></param>
    /// <param name="index"></param>
    /// <param name="oldSlot"></param>
    /// <param name="newSlot"></param>
    [ClientCallback]
    void OnCostumeSlotChanged(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldSlot, ItemSlot newSlot)
    {

       // ItemBaseModel oldItem = oldSlot.amount > 0 ? oldSlot.item.data : null;
       // ItemBaseModel newItem = newSlot.amount > 0 ? newSlot.item.data : null;
       // if (oldItem != newItem)
       // {
       //     // update the model
       //     RefreshLocation(index);
       // }
    }

    /// <summary>
    /// 着装栏发生变化
    /// </summary>
    /// <param name="op"></param>
    /// <param name="category"></param>
    /// <param name="item"></param>
    [ClientCallback]
    void OnCostumePutOnChanged(SyncDictionary<string, ItemSlot>.Operation op, string category, ItemSlot item)
    {

         KeyValuePair<string, ItemSlot> kv = new KeyValuePair<string, ItemSlot>(category, item);
         RefreshLocation(kv);
        
    }

    bool CanReplaceAllBones(SkinnedMeshRenderer equipmentSkin)
    {
        // are all equipment SkinnedMeshRenderer bones in the player bones?
        // (avoid Linq because it is HEAVY(!) on GC and performance)
        //foreach (Transform bone in equipmentSkin.bones)
        //    if (!skinBones.ContainsKey(bone.name))
        //        return false;
        return true;
    }

    // replace all equipment SkinnedMeshRenderer bones with the original player
    // bones so that the equipment animation works with IK too
    // (make sure to check CanReplaceAllBones before)
    void ReplaceAllBones(SkinnedMeshRenderer equipmentSkin)
    {
        // get equipment bones
        Transform[] bones = equipmentSkin.bones;

        // replace each one
      //  for (int i = 0; i < bones.Length; ++i)
      //  {
      //      string boneName = bones[i].name;
      //      if (!skinBones.TryGetValue(boneName, out bones[i]))
      //          Debug.LogWarning(equipmentSkin.name + " bone " + boneName + " not found in original player bones. Make sure to check CanReplaceAllBones before.");
      //  }

        // reassign bones
        equipmentSkin.bones = bones;
    }

    void RebindAnimators()
    {
        foreach (Animator anim in GetComponentsInChildren<Animator>())
            anim.Rebind();
    }

    /// <summary>
    /// 刷新任务着装
    /// </summary>
    /// <param name="index"></param>
    [Client]
    public void RefreshLocation(KeyValuePair<string, ItemSlot> kv)
    {
        ItemSlot slot = costumePutOn[kv.Key];
        CostumeInfo costumeInfo = costumeInfos.Find(info => info.category.Equals(kv.Key));

        if (!string.IsNullOrEmpty(costumeInfo.category))
        {
            // 此部位有着装
            if (slot.amount > 0)
            {

                Material modelPrefab = Resources.Load<Material>("Materials/" + slot.item.itemId);
                if (modelPrefab!=null)
                {

                    //连接着装位置
                    SkinnedMeshRenderer equipmentSkin = go.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (equipmentSkin != null && CanReplaceAllBones(equipmentSkin))
                        ReplaceAllBones(equipmentSkin);


                    //如果着装带动画重新绑定
                    Animator anim = go.GetComponent<Animator>();
                    if (anim != null)
                    {
                        // assign main animation controller to it
                        anim.runtimeAnimatorController = animator.runtimeAnimatorController;

                        // restart all animators, so that skinned mesh equipment will be
                        // in sync with the main animation
                        RebindAnimators();
                    }
                }
            }
        }
    }


    #region 换装逻辑

    [Server]
    public void SwapInventoryCostume(int inventoryIndex, int equipmentIndex)
    {
        // validate: make sure that the slots actually exist in the inventory
        // and in the equipment
        if (inventory.InventoryOperationsAllowed() &&
            0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
            0 <= equipmentIndex && equipmentIndex < slots.Count)
        {
            // item slot has to be empty (unequip) or equipabable
            ItemSlot slot = inventory.slots[inventoryIndex];
            if (slot.amount == 0 ||
                slot.item.data is CostumeItemModel itemData &&
                itemData.CanCostume(player, inventoryIndex, equipmentIndex))
            {
                // swap them
                ItemSlot temp = slots[equipmentIndex];
                slots[equipmentIndex] = slot;
                inventory.slots[inventoryIndex] = temp;
            }
        }
    }

    [Command]
    public void CmdSwapInventoryCostume(int inventoryIndex, int equipmentIndex)
    {
        SwapInventoryCostume(inventoryIndex, equipmentIndex);
    }


    [Server]
    public void MergeInventoryCostume(int inventoryIndex, int equipmentIndex)
    {
        if (inventory.InventoryOperationsAllowed() &&
            0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
            0 <= equipmentIndex && equipmentIndex < slots.Count)
        {
            ItemSlot slotFrom = inventory.slots[inventoryIndex];
            ItemSlot slotTo = slots[equipmentIndex];
            if (slotFrom.amount > 0 && slotTo.amount > 0)
            {

                if (slotFrom.item.Equals(slotTo.item))
                {

                    int put = slotTo.IncreaseAmount(slotFrom.amount);
                    slotFrom.DecreaseAmount(put);


                    inventory.slots[inventoryIndex] = slotFrom;
                    slots[equipmentIndex] = slotTo;
                }
            }
        }
    }

    [Command]
    public void CmdMergeInventoryCostume(int equipmentIndex, int inventoryIndex)
    {
        MergeInventoryCostume(equipmentIndex, inventoryIndex);
    }

    [Command]
    public void CmdMergeCostumeInventory(int equipmentIndex, int inventoryIndex)
    {
        if (inventory.InventoryOperationsAllowed() &&
            0 <= inventoryIndex && inventoryIndex < inventory.slots.Count &&
            0 <= equipmentIndex && equipmentIndex < slots.Count)
        {
            // both items have to be valid
            ItemSlot slotFrom = slots[equipmentIndex];
            ItemSlot slotTo = inventory.slots[inventoryIndex];
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
                    slots[equipmentIndex] = slotFrom;
                    inventory.slots[inventoryIndex] = slotTo;
                }
            }
        }
    }

    // durability //////////////////////////////////////////////////////////////
    public void OnDamageDealtTo(Entity victim)
    {
        // reduce weapon durability by one each time we attacked someone
        //  int weaponIndex = GetEquippedWeaponIndex();
        //  if (weaponIndex != -1)
        //  {
        //      ItemSlot slot = slots[weaponIndex];
        //      slot.item.durability = Mathf.Clamp(slot.item.durability - 1, 0, slot.item.maxDurability);
        //      slots[weaponIndex] = slot;
        //  }
    }

    public void OnReceivedDamage(Entity attacker, int damage)
    {
        // reduce durability by one in each equipped item
        for (int i = 0; i < slots.Count; ++i)
        {
            if (slots[i].amount > 0)
            {
                ItemSlot slot = slots[i];
                slot.item.durability = Mathf.Clamp(slot.item.durability - 1, 0, slot.item.maxDurability);
                slots[i] = slot;
            }
        }
    }

    /// <summary>
    /// 拖动物品从库存槽到换装槽
    /// </summary>
    /// <param name="slotIndices"></param>
    void OnDragAndDrop_InventorySlot_CostumeSlot(int[] slotIndices)
    {

        if (inventory.slots[slotIndices[0]].amount > 0 && slots[slotIndices[1]].amount > 0 &&
            inventory.slots[slotIndices[0]].item.Equals(slots[slotIndices[1]].item))
        {
            CmdMergeInventoryCostume(slotIndices[0], slotIndices[1]);
        }
        else
        {
            CmdSwapInventoryCostume(slotIndices[0], slotIndices[1]);
        }
    }

    /// <summary>
    /// 拖动物品从换装槽到库存槽
    /// </summary>
    /// <param name="slotIndices"></param>
    void OnDragAndDrop_CostumeSlot_InventorySlot(int[] slotIndices)
    {

        if (slots[slotIndices[0]].amount > 0 && inventory.slots[slotIndices[1]].amount > 0 &&
            slots[slotIndices[0]].item.Equals(inventory.slots[slotIndices[1]].item))
        {
            CmdMergeCostumeInventory(slotIndices[0], slotIndices[1]);
        }
        else
        {
            CmdSwapInventoryCostume(slotIndices[1], slotIndices[0]);
        }
    }

    #endregion

    /// <summary>
    /// 加载着装数据
    /// </summary>
    [Server]
    public void LoadCostume()
    {
        // fill all slots first
        for (int i = 0; i < slotInfo.Length; ++i)
            slots.Add(new ItemSlot());

        List<zverse_costume> costumeList = zverse_costume_dao.QueryUserCostume(player.user_id);  //读取用户着装数据

        foreach (var costume in costumeList)
        {
            if (costume.slot_index < slotInfo.Length)  //位置合法
            {
                if (ItemBaseModel.AllItem.TryGetValue(costume.item_id, out ItemBaseModel itemData))
                {
                    ZVerseItem item = new ZVerseItem(itemData);
                    item.durability = Mathf.Min(costume.durability, item.maxDurability);
                    slots[costume.slot_index] = new ItemSlot(item, costume.amount);
                }
                else
                {
                    Debug.LogError("LoadCostume未找到此物品配置信息");
                }
            }
            else
            {
                Debug.LogError("LoadCostume此着装无法装备");
            }
        }


    }



    */

}


