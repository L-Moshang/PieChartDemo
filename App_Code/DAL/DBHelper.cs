using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

/// <summary>
///DBHelper 的摘要说明
/// </summary>
public class DBHelper
{



    public DBHelper()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }


    /// <summary>
    /// 数据库连接
    /// </summary>
    /// <returns>conn连接对象</returns>
    public static SqlConnection getConn()
    {
        
        SqlConnection conn = new SqlConnection();
        //conn.ConnectionString = "server=172.18.23.88;database=keCheng;user id=sa;password=123";
        conn.ConnectionString = "server=(local);database=lianxiDB;user id=sa;password=123";
        conn.Open();
        return conn;
    }


    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="cmd">命令对象</param>
    /// <returns>数据集</returns>
    public static DataSet query(SqlCommand cmd)
    {
        DataSet ds = new DataSet();
        SqlConnection conn = getConn();
        cmd.Connection = conn;
        //笔记：数据适配器
        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        sda.Fill(ds);
        sda.Dispose();
        cmd.Dispose();
        conn.Close();
        conn.Dispose();

        return ds;
    }


    /// <summary>
    /// 增删改查
    /// </summary>
    /// <param name="cmd">命令对象</param>
    /// <returns>是否成功</returns>
    public static bool excute(SqlCommand cmd)
    {
        int row;
        SqlConnection conn = getConn();
        cmd.Connection = conn;
        try
        {
            row = cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            cmd.Dispose();
            conn.Close();
            conn.Dispose();

            throw ex;
        }

        if (row > 0)
        {
            return true;
        }
        else
        { 
            return false;
        }

    }





}