using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;
using System;

public partial class zverse_inventory
{

    public long id { get; set; }  //自增序列
    public long user_id {get;set;}  // 用户id
    public string item_id { get; set; }  //物品id
    public string item_name { get; set; } //物品名称
    public int slot_index { get; set; }  //物品栏中的索引
    public int amount { get; set; }   //物品数量
    public int durability { get; set; } //当前耐久度
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }




}
public partial class zverse_inventory_dao
{
    private static string name = "zverse_inventory";

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
            "durability int(11) NOT NULL COMMENT '物品当前耐久度'",
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


    public static List<zverse_inventory> QueryUserInventory(long user_id)
    {
        string sql = "select * from zverse_inventory where user_id=@user_id";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        List<zverse_inventory> list = new DatatableToEntity<zverse_inventory>().FillModel(ds);
        return list;

    }


}
