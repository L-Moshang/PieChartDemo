using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Data;

public partial class DefaultGDI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        #region 数据填充注释
        /*Dictionary<string, double> data = new Dictionary<string, double>();
        data.Add("优良",15);
        data.Add("杰出",5);
        data.Add("一般",20);
        data.Add("不合格",50);
        */
        //string connString = "server=(local);Database=lianxiDB;user id=sa;password=123";
        ////建立与数据库连接的对象
        //SqlConnection conn = new SqlConnection(connString);
        ////打开数据库连接
        //conn.Open();
        ////定义查询数据库的SQL语句
        //string cmdtxt = "select * from tushu";
        ////定义一个SqlCommand命令对象
        //SqlCommand comm = new SqlCommand(cmdtxt, conn);
        ////定义一个数据集
        //DataSet ds = new DataSet();
        ////定义一个数据适配器
        //SqlDataAdapter da = new SqlDataAdapter(comm);
        ////填充数据集
        //da.Fill(ds);
        //conn.Close(); 
        #endregion

        DataSet ds = new DataSet();
        IMGBLL imgBll = new IMGBLL();
        ds = imgBll.GetData();
        
        //输出图片格式为gif
        //Response.ContentType = "image/gif";
        GetBitmap(600, 300, 100, "宋体", ds).Save(Response.OutputStream, ImageFormat.Gif);
    }

    /// <summary>      
    /// 按照四率 获得扇形图    
    /// </summary>   
    /// <param name="width">画板长度</param>   
    /// <param name="heigh">画板宽度</param>     
    /// <param name="r">饼图半径</param>   
    /// <param name="familyName">字体</param> 
    /// <param name="data">数据</param> 
    /// <returns></returns>   
    public Bitmap GetBitmap(int width, int heigh, int r, string familyName, DataSet ds)
    {
        //public Bitmap GetBitmap(int width, int heigh, int r, string familyName, Dictionary<string, double> data)
        //创建 长度为width 宽度为heigh的Bitmap实例
        Bitmap bitmap = new Bitmap(width, heigh);
        //封装  GDI
        Graphics graphics = Graphics.FromImage(bitmap);
        //用白色填充全部图片,因为默认是黑色  
        graphics.Clear(Color.White);

        //抗锯齿 
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //高质量的文字           
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        //像素均偏移0.5个单位,以打消锯齿       
        graphics.PixelOffsetMode = PixelOffsetMode.Half;

        #region 随机颜色生成器
        //生成伪随机颜色生成器
        ArrayList colors = new ArrayList();
        Random rnd = new Random();
        for (int iLoop = 0; iLoop < ds.Tables[0].Rows.Count; iLoop++)
            colors.Add(new SolidBrush(Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255))));
        #endregion


        #region 绘制饼状图
        //扇形区地点边框的原点地位      
        //Point circlePoint = new Point(Convert.ToInt32(textPoint.X + 100), 35);
        Point circlePoint = new Point(100, 35);
        //Point circlePoint = new Point(width / 2, heigh / 2);
        //总比 初始值  
        float totalRate = 0;
        //肇端角度 Y周正标的目标   
        float startAngle = 0;
        //当前比 初始值          
        float currentRate = 0;
        //圆地点边框的大小    
        Size cicleSize = new Size(r * 2, r * 2);
        //圆地点边框的地位
        Rectangle circleRectangle = new Rectangle(circlePoint, cicleSize);
        //遍历键值对数据集中的值 累加总比
        //foreach (var item in data) 
        //    totalRate += float.Parse(item.Value.ToString());

        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            //Convert.ToSingle(ds.Tables[0].Rows[iLoop]["投票数量"]);
            totalRate += Convert.ToSingle(ds.Tables[0].Rows[i][ds.Tables[0].Columns[1].ColumnName]);
        }
        //getColor(ds.Tables[0].Rows.Count);

        //遍历键值对数据集中的值 判断颜色和当前比
        //foreach (var item in data)
        for (int iLoop = 0; iLoop < ds.Tables[0].Rows.Count; iLoop++)
        {
            //currentRate = float.Parse(item.Value.ToString()) / totalRate * 360;
            currentRate = Convert.ToSingle(ds.Tables[0].Rows[iLoop][ds.Tables[0].Columns[1].ColumnName]) / totalRate * 360;//比率
            //画扇形
            graphics.DrawPie(Pens.White, circleRectangle, startAngle, currentRate);
            //graphics.FillPie(new SolidBrush(getColor(item.Key.ToString())), circleRectangle, startAngle, currentRate);
            //填充颜色
            //graphics.FillPie(new SolidBrush(getColor((ds.Tables[0].Rows[iLoop]["YF"]).ToString())), circleRectangle, startAngle, currentRate);
            graphics.FillPie((SolidBrush)colors[iLoop], circleRectangle, startAngle, currentRate);

            //至此 扇形图已经画完,下面是在扇形图上写上申明文字
            //当前圆的圆心 相对图片边框原点的坐标  
            PointF cPoint = new PointF(circlePoint.X + r, circlePoint.Y + r);
            //当前圆弧上的点   
            //cos(弧度)=X轴坐标/r 
            //弧度=角度*π/180       
            double relativeCurrentX = r * Math.Cos((360 - startAngle - currentRate / 2) * Math.PI / 180);
            double relativecurrentY = r * Math.Sin((360 - startAngle - currentRate / 2) * Math.PI / 180);
            double currentX = relativeCurrentX + cPoint.X;
            double currentY = cPoint.Y - relativecurrentY;

            //内圆上弧上的 浮点型坐标      
            PointF currentPoint = new PointF(float.Parse(currentX.ToString()), float.Parse(currentY.ToString()));

            //外圆弧上的点 
            double largerR = r + 25;
            double relativeLargerX = largerR * Math.Cos((360 - startAngle - currentRate / 2) * Math.PI / 180);
            double relativeLargerY = largerR * Math.Sin((360 - startAngle - currentRate / 2) * Math.PI / 180);
            double largerX = relativeLargerX + cPoint.X;
            double largerY = cPoint.Y - relativeLargerY;

            //外圆上弧上的 浮点型坐标   
            // PointF largerPoint = new PointF(float.Parse(largerX.ToString()), float.Parse(largerY.ToString()));

            //将两个点连起来       
            //graphics.DrawLine(Pens.Black, currentPoint, largerPoint);

            //外圆上 申明文字的地位  
            PointF circleTextPoint = new PointF(float.Parse(largerX.ToString()), float.Parse(largerY.ToString()));

            //在外圆上的点的四周合适的地位 写上申明   
            if (largerX >= 0 && largerY >= 0)//第1象限  实际第二象限                
            {
                //circleTextPoint.Y -= 15;  
                circleTextPoint.X -= 35;
            }
            if (largerX <= 0 && largerY >= 0)//第2象限  实际第三象限                
            {
                //circleTextPoint.Y -= 15;  
                //circleTextPoint.X -= 65;   
            }
            if (largerX <= 0 && largerY <= 0)//第3象限  实际第四象限                
            {
                //circleTextPoint.X -= 45;     
                circleTextPoint.Y += 30;
            }
            if (largerX >= 0 && largerY <= 0)//第4象限  实际第一象限                
            {
                circleTextPoint.X -= 15;
                //circleTextPoint.Y += 5;     
            }
            //象限差别申明：在数学中 二维坐标轴中 右上方 全为正,在策画机处理惩罚图像时,右下方全为正。相当于顺时针移了一个象限序号   
            graphics.DrawString((ds.Tables[0].Rows[iLoop][ds.Tables[0].Columns[0].ColumnName]).ToString() + " " + (currentRate / 360).ToString("p2"), new Font(familyName, 11), Brushes.Black, circleTextPoint);
            startAngle += currentRate;

        }
        #endregion

        #region 绘制右详情
        //第一个色块的原点地位           
        PointF basePoint = new PointF(Convert.ToInt32(circlePoint.X + r * 2 + 100), circlePoint.Y);
        //色块的大小           
        SizeF theSize = new SizeF(45, 16);
        //标头坐标
        PointF textBiaotou = new PointF(basePoint.X + 45, 20);
        //第一个色块的申明文字的地位           
        PointF textPoint = new PointF(basePoint.X + 50, basePoint.Y);
        for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
        {
            graphics.DrawString(ds.Tables[0].Columns[i].ColumnName, new Font(familyName, 11), Brushes.Black, textBiaotou);
            textBiaotou.X += (ds.Tables[0].Columns[i].ColumnName.Length + 2) * 11;
        }

        //遍历键值对数据集中的键 绘画颜色详情与数值
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            RectangleF baseRectangle = new RectangleF(basePoint, theSize);
            //画代表色块     
            graphics.FillRectangle((SolidBrush)colors[i], baseRectangle);
            //绘制文本字符串
            graphics.DrawString((ds.Tables[0].Rows[i][ds.Tables[0].Columns[0].ColumnName]).ToString(), new Font(familyName, 11), Brushes.Black, textPoint);
            //+ " " + (ds.Tables[0].Rows[i][ds.Tables[0].Columns[1].ColumnName]).ToString()
            graphics.DrawString((ds.Tables[0].Rows[i][ds.Tables[0].Columns[1].ColumnName]).ToString(), new Font(familyName, 11), Brushes.Black, textPoint.X + (ds.Tables[0].Columns[0].ColumnName.Length + 2) * 11, textPoint.Y);
            basePoint.Y += 30;
            textPoint.Y += 30;
        }
        #endregion

        graphics.Dispose();
        return bitmap;
    }

}