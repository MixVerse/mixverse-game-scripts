using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;

public partial class zverse_users 
{
    
    public long user_id { get; set; }
    public string user_name { get; set; }
    public string password { get; set; }
    public string phone { get; set; }
    public DateTime create_at { get; set; }
    public DateTime update_at { get; set; }
    public DateTime last_login_at { get; set; }
    public bool freeze { get; set; }
    public bool master { get; set; }



    public override string ToString()
    {
        return string.Format("user_id={0},user_name={1},password={2},phone={3},create_at={4},update_at={5},last_login_at={6},freeze={7},master={8}", user_id, user_name, password, phone, create_at.ToLongDateString(), update_at.ToLongDateString(), last_login_at.ToLongDateString(),freeze,master);
        
    }
}

public partial class zverse_users_dao
{
    private static string name = "zverse_users";

    public static bool  CreateTB()
    {
        string[] cols = new string[]
        {
            "user_id bigint(20) NOT NULL AUTO_INCREMENT",
            "user_name varchar(16) NOT NULL COMMENT '用户名'",
            "password varchar(128) NOT NULL",
            "phone varchar(11) DEFAULT NULL",
            "create_at datetime DEFAULT NULL",
            "update_at datetime DEFAULT NULL",
            "last_login_at datetime DEFAULT NULL",
            "freeze tinyint(1) NOT NULL DEFAULT 0",
            "master tinyint(1) NOT NULL DEFAULT 0"
        };
        string[] others = new string[]
        {
            "PRIMARY KEY (user_id)",
            "KEY user_id (user_id)"
        };
        int size = ZVerseMysqlConnect.CreateTable(name,cols,others);
        return size >= 1 ? true : false;
    }

    public static int Insert(zverse_users user)
    {

        user.create_at = DateTime.Now;
        user.update_at = DateTime.Now;
        return ZVerseMysqlConnect.InsertTemplate<zverse_users>(user, "user_id");
        
    }

    public static void InsertBatch(List<zverse_users> users)
    {

        List<zverse_users> test = new List<zverse_users>();
        test.Add(new zverse_users() { user_name = "wew", password = "dfsdf", phone = "dfddfdfdf" });
        test.Add(new zverse_users() { user_name = "2222", password = "dfsdf", phone = "dfddfdfdf" });
        foreach (var user in users)
        {
            user.create_at = DateTime.Now;
            user.update_at = DateTime.Now;
        }
        ZVerseMysqlConnect.InsertBatchTemplate<zverse_users>(test, "user_id");

    }

    public static int UpdateInfo(zverse_users user)
    {
       user.update_at = DateTime.Now;
       return ZVerseMysqlConnect.UpdateTemplate<zverse_users>(user, "user_id");
    }

    public static void UpdateBatch(List<zverse_users> users)
    {
        foreach (var user in users)
        {
            user.update_at = DateTime.Now;
        }
        ZVerseMysqlConnect.UpdateBatchTemplate<zverse_users>(users, "user_id");
    }



    public static zverse_users QueryUser(long user_id)
    {

        string sql = "select * from zverse_users  where user_id=@user_id";
       // MySqlParameter p = new MySqlParameter("@user_id", MySqlDbType.Int64);
       // p.Value = user_id;
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        Debug.Log(ds.Tables[0].Rows.Count);
        List<zverse_users> list = new DatatableToEntity<zverse_users>().FillModel(ds);
        if (list != null)
            return list[0];
        return null;
    }

    public static bool CheckExistByPhone(string phoneNum)
    {
        string sql = "select * from zverse_users  where phone=@phone";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@phone", phoneNum) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        return ds.Tables[0].Rows.Count > 0 ? true : false;
    }

    public static zverse_users QueryUserByPhone(string phoneNum)
    {
        string sql = "select * from zverse_users  where phone=@phone";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@phone", phoneNum) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        List<zverse_users> list = new DatatableToEntity<zverse_users>().FillModel(ds);
        return list == null ? null : list[0];
    }


    public static List<zverse_users> QueryUserByName (string user_name)
    {

        string sql = "select * from zverse_users  where user_name=@user_name and freeze=0";
        // MySqlParameter p = new MySqlParameter("@user_id", MySqlDbType.Int64);
        // p.Value = user_id;
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_name", user_name) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        Debug.Log(ds.Tables[0].Rows.Count);
        List<zverse_users> list = new DatatableToEntity<zverse_users>().FillModel(ds);
        return list;
    }

    public static List<zverse_users> QueryUserByLastLoginAt(DateTime start,DateTime end)
    {

        string sql = "select * from zverse_users  where last_login_at between @start and @end";
        // MySqlParameter p = new MySqlParameter("@user_id", MySqlDbType.Int64);
        // p.Value = user_id;
        System.Object[] pts = new System.Object[] { new MySqlParameter("@start", start),new MySqlParameter("@end",end) };

        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql, pts);
        Debug.Log(ds.Tables[0].Rows.Count);
        List<zverse_users> list = new DatatableToEntity<zverse_users>().FillModel(ds);
        return list;
    }


    public static List<zverse_users> QueryUserByIds(List<string> ids)
    {

        string sql = "select * from zverse_users  where user_id in "+ZVerseMysqlConnect.listToSQL_IN_(ids);
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql);
        Debug.Log(ds.Tables[0].Rows.Count);
        List<zverse_users> list = new DatatableToEntity<zverse_users>().FillModel(ds);
        return list;
    }

    /// <summary>
    /// 玩家登陆
    /// </summary>
    /// <param name="user_name"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static zverse_users Login(string user_name,string password,out string  msg)
    {
        List<zverse_users> users = QueryUserByName(user_name);
        if(users!=null)
        {
            Debug.Log(users[0].ToString());
            if (users[0].password.Equals(password))
            {
                msg = "";
                users[0].last_login_at = DateTime.Now;
                //更新登陆时间
                UpdateInfo(users[0]);
                return users[0];
            }
            else
            {
                msg = MessageConstant.USER_PASSWORD_ERROR;
                return null;
            }   
        }else{

            msg = MessageConstant.USER_NAME_NOT_EXIST;
            return null;
        }
    }
}


