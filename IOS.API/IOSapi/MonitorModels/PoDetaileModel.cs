using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    /// <summary>
    /// 生产单详细信息
    /// </summary>
    public class PoDetaileModel
    {
        /// <summary>
        /// 生产单编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string ProducttypeName { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string customerstyleno { get; set; }
        /// <summary>
        /// 款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 标准工时
        /// </summary>
        public double Sam { get; set; }

        /// <summary>
        /// 工厂
        /// </summary>
        public string FactoryName { get; set; }
        /// <summary>
        /// 交货期
        /// </summary>
        public string Deliverydate { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        public int Troubletype { get; set; }
        /// <summary>
        /// 是否VIP客户（0不是；1是）
        /// </summary>
        public int VIP { get; set; }

        /// <summary>
        /// 生产单状态（0未完结；1已完结）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 生产单状态名称
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// 原始交期
        /// </summary>
        public DateTime? InitDeliveryDate { get; set; }
        /// <summary>
        /// 实单
        /// </summary>
        public string IsFirmOrderName { get; set; }
        /// <summary>
        /// 跟单员
        /// </summary>
        public string Merchandiser { get; set; }
        /// <summary>
        /// 关键事件流程
        /// </summary>
        public string EventFlow { get; set; }

        private DateTime? _EarlinessStartDate;
        /// <summary>
        /// 最早可开工期
        /// </summary>
        public DateTime? EarlinessStartDate
        {
            get
            {
                if (_EarlinessStartDate > new DateTime(2000, 01, 01))
                    return _EarlinessStartDate;
                else
                    return null;
            }
            set { _EarlinessStartDate = value; }
        }

        private DateTime? _MasterMaterialDate;
        /// <summary>
        /// 主料预计到货期
        /// </summary>
        public DateTime? MasterMaterialDate
        {
            get
            {
                if (_MasterMaterialDate > new DateTime(2000, 01, 01))
                    return _MasterMaterialDate;
                else
                    return null;
            }
            set { _MasterMaterialDate = value; }
        }

        private DateTime? _MasterMaterialReceivedDate;
        /// <summary>
        /// 主料到货期
        /// </summary>
        public DateTime? MasterMaterialReceivedDate
        {
            get
            {
                if (_MasterMaterialReceivedDate > new DateTime(2000, 01, 01))
                    return _MasterMaterialReceivedDate;
                else
                    return null;
            }
            set { _MasterMaterialReceivedDate = value; }
        }

        private DateTime? _MaterialDate;
        /// <summary>
        /// 辅料预计到货期
        /// </summary>
        public DateTime? MaterialDate
        {
            get
            {
                if (_MaterialDate > new DateTime(2000, 01, 01))
                    return _MaterialDate;
                else
                    return null;
            }
            set { _MaterialDate = value; }
        }

        private DateTime? _MaterialReceivedDate;
        /// <summary>
        /// 辅料到货期
        /// </summary>
        public DateTime? MaterialReceivedDate
        {
            get
            {
                if (_MaterialReceivedDate > new DateTime(2000, 01, 01))
                    return _MaterialReceivedDate;
                else
                    return null;
            }
            set { _MaterialReceivedDate = value; }
        }

        private DateTime? _PreProductionEventDate;
        /// <summary>
        /// 关键事件完成日期
        /// </summary>
        public DateTime? PreProductionEventDate
        {
            get
            {
                if (_PreProductionEventDate > new DateTime(2000, 01, 01))
                    return _PreProductionEventDate;
                else
                    return null;
            }
            set { _PreProductionEventDate = value; }
        }
        /// <summary>
        /// 排产起始日期
        /// </summary>
        public string Startdate { get; set; }
        /// <summary>
        /// 排产结束日期
        /// </summary>
        public string Enddate { get; set; }

        /// <summary>
        /// 耗时（天）
        /// </summary>
        public double Duration { get; set; }
        /// <summary>
        /// 车间
        /// </summary>
        public string Groupname { get; set; }
        /// <summary>
        /// 产线
        /// </summary>
        public string lineName { get; set; }

    }
}