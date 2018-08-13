using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.MonitorModels
{
    public class ProcessDatas
    {
        /// <summary>
        /// 节点id
        /// </summary>
        public int ProceId { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string ProceName { get; set; } //节点名称
        /// <summary>
        /// 正常数量
        /// </summary>
        public int Normal { get; set; } //正常数量
        /// <summary>
        /// 异常数量
        /// </summary>
        public int Abnormal { get; set; } //异常数量
        /// <summary>
        /// vip客户数量
        /// </summary>
        public int VIP { get; set; } //vip客户数量
        /// <summary>
        /// 节点类型（1产前、2工艺、3终端、4物料）
        /// </summary>
        public int Type { get; set; } //节点类型
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; } //排序
        /// <summary>
        /// 匹配到工艺节点id（物料类型节点才需要）
        /// </summary>
        public int Refid { get; set; } //物料匹配节点id
        /// <summary>
        /// 异常单数据(前8个异常单)
        /// </summary>
        public List<WarnPoData> WarnDatas { get; set; } //异常数据
        ///// <summary>
        ///// 车缝单进度数据
        ///// </summary>
        //public List<Progress> ProgressDatas { get; set; }  //裁剪单进度数据
    }


    public class WarnPoData
    {
        /// <summary>
        /// 生产单id
        /// </summary>
        public int Poid { get; set; }
        /// <summary>
        /// 生产单号
        /// </summary>
        public string PoCode { get; set; } //生产单号
        /// <summary>
        /// 创建时间(订单日期)
        /// </summary>
        public DateTime? Createdate { get; set; }//创建时间(订单日期)
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }//客户名称
        /// <summary>
        /// 订单数量
        /// </summary>
        public double Amount { get; set; }               //数量
        /// <summary>
        /// 本厂款号
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 客户款号
        /// </summary>
        public string Customerstyleno { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string Producttype { get; set; } 
        /// <summary>
        /// 交货期（本生产单涉及的最早一批需要提交给客户的日期）
        /// </summary>
        public DateTime? Deliverydate { get; set; } //交期
        /// <summary>
        /// 原始交期
        /// </summary>
        public DateTime? InitDeliverydate { get; set; } //原始交期
        /// <summary>
        /// 客户交期
        /// </summary>
        public DateTime? CaoTD { get; set; } //客户交期
        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }     //工厂
        /// <summary>
        /// 生产单状态（0未完结；1已完结）
        /// </summary>
        public int? Status { get; set; }     //状态
        /// <summary>
        /// 异常状态（１正常、２严重延误、３轻微延误、４严重提前）
        /// </summary>
        public int? Troubletype { get; set; } //状态
        /// <summary>
        /// 跟单员
        /// </summary>
        public string Merchandiser { get; set; }     //跟单员
        /// <summary>
        /// 是否VIP客户
        /// </summary>
        public int? VIP { get; set; } //是否VIP
    }

    public class SewingLine
    {
        /// <summary>
        /// 产线名称+当前生产单编号
        /// </summary>
        public string PoCode { get; set; } //生产单号
        /// <summary>
        /// 开始日期
        /// </summary>
        public string Startdate { get; set; }//开始日期
        /// <summary>
        /// 结束日期
        /// </summary>
        public string Enddate { get; set; } //结束日期
        /// <summary>
        /// 进度
        /// </summary>
        public double ProgressRate { get; set; } //进度
        /// <summary>
        /// 当前生产单状态（１正常、２严重延误、３轻微延误、４严重提前）
        /// </summary>
        public int Troubletype { get; set; } //状态

    }
}