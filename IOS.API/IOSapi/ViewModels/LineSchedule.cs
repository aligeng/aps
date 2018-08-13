using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 产线排产统计
    /// </summary>
    public class LineSchedule
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int fid { get; set; }
        /// <summary>
        /// 工厂名称
        /// </summary>
        public string factoryName { get; set; }
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string lineName { get; set; }
        /// <summary>
        /// 排产数量
        /// </summary>
        public double planAmount { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public double amount { get; set; }
    }
}