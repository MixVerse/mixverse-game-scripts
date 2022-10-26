using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;
using System;

public partial class zverse_friend_message
{

    public long id { get; set; }
    public long send_id { get; set; }
    public long receive_id { get; set; }
    public int status { get; set; }  //消息查看状态 0 已读 1 未读
    public string message { get; set; }  //消息内容
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
    public DateTime send_time_at { get; set; } //消息发送时间


}
public partial class zverse_friend_message_dao
{
    private static string name = "zverse_friend_message";

    public static bool CreateTB()
    {
        string[] cols = new string[]
        {
            "id bigint(20) NOT NULL AUTO_INCREMENT",
            "send_id bigint(20) NOT NULL",
            "receive_id bigint(20) NOT NULL",
            "status int(4) NOT NULL",
            "message varchar(512) DEFAULT ''",
            "create_at datetime DEFAULT NULL",
            "update_at datetime DEFAULT NULL",
            "send_time_at datetime DEFAULT NULL"
        };
        string[] others = new string[]
        {
            "PRIMARY KEY (id)",
            "KEY send_id (send_id)",
            "KEY receive_id (receive_id)",
            "KEY send_receive_id (send_id,receive_id)"
        };
        int size = ZVerseMysqlConnect.CreateTable(name, cols, others);
        return size >= 1 ? true : false;
    }

    public static zverse_friend QueryFriendApply(long user_id, long friend_id)
    {

        string sql = "select * from zverse_friend  where user_id=@user_id and friend_id=@friend_id and status=1";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id), new MySqlParameter("@friend_id", friend_id) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);

        List<zverse_friend> list = new DatatableToEntity<zverse_friend>().FillModel(ds);
        if (list != null)
            return list[0];
        return null;
    }

    /// <summary>
    /// 删除所有未读消息
    /// </summary>
    /// <param name="receiveId"></param>
    /// <returns></returns>
    public static int DeleteAllUnReadMessage(long receiveId,long sendId)
    {
        string sql = string.Format("delete from zverse_friend_message where receive_id={0} and send_id={1}", receiveId,sendId);
        object result = ZVerseMysqlConnect.ExecuteNonQuery(sql);

        return result == null ? 0 : Convert.ToInt32(result);
    }



    public static List<zverse_friend_message>  QueryAllUnReadMessage(long user_id)
    {
        string sql = "select * from zverse_friend_message  where receive_id=@user_id and status=1";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);

        List<zverse_friend_message> list = new DatatableToEntity<zverse_friend_message>().FillModel(ds);
        return list;
    }

    public static List<zverse_friend> QueryAllFriend(long user_id)
    {
        string sql = "select * from zverse_friend  where user_id=@user_id and status=0";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);

        List<zverse_friend> list = new DatatableToEntity<zverse_friend>().FillModel(ds);
        return list;
    }

    public static zverse_friend QueryFriend(long user_id, long friend_id)
    {

        string sql = "select * from zverse_friend  where user_id=@user_id and friend_id=@friend_id and status=0";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id), new MySqlParameter("@friend_id", friend_id) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);

        List<zverse_friend> list = new DatatableToEntity<zverse_friend>().FillModel(ds);
        if (list != null)
            return list[0];
        return null;
    }



    public static int UpdateInfo(zverse_friend friend)
    {
        friend.update_at = DateTime.Now;
        return ZVerseMysqlConnect.UpdateTemplate<zverse_friend>(friend);
    }


    public static int Insert(zverse_friend_message user)
    {

        user.create_at = DateTime.Now;
        user.update_at = DateTime.Now;
        return ZVerseMysqlConnect.InsertTemplate<zverse_friend_message>(user);

    }


    public static void UpdateBatch(List<zverse_friend> users)
    {
        foreach (var user in users)
        {
            user.update_at = DateTime.Now;
        }
        ZVerseMysqlConnect.UpdateBatchTemplate<zverse_friend>(users);
    }

}
