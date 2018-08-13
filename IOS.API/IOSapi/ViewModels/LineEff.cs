using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产线效率
    /// </summary>
    public class LineEff
    {

        public LineEff()
        {
            ProductionDate = DateTime.MinValue;
            Datas = new List<ViewModels.LineDayEff>();
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime ProductionDate { get; set; }
        /// <summary>
        /// 各生产线效率集
        /// </summary>
        public List<LineDayEff> Datas { get; set; }
    }

    /// <summary>
    /// 生产线日效率
    /// </summary>
    public class LineDayEff
    {
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 实际计算效率
        /// </summary>
        public double Eff { get; set; }
    }
}