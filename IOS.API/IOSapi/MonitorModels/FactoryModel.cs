using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 工厂
    /// </summary>
    public class FactoryModel
    {
        /// <summary>
        /// 工厂id
        /// </summary>
        public int FId { get; set; }
        /// <summary>
        /// 工厂名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工厂所在城市
        /// </summary>
        public string City { get; set; }
    }
}