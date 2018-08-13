using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// (异常事件提醒)生产计划进度落后
    /// </summary>
    public class JaProgressDelay : JaDetailBase
    {
        /// <summary>
        /// 获取或设置 计划ID
        /// </summary>
        public int ProductionEventID { get; set; }

        /// <summary>
        /// 获取或设置 工厂名称
        /// </summary>
        public string FactoryName { get; set; }


        /// <summary>
        /// 获取或设置 生产单数量
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 生产线
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 获取或设置 生产单交货期
        /// </summary>
        public string DeliveryDate { get; set; }

        /// <summary>
        /// 获取或设置 排单数
        /// </summary>
        public double PlanAmount { get; set; }

        /// <summary>
        /// 获取或设置 计划开始日期
        /// </summary>
        public DateTime PlanStart { get; set; }

        /// <summary>
        /// 获取或设置 实际开始日期
        /// </summary>
        public DateTime ActualStart { get; set; }



        /// <summary>
        /// 获取或设置 计划应完成数
        /// </summary>
        public double PlanShouldFinish { get; set; }

        /// <summary>
        /// 获取或设置 实际完成数
        /// </summary>
        public double ActualFinish { get; set; }

        /// <summary>
        /// 获取或设置 实际进度%
        /// </summary>
        public string ActualProgress { get; set; }

        /// <summary>
        /// 获取或设置 进度落后%
        /// </summary>
        public string ProgressDelay { get; set; }
    }
}