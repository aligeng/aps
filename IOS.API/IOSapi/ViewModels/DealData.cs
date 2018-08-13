using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class DealData
    {
        /// <summary>
        /// 处理类型：0订单延误；1事件延误；2工序延误
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 节点id
        /// </summary>
        public int KeyId { get; set; }
        /// <summary>
        /// 处理内容
        /// </summary>
        public string Remark { get; set; }
    }
}