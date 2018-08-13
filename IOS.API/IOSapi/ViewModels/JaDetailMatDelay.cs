using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class JaDetailMatDelay
    {
        /// <summary>
        /// 数据id
        /// </summary>
        public string POMaterialRequestID { get; set; }
        /// <summary>
        /// 采购单号
        /// </summary>
        public string MPNO { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 采购日期
        /// </summary>
        public string MPDate { get; set; }
        /// <summary>
        /// 预计到货期
        /// </summary>
        public string ExpectedReciveDate { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 物料颜色
        /// </summary>
        public string MaterialColor { get; set; }
        /// <summary>
        /// 尺码
        /// </summary>
        public string MaterialSize { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// 需求日期
        /// </summary>
        public string NeedDate { get; set; }
        /// <summary>
        /// 延误天数
        /// </summary>
        public string DelayDays { get; set; }
        /// <summary>
        /// 物料类型
        /// </summary>
        public string MaterialStyle { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string CustomerStyleNo { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
    }
}