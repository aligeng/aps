using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单物料到货信息
    /// </summary>
    public class PoMateriel
    {
        /// <summary>
        /// 采购单号
        /// </summary>
        public string PurchCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string MatCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MatName { get; set; }
        /// <summary>
        /// 到货数量
        /// </summary>
        public double ArrivedAmount { get; set; }
        /// <summary>
        /// 订料时间
        /// </summary>
        public DateTime BookTime { get; set; }
        /// <summary>
        /// 期望到货期
        /// </summary>
        public DateTime ExpectTime { get; set; }
        /// <summary>
        /// 实际到货期
        /// </summary>
        public DateTime ArrivedTime { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Standard { get; set; }
        /// <summary>
        /// 用量
        /// </summary>
        public double Dosage { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}