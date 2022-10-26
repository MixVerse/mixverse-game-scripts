using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 服务端KEY值常量
/// </summary>
public class CommonConstant
{

    public static readonly bool IS_SERVER = false;  //是否服务器


    #region 云片短信
    public static string VERIFICATION_TEMPLATE = "【潮玩抓娃娃】您的验证码是";
    public static string VERIFICATION_TEMPLATE_END = "。如非本人操作，请忽略本短信";
    public static string YUN_PIAN_APP_KEY = "a7c93e03ed8ba14e39d89e9eda941b0b";
    public static string YUN_PIAN_SEND_URL = "http://yunpian.com/v1/sms/send.json";

    #endregion


    public static string LOGIN_INFO = "Login_Auth_Info"; //账号密码登录验证信息


}
