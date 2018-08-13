using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    public class RollPlanReportData
    {
        /// <summary>
        /// 总数目
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 当前页数据；  类型：DataTable； 参数说明： 
        ///GP 生产线
        ///Seq 序号
        ///PoCode 生产单号
        ///Customer 客户名称
        ///Style 客户款号
        ///Pattern 本厂款号
        ///OrderQty 生产单数量
        ///ScheduleQty 排单数
        ///Wkr 车缝人数
        ///Succ 效率(%)
        ///Workday 工作天数
        ///OutputDay 平均日产量
        ///CutDatePlanned 发裁片
        ///SewingStart 车缝开始
        ///SewingEnd 车缝结束
        ///DayOff 休息天数
        ///OTD 原始交期
        ///CAOTD 客户可接受交期
        ///FtyDel 车间交期
        ///FtyVsOTD 车间交期/交货期
        ///Color   颜色
        ///Capacity    标准工时
        ///SeaNO   季节号
        ///EffGSD  效率后GSD IE
        ///MaterialETA 原物料ETA
        ///ActualMaterialETA 实际原物料ETA
        ///WorkHours 工作时数
        ///PartOnline 零件区上线日
        ///DelayDays 延迟天数
        ///Remark 备注
        ///ExCountry 交货地点
        ///ExportWay 走货方式
        ///FactoryDel 工厂交期
        ///ProdType 产品分类
        ///CustomerOrderNO 客单号
        ///SalesOrderType 销售组别
        ///MasterMaterialDate 主料到厂日期
        ///MaterialDate 辅料到厂日期
        ///PlanStart 计划开始日期
        ///DepotDate 进仓期
        ///SampleDate 样品寄送日
        ///Price 单价
        ///TotalOutputValue 总产值
        ///GroupName 车间
        /// </summary>
        public DataTable dt { get; set; }
    }
}