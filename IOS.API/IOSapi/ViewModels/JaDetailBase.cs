using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 任务提醒明细 基类
    /// </summary>
    public class JaDetailBase
    {

        /// <summary>
        /// 获取或设置 生产单号
        /// </summary>
        public string PoCode { get; set; }

        /// <summary>
        /// 获取或设置 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 获取或设置 客户款号
        /// </summary>
        public string CustomerStyleNO { get; set; }
        /// <summary>
        /// 获取或设置 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 获取或设置 产品分类
        /// </summary>
        public string ProductTypeName { get; set; }

        /// <summary>
        /// 获取或设置 跟单员
        /// </summary>
        public string Merchandiser { get; set; }

        /// <summary>
        /// 获取或设置 备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置 工厂ID
        /// </summary>
        public int FactoryID { get; set; }
    }
}