using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产日进度数据报表
    /// </summary>
    public class ProdSchedule
    {
        ///// <summary>
        ///// 生产日进度ID
        ///// </summary>
        //public int id { get; set; }
        ///// <summary>
        ///// 工序ID
        ///// </summary>
        //public int processid { get; set; }
        ///// <summary>
        ///// 工序名称
        ///// </summary>
        //public string processname { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime productiondate { get; set; }
        ///// <summary>
        ///// 颜色
        ///// </summary>
        //public string pocolor { get; set; }
        ///// <summary>
        ///// 尺码
        ///// </summary>
        //public string posize { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double amount { get; set; }
    }
}