using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class JaEventNotMaintain
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int FactoryID { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 客户款号
        /// </summary>
        public string CustomerStyleNO { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string Merchandiser { get; set; }
        /// <summary>
        /// 产品分类
        /// </summary>
        public string ProductTypeName { get; set; }

        /// <summary>
        /// 关键事件ID
        /// </summary>
        public int PoEventID { get; set; }

        /// <summary>
        /// 关键事件名称
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 关键事件负责人ID
        /// </summary>
        public int CriticalEventManID { get; set; }
        /// <summary>
        /// 生产单交货期
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// 预计完成日期
        /// </summary>
        public DateTime? ExpectFinishDate { get; set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime? PlanStart { get; set; }

    }
}