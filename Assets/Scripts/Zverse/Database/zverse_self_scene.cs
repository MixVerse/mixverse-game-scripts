using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySql.Data.MySqlClient;
using System.Data;

public class zverse_self_scene 
{
    public long id { get; set; }  //自增序列
    public long user_id { get; set; }  // 用户id
    public int max_number { get; set; } //房间最大人数
    public int permission { get; set; } //房间权限
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
}

public partial class zverse_self_scene_dao
{
    private static string name = "zverse_self_scene";

    public static bool CreateTB()
    {
        string[] cols = new string[]
        {
            "id bigint(20) NOT NULL AUTO_INCREMENT",
            "user_id bigint(20) NOT NULL ",
            "max_number int(11) NOT NULL ",
            "permission int(11) NOT NULL ",
            "create_at datetime DEFAULT NULL",
            "update_at datetime DEFAULT NULL"
        };
        string[] others = new string[]
        {
            "PRIMARY KEY (id)",
            "KEY user_id (user_id)"
        };
        int size = ZVerseMysqlConnect.CreateTable(name, cols, others);
        return size >= 1 ? true : false;
    }


    public static zverse_self_scene QueryUserScene(long user_id)
    {
        string sql = "select * from zverse_self_scene where user_id=@user_id";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        List<zverse_self_scene> list = new DatatableToEntity<zverse_self_scene>().FillModel(ds);
        return list == null ? null : list[0];

    }



    public static int Update(zverse_self_scene user)
    {
        user.update_at = DateTime.Now;

        return ZVerseMysqlConnect.UpdateTemplate(user);
    }




    public static int Insert(zverse_self_scene user)
    {

        user.create_at = DateTime.Now;
        user.update_at = DateTime.Now;
        
       return ZVerseMysqlConnect.InsertTemplate(user);

    }
}
