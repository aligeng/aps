using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// (任务提醒) 物料 [XX个物料未按生产计划到货] 与 [XX个物料需在YY内天到货]
    /// </summary>
    public class JaMaterial
    {
        /// <summary>
        /// 获取或设置 生产单号
        /// </summary>
        public string PoCode { get; set; }

        /// <summary>
        /// 获取或设置 本厂款号
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 获取或设置 客户款号
        /// </summary>
        public string CustomerStyleNo { get; set; }

        /// <summary>
        /// 获取或设置 材料编码
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 获取或设置 材料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 获取或设置 材料颜色
        /// </summary>
        public string MaterialColor { get; set; }
        /// <summary>
        /// 获取或设置 材料规格
        /// </summary>
        public string MaterialSize { get; set; }

        /// <summary>
        /// 获取或设置 采购单号
        /// </summary>
        public string MPNO { get; set; }
        /// <summary>
        /// 获取或设置 采购日期
        /// </summary>
        public string MPDate { get; set; }
        /// <summary>
        /// 获取或设置 采购数量
        /// </summary>
        public double MPAmount { get; set; }
        /// <summary>
        /// 获取或设置 采购计量单位
        /// </summary>
        public string MPUnit { get; set; }
        /// <summary>
        /// 获取或设置 预计到货期 (ETD Date)
        /// </summary>
        public DateTime ExpectedReciveDate { get; set; }

        /// <summary>
        /// 获取或设置 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 获取或设置物料类型，1.面布 2.别布 3.里布 4.衬布 5.上线辅料 6.包装辅料,通用物料
        /// （旧义 0:主料 1:辅料 2:包装辅料）
        /// </summary>
        public int MaterialStyle { get; set; }

        /// <summary>
        /// 获取或设置 需求日期
        /// </summary>
        public string NeedDate { get; set; }

        /// <summary>
        /// 获取或设置 延误天数
        /// </summary>
        public int DelayDays { get; set; }
    }
}