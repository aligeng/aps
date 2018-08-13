using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class LineDefaultDuration
    {
        /// <summary>
        /// 产线id
        /// </summary>
        public int LineId { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime ProDate { get; set; }

        /// <summary>
        /// 默认人工时
        /// </summary>
        public double WorkHours { get; set; }
    }
}