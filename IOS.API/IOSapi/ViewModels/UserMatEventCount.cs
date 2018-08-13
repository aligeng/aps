using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 物料与关键事件提醒的统计数据
    /// </summary>
    public class UserMatEventCount
    {
        /// <summary>
        /// 需要在7天内到货的物料个数
        /// </summary>
        public int MatReceive { get; set; }
        /// <summary>
        /// 需在7天内完成的关键事件个数
        /// </summary>
        public int EventFinish { get; set; }
        /// <summary>
        /// 生产单7天内进度跟踪个数
        /// </summary>
        public int ProgressTrack { get; set; }
    }
}