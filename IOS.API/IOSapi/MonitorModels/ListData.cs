using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    public class ListData
    {
        public int total { get; set; }
        public List<WarnPoData> objList { get; set; }
    }

    public class PlanListData
    {
        public int total { get; set; }
        public List<IOSapi.ViewModels.PlanPo> objList { get; set; }
    }
}