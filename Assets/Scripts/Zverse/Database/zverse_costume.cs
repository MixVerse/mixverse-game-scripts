using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;

public partial class zverse_costume : zverse_inventory
{
    




}
public partial class zverse_costume_dao
{
    private static string name = "zverse_costume";


    public static List<zverse_costume> QueryUserCostume(long user_id)
    {
        string sql = "select * from zverse_costume where user_id=@user_id";
        System.Object[] pts = new System.Object[] { new MySqlParameter("@user_id", user_id) };
        DataSet ds = ZVerseMysqlConnect.ExcuteQuery(sql,pts);
        List<zverse_costume> list = new DatatableToEntity<zverse_costume>().FillModel(ds);
        return list;

    }



}
