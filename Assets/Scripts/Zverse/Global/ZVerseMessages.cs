using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System;

#region ZVerse 消息结构体

public partial struct ZVerseRegisterMsg: NetworkMessage
{
    public string phone_num;
    public string user_name;
    public string password;
    public string code;
}

public partial struct ZVerseRegisterSuccessMsg:NetworkMessage
{
    public long user_id;
}

public partial struct ZVerseLoginMsg : NetworkMessage
{
    public string user_name;
    public string password;
    public string phone_num;
    public long user_id;
    public string version;
    public short type;   //登录方式
    public string code;   //手机验证码
}

public partial struct ZVerseLoginSuccessMsg : NetworkMessage
{


}

/// <summary>
/// 创建角色消息
/// </summary>
public partial struct ZVerseLoadPlayerMsg:NetworkMessage
{
    public long user_id;
   // public string user_name;
    public bool gameMaster; 

}

public partial struct PhoneVerificationCodeMsg:NetworkMessage
{
    public long user_id;
    public string phone_number;
    public string code;
    public short type;   
}


public partial struct PlayerCreateRoomSceneMsg:NetworkMessage
{
    public long user_id;
    public int maxNumber; 
    public int roomPermission; 
}
public partial struct PlayerEnterRoomSceneMsg:NetworkMessage
{
    public long user_id;
    public long roomId;

}
public partial struct PlayerBackToMainSceneMsg:NetworkMessage
{
    public long user_id;
}

public partial struct RoomSceneCreateSuccessMsg:NetworkMessage
{
    public long user_id;
    public int sceneType;
}

public partial struct PlayerInPublicSceneMsg:NetworkMessage
{
    public int id;
    public long user_id;
    public string sceneType;
}


/// <summary>
///服务器返回错误消息
/// </summary>
public partial struct ErrorMsg : NetworkMessage
{
    public string text;
    public bool causesDisconnect;
}




#endregion





#region  服务器搜寻消息

public partial struct ZVerseFindServerReq:NetworkMessage
{

}

public partial struct ZVerseFindServerRes : NetworkMessage
{

    public IPEndPoint EndPoint { get; set; }

    public Uri uri;

    public int connectCount;

    public int serverName;

    // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
    public long serverId;
}


#endregion

