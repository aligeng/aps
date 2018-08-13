using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// (任务提醒)生产计划开工期延误 model类
    /// </summary>
    public class JaStartDelay : JaDetailBase
    {
        /// <summary>
        /// 获取或设置 计划ID
        /// </summary>
        public int ProductionEventID { get; set; }

        /// <summary>
        /// 获取或设置 生产线
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 获取或设置 生产单数量
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 获取或设置 计划数量
        /// </summary>
        public int PlanAmount { get; set; }

        /// <summary>
        /// 获取或设置 交货期
        /// </summary>
        public string DeliveryDate { get; set; }

        /// <summary>
        /// 获取或设置 计划开始日期
        /// </summary>
        public DateTime PlanStart { get; set; }
        /// <summary>
        /// 获取或设置 最早可开工期
        /// </summary>
        public DateTime EarliestStartDate { get; set; }
        /// <summary>
        /// 获取或设置 延误天数
        /// </summary>
        public int DelayDays { get; set; }
        /// <summary>
        /// 获取或设置 主料完成日期
        /// </summary>
        public string MasterMatFinishDate { get; set; }

        /// <summary>
        /// 获取或设置 辅料完成日期
        /// </summary>
        public string NotMasterMatFinishDate { get; set; }

        /// <summary>
        /// 获取或设置 产前事件完成日期
        /// </summary>
        public string PreProductionEventDate { get; set; }

    }
}