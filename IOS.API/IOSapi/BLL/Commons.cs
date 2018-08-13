using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace IOSapi.BLL
{
    public class Commons
    {
        APSEntities db = new APSEntities();


        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable SqlQueryForDataTatable(Database db,
                 string sql)
        {

            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            //conn.ConnectionString = db.Connection.ConnectionString;
            //if (conn.State != ConnectionState.Open)
            //{
            //    conn.Open();
            //}

            conn = (SqlConnection)db.Connection;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            conn.Close();//连接需要关闭
            conn.Dispose();
            return table;
        }


        /// <summary>
        /// EF SQL 语句返回 DataSet
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet SqlQueryForDataSet(Database db,
                 string sql)
        {

            SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            //conn.ConnectionString = db.Connection.ConnectionString;
            //if (conn.State != ConnectionState.Open)
            //{
            //    conn.Open();
            //}

            conn = (SqlConnection)db.Connection;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            conn.Close();//连接需要关闭
            conn.Dispose();
            return ds;
        }

        /// <summary>
        /// 获取用户可见工厂id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetAllowedFactory(int userId = 0)
        {
            List<FACTORY> ls = new List<Models.FACTORY>();
            string ids = string.Empty;

            if (userId == 0)
            {
                ids = string.Join(",", db.FACTORies.ToList().Select(c => c.ID.ToString()).ToArray());
            }
            else
            {
                ids = string.Join(",", db.VIEWDATAPERMISSIONs.Where(c => c.USERID == userId).Select(c => c.FACTORYID.ToString()).ToArray());
            }

            return ids;
        }


        /// <summary>
        /// 根据产品大类获取产品类别id
        /// </summary>
        /// <param name="str">大类id集</param>
        /// <returns></returns>
        public static List<int> GetProductTypeList(string str)
        {
            List<int> fidLs = new List<int>();

            if (!string.IsNullOrEmpty(str))
            {
                string[] fidsArry = str.Split(',');
                foreach (var item in fidsArry)
                {
                    fidLs.Add(Convert.ToInt32(item));
                }
            }

            List<int> ls = new List<int>();
            if (fidLs.Count > 0)
            {
                ls = new APSEntities().PRODUCTTYPEs.Where(c => fidLs.Contains(c.PARENTID.Value)).Select(s => s.ID).Distinct().ToList();
                fidLs.AddRange(ls);
            }

            return fidLs;
        }


        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <param name="key">秘钥</param>
        /// <returns></returns>
        public static string Encode(string str, string key)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(str);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                StringBuilder builder = new StringBuilder();
                foreach (byte num in stream.ToArray())
                {
                    builder.AppendFormat("{0:X2}", num);
                }
                stream.Close();
                return builder.ToString();
            }
            catch (Exception) { return "xxxx"; }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">秘钥</param>
        /// <returns></returns>
        public static string Decode(string str, string key)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, 8));
                byte[] buffer = new byte[str.Length / 2];
                for (int i = 0; i < (str.Length / 2); i++)
                {
                    int num2 = Convert.ToInt32(str.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num2;
                }
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                stream.Close();
                return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
            }
            catch (Exception) { return ""; }
        }
    }
}