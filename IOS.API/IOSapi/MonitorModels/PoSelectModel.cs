using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 生产单搜索列表
    /// </summary>
    public class PoSelectModel
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 生产单编号
        /// </summary>
        public string PoCode { get; set; }
    }
}