using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 异常事件提醒的统计数据
    /// </summary>
    public class UserDelayCount
    {
        /// <summary>
        /// 生产单交期延误个数
        /// </summary>
        public int DeliveryDelay { get; set; }

        /// <summary>
        /// 生产计划开工期延后个数
        /// </summary>
        public int StartDelay { get; set; }
        /// <summary>
        /// 物料未按生产计划到货个数
        /// </summary>
        public int MatDelay { get; set; }
        /// <summary>
        /// 关键事件未按生产计划完成个数
        /// </summary>
        public int EventDelay { get; set; }

        /// <summary>
        /// 工艺延误(从监控中心表获取数据)
        /// </summary>
        public int ProcessDelay { get; set; }


        ///// <summary>
        ///// n天内未维护生产日进度的生产计划个数
        ///// </summary>
        //public int ScheduleNotMaintain { get; set; }


    }
}