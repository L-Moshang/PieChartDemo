using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

/// <summary>
///IMGDAL 的摘要说明
/// </summary>
public class IMGDAL
{
	public IMGDAL()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

    internal DataSet GetData()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "select * FROM tushu ";
        return DBHelper.query(cmd);
    }
}