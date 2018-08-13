using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace IOSapi.BLL
{
    public class LogTest
    {
        public static void Write(string str)
        {
            FileStream fs = new FileStream("D:\\ak.txt", FileMode.Create);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(str);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
    }
}