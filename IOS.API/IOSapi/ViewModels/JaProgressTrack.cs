using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class JaProgressTrack
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int POID { get; set; }
        /// <summary>
        /// 生产单编号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string CustomerStyleNO { get; set; }
        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 产品分类名称
        /// </summary>
        public string ProductTypeName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 最迟生产日期
        /// </summary>
        public DateTime LastProDate { get; set; }
        /// <summary>
        /// 工序完成情况
        /// </summary>
        public List<JaProcess> JaProcessList { get; set; }

        ///// <summary>
        ///// 车缝数
        ///// </summary>
        //public string SewingAmount { get; set; }
        ///// <summary>
        ///// 累计车缝数
        ///// </summary>
        //public string SewingTotal { get; set; }


    }


    public class JaProcess
    {
        /// <summary>
        /// 工序id
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 完成数量
        /// </summary>
        public double? Amount { get; set; }
        /// <summary>
        /// 累计完成数量
        /// </summary>
        public double TotalAmount { get; set; }
    }
}