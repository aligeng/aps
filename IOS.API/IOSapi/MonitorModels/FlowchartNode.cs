using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 流程图节点
    /// </summary>
    public class FlowchartNode
    {
        /// <summary>
        /// 节点id（跟aps基础表id一致）
        /// </summary>
        public int nodeId { get; set; }

        /// <summary>
        /// 节点类型（1关键事件，2工艺节点，3终端节点，4物料节点）
        /// </summary>
        public int nodeType { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string nodeName { get; set; }

        /// <summary>
        ///说明
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 节点关联id（0表示无关联，>1表示关联到的工艺节点id，即物料节点时才大于0）
        /// </summary>
        public int relatedId { get; set; }

        /// <summary>
        /// 是否必须展示节点（需要排产的工艺节点）0否，1是
        /// </summary>
        public int isPrimary { get; set; }
        /// <summary>
        /// 是否监控节点(0否，1是)
        /// </summary>
        public int isMonitor { get; set; }
    }
}