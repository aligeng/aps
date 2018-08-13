using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    public class NChartNode
    {
        /// <summary>
        /// x轴名称
        /// </summary>
        public string xname { get; set; }

        public List<Series> SeriesList { get; set; }
    }

    public class Series
    {
        /// <summary>
        /// 系列名称
        /// </summary>
        public string seriesName { get; set; }

        /// <summary>
        /// y轴值1(正常)
        /// </summary>
        public string yvalue1 { get; set; }
        /// <summary>
        /// y轴值2（异常）
        /// </summary>
        public string yvalue2 { get; set; }
    }

}