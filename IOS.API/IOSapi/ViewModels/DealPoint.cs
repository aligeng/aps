using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{

    public class PoDeal
    {
        public int PRIORITY { get; set; }
        public int Status { get; set; }
    }


    /// <summary>
    /// 处理情况统计数节点
    /// </summary>
    public class DelayDeal
    {
        /// <summary>
        /// 已处理数量
        /// </summary>
        public int DealedAmount { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 优先级,,6-普通，8-重要,10-紧急
        /// </summary>
        public int Priority { get; set; }

    }

    /// <summary>
    /// 处理情况统计数节点
    /// </summary>
    public class DealPoint
    {
        /// <summary>
        /// 分类节点id
        /// </summary>
        public int KeyId { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string LableText { get; set; }
        /// <summary>
        /// 已处理数量
        /// </summary>
        public int DealedAmount { get; set; }
        /// <summary>
        /// 未处理数量
        /// </summary>
        public int NotDealedAmount { get; set; }


    }
}