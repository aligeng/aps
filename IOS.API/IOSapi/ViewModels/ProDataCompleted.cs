using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class ProDataCompleted
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 各尺码完成数
        /// </summary>
        public List<ProSizeCompleted> SizeCompletes { get; set; }

    }
    /// <summary>
    /// 排产工序
    /// </summary>
    public class ProSizeCompleted
    {
        /// <summary>
        /// 尺码
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 排产数量（非排产工序订单数量）
        /// </summary>
        public double? PlanAmount { get; set; }
        /// <summary>
        /// 已完成数量
        /// </summary>
        public double? CompleteAmount { get; set; }
        /// <summary>
        /// 未完成数
        /// </summary>
        public double? NonAMount { get { return PlanAmount - CompleteAmount; } }
    }
}