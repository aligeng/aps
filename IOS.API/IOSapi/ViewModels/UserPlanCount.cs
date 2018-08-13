using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 计划与进度提醒的统计数据
    /// </summary>
    public class UserPlanCount
    {
        /// <summary>
        /// 在7天内开始的生产计划个数
        /// </summary>
        public int DayStart { get; set; }
        /// <summary>
        /// 在7天内结束的生产单个数
        /// </summary>
        public int DayEnd { get; set; }
        /// <summary>
        /// 正在生产中的生产计划个数(未做)
        /// </summary>
        public int Producting { get; set; }
        /// <summary>
        /// n天内生产线效率分析(未做)
        /// </summary>
        public int EffAnalysis { get; set; }
        /// <summary>
        /// 生产单7天内进度跟踪个数
        /// </summary>
        public int ProgressTrack { get; set; }
    }
}