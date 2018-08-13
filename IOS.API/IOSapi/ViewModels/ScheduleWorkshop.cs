using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 非排产工序录入车间产线数据
    /// </summary>
    public class ScheduleWorkshop
    {
        /// <summary>
        /// 非排产工序录入类型（0不区分车间生产线，1按车间录入，2按产线录入），若为1、2时必须选择才能录入数据
        /// </summary>
        public string ScheduleWorkshopType { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        public List<Workshop> Shops { get; set; }
    }
}