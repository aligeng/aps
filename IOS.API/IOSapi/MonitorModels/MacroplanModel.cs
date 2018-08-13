using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 宏观计划
    /// </summary>
    public class MacroplanModel
    {
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 产能
        /// </summary>
        public int Capacity { get; set; }
        /// <summary>
        /// 负载
        /// </summary>
        public int ArrangedAmount { get; set; }
        /// <summary>
        /// 非实单负载
        /// </summary>
        public int NotArrangedAmount { get; set; }
        /// <summary>
        /// 预测数
        /// </summary>
        public int ExpectedAmount { get; set; }
    }
}