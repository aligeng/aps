using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产每日产量
    /// </summary>
    public class PoDailyProd
    {
        /// <summary>
        /// 计划id
        /// </summary>
        public int productionEventID { get; set; }
        /// <summary>
        /// 产线ID
        /// </summary>
        public int lineID { get; set; }
        /// <summary>
        /// 产线名称
        /// </summary>
        public string lineName { get; set; }
        /// <summary>
        /// 生产单id
        /// </summary>
        public int poId { get; set; }
        /// <summary>
        /// 生产单编号
        /// </summary>
        public string pocode { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public string customer { get; set; }
        /// <summary>
        /// 款号
        /// </summary>
        public string patternNo { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 排单数
        /// </summary>
        public double planAmount { get; set; }
        /// <summary>
        /// 交货期
        /// </summary>
        public DateTime DeliveryDate { get; set; }
        /// <summary>
        /// 每日产量
        /// </summary>
        public List<DailyAmount> dailyAmount { get; set; }
    }
}