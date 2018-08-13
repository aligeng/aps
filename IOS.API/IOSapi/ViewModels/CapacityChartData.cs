using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class CapacityChartRow 
    {
        /// <summary>
        /// （工厂）名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图表数据
        /// </summary>
        public List<CapacityChartData> CapacityChartDataList { get; set; }

        /// <summary>
        /// 最大轴值
        /// </summary>
        public double MaxValue
        {
            get
            {
                if (CapacityChartDataList!=null)
                {
                    return CapacityChartDataList.Max(c => c.MaxValue);
                }
                else
                {
                    return 0;
                }
             
            }
        }
    }

    public class CapacityChartData
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string dataTime { get; set; }
        /// <summary>
        /// 产能数据
        /// </summary>
        public List<ChartDataItem> capData { get; set; }
        /// <summary>
        /// 负载数据
        /// </summary>
        public List<ChartDataItem> loadingData { get; set; }

        /// <summary>
        /// 最大轴值
        /// </summary>
        public double MaxValue
        {
            get
            {
                double cap = capData.Sum(c => c.value);
                double loading = loadingData.Sum(c => c.value);

                if (cap >= loading)
                {
                    return cap;
                }
                else
                    return loading;               
            }
        }

    }


    public class ChartDataItem
    {
        public string name { get; set; }
        public double value { get; set; }
        public string color { get; set; }
    }




    /// <summary>
    /// 宏观计划的产能
    /// </summary>
    public class CapData
    {
        /// <summary>
        /// 外发产能
        /// </summary>
        public double OutCapacity { get; set; }

        /// <summary>
        /// 普能产能
        /// </summary>
        public double NormalCapacity { get; set; }

        /// <summary>
        /// 加班产能
        /// </summary>
        public double OTCapacity { get; set; }

        /// <summary>
        /// 总产能
        /// </summary>
        public double TotalCapacity { get; set; }
    }

    /// <summary>
    /// 宏观计划的负载
    /// </summary>
    public class LoadingData
    {
        /// <summary>
        /// 实单已排
        /// </summary>
        public double RealPlanLoading { get; set; }
        /// <summary>
        /// 非实单已排
        /// </summary>
        public double NonRealPlanLoading { get; set; }
        /// <summary>
        /// 实单未排
        /// </summary>
        public int RealNonPlanLoading { get; set; }
        /// <summary>
        /// 非实单未排
        /// </summary>
        public int NonRealNonPlanLoading { get; set; }
    }


}