using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 添加外协厂排产工序生产日进度数据
    /// </summary>
    public class OutProScheduleAdd
    {
        /// <summary>
        /// 生产线id
        /// </summary>
        public int LineId { get; set; }
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime ProDate { get; set; }

        /// <summary>
        /// 产量数据
        /// </summary>
        public List<ProData> ProDatas { get; set; }
    }
}