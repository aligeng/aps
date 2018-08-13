using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 工厂/车间/生产线
    /// </summary>
    public class FactoryLines
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int FId { get; set; }
        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FName { get; set; }
        /// <summary>
        /// 车间
        /// </summary>
        public List<Workshop> Shops { get; set; }

    }
    /// <summary>
    /// 车间
    /// </summary>
    public class Workshop
    {
        /// <summary>
        /// 车间id
        /// </summary>
        public int WId { get; set; }
        /// <summary>
        /// 车间名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 生产线
        /// </summary>
        public List<Line> Lines { get; set; }
    }

    /// <summary>
    /// 生产线
    /// </summary>
    public class Line
    {
        /// <summary>
        /// 生产线id
        /// </summary>
        public int LId { get; set; }
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string LineName { get; set; }
    }
}