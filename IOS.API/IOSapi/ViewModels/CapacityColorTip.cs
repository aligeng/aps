using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 宏观计划颜色图例
    /// </summary>
    public class CapacityColorTip
    {
        /// <summary>
        /// 类型 0产能 1负载
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 值名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
    }
}