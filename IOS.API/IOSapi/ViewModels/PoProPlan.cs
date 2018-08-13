using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单的计划
    /// </summary>
    public class PoProPlan
    {
        /// <summary>
        /// 计划id
        /// </summary>
        public int PlanId { get; set; }
        /// <summary>
        /// 产线名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 排单数量
        /// </summary>
        public double PlanAmount { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public double FinishAmount { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        public double FinishEff { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StarTime { get; set; }
        /// <summary>
        /// 完成时候
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 生产状态  0生产中；1完成
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 工时
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}