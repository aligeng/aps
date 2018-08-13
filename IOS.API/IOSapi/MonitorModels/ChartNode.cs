using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 图表
    /// </summary>
    public class ChartData
    {
        /// <summary>
        /// 图表标志（工厂名称/客户名称）
        /// </summary>
       public string Name { get; set; }
        /// <summary>
        /// 图表节点数据
        /// </summary>
       public List<ChartNode> ChartNodes { get; set; }
        /// <summary>
        /// 平均值
        /// </summary>
       public double Average { get; set; }
    }


    public class ChartNode
    {
        /// <summary>
        /// x轴名称
        /// </summary>
        public string xname { get; set; }
        /// <summary>
        /// y轴值1
        /// </summary>
        public string yvalue1 { get; set; }
        /// <summary>
        /// y轴值2
        /// </summary>
        public string yvalue2 { get; set; }
    }
}