using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using BestHTTP;
using Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 手机验证类
/// </summary>
public class PhoneVerification : MonoBehaviour
{

    public static PhoneVerification Instance;

    public ZVerseNetworkManager manager;

    private Dictionary<string, CodeDetail> codes = new Dictionary<string, CodeDetail>();  //验证码缓存字典


    /// <summary>
    /// 生成并发送验证码
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="phoneNum"></param>
    public void GenerateCaptcha(NetworkConnection conn,string phoneNum)
    {

        if (codes.ContainsKey(phoneNum))
        {
            DateTime start = codes[phoneNum].start_time;
            TimeSpan ts = start.Subtract(DateTime.Now);
            if (ts.TotalSeconds <= 60)
            {
                Debug.Log("不能重复发送验证码");
                return;
            }
        }

        System.Random rand = new System.Random();
        string code= rand.Next(1000, 9999).ToString();
        StringBuilder content = new StringBuilder();
        string message = content.Append(CommonConstant.VERIFICATION_TEMPLATE).Append(code).Append(CommonConstant.VERIFICATION_TEMPLATE_END).ToString();

        //发送请求验证码
        HTTPRequest req = new HTTPRequest(new Uri(CommonConstant.YUN_PIAN_SEND_URL), HTTPMethods.Post, (request, response) => { 
        

            if(response.IsSuccess&&response.StatusCode==200)
            {
                JObject obj = JObject.Parse(response.DataAsText);
                int reqCode = (int)obj["code"];
                if(reqCode==0)
                {
                    if (!codes.ContainsKey(phoneNum))
                        codes.Add(phoneNum, new CodeDetail());
                    codes[phoneNum].code = code;
                    codes[phoneNum].start_time = DateTime.Now;

                    PhoneVerificationCodeMsg result = new PhoneVerificationCodeMsg { phone_number = phoneNum, code = code };
                    conn.Send(result);  //返回客户端验证码消息
                }
                else
                {
                    manager.ServerSendError(conn, MessageConstant.YUN_PIAN_ERROR_CODE, false);
                }
            }
            else
            {
                manager.ServerSendError(conn, MessageConstant.ACCESS_URL_ERROR, false);
            }
        
        
        });
        req.SetHeader("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
        req.AddField("apikey", CommonConstant.YUN_PIAN_APP_KEY);
        req.AddField("mobile", phoneNum);
        req.AddField("text", message);
        req.Send();

        
    }

    /// <summary>
    /// 验证验证码的正确性
    /// </summary>
    /// <param name="phoneNum"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public bool Verification(string phoneNum,string code)
    {
        if (codes.ContainsKey(phoneNum))
        {
            DateTime start = codes[phoneNum].start_time;
            TimeSpan ts = start.Subtract(DateTime.Now);
            if (ts.TotalSeconds <= 300&& code.Equals(codes[phoneNum].code))  //验证码过期时间5分钟
            {
                return true;
            }
        }
        return false;
    }



    /// <summary>
    /// 验证手机号码格式的合法性
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool CheckPhoneNumber(string num)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(num, @"^1[3456789]\d{9}$");
    }


}
public class CodeDetail
{
    public string code;  //验证码
    public DateTime start_time; //验证码获取时间
}

public class CodeResponseInfo
{
  public int code;
  public string msg ;
  public string count;
  public double fee ;
  public string unit;
  public string mobile;
  public long sid;
}
