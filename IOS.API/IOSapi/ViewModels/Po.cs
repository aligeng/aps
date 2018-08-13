using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单数据
    /// </summary>
    public class Po
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 生产单编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 客户id
        /// </summary>
        public int customerid { get; set; }
        /// <summary>
        /// 工厂id
        /// </summary>
        public int factoryid { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string customername { get; set; }
        /// <summary>
        /// 订单确认日期
        /// </summary>
        private DateTime? _confirmdate;
        /// <summary>
        /// 订单确认日期
        /// </summary>
        public DateTime? confirmdate
        {
            get
            {
                if (_confirmdate > new DateTime(2000, 01, 01))
                    return _confirmdate;
                else
                    return null;
            }
            set { _confirmdate = value; }
        }
        /// <summary>
        /// 生产单交货期
        /// </summary>
        public DateTime deliverydate { get; set; }
        /// <summary>
        /// 原始交期
        /// </summary>
        public DateTime? initDeliveryDate { get; set; }
        /// <summary>
        /// 车间交期
        /// </summary>
        public DateTime? workshopDate { 
            get 
            {
                return deliverydate.AddDays(-workshopDays);
            } 
        }
        /// <summary>
        /// 车间交期提前天数
        /// </summary>
        public int workshopDays { get; set; }
        /// <summary>
        /// 产品分类
        /// </summary>
        public string producttype { get; set; }
        /// <summary>
        /// 款号
        /// </summary>
        public string pattern { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string customerPattern { get; set; }
        /// <summary>
        /// 订单数量
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 排产数量
        /// </summary>
        public double planAmount { get; set; }
        /// <summary>
        /// 优先级,,6-普通，8-重要,10-紧急
        /// </summary>
        public int priority { get; set; }
        /// <summary>
        /// 跟单员
        /// </summary>
        public string merchandiser { get; set; }

        private DateTime? _StarDate { get; set; }
        /// <summary>
        /// 排产(车缝)计划开始日期
        /// </summary>
        public DateTime? StarDate
        {
            get
            {
                if (_StarDate > new DateTime(2000, 01, 01))
                    return _StarDate;
                else
                    return null;
            }
            set { _StarDate = value; }
        }

        /// <summary>
        /// 排产(车缝)计划结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 已完成数（排产）
        /// </summary>
        public double CompleteAmount { get; set; }

        /// <summary>
        /// 裁剪数
        /// </summary>
        public int CuttingAmount { get; set; }

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

        private DateTime? _OthMaterielArriveDate;
        /// <summary>
        /// 辅料到厂日期
        /// </summary>
        public DateTime? OthMaterielArriveDate
        {
            get
            {
                if (_OthMaterielArriveDate > new DateTime(2000, 01, 01))
                    return _OthMaterielArriveDate;
                else
                    return null;
            }
            set { _OthMaterielArriveDate = value; }
        }

        /// <summary>
        /// 关键事件流程名称
        /// </summary>
        public string EventFlowName { get; set; }

        ///// <summary>
        ///// 生产单工序集合
        ///// </summary>
        //public List<SelectModel> Processes { get; set; }

    }
}