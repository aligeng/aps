using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 生产单状态数量
    /// </summary>
    public class PoesStatusModel
    {
        /// <summary>
        /// 所有订单数
        /// </summary>
        public int sall { get; set; }
        /// <summary>
        /// 正常订单数
        /// </summary>
        public int s1 { get; set; }
        /// <summary>
        /// 严重延误订单数
        /// </summary>
        public int s2 { get; set; }
        /// <summary>
        /// 轻微延误订单数
        /// </summary>
        public int s3 { get; set; }
        /// <summary>
        /// 严重提前订单数
        /// </summary>
        public int s4 { get; set; }
    }
}