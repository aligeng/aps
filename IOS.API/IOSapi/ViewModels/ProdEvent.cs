using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单关键事件完成统计
    /// </summary>
    public class ProdEvent
    {
        /// <summary>
        /// 关键事件流程节点id
        /// </summary>
        public int evtid { get; set; }
        /// <summary>
        /// 关键事件流程节点名称
        /// </summary>
        public string evtname { get; set; }
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
        public DateTime startdate { get; set; }
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
