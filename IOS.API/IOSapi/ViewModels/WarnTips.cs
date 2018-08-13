using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class WarnTips
    {
        /// <summary>
        /// 今日已处理
        /// </summary>
        public int TodayHandledCount { get; set; }
        /// <summary>
        /// 异常数据数量
        /// </summary>
        public List<TipsCount> TipsCountList { get; set; }
    }
}