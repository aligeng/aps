using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 工厂信息
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否外协 0否；1是
        /// </summary>
        public int isoutside { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }

    }
}