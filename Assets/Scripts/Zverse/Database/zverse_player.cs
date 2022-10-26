using System.Collections.Generic;
using UnityEngine;
using System;
using MySql.Data.MySqlClient;
using System.Data;
public partial class zverse_player
{

    public long user_id { get; set; }
    public string user_name { get; set; }
    public decimal x { get; set; }
    public decimal y { get; set; }
    public decimal z { get; set; }
    public int level { get; set; }
    public long health { get; set; }
    public long mana { get; set; }
    public long experience { get; set; } 
    public long gold { get; set; }
    public bool online { get; set; }
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
    public bool deleted { get; set; }


}
public partial class zverse_player_dao
{
    private static string name = "zverse_player";

    public static bool CreateTB()
    {
        string[] cols = new string[]
        {
            "user_id bigint(20) NOT NULL",
            "user_name varchar(20) NOT NULL ",
            "x decimal(18,7) NOT NULL DEFAULT 0",
            "Y decimal(18,7) NOT NULL DEFAULT 0",
            "Z decimal(18,7) NOT NULL DEFAULT 0",
            "level int(11) NOT NULL ",
            "health bigint(20) NOT NULL ",
            "mana bigint(20) NOT NULL ",
            "experience bigint(20) NOT NULL DEFAULT 0",
            "gold bigint(20) NOT NULL DEFAULT 0",
            "online tinyint(1) NOT NULL DEFAULT 0",
            "create_at datetime DEFAULT NULL",
            "update_at datetime DEFAULT NULL",
            "deleted tinyint(1) NOT NULL DEFAULT 0"

            
        };
        string[] others = new string[]
        {
            "PRIMARY KEY (user_id)",
            "KEY user_id (user_id)",
            "KEY user_name (user_name)"
        };
        int size = ZVerseMysqlConnect.CreateTable(name, cols, others);
        return size >= 1 ? true : false;
    }

    /// <summary>
    /// 角色名是否存在
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public static bool CharacterExist(string characterName)
    {

        string sql = string.Format("select count(character_name) from zverse_character where character_name={0}", characterName);
        object result = ZVerseMysqlConnect.GetSingle(sql);


        return result == null ? false : true;
    }
    /// <summary>
    /// 检查此账户下的角色数量
    /// </summary>
    /// <param name="user_id"></param>
    /// <returns></returns>
    public static int PlayerCount(long user_id)
    {
        string sql = string.Format("select count(user_id) from zverse_player where user_id={0}", user_id);
        object result = ZVerseMysqlConnect.GetSingle(sql);

        Debug.Log(result);
        return result == null ? 0 : Convert.ToInt32(result);
    }

    public static zverse_player QueryPlayerById(long user_id)
    {

        string sql = string.Format("select * from zverse_player where user_id={0} and deleted=0", user_id);
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql);
        List<zverse_player> list = new DatatableToEntity<zverse_player>().FillModel(ds);
        return list == null ? null : list[0];
    }

    public static List<zverse_player> QueryPlayerByLikeName(string user_name)
    {
        string sql = string.Format("select * from zverse_player where user_name like '%{0}%' and deleted=0", user_name);
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql);
        List<zverse_player> list = new DatatableToEntity<zverse_player>().FillModel(ds);
        return list;
    }


    public static int Insert(zverse_player user)
    {

        user.create_at = DateTime.Now;
        user.update_at = DateTime.Now;
        return ZVerseMysqlConnect.InsertTemplate<zverse_player>(user, "user_id",false);

    }

}

