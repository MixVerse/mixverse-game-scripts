using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageConstant {

    #region 错误消息
    /** 第三方插件错误  **/
    public static string YUN_PIAN_ERROR_CODE = "云片短信错误码";
    public static string DB_INSERT_ERROR = "数据创建失败";

    #region 业务消息

    public static string PHONE_NUM_ALREADY_EXIST = "此手机号以被绑定";
    public static string PHONE_VERIFICATION_ERROR = "手机验证码错误";
    public static string USER_NAME_NOT_EXIST = "用户名不存在";
    public static string USER_PASSWORD_ERROR = "用户登录密码错误";
    public static string USER_ALREADY_LOGIN = "用户早已登录了";
    public static string OPERATOIN_FRIEND_APPLY_ERROR = "处理好友请求信息失败";
    public static string APPLICATION_EXIST = "申请已经存在于对方申请列表";
    public static string FRIEND_NOT_EXIST = "好友不存在";
    #endregion

    #endregion

    #region 通知消息
    #endregion


    #region 网络消息
    /**  访问链接失败   **/
    public static string ACCESS_URL_ERROR = "访问链接失败";
    #endregion
}
