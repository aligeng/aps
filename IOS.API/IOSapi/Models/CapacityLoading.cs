using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.Models
{
    public class CapacityLoading
    {
        /// <summary>
        /// 负载
        /// </summary>
        public double Loading { get; set; }
        /// <summary>
        /// 产能
        /// </summary>
        public double Capacity { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime From { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime To { get; set; }
        /// <summary>
        /// 开始日期（年月）
        /// </summary>
        public string FromStr { get; set; }
    }
}