using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    ///(计划与进度提醒) [XX个生产计划在YY天内开始] 与 [XX个生产计划在YY天内结束]
    /// </summary>
    public class JaDayStartDayEnd : JaDetailBase
    {


        /// <summary>
        /// 获取或设置 计划ID
        /// </summary>
        public int ProductionEventID { get; set; }

        /// <summary>
        /// 获取或设置 生产线名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 获取或设置 生产单数量
        /// </summary>
        public string POAmount { get; set; }

        /// <summary>
        /// 获取或设置 排单数
        /// </summary>
        public double PlanAmount { get; set; }


        /// <summary>
        /// 获取或设置 交货期
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// 获取或设置 计划开始日期
        /// </summary>
        public DateTime PlanStart { get; set; }


        /// <summary>
        /// 获取或设置 计划结束日期
        /// </summary>
        public DateTime PlanEnd { get; set; }


        /// <summary>
        /// 获取或设置 最早可开工期
        /// </summary>
        public DateTime EarlinessStartDate { get; set; }

    }
}