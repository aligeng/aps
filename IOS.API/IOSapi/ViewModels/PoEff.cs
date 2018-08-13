using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单效率
    /// </summary>
    public class PoEff
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime ProductionDate { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public Double FinishedAmount { get; set; }
        /// <summary>
        /// 录入效率
        /// </summary>
        public Double Capacity { get; set; }
        /// <summary>
        /// 人工数
        /// </summary>
        public Int32 Workers { get; set; }
        /// <summary>
        /// 生产工时
        /// </summary>
        public Double Duration { get; set; }
        /// <summary>
        /// 实际计算效率
        /// </summary>
        public Double Eff { get; set; }
    }
}