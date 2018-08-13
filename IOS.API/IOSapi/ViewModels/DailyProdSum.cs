using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 日计划产量统计
    /// </summary>
    public class DailyProdSum
    {
        /// <summary>
        /// 工厂/产线id
        /// </summary>
        public int itemId { get; set; }

        /// <summary>
        /// 工厂/产线名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 产量
        /// </summary>
        public double? amount { get; set; }

    }
}