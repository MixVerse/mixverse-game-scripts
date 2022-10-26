using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.Events;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Mirror.Examples.AdditiveLevels;

//网络状态枚举
public enum ZVerseNetworkState { Offline, Handshake, World }

[Serializable] public class ZVerseEventPlayerLoad : UnityEvent<ZVerseLoadPlayerMsg, ZVersePlayer> { }

public enum ScenePermission { Public, Friend, Mine }
public class ZVerseNetworkManager : NetworkManager
{

    public static ZVerseNetworkManager Instance;

    [Tooltip("当前玩家网络连接状态")]
    public ZVerseNetworkState state = ZVerseNetworkState.Offline;

    [Tooltip("准备登录暂存用户")]
    public Dictionary<NetworkConnection, long> readyLoginUsers = new Dictionary<NetworkConnection, long>();

    [Tooltip("渐入渐出")]
    public FadeInOut fadeInOut;

    public LoadingManager loading;

    [Serializable]
    public class ServerInfo
    {
        public string name;
        public string ip;
    }
    [Tooltip("可选服务器列表")]
    public List<ServerInfo> serverList = new List<ServerInfo>() {
        new ServerInfo{ name="本地服务器", ip="localhost"},
        new ServerInfo{ name="局域网服务器", ip="192.168.50.193"},
    };

    [Tooltip("玩家退出延迟")]
    public float combatLogoutDelay = 5;


    [Header("玩家创建选择")]
    public Transform selectPos;
    public Transform selectCameraPos;
    [HideInInspector] public List<ZVersePlayer> playerClasses = new List<ZVersePlayer>();


    [Header("数据库")]
    [Tooltip("角色名最大长度")]
    public int characterLimit = 4;
    public int characterNameMaxLength = 16;
    [Tooltip("保存超时时间(s)")]
    public float saveInterval = 60f; // in seconds


    [Header("Events")]
    public UnityEvent onStartClient;
    public UnityEvent onStopClient;
    public UnityEvent onStartServer;
    public UnityEvent onStopServer;
    public UnityEventNetworkConnection onClientConnect;
    public UnityEventNetworkConnection onServerConnect;
    public ZVerseEventPlayerLoad onServerPlayerLoad;
    public UnityEventNetworkConnection onClientDisconnect;
    public UnityEventNetworkConnection onServerDisconnect;

    [Header("运行时单例类")]
    public PhoneVerification phoneVerification;

    [Header("MainScene")]
    [Scene]
    public string mainScene;

    private Scene mainSceneInstance;  //加载出来的主场景实例

    bool isInTransition; //是否在转换场景过程中


    [Header("LoadingScene")]
    [Scene]
    public string LoadingScene;


    [Header("公共场景类型")]
    [Scene]
    public string[] publicScenes;

    public int publicSceneInstance; //每种公有场景最大个数

    public int publicScenePlayersCount; //每个公有场景的最大玩家个数
    [Serializable]
    public class PublicSceneInfo
    {
        public int id;
        public int sceneType;
        public int maxNumber;
        public List<NetworkConnection> players;
        public Scene scene;

        public PublicSceneInfo()
        {
            players = new List<NetworkConnection>();
        }
    }

    public Dictionary<string, List<PublicSceneInfo>> publicScenesDic = new Dictionary<string, List<PublicSceneInfo>>();  //公共场景


    [Header("玩家自建场景类型")]
    [Scene]
    public string selfBuildScene;


    public int maxSubSceneCount;  //创建的最大房间数量
    //玩家创建房间消息
    [Serializable]
    public class SceneRoomInfo
    {
        //public long roomId;  //玩家创建的房间Id,这里为房主的userid
        public int maxNumber; //房间限制最大的玩家人数
        public int currentNumber;  //房间当前人数
        public int roomPermission; //房间进入权限
        //public int sceneType;   //房间类型
        public List<NetworkConnection> players;  //房间中的玩家 ， players[0]为房主
        public Scene scene;  //房间场景

        public SceneRoomInfo()
        {
            players = new List<NetworkConnection>();
        }

    }

    public Dictionary<long, SceneRoomInfo> ownerScenes = new Dictionary<long, SceneRoomInfo>();  //当前房间数组 ,key为roomId，暂定为房主的userId


    #region 服务端运行时方法
    private void initInstance()
    {
        PhoneVerification.Instance = phoneVerification;
    }


    #endregion


    /// <summary>
    /// 检测角色名合法性
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool CheckCharacterName(string name)
    {
        return name.Length <= characterNameMaxLength &&
               Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$");
    }

    /// <summary>
    /// 获取最近出生点的位置
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public static Transform GetNearestStartPosition(Vector3 from) =>
        Utils.GetNearestTransform(startPositions, from);


    /// <summary>
    /// 查询玩家主脚本
    /// </summary>
    /// <returns></returns>
    public List<ZVersePlayer> FindPlayerClasses()
    {

        List<ZVersePlayer> classes = new List<ZVersePlayer>();
        foreach (GameObject prefab in spawnPrefabs)
        {
            ZVersePlayer player = prefab.GetComponent<ZVersePlayer>();
            if (player != null)
                classes.Add(player);
        }
        return classes;
    }

    public override void Awake()
    {
        base.Awake();
        if (Instance == null)
            Instance = this;

        initInstance();
        playerClasses = FindPlayerClasses();

    }

    void Update()
    {

        if (NetworkClient.localPlayer != null)
            state = ZVerseNetworkState.World;
    }


    #region Server相关----------------------------------------------------------------------------------------------


    public override void OnStartServer()
    {

        //初始化数据库
        ZVerseMysqlConnect.Init();
        //zverse_inventory_dao.CreateTB();
        //zverse_player_dao.CreateTB();
        //zverse_weapon_dao.CreateTB();
        //zverse_friend_dao.CreateTB();
        //zverse_friend_message_dao.CreateTB();
        //zverse_self_scene_dao.CreateTB();

        //注册创建角色消息回调
        NetworkServer.RegisterHandler<ZVerseLoadPlayerMsg>(LoadPlayMessage);
        //注册玩家创建场景房间回调
        NetworkServer.RegisterHandler<PlayerCreateRoomSceneMsg>(CreateRoomScene);
        //注册玩家加入自建场景回调
        NetworkServer.RegisterHandler<PlayerEnterRoomSceneMsg>(PlayerEnterRoomScene);
        //进入公共场景
        NetworkServer.RegisterHandler<PlayerInPublicSceneMsg>(EnterPublicScene);
        //间隔固定时间保存在线用户数据
        InvokeRepeating(nameof(SavePlayers), saveInterval, saveInterval);

        //加载有关场景
        //StartCoroutine(ServerLoadAllScenes());

        //执行开启服务器事件(如果有的话)
        onStartServer.Invoke();
    }


    private void SavePlayers()
    {
        foreach (ZVersePlayer player in ZVersePlayer.onlinePlayers.Values)
        {
            player.weapon.UpdateItems();
        }

    }


    /// <summary>
    /// 服务器发送错误消息
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="error"></param>
    /// <param name="disconnect"></param>
    public void ServerSendError(NetworkConnection conn, string error, bool disconnect)
    {
        conn.Send(new ErrorMsg { text = error, causesDisconnect = disconnect });
    }


    public override void OnServerConnect(NetworkConnection conn)
    {
        //long user_id = loginUsers[conn];

        //conn.Send(LoadPlayMessage(user_id));

        // addon system hooks
        onServerConnect.Invoke(conn);
    }


    /// <summary>
    ///  服务器发送加载角色信息的消息
    /// </summary>
    /// <param name="user_id"></param>
    /// <returns></returns>
    private void LoadPlayMessage(NetworkConnection conn, ZVerseLoadPlayerMsg msg)
    {

        if (readyLoginUsers.ContainsKey(conn))
        {
            // read the index and find the n-th character
            // (only if we know that he is not ingame, otherwise lobby has
            //  no netMsg.conn key)
            long user_id = readyLoginUsers[conn];

            // NetworkClient.Ready();

            zverse_users user = zverse_users_dao.QueryUser(user_id);

            int playerCount = zverse_player_dao.PlayerCount(user_id);

            if (playerCount == 0)
            {
                zverse_player newPlayer = new zverse_player();

                newPlayer.user_id = user_id;
                newPlayer.user_name = user.user_name;
                newPlayer.level = 0;
                newPlayer.health = 0;
                newPlayer.mana = 0;


                zverse_player_dao.Insert(newPlayer);


            }

            //创建预制体并加载玩家

            GameObject playerObj = CreatePlayer(user_id);

            //移动玩家到主场景
            SceneManager.MoveGameObjectToScene(playerObj, mainSceneInstance);


            StartCoroutine(AddPlayerDelay(conn, playerObj));
            //NetworkServer.AddPlayerForConnection(conn, playerObj);

            // conn.Send(new SceneMessage { sceneName = subScenes[0], sceneOperation = SceneOperation.LoadAdditive, customHandling = true });
            //
            // NetworkServer.AddPlayerForConnection(conn, playerObj);

            // onServerCharacterSelect.Invoke(account, go, conn, message);

            // remove from lobby
            readyLoginUsers.Remove(conn);

        }
        else
        {
            ServerSendError(conn, "玩家未在登录状态", true);
        }

    }
    /// <summary>
    /// 延迟创建玩家信息
    /// </summary>
    /// <returns></returns>
    IEnumerator AddPlayerDelay(NetworkConnection conn, GameObject playerObj)
    {
        conn.Send(new SceneMessage { sceneName = mainScene, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });
        //保证切换场景后创建对象
        yield return new WaitForEndOfFrame();

        NetworkServer.AddPlayerForConnection(conn, playerObj);
    }



    /// <summary>
    /// 服务器创建玩家实体
    /// </summary>
    /// <param name="user_id"></param>
    /// <returns></returns>
    private GameObject CreatePlayer(long user_id)
    {
        zverse_player player_data = zverse_player_dao.QueryPlayerById(user_id);
        if (player_data != null)
        {
            ZVersePlayer prefab = playerClasses.Find(p => p.name == "Player");
            if (prefab != null)
            {
                Transform start = GetStartPosition();
                GameObject go = Instantiate(prefab.gameObject, start);
                go.transform.SetParent(null);

                ZVersePlayer player = go.GetComponent<ZVersePlayer>();


                player.user_id = user_id;
                player.user_name = player_data.user_name;
                player.name = player_data.user_name;
                player.nameOverlay.text = player_data.user_name;

                //player.weapon.LoadWeapon();
                //player.inventory.LoadInventory();
                //player.constume.LoadCostume();   //加载玩家着装
                /**加载 玩家数据信息 **/
                //TODO
                return go;

            }
        }
        return null;
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        //加载所有主场景和二级共有场景
        if (sceneName == onlineScene)
            StartCoroutine(ServerLoadAllScenes());
        //加载主场景
        // StartCoroutine(ServerLoadMainScene());
    }

    /// <summary>
    /// 加载所有公共场景
    /// </summary>
    /// <returns></returns>
    [Server]
    IEnumerator ServerLoadAllScenes()
    {

        yield return StartCoroutine(ServerLoadMainScene());
        //SceneManager.sceneLoaded += SceneLoadedEvent;
        for (int index = 0; index < publicScenes.Length; index++)
        {
            publicScenesDic.Add(publicScenes[index], new List<PublicSceneInfo>());

            for (int i = 0; i < publicSceneInstance; i++)
            {
                yield return SceneManager.LoadSceneAsync(publicScenes[index], new LoadSceneParameters
                {
                    loadSceneMode = LoadSceneMode.Additive,
                    localPhysicsMode = LocalPhysicsMode.Physics3D
                });
                Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                Debug.LogError(newScene.name);
                PublicSceneInfo info = new PublicSceneInfo();
                info.id = newScene.GetHashCode();
                info.maxNumber = publicScenePlayersCount;
                info.sceneType = index;
                info.scene = newScene;
                publicScenesDic[publicScenes[index]].Add(info);


            }

        }
        Debug.Log("Server load All Scenes Finish");
        //SceneManager.sceneLoaded -= SceneLoadedEvent;

    }

    /// <summary>
    /// 加载主场景
    /// </summary>
    /// <returns></returns>
    [Server]
    IEnumerator ServerLoadMainScene()
    {
        yield return SceneManager.LoadSceneAsync(mainScene, new LoadSceneParameters
        {

            loadSceneMode = LoadSceneMode.Additive,
            localPhysicsMode = LocalPhysicsMode.Physics3D
        });
        mainSceneInstance = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    }


    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        if (conn.identity == null)
            LoadPlayMessage(conn, new ZVerseLoadPlayerMsg());
    }



    /// <summary>
    /// 玩家进入公共场景房间
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Server]
    private void EnterPublicScene(NetworkConnection conn, PlayerInPublicSceneMsg msg)
    {

        if (publicScenesDic.ContainsKey(msg.sceneType))
        {
            PublicSceneInfo info = publicScenesDic[msg.sceneType].Find((_info) => _info.id == msg.id);
            if (info != null)
            {
                if (publicScenesDic[msg.sceneType].Count < info.maxNumber)
                {
                    if (!info.players.Contains(conn))
                    {
                        info.players.Add(conn);
                        StartCoroutine(ServerPlayerEnterScene(info.scene, conn));
                    }
                    else
                    {
                        ServerSendError(conn, "你已经在场景中", false);
                    }

                }
                else
                {
                    ServerSendError(conn, "此房间已达到人数上限", false);
                }
            }
            else
            {
                ServerSendError(conn, "此房间不存在", false);
            }

        }
        else
        {
            ServerSendError(conn, "没有对应类型的场景", false);
        }




    }

    /// <summary>
    /// 玩家切换场景通用
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="conn"></param>
    /// <returns></returns>
    [ServerCallback]
    IEnumerator ServerPlayerEnterScene(Scene scene, NetworkConnection conn)
    {


        
        if (conn == null) yield break;

        GameObject player = conn.identity.gameObject;

        conn.Send(new SceneMessage { sceneName = player.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });

        yield return new WaitForSeconds(2f);

        //切换场景前保存玩家数据
        SavePlayerInfo(player.GetComponent<ZVersePlayer>());

        NetworkServer.RemovePlayerForConnection(conn, false);


        player.transform.position = Vector3.zero;


        SceneManager.MoveGameObjectToScene(player, scene);

        conn.Send(new SceneMessage { sceneName = scene.path, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

        NetworkServer.AddPlayerForConnection(conn, player);

        // ownerScenes[user_id].owner.Send(new RoomSceneCreateSuccessMsg { user_id = user_id, sceneType = sceneType });


    }
    /// <summary>
    /// 玩家创建房间场景
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Server]
    private void CreateRoomScene(NetworkConnection conn, PlayerCreateRoomSceneMsg msg)
    {

        foreach (var item in ownerScenes)
        {
            if (item.Value.players.Contains(conn))
            {
                ServerSendError(conn, "玩家已经在自建场景中，无法重复创建", false);
                return;
            }
        }

        if (ownerScenes.Count >= maxSubSceneCount)
        {
            ServerSendError(conn, "当前自建场景数已达上限", false);
            return;

        }

        //创建玩家自建场景记录
        zverse_self_scene sceneInfo= zverse_self_scene_dao.QueryUserScene(msg.user_id);
        if(sceneInfo==null)
        {
            sceneInfo = new zverse_self_scene();
            sceneInfo.user_id = msg.user_id;
            sceneInfo.max_number = msg.maxNumber;
            sceneInfo.permission = msg.roomPermission;
            zverse_self_scene_dao.Insert(sceneInfo);
        }


        //加载玩家自建场景
        StartCoroutine(ServerLoadRoomScene((NetworkConnectionToClient)conn, msg));




    }




    /// <summary>
    /// 玩家创建并加入自建场景
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    [ServerCallback]
    IEnumerator ServerLoadRoomScene(NetworkConnectionToClient conn, PlayerCreateRoomSceneMsg msg)
    {

        //创建自建场景
        yield return SceneManager.LoadSceneAsync(selfBuildScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        if (conn == null) yield break;

        GameObject player = conn.identity.gameObject;

        //开始创建房间
        SceneRoomInfo info = new SceneRoomInfo();
        info.maxNumber = msg.maxNumber;
        info.roomPermission = msg.roomPermission;
        info.players.Add(conn);
        info.scene = newScene;

        //添加玩家场景
        ownerScenes.Add(player.GetComponent<ZVersePlayer>().user_id, info);
        

        //通知客户端卸载当前场景
        conn.Send(new SceneMessage { sceneName = player.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });

        yield return new WaitForSeconds(2f);

        //切换场景前保存玩家数据
        SavePlayerInfo(player.GetComponent<ZVersePlayer>());

        NetworkServer.RemovePlayerForConnection(conn, false);


        player.transform.position = Vector3.zero;


        SceneManager.MoveGameObjectToScene(player, newScene);

        //通知客户端创建自建场景
        conn.Send(new SceneMessage { sceneName = newScene.path, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

        NetworkServer.AddPlayerForConnection(conn, player);




    }

    /// <summary>
    /// 玩家加入自建场景
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    private void PlayerEnterRoomScene(NetworkConnection conn,PlayerEnterRoomSceneMsg msg)
    {


        if(!ownerScenes.ContainsKey(msg.roomId))
        {
            ServerSendError(conn, "此场景不存在", false);
            return;
        }
        //待加入自建场景的玩家
        ZVersePlayer p1 = conn.identity.gameObject.GetComponent<ZVersePlayer>();

        if(p1.gameObject.scene.name.Equals(selfBuildScene))
        {
            ServerSendError(conn, "玩家已经在自建场景中，无法重复创建", false);
            return;
        }

        SceneRoomInfo info = ownerScenes[msg.roomId];

        if (info.players.Count>=info.maxNumber)
        {
            ServerSendError(conn, "房间人数已达上限，无法加入", false);
            return;
        }

        // 此房间的房主
        ZVersePlayer p2 =info.players[0].identity.gameObject.GetComponent<ZVersePlayer>();

        ///权限判断
        if((ScenePermission)info.roomPermission==ScenePermission.Mine)
        {
            ServerSendError(conn, "私有房间无法加入", false);
            return;
        }

        if ((ScenePermission)info.roomPermission == ScenePermission.Friend)
        {
            FriendListInfo friend = p2.friend.friendList.Find(item => item.userId == p1.user_id);
            if(friend.userId==0)
            {
                ServerSendError(conn, "此房间仅限好友进入", false);
                return;
            }
        }

        //加入列表
        info.players.Add(conn);

        //进入房间
        PlayerEnterRoomSceneCor((NetworkConnectionToClient)conn, msg.roomId);

    }

    [ServerCallback]
    IEnumerator  PlayerEnterRoomSceneCor(NetworkConnectionToClient conn,long roomId)
    {
        
        if (conn == null) yield break;

        GameObject player = conn.identity.gameObject;

        //通知客户端卸载当前场景
        conn.Send(new SceneMessage { sceneName = player.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });

        yield return new WaitForSeconds(2f);

        //切换场景前保存玩家数据
        SavePlayerInfo(player.GetComponent<ZVersePlayer>());

        NetworkServer.RemovePlayerForConnection(conn, false);


        player.transform.position = Vector3.zero;


        SceneManager.MoveGameObjectToScene(player, ownerScenes[roomId].scene);

        //通知客户端创建自建场景
        conn.Send(new SceneMessage { sceneName = ownerScenes[roomId].scene.path, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

        NetworkServer.AddPlayerForConnection(conn, player);
    }


    /// <summary>
    /// 返回主场景
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="msg"></param>
    [Server]
    public void PlayerBackToMainScene(NetworkConnection conn, PlayerBackToMainSceneMsg msg)
    {
        // 玩家移出当前场景队列
        GameObject player = conn.identity.gameObject;

        if(player.scene.name.Equals(mainScene))
        {
            ServerSendError(conn, "玩家已在主场景，无需返回", false);
            return;
        }

        //玩家位于公共场景
        if(publicScenesDic.ContainsKey(player.scene.name))
        {
            foreach (var item in publicScenesDic[player.scene.name])
            {
                if (item.players.Contains(conn))
                {
                    item.players.Remove(conn);
                    break;
                }
            }

            //切换到主场景
            StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, conn));
        }
        // 玩家位于自建场景
        else if(selfBuildScene.Equals(player.scene.name))
        {
            //如果玩家是房主
            if(ownerScenes.ContainsKey(msg.user_id))
            {
                SceneRoomInfo info = ownerScenes[msg.user_id];
                ownerScenes.Remove(msg.user_id);
                //房主玩家返回主场景，并删除自建场景
                StartCoroutine(RemoveSelfBuildScene(conn, info.scene));

                //房中其余玩家返回主场景
                for(int i=1;i<info.players.Count;i++)
                {
                    StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, info.players[i]));
                }

               
            }
            else
            {
                //long key = 0;
                foreach (var item in ownerScenes)
                {
                    if (item.Value.players.Contains(conn))
                    {
                        item.Value.players.Remove(conn);
                       // if (item.Value.players.Count == 0)
                       //     key = item.Key;
                        break;
                    }
                }
                //如果此自建场景中没有玩家，就删除此场景
                //if (key != 0)
                //{
                //    SceneRoomInfo info = ownerScenes[key];
                //    ownerScenes.Remove(key);
                //    StartCoroutine(RemoveSelfBuildScene(conn, info.scene));
                //}
                // 否则直接切换场景，如果房主退出，默认列表第二个玩家变为房主
                //else
                //{
                //    StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, conn));
                //}
                StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, conn));
            }
            
        }
       


    }

    [Server]
    private IEnumerator RemoveSelfBuildScene(NetworkConnection conn,Scene scene)
    {
        yield return StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, conn));

        //删除空的自建场景
        StartCoroutine(ServerUnloadSubScenes(scene));
    }


    /// <summary>
    /// 服务端删除场景
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    [Server]
    IEnumerator ServerUnloadSubScenes(Scene scene)
    {

        yield return SceneManager.UnloadSceneAsync(scene);
        yield return Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 客户端断开连接处理
    /// </summary>
    /// <param name="conn"></param>
    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        

        //玩家退出时保存玩家数据
        SavePlayerInfo(conn.identity.GetComponent<ZVersePlayer>());


        if (readyLoginUsers.ContainsKey(conn))
            readyLoginUsers.Remove(conn);


        //公共场景删除玩家
        foreach (var k_v in publicScenesDic)
        {
            foreach (var p in k_v.Value)
            {
                if (p.players.Contains(conn))
                {
                    p.players.Remove(conn);
                    return;
                }
            }
        }


        //自建场景删除玩家
        ZVersePlayer zp = conn.identity.GetComponent<ZVersePlayer>();
        if (ownerScenes.ContainsKey(zp.user_id))
        {
            SceneRoomInfo info = ownerScenes[zp.user_id];
            ownerScenes.Remove(zp.user_id);
            //删除自建场景
            StartCoroutine(ServerUnloadSubScenes(info.scene));

            //房中其余玩家返回主场景
            for (int i = 1; i < info.players.Count; i++)
            {
                StartCoroutine(ServerPlayerEnterScene(mainSceneInstance, info.players[i]));
            }


        }
        else
        {
            foreach (var item in ownerScenes)
            {
                if (item.Value.players.Contains(conn))
                {
                    item.Value.players.Remove(conn);
                    break;
                }
            }
        }


        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// 保存Player信息到数据库
    /// </summary>
    /// <param name="player"></param>
    [Server]
    public void SavePlayerInfo(ZVersePlayer player)
    {
        player.weapon.UpdateItems();
    }




    #endregion

    #region Client相关-----------------------------------------------------------------------------------------------


    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<ErrorMsg>(OnClientError, false); // allowed before auth!
        NetworkClient.RegisterHandler<RoomSceneCreateSuccessMsg>(OnClientCreateRoomSceneSuccess);
        // addon system hooks
        onStartClient.Invoke();
    }


    /// <summary>
    /// 客户端接收错误消息
    /// </summary>
    /// <param name="message"></param>
    void OnClientError(ErrorMsg message)
    {
        Debug.Log("OnClientError: " + message.text);
        // UIPopup.Instance.Show(message.text);

        if (message.causesDisconnect)
        {
            NetworkClient.connection.Disconnect();
            if (NetworkServer.active) StopHost();
        }
    }

    /// <summary>
    /// 房间场景创建成功
    /// </summary>
    /// <param name="message"></param>
    void OnClientCreateRoomSceneSuccess(RoomSceneCreateSuccessMsg message)
    {

    }


    public override void OnClientConnect()
    {
        onClientConnect.Invoke(NetworkClient.connection);

        state = ZVerseNetworkState.World;

        // NetworkClient.Ready();
        //
        //  //通知服务端加载玩家
        //  NetworkClient.Send(new ZVerseLoadPlayerMsg());
    }


    public override void OnClientChangeScene(string sceneName, SceneOperation sceneOperation, bool customHandling)
    {
        //if (sceneOperation == SceneOperation.Normal)
        //    StartCoroutine(LoadNormalScene(sceneName));
        if (sceneOperation == SceneOperation.UnloadAdditive)
            StartCoroutine(UnloadAdditive(sceneName));
        if (sceneOperation == SceneOperation.LoadAdditive)
            StartCoroutine(LoadAdditive(sceneName));
    }


    /// <summary>
    /// 客户端加载其他场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator LoadAdditive(string sceneName)
    {
        isInTransition = true;


        // yield return fadeInOut.FadeIn();
        loading.Open();

        if (mode == NetworkManagerMode.ClientOnly)
        {
            // Start loading the additive subscene
            loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (loadingSceneAsync != null && !loadingSceneAsync.isDone)
            {
                yield return null;
                loading.SetProgress(loadingSceneAsync.progress);
            }

        }

        // Reset these to false when ready to proceed
        NetworkClient.isLoadingScene = false;
        isInTransition = false;

        OnClientSceneChanged();

        loading.Close();

        //  yield return fadeInOut.FadeOut();
    }

    /// <summary>
    /// 客户端卸载其他场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator UnloadAdditive(string sceneName)
    {
        isInTransition = true;

        // yield return fadeInOut.FadeIn();
        loading.Open();

        if (mode == NetworkManagerMode.ClientOnly)
        {

            yield return SceneManager.UnloadSceneAsync(sceneName);
            yield return Resources.UnloadUnusedAssets();
        }

        // Reset these to false when ready to proceed
        NetworkClient.isLoadingScene = false;
        isInTransition = false;

        OnClientSceneChanged();

    }




    /// <summary>
    /// 场景加载后的操作
    /// </summary>
    public override void OnClientSceneChanged()
    {

        if (!isInTransition)
            base.OnClientSceneChanged();
    }

    

    #endregion




    public static void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
