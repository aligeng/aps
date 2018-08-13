using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 生产单流程图节点
    /// </summary>
    public class PoProcessModel
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public int Processid { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Processname { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public int Type { get; set; }
        ///// <summary>
        ///// 排序
        ///// </summary>
        //public int Sort { get; set; }
        /// <summary>
        /// 匹配到工艺节点id（物料类型节点才需要）
        /// </summary>
        public int Refid { get; set; }
        /// <summary>
        /// 节点是否存在（0不存在，1存在）
        /// </summary>
        public int Active { get; set; }
        /// <summary>
        /// 节点进度状态（-1未开始，0进行中，1完成）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 异常类型（１正常、２严重延误、３轻微延误、４严重提前）
        /// </summary>
        public int Troubletype { get; set; }
        /// <summary>
        /// 当日完成数量
        /// </summary>
        public int Dayqty { get; set; }
        /// <summary>
        /// 累计完成数量
        /// </summary>
        public int Totalqty { get; set; }
        /// <summary>
        /// 目前耗时
        /// </summary>
        public double Duration { get; set; }
        /// <summary>
        /// 完成百分比
        /// </summary>
        public double Schedulepercent { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public int Planqty { get; set; }
        /// <summary>
        /// 计划开始日期
        /// </summary>
        public string Startdate { get; set; }//开始日期
        /// <summary>
        /// 计划结束日期
        /// </summary>
        public string Enddate { get; set; } //结束日期
    }
}