using Mirror;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable] public class UnityEventPlayer : UnityEvent<Player> { }


public partial class ZVersePlayer : NetworkBehaviour
{

    [Header("Text Meshes")]
    public TextMeshPro nameOverlay;

    public bool isBulide = false;

    public string nameOverlayGameMasterPrefix = "[GM] ";

    [Header("Icons")]
    public Sprite classIcon; // for character selection
    public Sprite portraitIcon; // for top left portrait

    // some meta info
    [SyncVar,HideInInspector] public long user_id = 0;
    [SyncVar,HideInInspector] public string user_name = "";


    [SyncVar] public bool isGameMaster;

    [SyncVar, SerializeField] long _gold = 0;
    public long gold { get { return _gold; } set { _gold = Math.Max(value, 0); } }

    [SyncVar, HideInInspector] public double nextRiskyActionTime = 0;   //下一次风险操作的时间

    // localPlayer singleton for easier access from UI scripts etc.
    public static ZVersePlayer localPlayer;
    public Player player;

    // 服务端表示所有玩家，客户端表示玩家观察者
    public static Dictionary<string, ZVersePlayer> onlinePlayers = new Dictionary<string, ZVersePlayer>();

    /// <summary>
    /// 物品栏物品同步字典
    /// </summary>
    public SyncDictionary<string, double> itemCooldowns = new SyncDictionary<string, double>();

    [Header("playerComponent")]
    //public ZVersePlayerCostume costume;
    //public ZVersePlayerInventory inventory;
    //public ZVerseItemMall mall;
    public ZversePlayerWeapon weapon;
    public ZversePlayerFurniture furniture;
    public ZVerseChat zverseChat;
    public ZversePlayerScene sceneInfo;
    public ZversePlayerFriend friend;


    void Start()
    {
        onlinePlayers[user_name] = this;
        if (localPlayer!=null && !user_name.Equals(localPlayer.user_name))
        {
            gameObject.layer = 0;
            player = GetComponent<Player>();
            player.leftController.leftHandUI.HiddenAllPanel();
            player.PlayerMode.SetModLay((int)CheckLayer.Player);
            Destroy(GetComponentInChildren<Camera>());
        }
    }

    // networkbehaviour ////////////////////////////////////////////////////////
    public override void OnStartLocalPlayer()
    {
        // set singleton
        localPlayer = this;
        player = GetComponent<Player>();
        isBulide = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name == "MyScene";
        // setup camera targets
        //GameObject.FindWithTag("MinimapCamera").GetComponent<CopyPosition>().target = transform;
    }

    void LateUpdate()
    {

        // update overlays in any case, except on server-only mode
        // (also update for character selection previews etc. then)
        if (!isServerOnly)
        {
            if (nameOverlay != null)
            {
                string prefix = isGameMaster ? nameOverlayGameMasterPrefix : "";
                nameOverlay.text = prefix + name;

            }
        }
    }

    void OnDestroy()
    {
        if (onlinePlayers.TryGetValue(user_name, out ZVersePlayer entry) && entry == this)
            onlinePlayers.Remove(user_name);

        if (!isServer && !isClient) return;

        if (isLocalPlayer)
            localPlayer = null;

        
    }


    /// <summary>
    /// 获取物品的冷却时间
    /// </summary>
    /// <param name="cooldownCategory"></param>
    /// <returns></returns>
    public float GetItemCooldown(string cooldownCategory)
    {
        // find cooldown for that category
        if (itemCooldowns.TryGetValue(cooldownCategory, out double cooldownEnd))
        {
            return NetworkTime.time >= cooldownEnd ? 0 : (float)(cooldownEnd - NetworkTime.time);
        }

        return 0;
    }

    /// <summary>
    /// 设置物品的冷却时间
    /// </summary>
    /// <param name="cooldownCategory"></param>
    /// <param name="cooldown"></param>
    public void SetItemCooldown(string cooldownCategory, float cooldown)
    {
        // save end time
        itemCooldowns[cooldownCategory] = NetworkTime.time + cooldown;
    }

}
