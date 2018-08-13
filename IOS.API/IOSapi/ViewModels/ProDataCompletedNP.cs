using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 工序完成情况
    /// </summary>
    public class ProDataCompletedNP
    {
        /// <summary>
        /// 工序id
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// （非排产工序/生产日进度按生产单录入）已完成数量
        /// </summary>
        public double? CompleteAmount { get; set; }

        /// <summary>
        /// 人工数
        /// </summary>
        public int WorkerAmount { get; set; }
        /// <summary>
        /// 人工时
        /// </summary>
        public double WorkHours { get; set; }

        /// <summary>
        /// 默认生产日期
        /// </summary>
        public DateTime ProTime { get; set; }

        public List<ProDataCompleted> ProDataCompletedList { get; set; }
    }
}