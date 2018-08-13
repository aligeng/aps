using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单关键事件信息
    /// </summary>
    public class EventDate
    {
        /// <summary>
        /// 关键事件id
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// 关键事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 关键事件计划日期
        /// </summary>
        public DateTime PlanTime { get; set; }
        /// <summary>
        /// 完成日期
        /// </summary>
        public DateTime EndTime  { get; set; }
    }
}