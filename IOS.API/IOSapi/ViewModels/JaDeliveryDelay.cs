using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class JaDeliveryDelay
    {
        /// <summary>
        /// 计划开始日期
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 产线名称
        /// </summary>
        public string FacilityName { get; set; }
        /// <summary>
        /// 工厂id
        /// </summary>
        public int FactoryID { get; set; }
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
        /// 交货期
        /// </summary>
        public string DeliveryDate { get; set; }

        private DateTime? _CAOTD;
        /// <summary>
        /// 客户交期
        /// </summary>
        public DateTime? CAOTD
        {
            get
            {
                if (_CAOTD > new DateTime(2000, 01, 01))
                    return _CAOTD;
                else
                    return null;
            }
            set { _CAOTD = value; }
        }
        /// <summary>
        /// 车间交期
        /// </summary>
        public string FactoryDelivery { get; set; }
        /// <summary>
        /// 原始交期
        /// </summary>
        public DateTime? InitDeliveryDate { get; set; }


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

        private DateTime? _MainMaterielArriveDate;
        /// <summary>
        /// 主料到厂日期
        /// </summary>
        public DateTime? MainMaterielArriveDate
        {
            get
            {
                if (_MainMaterielArriveDate > new DateTime(2000, 01, 01))
                    return _MainMaterielArriveDate;
                else
                    return null;
            }
            set { _MainMaterielArriveDate = value; }
        }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string Merchandiser { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 延迟天数
        /// </summary>
        public int DelayDays { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public double PlanAmount { get; set; }
        /// <summary>
        /// 生产单订单数量
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 优先级,6-普通，8-重要,10-紧急
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 延误等级 1一般延误，2严重延误
        /// </summary>
        public int DelayGrade { get; set; }
        /// <summary>
        /// 标准工时
        /// </summary>
        public double Capacity { get; set; }

    }
}