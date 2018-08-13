using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 添加非工序生产日进度数据
    /// </summary>
    public class ProScheduleAdd
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 工序节点id
        /// </summary>
        public int ProceesId { get; set; }
        /// <summary>
        /// 车间id
        /// </summary>
        public int WorkShopId { get; set; }
        /// <summary>
        /// 产线id
        /// </summary>
        public int LineId { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProDate { get; set; }
        /// <summary>
        /// 计划是否已生产完毕
        /// </summary>
        public int IsProCompleted { get; set; }

        /// <summary>
        /// 产量数据
        /// </summary>
        public List<ProData> ProDatas { get; set; }
    }

    /// <summary>
    /// 添加排产工序生产日进度数据
    /// </summary>
    public class ProPlanScheduleAdd
    {
        /// <summary>
        /// 排产计划id
        /// </summary>
        public int ProductionEventId { get; set; }
        /// <summary>
        /// 生产线id
        /// </summary>
        public int LineId { get; set; }
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 工序节点id
        /// </summary>
        public int ProceesId { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProDate { get; set; }
        /// <summary>
        /// 计划是否已生产完毕
        /// </summary>
        public int IsProCompleted { get; set; }

        /// <summary>
        /// 人工数
        /// </summary>
        public int WorkerAmount { get; set; }
        /// <summary>
        /// 人工时
        /// </summary>
        public double WorkHours { get; set; }
        /// <summary>
        /// 合格率（%）
        /// </summary>
        public double FPY { get; set; }

        /// <summary>
        /// 产量数据
        /// </summary>
        public List<ProData> ProDatas { get; set; }
    }

    /// <summary>
    /// 产量数据
    /// </summary>
    public class ProData
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 尺码
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { get; set; }
    }
}