using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class JaEventDelay
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int FactoryID { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string CustomerStyleNO { get; set; }
        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 产品类型名称
        /// </summary>
        public string ProductTypeName { get; set; }
        /// <summary>
        /// 生产单交货期
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// 关键事件ID
        /// </summary>
        public int EventId{ get; set; }
        /// <summary>
        /// 关键事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 关键事件负责人
        /// </summary>
        public string EventMan { get; set; }
        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime? PlanStarDate { get; set; }
        /// <summary>
        /// 计划完成期（推荐日期）
        /// </summary>
        public DateTime? PlanFinishDate { get; set; }
        /// <summary>
        /// 延迟天数
        /// </summary>
        public int DelayDays { get; set; }/// <summary>
        /// 跟单员
        /// </summary>
        public string Merchandiser { get; set; }
        /// <summary>
        /// 关键事件负责人id
        /// </summary>
        public string CriticalEventManID { get; set; }
    }
}