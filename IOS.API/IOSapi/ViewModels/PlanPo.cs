using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 已排产的生产单数据
    /// </summary>
    public class PlanPo: Po
    {
        /// <summary>
        /// 计划id
        /// </summary>
        public int ProductionEventID { get; set; }
        /// <summary>
        /// 产线ID
        /// </summary>
        public int LineID { get; set; }
        /// <summary>
        /// 产线名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 车间
        /// </summary>
        public string WorkshopName { get; set; }
        /// <summary>
        /// 开工期
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 工艺路线
        /// </summary>
        public string ProcessRoleName { get; set; }
        /// <summary>
        /// 标准工时
        /// </summary>
        public double StandardHour { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public double WorkerNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}