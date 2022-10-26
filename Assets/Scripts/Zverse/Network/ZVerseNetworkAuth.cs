using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public enum AuthState
{
    None=-1,
    Login_Password=0,
    Login_PhoneVerification=1,
    Register=2,
    Register_PhoneVerification=3
}
public class ZVerseNetworkAuth : NetworkAuthenticator
{

    [HideInInspector]
    public AuthState authState = AuthState.None;
    
    [Header("NetworkManager")]
    public ZVerseNetworkManager manager;

    // login info for the local player
    // we don't just name it 'account' to avoid collisions in handshake
    [Header("Auth")]
    public string userName = "";
    public string password = "";
    public string confirmPassword = "";
    public string phoneNumber = "";
    public string verificationCode = "";

    [Header("Security")]
    public string passwordSalt = "at_least_16_byte";
    public int accountMaxLength = 16;


    #region Client相关-----------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 客户端开始验证账号密码
    /// </summary>
    public override void OnClientAuthenticate()
    { 
      //  string hash = Utils.PBKDF2Hash(password, passwordSalt + userName);
      //  ZVerseLoginMsg message = new ZVerseLoginMsg { user_name = userName, password = hash, version = Application.version };
      //  NetworkClient.connection.Send(message);
      //  Debug.Log("login message was sent");
      //
      //  // set state
      //  manager.state = ZVerseNetworkState.Handshake;
    }

    /// <summary>
    /// 用户点击注册获取手机验证码
    /// </summary>
    public void ClientUserSendVerificatoinCode(AuthState state)
    {
        authState = state;
        if (!PhoneVerification.CheckPhoneNumber(phoneNumber))
        {
            Debug.LogError("手机号码格式错误");
            return;
        }

        if (state == AuthState.Register_PhoneVerification)
        {
            PhoneVerificationCodeMsg msg = new PhoneVerificationCodeMsg { phone_number = phoneNumber, type = (short)state };

            //发送获取手机验证码消息
            NetworkClient.connection.Send(msg);
        }else if(state==AuthState.Login_PhoneVerification)
        {
            PhoneVerificationCodeMsg msg = new PhoneVerificationCodeMsg { phone_number = phoneNumber, type = (short)state };

            //发送获取手机验证码消息
            NetworkClient.connection.Send(msg);
        }
    }

    /// <summary>
    /// 用户注册提交用户信息
    /// </summary>
    public void ClientStartUserRegister()
    {
        if(authState!=AuthState.Register_PhoneVerification)
        {
            Debug.LogError("状态错误");
            return;
        }
        ZVerseRegisterMsg msg = new ZVerseRegisterMsg { user_name = userName, password = password, phone_num = phoneNumber ,code=verificationCode };
        NetworkClient.connection.Send(msg);
    }


    /// <summary>
    /// 客户端用户登录
    /// </summary>
    public void ClientUserLogin(AuthState state)
    {
        authState = state;
        if(state==AuthState.Login_Password)
        {
           // string hash = Utils.PBKDF2Hash(password, passwordSalt + userName);
            ZVerseLoginMsg msg = new ZVerseLoginMsg { user_name = userName, password = password, version = Application.version,type=(short)state};
            
            //账号密码登录
            NetworkClient.connection.Send(msg);

            // set state
            manager.state = ZVerseNetworkState.Handshake;
        }
        else if(state==AuthState.Login_PhoneVerification)
        {
            ZVerseLoginMsg msg = new ZVerseLoginMsg {phone_num=phoneNumber, code= verificationCode, version = Application.version, type = (short)state };

            //验证码登录
            NetworkClient.connection.Send(msg);

            // set state
            manager.state = ZVerseNetworkState.Handshake;


        }



    }



    public override void OnStartClient()
    {
        //base.OnStartClient();
        NetworkClient.RegisterHandler<ZVerseRegisterSuccessMsg>(OnClientRegisterSuccess, false);
        NetworkClient.RegisterHandler<ZVerseLoginSuccessMsg>(OnClientLoginSuccess, false);
        NetworkClient.RegisterHandler<PhoneVerificationCodeMsg>(OnClientReceiveVerificationPhoneCode, false);

    }

    public void OnClientRegisterSuccess(ZVerseRegisterSuccessMsg msg)
    {
        if (authState == AuthState.Register_PhoneVerification)
        {
            authState = AuthState.None;
            Debug.Log("用户注册成功，user_id:" + msg.user_id);
            ZVerseUILogin.Instance.RegisterSuccess(msg.user_id);
        }
    }


    public void OnClientLoginSuccess(ZVerseLoginSuccessMsg msg)
    {
        //保存账号密码到本地
        if(authState==AuthState.Login_Password)
        {
            ZVerseUILogin.Instance.SaveLoginInfo();
        }
        authState = AuthState.None;
        OnClientAuthenticated.Invoke();
    }

    /// <summary>
    /// 客户端收到验证码
    /// </summary>
    /// <param name="msg"></param>
    public void OnClientReceiveVerificationPhoneCode(PhoneVerificationCodeMsg msg)
    {
         if(authState==AuthState.Register_PhoneVerification)
         {

            ZVerseUILogin.Instance.SendCodeTimer();


            Debug.Log("客户端开始验证码计时，收到code:"+msg.code);
         }else if(authState==AuthState.Login_PhoneVerification)
         {
            ZVerseUILogin.Instance.SendCodeTimer();
            Debug.Log("客户端开始验证码计时，收到code:" + msg.code);
        }
    }






    #endregion


    #region Server相关-------------------------------------------------------------------------------------------------------------------------

    public override void OnServerAuthenticate(NetworkConnection conn)
    {
        
    }
    public override void OnStartServer()
    {
        // base.OnStartServer();
        NetworkServer.RegisterHandler<ZVerseLoginMsg>(OnServerCheckLogin, false);
        NetworkServer.RegisterHandler<ZVerseRegisterMsg>(OnServerRegisterUser, false);
        NetworkServer.RegisterHandler<PhoneVerificationCodeMsg>(OnServerVerificationPhoneCode, false);
    }
    public void OnServerCheckLogin(NetworkConnection conn, ZVerseLoginMsg msg)
    {
        zverse_users user = null;
        if (msg.version == Application.version)
        {
            if(AuthState.Login_Password==(AuthState)msg.type)
            { 
                if ((user=zverse_users_dao.Login(msg.user_name, msg.password,out string loginMsg))!=null)
                {
                    //防止重复登录
                    if (!manager.readyLoginUsers.ContainsValue(user.user_id))
                    {
                       
                        manager.readyLoginUsers[conn] = user.user_id;

                        //发送登录成功消息
                        conn.Send(new ZVerseLoginSuccessMsg());

                        //执行事件
                        OnServerAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        //Debug.Log("account already logged in: " + message.account); <- don't show on live server
                        manager.ServerSendError(conn,MessageConstant.USER_ALREADY_LOGIN, false);
                    }
                }
                else
                {
                    //Debug.Log("invalid account or password for: " + message.account); <- don't show on live server
                    manager.ServerSendError(conn, loginMsg, false);
                }
            }
            else if(AuthState.Login_PhoneVerification==(AuthState)msg.type)
            {

                user= zverse_users_dao.QueryUserByPhone(msg.phone_num);
                if (user!=null && PhoneVerification.Instance.Verification(msg.phone_num,msg.code))  //检测用户是否存在和验证码
                {
                    // not in lobby and not in world yet?
                    if (!manager.readyLoginUsers.ContainsValue(user.user_id))
                    {
                        // add to logged in accounts
                        manager.readyLoginUsers[conn] = user.user_id;

                        // login successful
                        //LogManager.Log(manager, "登录成功！user_id:" + user.user_id);

                        // notify client about successful login. otherwise it
                        // won't accept any further messages.
                        conn.Send(new ZVerseLoginSuccessMsg());

                        // authenticate on server
                        OnServerAuthenticated.Invoke(conn);
                    }
                    else
                    {
                        //Debug.Log("account already logged in: " + message.account); <- don't show on live server
                        manager.ServerSendError(conn, MessageConstant.USER_ALREADY_LOGIN, false);
                    }
                }
                else
                {
                    //Debug.Log("invalid account or password for: " + message.account); <- don't show on live server
                    manager.ServerSendError(conn, MessageConstant.PHONE_VERIFICATION_ERROR, false);
                }
            }
        }
        else
        {
            //Debug.Log("version mismatch: " + message.account + " expected:" + Application.version + " received: " + message.version); <- don't show on live server
            manager.ServerSendError(conn, "版本过低", false);
        }
    }

    public void OnServerRegisterUser(NetworkConnection conn, ZVerseRegisterMsg msg)
    {

        if(!PhoneVerification.Instance.Verification(msg.phone_num, msg.code))
        {
            manager.ServerSendError(conn, MessageConstant.PHONE_VERIFICATION_ERROR, false);
        }

        //创建新用户
        zverse_users user = new zverse_users();
        user.user_name = msg.user_name;
        user.password = msg.password;
        user.phone = msg.phone_num;
        user.last_login_at = DateTime.Now;
        user.freeze = false;
        user.master = false;
        if( zverse_users_dao.Insert(user)>0)
        {
            
            zverse_users u = zverse_users_dao.QueryUserByPhone(msg.phone_num);
            ZVerseRegisterSuccessMsg rs = new ZVerseRegisterSuccessMsg { user_id=u.user_id};
            //注册成功返回user_id
            conn.Send(rs);
           // LogManager.Log(manager, "用户注册成功user_id" + u.user_id);
        }
        else
        {
            //信息未添加
            manager.ServerSendError(conn,MessageConstant.DB_INSERT_ERROR, false);
        }



    }

    public void OnServerVerificationPhoneCode(NetworkConnection conn, PhoneVerificationCodeMsg msg)
    {

        if((AuthState)msg.type==AuthState.Register_PhoneVerification)
        {
            //手机号已有绑定信息
            if (zverse_users_dao.CheckExistByPhone(msg.phone_number))
            {
                manager.ServerSendError(conn, MessageConstant.PHONE_NUM_ALREADY_EXIST, false);
                return;
            }
            PhoneVerification.Instance.GenerateCaptcha(conn, msg.phone_number);

        }else if((AuthState)msg.type==AuthState.Login_PhoneVerification)
        {
            zverse_users user = zverse_users_dao.QueryUserByPhone(msg.phone_number);
            if(user!=null)
            {
                PhoneVerification.Instance.GenerateCaptcha(conn, msg.phone_number);
            }else
            {
                manager.ServerSendError(conn, MessageConstant.USER_NAME_NOT_EXIST, false);
                return;
            }

           
        }

        //PhoneVerificationCodeMsg result = new PhoneVerificationCodeMsg { phone_number = msg.phone_number, code = code };

        //返回服务端生成验证码
        //conn.Send(result);
    }

    #endregion






    public bool UserNameValidityCheck(string name)
    {
        return name.Length <= accountMaxLength &&
               Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$");
    }



}
