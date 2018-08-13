using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 客户id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 上级客户id
        /// </summary>
        public int parentid { get; set; }
        /// <summary>
        /// 更新用户名称
        /// </summary>
        public string updateuser { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updatedate { get; set; }
    }
}