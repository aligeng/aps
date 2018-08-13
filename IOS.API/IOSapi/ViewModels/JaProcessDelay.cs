using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 工艺延误信息
    /// </summary>
    public class JaProcessDelay
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int PoId { get; set; }

        /// <summary>
        /// 生产单编号
        /// </summary>
        public string PoCode { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string CustomerStyleNO { get; set; }
        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 工厂ID
        /// </summary>
        public Nullable<int> Factoryid { get; set; }
        /// <summary>
        /// 工序id
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 最新生产日期
        /// </summary>
        public Nullable<System.DateTime> ProcessDate { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public Nullable<System.DateTime> StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public Nullable<int> Planqty { get; set; }
        /// <summary>
        /// 最新日生产数据
        /// </summary>
        public Nullable<int> Dayqty { get; set; }
        /// <summary>
        /// 累计生产数量
        /// </summary>
        public Nullable<int> Totalqty { get; set; }
        /// <summary>
        /// 目前耗时
        /// </summary>
        public Nullable<double> Duration { get; set; }
        /// <summary>
        /// 计划总耗时
        /// </summary>
        public Nullable<double> Planduration { get; set; }
        /// <summary>
        /// 延误类型 2一般延误，3严重延误
        /// </summary>
        public Nullable<int> Troubletype { get; set; }
        /// <summary>
        /// 完成百分百
        /// </summary>
        public Nullable<double> Schedulepercent { get; set; }
        /// <summary>
        /// 交期
        /// </summary>
        public DateTime DeliveryDate { get; set; }
        /// <summary>
        /// 延误天数
        /// </summary>
        public double DelayDays { get; set; }

        /// <summary>
        /// 标准工时
        /// </summary>
        public double Capacity { get; set; }

        /// <summary>
        /// 产品分类名称
        /// </summary>
        public string ProductTypeName { get; set; }

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
        private DateTime? _MaterielArriveDate;
        /// <summary>
        /// 辅料到厂日期
        /// </summary>
        public DateTime? MaterielArriveDate
        {
            get
            {
                if (_MaterielArriveDate > new DateTime(2000, 01, 01))
                    return _MaterielArriveDate;
                else
                    return null;
            }
            set { _MaterielArriveDate = value; }
        }
        /// <summary>
        /// 工艺路线名称
        /// </summary>
        public string RouteName { get; set; }

    }
}