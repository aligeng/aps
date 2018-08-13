using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单细分数据
    /// </summary>
    public class PoDetaile
    {
        /// <summary>
        /// 细分id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string pocolor { get; set; }
        /// <summary>
        /// 尺码
        /// </summary>
        public string posize { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 交货期
        /// </summary>
        public System.DateTime deliverydate { get; set; }
        /// <summary>
        /// 布到货期
        /// </summary>
        public System.DateTime receiveDate { get; set; }
        /// <summary>
        /// 交货地点
        /// </summary>
        public string excountry { get; set; }
        /// <summary>
        /// 走货方式
        /// </summary>
        public string extype { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
    }
}