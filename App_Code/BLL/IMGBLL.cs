using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
///IMGBLL 的摘要说明
/// </summary>
public class IMGBLL
{
    public IMGBLL()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }
    //
    public DataSet GetData()
    {
        IMGDAL imgDal = new IMGDAL();
        return imgDal.GetData();
    }
}