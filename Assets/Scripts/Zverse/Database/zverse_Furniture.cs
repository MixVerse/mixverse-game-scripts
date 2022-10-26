/* ========================================================
*      作 者：Lixi 
*      主 题：
*      主要功能：

*      详细描述：

*      创建时间：2022-04-02 12:13:48
*      修改记录：
*      版 本：1.0
 ========================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySql.Data.MySqlClient;
using System.Data;

public class zverse_Furniture
{
    public long id { get; set; }  //自增序列
    public long user_id { get; set; }  // 用户id
    public string item_id { get; set; }  //物品id
    public string item_name { get; set; } //物品名称
    public int slot_index { get; set; }  //物品栏中的索引
    public int amount { get; set; }   //物品数量
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }




}
public partial class zverse_furniture_dao
{
    private static string name = "zverse_furniture";

    public static bool CreateTB()
    {
        string[] cols = new string[]
        {
            "id bigint(20) NOT NULL AUTO_INCREMENT",
            "user_id bigint(20) NOT NULL ",
            "item_id varchar(20) NOT NULL ",
            "item_name varchar(20) NOT NULL COMMENT '物品名称'",
            "slot_index int(11) NOT NULL COMMENT '此物品在物品栏中的索引'",
            "amount int(11) NOT NULL COMMENT '物品数量'",
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


    public static List<zverse_Furniture> QueryUserFurniture(long user_id)
    {
        string sql = "select * from zverse_furniture where user_id=@user_id";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        List<zverse_Furniture> list = new DatatableToEntity<zverse_Furniture>().FillModel(ds);
        return list;

    }

    public static int UpdateInfo(long user_id, List<zverse_Furniture> users)
    {
        string sql = string.Format("delete from zverse_weapon where user_id={0}", user_id);
        object result = ZVerseMysqlConnect.ExecuteNonQuery(sql);

        Debug.Log(result);
        //return result == null ? 0 : Convert.ToInt32(result);

        InsertBatch(users);

        return result == null ? 0 : Convert.ToInt32(result);
    }



    public static void InsertBatch(List<zverse_Furniture> users)
    {

        foreach (var user in users)
        {
            user.create_at = DateTime.Now;
            user.update_at = DateTime.Now;
        }
        ZVerseMysqlConnect.InsertBatchTemplate<zverse_Furniture>(users);

    }
}
