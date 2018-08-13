using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.Models
{
    /// <summary>
    /// 长线排产统计接口传入参数
    /// </summary>
    public class ParaLineSchedule
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int fid { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customer { get; set; }
        /// <summary>
        /// 查询范围起始日期
        /// </summary>
        public DateTime StarDate { get; set; }
        /// <summary>
        /// 查询范围截止日期
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}