using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 滚动排期表详情
    /// </summary>
    public class RollPlanReportDetail
    {
        /// <summary>
        /// 产线id
        /// </summary>
        public int lineId { get; set; }
        /// <summary>
        /// 产线名称（组别）
        /// </summary>
        public int lineName { get; set; }
        /// <summary>
        /// 工作天数
        /// </summary>
        public int WorkDays { get; set; }
    }
}