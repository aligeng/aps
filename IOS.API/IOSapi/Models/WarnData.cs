using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.Models
{
    /// <summary>
    /// 异常事件提醒个数数据类型
    /// </summary>
    public class WarnData
    {
        /// <summary>
        /// 生产单交期延误个数
        /// </summary>
        public int DeliveryDelay { get; set; }
        /// <summary>
        /// 工序延误个数（与监控中心一致）
        /// </summary>
        public int ProgressDelay { get; set; }
        /// <summary>
        /// 生产计划开工期延后个数
        /// </summary>
        public int StartDelay { get; set; }
        /// <summary>
        /// 物料未按生产计划到货个数
        /// </summary>
        public int MatDelay { get; set; }
        /// <summary>
        /// 关键事件延误个数（与监控中心一致）
        /// </summary>
        public int EventDelay { get; set; }
        /// <summary>
        /// n天内未维护生产日进度的生产计划个数
        /// </summary>
        public int ScheduleNotMaintain { get; set; }
    }
}