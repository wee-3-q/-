using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insertion_back_loss.SQL
{
    class Dao
    {
        SqlConnection sc;
        public SqlConnection Connection()
        {
            string str = @"Data source=LAPTOP-BJ78KVD7;Initial Catalog=IRDB;Persist Security Info=True;User ID=sa;Password=1000;";//数据库连接字符串
            sc = new SqlConnection(str);//创建连接对象
            try
            {
                sc.Open();//打开连接
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据库连接失败：" + ex.Message);
            }
            //finally
            //{
            //    sc.Close();// 连接关闭的逻辑可以在这里处理
            //}
            return sc;//返回连接对象
        }
        public SqlCommand command(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection());
            return cmd;
        }
        public int Execute(string sql)//更新操作，增、删、改都可以     
        {
            try
            {
                SqlCommand cmd = command(sql);
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return 0;
            }
            finally
            {
                DaoClose();
            }
        }
        public SqlDataReader read(string sql)//查询操作
        {
            SqlCommand cmd = command(sql);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;
        }
        public void DaoClose()//关闭连接
        {
            if (sc != null)
            {
                sc.Close();
            }
        }
    }
}
