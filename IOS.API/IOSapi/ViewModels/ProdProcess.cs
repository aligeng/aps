using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单工序节点完成情况统计
    /// </summary>
    public class ProdProcess
    {
        /// <summary>
        /// 工序节点id
        /// </summary>
        public int processid { get; set; }
        /// <summary>
        /// 工序节点名称
        /// </summary>
        public string processname { get; set; }
        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime planstartdate { get; set; }
        /// <summary>
        /// 计划结束日期
        /// </summary>

        public DateTime planenddate { get; set; }
        /// <summary>
        /// 实际开始日期
        /// </summary>

        public DateTime? startdate { get; set; }
        /// <summary>
        /// 实际结束日期
        /// </summary>

        public DateTime enddate { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>

        public Nullable<double> amount { get; set; }
    }
}