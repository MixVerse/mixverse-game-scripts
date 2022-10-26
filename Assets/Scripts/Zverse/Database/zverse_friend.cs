using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;
using System;

/// <summary>
/// 玩家好友类
/// </summary>
public partial class zverse_friend 
{

    public long id { get; set; }
    public long user_id { get; set; }
    public long friend_id { get; set; }
    public string friend_name { get; set; }
    public int status { get; set; }  //好友添加状态 0 添加成功 1 待审核 2 已拒绝 3 已删除
    public string apply_message { get; set; }  //添加好友验证消息
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
    public DateTime add_time_at { get; set; }

}

public partial class zverse_friend_dao
{

    private static string name = "zverse_friend";

    public static bool CreateTB()
    {
        string[] cols = new string[]
        {
            "id bigint(20) NOT NULL AUTO_INCREMENT",
            "user_id bigint(20) NOT NULL",
            "friend_id bigint(20) NOT NULL",
            "friend_name varchar(20) NOT NULL",
            "status int(4) NOT NULL",
            "apply_message varchar(128) DEFAULT ''",
            "create_at datetime DEFAULT NULL",
            "update_at datetime DEFAULT NULL",
            "add_time_at datetime DEFAULT NULL"
        };
        string[] others = new string[]
        {
            "PRIMARY KEY (id)",
            "KEY user_id (user_id)"
        };
        int size = ZVerseMysqlConnect.CreateTable(name, cols, others);
        return size >= 1 ? true : false;
    }

    public static zverse_friend  QueryFriendApply(long user_id,long friend_id)
    {

        string sql = "select * from zverse_friend  where user_id=@user_id and friend_id=@friend_id and status=1";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) ,new MySqlParameter("@friend_id",friend_id)};

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
  
        List<zverse_friend> list = new DatatableToEntity<zverse_friend>().FillModel(ds);
        if (list != null)
            return list[0];
        return null;
    }

    public static List<zverse_friend> QueryAllFriendApply(long user_id)
    {
        string sql = "select * from zverse_friend  where user_id=@user_id and status=1";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id)};

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);

        List<zverse_friend> list = new DatatableToEntity<zverse_friend>().FillModel(ds);
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


    public static int Insert(zverse_friend user)
    {

        user.create_at = DateTime.Now;
        user.update_at = DateTime.Now;
        return ZVerseMysqlConnect.InsertTemplate<zverse_friend>(user);

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



