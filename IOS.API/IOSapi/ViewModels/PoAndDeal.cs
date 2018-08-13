using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单处理详情
    /// </summary>
    public class PoAndDeal:Po
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public int keyId { get; set; }
        /// <summary>
        /// 处理状态：0未处理；已处理
        /// </summary>
        public int dealStatus { get; set; }
        /// <summary>
        /// 处理内容
        /// </summary>
        public string dealRemark { get; set; }
    }
}