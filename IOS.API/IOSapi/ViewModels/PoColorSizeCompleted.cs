using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class PoColorSizeCompleted
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 尺码
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 订单数量
        /// </summary>
        public double? BillAmount { get; set; }
        /// <summary>
        /// 排产数量
        /// </summary>
        public double? PlanAmount { get; set; }
        /// <summary>
        /// 已完成数量
        /// </summary>
        public double? CompleteAmount { get; set; }
    }
}