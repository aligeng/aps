using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单模糊搜索
    /// </summary>
    public class PoSearch
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 款号（本厂款号）
        /// </summary>
        public string PatternNo { get; set; }
    }
}