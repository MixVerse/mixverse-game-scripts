using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Reflection;

public class ZVerseMysqlConnect
{

    // private static ZVerseMysqlConnect instance;
    //
    // public static ZVerseMysqlConnect Instance {
    //     get {
    //         if(instance==null)
    //         {
    //             instance= new ZVerseMysqlConnect();
    //             connStr = string.Format("database={0};server={1};user={2};password={3};port={4}"
    //             , databaseName, host, userName, password, port);
    //         }
    //         return instance;
    //     }
    // }


    private static string host = @"121.196.192.184";
    //端口号
    private static string port = "3306";
    //用户名
    private static string userName = "root";
    //密码
    private static string password = "c19951108";
    //数据库名称
    private static string databaseName = "zverse";

    private static string connStr = "";

    public static void Init()
    {
        connStr = string.Format("database={0};server={1};user={2};password={3};port={4};Allow User Variables=True"
             , databaseName, host, userName, password, port);
    }


    #region  最终方案帮助接口

    /// <summary>
    /// 创建表
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cols"></param>
    /// <param name="others"></param>
    /// <param name="tableothers"></param>
    /// <returns></returns>
    public static int CreateTable(string name, string[] cols, string[] others, string tableothers = "")
    {

        string query = "CREATE TABLE " + name + " (" + cols[0];

        for (int i = 1; i < cols.Length; ++i)
        {
            query += ", " + cols[i];
        }
        for (int i = 0; i < others.Length; ++i)
        {
            query += ", " + others[i];
        }
        query += ") ";

        if (!string.IsNullOrEmpty(tableothers))
            query += tableothers;


        return ExecuteNonQuery(query);
    }

    /// <summary>
    /// 插入数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="row"></param>
    /// <returns></returns>
    public static int InsertTemplate<T>(T row, string pKey = "id",bool autoIncrease=true)
    {

        string tableName = row.GetType().Name;
        Debug.Log(tableName);
        PropertyInfo[] infos = row.GetType().GetProperties();
        string sql1 = "( ";
        string sql2 = "( ";
        for (int i = 0; i < infos.Length; i++)
        {
            PropertyInfo item = infos[i];
            if (!((item.Name.Equals(pKey)&&autoIncrease) || item.GetValue(row) == null || (item.PropertyType.Name.Equals("DateTime") && (DateTime)item.GetValue(row) == default(DateTime))))
            {
                sql1 = sql1 + item.Name + ",";
                if(item.PropertyType.Name.Equals("Boolean"))
                    sql2 = sql2 + item.GetValue(row).ToString() + ",";
                else
                    sql2 = sql2 + "'" + item.GetValue(row).ToString() + "',";

            }

        }
        sql1 = sql1.Substring(0, sql1.Length - 1) + " )";
        sql2 = sql2.Substring(0, sql2.Length - 1) + " )";
        string sql = string.Format("Insert Into {0} {1} Values {2}", tableName, sql1, sql2);
        Debug.Log(sql);
        return ExecuteNonQuery(sql);


    }

    public static void InsertBatchTemplate<T>(List<T> rows, string pKey = "id", bool autoIncrease = true)
    {

        List<string> sqls = new List<string>();
        for (int r = 0; r < rows.Count; r++)
        {

            T row = rows[r];
            string tableName = row.GetType().Name;
            Debug.Log(tableName);
            PropertyInfo[] infos = row.GetType().GetProperties();
            string sql1 = "( ";
            string sql2 = "( ";
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo item = infos[i];
                if (!((item.Name.Equals(pKey) && autoIncrease) || item.GetValue(row) == null || (item.PropertyType.Name.Equals("DateTime") && (DateTime)item.GetValue(row) == default(DateTime))))
                {
                    sql1 = sql1 + item.Name + ",";
                    if (item.PropertyType.Name.Equals("Boolean"))
                        sql2 = sql2 + item.GetValue(row).ToString() + ",";
                    else
                        sql2 = sql2 + "'" + item.GetValue(row).ToString() + "',";

                }

            }
            sql1 = sql1.Substring(0, sql1.Length - 1) + " )";
            sql2 = sql2.Substring(0, sql2.Length - 1) + " )";
            string sql = string.Format("Insert Into {0} {1} Values {2}", tableName, sql1, sql2);
            sqls.Add(sql);
            Debug.Log(sql);
        }
        //执行事务
        ExecuteNonQueryTran(sqls);


    }

    /// <summary>
    /// 获取批量插入的sql语句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rows"></param>
    /// <param name="pKey"></param>
    /// <param name="autoIncrease"></param>
    /// <returns></returns>
    public static List<string> GetInsertBatchTemplateSqls<T>(List<T> rows, string pKey = "id", bool autoIncrease = true)
    {

        List<string> sqls = new List<string>();
        for (int r = 0; r < rows.Count; r++)
        {

            T row = rows[r];
            string tableName = row.GetType().Name;
            Debug.Log(tableName);
            PropertyInfo[] infos = row.GetType().GetProperties();
            string sql1 = "( ";
            string sql2 = "( ";
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo item = infos[i];
                if (!((item.Name.Equals(pKey) && autoIncrease) || item.GetValue(row) == null || (item.PropertyType.Name.Equals("DateTime") && (DateTime)item.GetValue(row) == default(DateTime))))
                {
                    sql1 = sql1 + item.Name + ",";
                    if (item.PropertyType.Name.Equals("Boolean"))
                        sql2 = sql2 + item.GetValue(row).ToString() + ",";
                    else
                        sql2 = sql2 + "'" + item.GetValue(row).ToString() + "',";

                }

            }
            sql1 = sql1.Substring(0, sql1.Length - 1) + " )";
            sql2 = sql2.Substring(0, sql2.Length - 1) + " )";
            string sql = string.Format("Insert Into {0} {1} Values {2}", tableName, sql1, sql2);
            sqls.Add(sql);
            Debug.Log(sql);
        }

        return sqls;


    }




    public static int UpdateTemplate<T>(T row, string pKey = "id")
    {

        string tableName = row.GetType().Name;
        Debug.Log(tableName);
        PropertyInfo[] infos = row.GetType().GetProperties();
        string sql1 = "";
        string pValue = "";
        for (int i = 0; i < infos.Length; i++)
        {
            PropertyInfo item = infos[i];
            if (item.Name.Equals(pKey))
                pValue = item.GetValue(row).ToString();
            if (!(item.Name.Equals(pKey) || item.GetValue(row) == null || (item.PropertyType.Name.Equals("DateTime") && (DateTime)item.GetValue(row) == default(DateTime))))
            {
                if (item.PropertyType.Name.Equals("Boolean"))
                    sql1 = sql1 + item.Name + "= " + item.GetValue(row).ToString() + ",";
                else
                    sql1 = sql1 + item.Name + "= '" + item.GetValue(row).ToString() + "',";


            }

        }
        sql1 = sql1.Substring(0, sql1.Length - 1);
        string sql = string.Format("Update {0} Set {1} Where {2}={3}", tableName, sql1,pKey,pValue);
        Debug.Log(sql);
        return ExecuteNonQuery(sql);


    }

    public static void UpdateBatchTemplate<T>(List<T> rows, string pKey = "id")
    {

        List<string> sqls = new List<string>();
        for(int r=0;r<rows.Count;r++)
        {
            T row = rows[r];
            string tableName = row.GetType().Name;
            Debug.Log(tableName);
            PropertyInfo[] infos = row.GetType().GetProperties();
            string sql1 = "";
            string pValue = "";
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo item = infos[i];
                if (item.Name.Equals(pKey))
                    pValue = item.GetValue(row).ToString();
                if (!(item.Name.Equals(pKey) || item.GetValue(row) == null || (item.PropertyType.Name.Equals("DateTime") && (DateTime)item.GetValue(row) == default(DateTime))))
                {
                    if (item.PropertyType.Name.Equals("Boolean"))
                        sql1 = sql1 + item.Name + "= " + item.GetValue(row).ToString() + ",";
                    else
                        sql1 = sql1 + item.Name + "= '" + item.GetValue(row).ToString() + "',";

                }

            }
            sql1 = sql1.Substring(0, sql1.Length - 1);
            string sql = string.Format("Update {0} Set {1} Where {2}={3}", tableName, sql1, pKey, pValue);
            sqls.Add(sql);
            Debug.Log(sql);
        }
       
        ExecuteNonQueryTran(sqls);


    }



    #endregion


    /// <summary>
    /// 测试是否链接上数据库
    /// </summary>
    /// <returns></returns>
    public bool TestConnection()
    {
        bool isConnected = true;
        //发送数据库连接字段 创建连接通道
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            try
            {
                //打开连接通道
                connection.Open();
            }
            catch (MySqlException E)
            {
                //如果有异常 则连接失败
                isConnected = false;
                throw new Exception(E.Message);
            }
            finally
            {
                //关闭连接通道
                connection.Close();
            }
        }

        return isConnected;
    }



    #region 拼接式sql语句


    /// <summary>
    /// 插入一条数据，包括所有，不适用自动累加ID。
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int InsertInto(string tableName, string[] values)
    {
        string query = "INSERT INTO " + tableName + " VALUES (" + "'" + values[0] + "'";

        for (int i = 1; i < values.Length; ++i)
        {
            query += ", " + "'" + values[i] + "'";
        }

        query += ")";

        return ExecuteNonQuery(query);
    }

    /// <summary>
    /// 插入部分ID
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static int InsertInto(string tableName, string[] cols, string[] values)
    {
        if (cols.Length != values.Length)
        {
            throw new Exception("columns.Length != colType.Length");
        }

        string query = "INSERT INTO " + tableName + " (" + cols[0];
        for (int i = 1; i < cols.Length; ++i)
        {
            query += ", " + cols[i];
        }

        query += ") VALUES (" + "'" + values[0] + "'";
        for (int i = 1; i < values.Length; ++i)
        {
            query += ", " + "'" + values[i] + "'";
        }

        query += ")";

        return ExecuteNonQuery(query);
    }

    /// <summary>
    /// 当指定字段满足一定条件时，更新指定字段的数据
    /// 例如更新在user这个表中字段名为userAccount的值等于10086时，将对应userPwd字段的值改成newMd5SumPassword
    /// ("users", new string[] { "userPwd" }, new string[] { newMd5SumPassword }, "userAccount", "10086")
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols">字段</param>
    /// <param name="colsValues">字段值</param>
    /// <param name="selectKey">指定的字段</param>
    /// <param name="selectValue">指定字段满足的条件</param>
    /// <returns></returns>
    public static int UpdateInto(string tableName, string[] cols, string[] colsValues, string selectKey, string selectValue)
    {
        string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + "'" + colsValues[0] + "'";

        for (int i = 1; i < colsValues.Length; ++i)
        {
            query += ", " + cols[i] + " =" + "'" + colsValues[i] + "'";
        }

        query += " WHERE " + selectKey + " = " + selectValue;

        return ExecuteNonQuery(query);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="cols">字段</param>
    /// <param name="colsValues">字段值</param>
    /// <returns></returns>
    public static int Delete(string tableName, string[] cols, string[] colsValues)
    {
        string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + colsValues[0];

        for (int i = 1; i < colsValues.Length; ++i)
        {
            query += " or " + cols[i] + " = " + colsValues[i];
        }
        return ExecuteNonQuery(query);
    }

    /// <summary>
    /// 查询指定字段数据中满足条件的
    /// DataSet内存中的数据库，DataSet是不依赖于数据库的独立数据集合,是一种不包含表头的纯数据文件
    /// 有条件的查询，查询在users这个表当中，只需要字段名为userAccount，userPwd，userName，ID这几个字段对应的数据，
    /// 满足条件为 userAccount对应的value=account， userPwd对应的value=md5Password；
    /// ("users", new string[] { "userAccount", "userPwd", "userName", "ID" }, new string[] { "userAccount", "userPwd" }, new string[] { "=", "=" }, new string[] { account, md5Password });
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="items">字段名</param>
    /// <param name="cols">字段名</param>
    /// <param name="operations">条件运算符</param>
    /// <param name="values">满足的条件值</param>
    /// <returns></returns>
    public static DataSet SelectWhere(string tableName, string[] items, string[] cols, string[] operations, string[] values)
    {
        if (cols.Length != operations.Length || operations.Length != values.Length)
        {
            throw new Exception("col.Length != operation.Length != values.Length");
        }

        string query = "SELECT " + items[0];

        for (int i = 1; i < items.Length; ++i)
        {
            query += ", " + items[i];
        }

        query += " FROM " + tableName + " WHERE " + cols[0] + operations[0] + "'" + values[0] + "' ";

        for (int i = 1; i < cols.Length; ++i)
        {
            query += " AND " + cols[i] + operations[i] + "'" + values[i] + "' ";
        }

        return ExecuteQuery(query);
    }
    /// <summary>
    /// 查询指定字段
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="items">字段名</param>
    /// <returns></returns>
    public static DataSet Select(string tableName, string[] items)
    {
        string query = "SELECT " + items[0];

        for (int i = 1; i < items.Length; ++i)
        {
            query += ", " + items[i];
        }

        query += " FROM " + tableName;

        return ExecuteQuery(query);
    }

    /// <summary>
    /// 查询所有字段
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <returns></returns>
    public static DataSet Select(string tableName)
    {
        string query = "SELECT * FROM " + tableName;
        return ExecuteQuery(query);
    }

    #endregion

    #region 执行简单SQL语句

    /// <summary>
    /// 执行SQL语句，返回影响的记录数。用于Update、Insert和Delete
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteNonQuery(string SQLString)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行SQL语句，设置命令的执行等待时间
    /// </summary>
    /// <param name="SQLString"></param>
    /// <param name="Times"></param>
    /// <returns></returns>
    public static int ExecuteNonQueryByTime(string SQLString, int Times)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    cmd.CommandTimeout = Times;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行多条SQL语句，实现数据库事务。
    /// </summary>
    /// <param name="SQLStringList">多条SQL语句</param>
    public static void ExecuteNonQueryTran(List<string> SQLStringList)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            MySqlTransaction tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                for (int n = 0; n < SQLStringList.Count; n++)
                {
                    string strsql = SQLStringList[n];
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (MySqlException E)
            {
                tx.Rollback();
                throw new Exception(E.Message);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
    }

    /// <summary>
    /// 执行带一个存储过程参数的的SQL语句。
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteNonQuery(string SQLString, string content)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            MySqlCommand cmd = new MySqlCommand(SQLString, connection);
            MySqlParameter myParameter = new MySqlParameter("@content", MySqlDbType.Text);
            myParameter.Value = content;
            cmd.Parameters.Add(myParameter);
            try
            {
                connection.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (MySqlException E)
            {
                throw new Exception(E.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }

    /// <summary>
    /// 执行带一个存储过程参数的的SQL语句。
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
    /// <returns>影响的记录数</returns>
    public static object ExecuteScalar(string SQLString, string content)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            MySqlCommand cmd = new MySqlCommand(SQLString, connection);
            MySqlParameter myParameter = new MySqlParameter("@content", MySqlDbType.Text);
            myParameter.Value = content;
            cmd.Parameters.Add(myParameter);
            try
            {
                connection.Open();
                object obj = cmd.ExecuteScalar();
                if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
                else
                {
                    return obj;
                }
            }
            catch (MySqlException E)
            {
                throw new Exception(E.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }

    /// <summary>
    /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
    /// </summary>
    /// <param name="strSQL">SQL语句</param>
    /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteNonQueryInsertImg(string strSQL, byte[] fs)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            MySqlParameter myParameter = new MySqlParameter("@fs", MySqlDbType.Binary);
            myParameter.Value = fs;
            cmd.Parameters.Add(myParameter);
            try
            {
                connection.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (MySqlException E)
            {
                throw new Exception(E.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }

    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="SQLString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string SQLString)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySqlException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行查询语句，返回MySqlDataReader(使用该方法切记要手工关闭MySqlDataReader和连接)
    /// </summary>
    /// <param name="strSQL">查询语句</param>
    /// <returns>MySqlDataReader</returns>
    public static MySqlDataReader ExecuteReader(string strSQL)
    {
        MySqlConnection connection = new MySqlConnection(connStr);
        MySqlCommand cmd = new MySqlCommand(strSQL, connection);
        try
        {
            connection.Open();
            MySqlDataReader myReader = cmd.ExecuteReader();
            return myReader;
        }
        catch (MySqlException e)
        {
            throw new Exception(e.Message);
        }
        //finally //不能在此关闭，否则，返回的对象将无法使用
        //{
        // cmd.Dispose();
        // connection.Close();
        //}
    }

    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="SQLString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet ExecuteQuery(string SQLString)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            DataSet ds = new DataSet();
            try
            {
                connection.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(SQLString, connection);
                da.Fill(ds);
            }
            catch (MySqlException ex)
            {
                connection.Close();
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }
    }

    /// <summary>
    /// 执行查询语句，返回DataSet,设置命令的执行等待时间
    /// </summary>
    /// <param name="SQLString"></param>
    /// <param name="Times"></param>
    /// <returns></returns>
    public static DataSet ExecuteQuery(string SQLString, int Times)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            DataSet ds = new DataSet();
            try
            {
                connection.Open();
                MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                command.SelectCommand.CommandTimeout = Times;
                command.Fill(ds, "ds");
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }
    }

    /// <summary>
    /// 获取SQL查询记录条数
    /// </summary>
    /// <param name="sqlstr">SQL语句</param>
    /// <returns></returns>
    public static int GetRowsNum(string SQLString)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            DataSet ds = new DataSet();
            try
            {
                connection.Open();
                MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                command.Fill(ds, "ds");
                return ds.Tables[0].Rows.Count;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    #endregion 执行简单SQL语句

    #region 执行带参数的SQL语句

    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="SQLString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteNonQuery(string SQLString, params System.Object[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (MySqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行多条SQL语句，实现数据库事务。
    /// </summary>
    /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的 Object[]）</param>
    public static void ExecuteNonQueryTran(Hashtable SQLStringList)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            conn.Open();
            using (MySqlTransaction trans = conn.BeginTransaction())
            {
                MySqlCommand cmd = new MySqlCommand();
                try
                {
                    //循环
                    foreach (DictionaryEntry myDE in SQLStringList)
                    {
                        string cmdText = myDE.Key.ToString();
                        System.Object[] cmdParms = (System.Object[])myDE.Value;
                        PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                        int val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        trans.Commit();
                    }
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="SQLString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string SQLString, params System.Object[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((System.Object.Equals(obj, null)) || (System.Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySqlException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
    }

    /// <summary>
    /// 执行查询语句，返回MySqlDataReader (使用该方法切记要手工关闭MySqlDataReader和连接)
    /// </summary>
    /// <param name="strSQL">查询语句</param>
    /// <returns>MySqlDataReader</returns>
    public static MySqlDataReader ExecuteReader(string SQLString, params System.Object[] cmdParms)
    {
        MySqlConnection connection = new MySqlConnection(connStr);
        MySqlCommand cmd = new MySqlCommand();
        try
        {
            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
            MySqlDataReader myReader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return myReader;
        }
        catch (MySqlException e)
        {
            throw new Exception(e.Message);
        }
        //finally //不能在此关闭，否则，返回的对象将无法使用
        //{
        // cmd.Dispose();
        // connection.Close();
        //}
    }

    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="SQLString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet ExcuteQuery(string SQLString, params System.Object[] cmdParms)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {

            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
            Debug.Log(cmd.CommandText);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {

                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
                return ds;
            }
        }
    }


    private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, System.Object[] cmdParms)
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();

        cmd.Connection = conn;
        cmd.CommandText = cmdText;

        if (trans != null)
            cmd.Transaction = trans;

        cmd.CommandType = CommandType.Text;//cmdType;

        if (cmdParms != null)
        {
            foreach (MySqlParameter parameter in cmdParms)
            {
                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                    (parameter.Value == null))
                {
                    parameter.Value = DBNull.Value;
                }

                cmd.Parameters.Add(parameter);
            }
        }
    }

    #endregion 执行带参数的SQL语句



    #region 存储过程操作

    /// <summary>
    /// 执行存储过程  (使用该方法切记要手工关闭MySqlDataReader和连接)
    /// 手动关闭不了，所以少用，MySql.Data组组件还没解决该问题
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>MySqlDataReader</returns>
    public static MySqlDataReader RunProcedure(string storedProcName, System.Object[] parameters)
    {
        MySqlConnection connection = new MySqlConnection(connStr);
        MySqlDataReader returnReader;
        connection.Open();
        MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
        command.CommandType = CommandType.StoredProcedure;
        returnReader = command.ExecuteReader();
        //Connection.Close(); 不能在此关闭，否则，返回的对象将无法使用
        return returnReader;
    }

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <param name="tableName">DataSet结果中的表名</param>
    /// <returns>DataSet</returns>
    public static DataSet RunProcedure(string storedProcName, System.Object[] parameters, string tableName)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            DataSet dataSet = new DataSet();
            connection.Open();
            MySqlDataAdapter sqlDA = new MySqlDataAdapter();
            sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
            sqlDA.Fill(dataSet, tableName);
            connection.Close();
            return dataSet;
        }
    }

    public static DataSet RunProcedure(string storedProcName, System.Object[] parameters, string tableName, int Times)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            DataSet dataSet = new DataSet();
            connection.Open();
            MySqlDataAdapter sqlDA = new MySqlDataAdapter();
            sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
            sqlDA.SelectCommand.CommandTimeout = Times;
            sqlDA.Fill(dataSet, tableName);
            connection.Close();
            return dataSet;
        }
    }

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns></returns>
    public static void RunProcedureNull(string storedProcName, System.Object[] parameters)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            connection.Open();
            MySqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    /// <summary>
    /// 执行存储过程，返回第一行第一列的数据
    /// </summary>
    /// <param name="CommandText">T-SQL语句；例如："pr_shell 'dir *.exe'"或"select * from sysobjects where xtype=@xtype"</param>
    /// <param name="parameters">SQL参数</param>
    /// <returns>返回第一行第一列，用Convert.To{Type}把类型转换为想要的类型</returns>
    public object ExecuteScaler(string storedProcName, System.Object[] parameters)
    {
        using (MySqlConnection connection = new MySqlConnection(connStr))
        {
            object returnObjectValue;
            connection.Open();
            MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            returnObjectValue = command.ExecuteScalar();
            connection.Close();
            return returnObjectValue;
        }
    }

    /// <summary>
    /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>SqlCommand</returns>
    private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName, System.Object[] parameters)
    {
        MySqlCommand command = new MySqlCommand(storedProcName, connection);
        command.CommandType = CommandType.StoredProcedure;
        foreach (MySqlParameter parameter in parameters)
        {
            if (parameter != null)
            {
                // 检查未分配值的输出参数,将其分配以DBNull.Value.
                if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                    (parameter.Value == null))
                {
                    parameter.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter);
            }
        }

        return command;
    }

    /// <summary>
    /// 创建 MySqlCommand 对象实例(用来返回一个整数值)
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>MySqlCommand 对象实例</returns>
    private static MySqlCommand BuildIntCommand(MySqlConnection connection, string storedProcName, System.Object[] parameters)
    {
        MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
        command.Parameters.Add(new MySqlParameter("ReturnValue",
            MySqlDbType.Int32, 4, ParameterDirection.ReturnValue,
            false, 0, 0, string.Empty, DataRowVersion.Default, null));
        return command;
    }

    #endregion 存储过程操作

    #region 字符串工具
    public static string listToSQL_IN_(List<string> list)
    {
        string str = "";
        if (list == null || list.Count < 1)
            return str;
        str += "('" + list[0] + "'";
        for (int i = 1; i < list.Count; i++)
        {
            str += ",'" + list[i] + "'";
        }
        str += ")";
        return str;
    }



    #endregion


}

#region DataSet转化为实体类
public class DatatableToEntity<T> where T : new()
{
    /// <summary>
    /// 填充对象列表：用DataSet的第一个表填充实体类
    /// </summary>
    /// <param name="ds">DataSet</param>
    /// <returns></returns>
    public List<T> FillModel(DataSet ds)
    {
        if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return FillModel(ds.Tables[0]);
        }
    }

    // <summary>  
    /// 填充对象列表：用DataSet的第index个表填充实体类
    /// </summary>  
    public List<T> FillModel(DataSet ds, int index)
    {
        if (ds == null || ds.Tables.Count <= index || ds.Tables[index].Rows.Count == 0)
        {
            return null;
        }
        else
        {
            return FillModel(ds.Tables[index]);
        }
    }

    /// <summary>  
    /// 填充对象列表：用DataTable填充实体类
    /// </summary>  
    public List<T> FillModel(DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0)
        {
            return null;
        }
        List<T> modelList = new List<T>();
        foreach (DataRow dr in dt.Rows)
        {
            //T model = (T)Activator.CreateInstance(typeof(T));  
            T model = new T();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                {
                    // string value = dr[i] == null ? "" : dr[i].ToString();
                    //string typestr = dr[i].GetType().Name;
                    //if(typestr.Equals("DateTime"))
                    //value = dr[i].ToString();
                    propertyInfo.SetValue(model, dr[i], null);
                }

            }

            modelList.Add(model);
        }
        return modelList;
    }

    /// <summary>  
    /// 填充对象：用DataRow填充实体类
    /// </summary>  
    public T FillModel(DataRow dr)
    {
        if (dr == null)
        {
            return default(T);
        }

        //T model = (T)Activator.CreateInstance(typeof(T));  
        T model = new T();

        for (int i = 0; i < dr.Table.Columns.Count; i++)
        {
            PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null && dr[i] != DBNull.Value)
            {
                string value = dr[i] == null ? "" : dr[i].ToString();
                //string typestr = dr[i].GetType().Name;
                //if(typestr.Equals("DateTime"))
                //value = dr[i].ToString();
                propertyInfo.SetValue(model, value, null);
            }
        }
        return model;
    }
    #endregion

  
}





