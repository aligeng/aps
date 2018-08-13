using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class TipsCount
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount{ get; set; }
    }
}