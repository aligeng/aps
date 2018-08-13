using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 记录生产单关键事件完成日期
    /// </summary>
    public class PoEvents
    {
        /// <summary>
        /// 生产单日期
        /// </summary>
        public int PoId { get; set; }
        /// <summary>
        /// 关键事件完成日期
        /// </summary>
        public List<EventDateAdd> Events { get; set; }
    }

    /// <summary>
    /// 添加关键事件完成日期
    /// </summary>
    public class EventDateAdd
    {
        /// <summary>
        /// 关键事件流程节点id
        /// </summary>
        public int EventflownodeId { get; set; }
        /// <summary>
        /// 预计完成日期
        /// </summary>
        public string ExpectTime { get; set; }
        /// <summary>
        /// 完成日期
        /// </summary>
        public string EndTime { get; set; }
    }
}