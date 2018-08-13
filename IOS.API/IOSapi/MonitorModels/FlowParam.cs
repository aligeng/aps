using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 获取流程图数据的查询参数
    /// </summary>
    public class FlowParam
    {
        /// <summary>
        /// 工厂id/生产单id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 当天日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 需要显示的节点
        /// </summary>
        public List<FlowPoint> FlowPoints { get; set; }
    }


    public class FlowPoint 
    {
        /// <summary>
        /// 节点类型（1关键事件，2工序节点，3终端节点，4物料节点）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 节点id
        /// </summary>
        public int ProcessId { get; set; }
       
    }
}