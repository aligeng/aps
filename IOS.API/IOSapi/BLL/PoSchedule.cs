using AMO.Reports;
using AMOData;
using IOS.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IOSapi.BLL
{
    /// <summary>
    /// 滚动生产排期报表
    /// </summary>
    public class PoSchedule
    {
        // 报表列设置
        private List<ReportSetting> LstReportSetting;
        //列表结构
        private DataTable SettingTable;

        /// <summary>
        /// key:FactoryID,value:Simulation实例
        /// </summary>
        private Dictionary<int, Simulation> DicSimulation = new Dictionary<int, Simulation>();

        private Dictionary<int, VirtualProdEvent> DicVirProdEvent = new Dictionary<int, VirtualProdEvent>();

        /// <summary>
        /// 相关生产线
        /// </summary>
        private List<VirtualFacility> LstVirtualFacility { get; set; }

        /// <summary>
        /// 相关的生产线动态人数 key:生产线ID
        /// </summary>
        private Dictionary<int, IEnumerable<FacilityWorkerNumber>> DicFwn { get; set; }
        /// <summary>
        /// 相关的生产计划
        /// </summary>
        private List<ProductionEvent> LstProdEvent { get; set; }

        /// <summary>
        /// 相关的生产计划 key:生产线ID
        /// </summary>
        private Dictionary<int, IEnumerable<ProductionEvent>> DicFaProdEvent { get; set; }

        /// <summary>
        /// 所有相关的生产单细分 key：计划ID
        /// </summary>
        private Dictionary<int, List<ProductionEventDetail>> DicProdEventDetail { get; set; }

        /// <summary>
        /// 所有相关的生产单信息
        /// </summary>
        private Dictionary<int, PO> DicPO { get; set; }

        private List<PODetail> LstPODetail { get; set; }

        /// <summary>
        /// 所有客户信息
        /// </summary>
        private Dictionary<int, Customer> DicAllCustomers { get; set; }


        private List<POMaterialRequest> LstPOMaterialRequest = new List<POMaterialRequest>();


        private DataTable groupIdCodes { get; set; }



        /// <summary>
        /// 所有车间
        /// </summary>
        private List<AMOData.WorkShop> LstWorkShop { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        private Dictionary<int, DateTime> DicProdFinishDates;

        /// <summary>
        /// 关键事件进度日期
        /// </summary>
        private Dictionary<int, POEvent> DicPOEvents;

        /// <summary>
        /// 产品分类
        /// </summary>
        private Dictionary<int, ProductType> DicProductType;

        /// <summary>
        /// 获取滚动排期表数据
        /// </summary>
        /// <param name="factoryID">工厂id</param>
        /// <param name="poCode">生产单号</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="orderType">Eric添加 生产单类型：实单1，非实单0 </param>
        private void InitData(int factoryID, string poCode, DateTime startDate, DateTime endDate, int orderType, string lotNO)
        {
            //客户信息
            this.DicAllCustomers = Customer.GetDictionary(null);
            //产品分类
            this.DicProductType = ProductType.GetDictionary(null);
            this.InitLstReportSetting();

            //返回选中的工厂ID字符串
            /// 多个用逗号隔开，默认全选
            /// 格式："1,2,3,4,5,6"
            //string FactorySelectIDsStr = "1,2,3,12,14,15,16,17";
            string FactorySelectIDsStr = factoryID.ToString();
            this.DicSimulation.Clear();
            this.DicVirProdEvent.Clear();
      
            // 目标生产线集合
            Filter filter = new Filter();
            filter.AddSort("FactoryID");
            filter.AddSort("ID");
            //if (!string.IsNullOrEmpty(groupNames))
            //    filter.Add("GroupName", groupNames, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filter.Add("FactoryID", FactorySelectIDsStr, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);

            List<Facility> lstFacility = Facility.GetList(filter);
            this.LstVirtualFacility = new List<VirtualFacility>();
            List<int> ids = new List<int>();
            //
            // 相关的生产计划
            foreach (Facility fa in lstFacility)
            {
                if (!AMOData.Settings.ImpSetting.IsOutsourcedScheduling && fa.IsSubCon)//外发不排产需过滤外发线
                    continue;
                ids.Add(fa.ID);
                LstVirtualFacility.Add(new VirtualFacility(fa));
            }
            string wherein = string.Join(",", (from p in ids.Distinct() select p.ToString()).ToArray());

            if (string.IsNullOrEmpty(wherein))
                return;

            filter = new Filter();
            filter.Add(AMODataHelper.GetStringFromList(ids, 1000, "FacilityID"), LOGIC_TYPE.AND);
            filter.Add("StartTime", endDate.Date.AddDays(1), RELEATTION_TYPE.LESS, LOGIC_TYPE.AND);
            filter.Add("EndTime", startDate.Date, RELEATTION_TYPE.GREATEREQUAL, LOGIC_TYPE.AND);
            /*
            if (orderType > -1)
                filter.Add("ISFIRMORDER", orderType, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            */
            if (orderType > -1)
            {
                filter.AddFrom("productioneventdetail", "ProductionEvent.ID=productioneventdetail.ProductionEventID", JOIN_TYPE.LEFT);
                filter.AddFrom("PO", "PO.ID = productioneventdetail.POID", JOIN_TYPE.LEFT);
                filter.Add("PO.ISFIRMORDER", orderType, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            }
            if (string.IsNullOrEmpty(lotNO) == false)
            {
                string sql = string.Format("(select id from po where LotNO like '%{0}%')", lotNO);
                filter.Add("ProductionEvent.POID", sql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            }
            filter.AddSort("StartTime");
            this.LstProdEvent = ProductionEvent.GetList(filter);
            this.DicFaProdEvent = new Dictionary<int, IEnumerable<ProductionEvent>>();
            foreach (var g in this.LstProdEvent.GroupBy(x => x.FacilityID))
            {
                this.DicFaProdEvent[g.Key] = g.ToList();
            }
            //获取计划完成日期
            var peFilter = new Filter();
            peFilter.Add(AMODataHelper.GetStringFromList(ids, 1000, "PRODUCTIONEVENT.FacilityID"), LOGIC_TYPE.AND);
            peFilter.Add("PRODUCTIONEVENT.StartTime", endDate.Date.AddDays(1), RELEATTION_TYPE.LESS, LOGIC_TYPE.AND);
            peFilter.Add("PRODUCTIONEVENT.EndTime", startDate.Date, RELEATTION_TYPE.GREATEREQUAL, LOGIC_TYPE.AND);
            peFilter.Add("PRODUCTIONEVENT.STATUS", 1, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            this.DicProdFinishDates = ProductionSchedule.GetProductionFinishedDate(peFilter);

            //动态人数
            Filter fwnFilter = new Filter();
            string idStr = string.Join(",", (from p in ids.Distinct() select p.ToString()).ToArray());
            fwnFilter.Add("FacilityID", idStr, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            var lstFwn = FacilityWorkerNumber.GetList(fwnFilter);
            this.DicFwn = new Dictionary<int, IEnumerable<FacilityWorkerNumber>>();
            foreach (var g in lstFwn.GroupBy(x => x.FacilityID))
            {
                DicFwn[g.Key] = g.ToList();
            }
            //ids = new List<int>();
            //ids = (from item in this.LstProdEvent select item.POID).ToList();
            //this.LstProdEvent = ProductionEvent.GetList(filter);

            if (this.LstProdEvent.Count > 0)
            {
                var mylist = this.LstProdEvent.Where(x => !string.IsNullOrEmpty(x.GroupID));
                if (mylist.Any())
                {
                    var gids = mylist.Select(x => x.GroupID).Distinct();
                    var lst = gids.ToList();
                    groupIdCodes = AMO.BLL.ProductionEvent.GetAboutGroupIdList(lst); //获取与组ID关联的POCODE列表
                }
            }




            //生产计划细分
            ids = new List<int>();
            filter = new Filter();
            ids = (from item in this.LstProdEvent select item.ID).ToList();
            this.DicProdEventDetail = new Dictionary<int, List<ProductionEventDetail>>();
            List<ProductionEventDetail> lstProdEventDetail = new List<ProductionEventDetail>();
            if (ids.Count > 0)
            {
                filter.Add(AMODataHelper.GetStringFromList(ids, 1000, "ProductionEventID"), LOGIC_TYPE.AND);
                if (!string.IsNullOrEmpty(poCode))
                {
                    filter.Add("POID", string.Format("SELECT id FROM dbo.PO WHERE CODE LIKE '%{0}%'",poCode), RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
                }

                lstProdEventDetail = ProductionEventDetail.GetList(filter);
                foreach (var g in lstProdEventDetail.GroupBy(x => x.ProductionEventID))
                {
                    this.DicProdEventDetail[g.Key] = g.ToList();
                }
            }

            //生产单信息
            ids = new List<int>();
            filter = new Filter();
            ids = (from item in lstProdEventDetail select item.POID).Distinct().ToList();
            string sokeySql = string.Empty;
            if (ids.Count > 0)
            {
                filter.Add(AMODataHelper.GetStringFromList(ids, 1000, "ID"), LOGIC_TYPE.AND);
                if (!string.IsNullOrEmpty(poCode))
                {
                    filter.Add("Code", poCode, RELEATTION_TYPE.LIKE, LOGIC_TYPE.AND);
                }
                string poidSql = DBHelper.GetDistinctSql("PO", "ID", filter);
                sokeySql = DBHelper.GetDistinctSql("PO", "SoKeyID", filter);
                this.DicPO = PO.GetDictionary(filter);
                filter = new Filter();
                filter.Add("POID", poidSql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
                this.LstPODetail = PODetail.GetList(filter, false);

                //关键事件（物料ETA）日期，有实际区实际，没实际区预计
                var eventFilter = new Filter();
                eventFilter.AddFrom("EVENTFLOWNODE", "EVENTFLOWNODE.ID=POEVENT.EVENTFLOWNODEID", JOIN_TYPE.INNER);
                eventFilter.Add("EVENTFLOWNODE.CRITICALEVENTID", AMOData.Settings.SysSetting.Rpt_ActualMaterialETA, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
                eventFilter.Add("POEVENT.POID", poidSql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
                List<POEvent> lstPOEvent = POEvent.GetList(eventFilter);
                this.DicPOEvents = new Dictionary<int, POEvent>();
                foreach (POEvent model in lstPOEvent)
                {
                    if (DicPOEvents.ContainsKey(model.POID) == false)
                    {
                        DicPOEvents.Add(model.POID, model);
                    }
                }
            }
            else
            {
                this.DicPO = new Dictionary<int, PO>();
                this.LstPODetail = new List<PODetail>();
            }




            //
            if (!string.IsNullOrEmpty(sokeySql))
            {
                Filter sokeyFilter = new Filter();
                sokeyFilter.Add("SoKeyID", sokeySql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
                sokeyFilter.Add("MaterialStyle", 6, RELEATTION_TYPE.NOTEQUAL, LOGIC_TYPE.AND);
                LstPOMaterialRequest = POMaterialRequest.GetListOnlySample(sokeyFilter);
                LstPOMaterialRequest.Sort(POMaterialRequest.ComparerByDate);
            }

            #region 用于计算 计划的每日计划产量
            List<VirtualProdEvent> lstVirProdEvent = this.CreateVirtualProdEvent(this.LstProdEvent, lstProdEventDetail);
            this.DicVirProdEvent = lstVirProdEvent.ToDictionary(x => x.ID);

            if (this.LstReportSetting.Find(x => x.ColumnName == "AreaPlanQty" || x.ColumnName == "AreaOutput") != null)//AMOHB-3684 增加区间排单数、区间产值
            {
                var gsVpe = lstVirProdEvent.GroupBy(x => x.FacilityID);
                foreach (var g in gsVpe)
                {
                    var virFacility = this.LstVirtualFacility.Find(x => x.ID == g.Key);
                    if (virFacility != null)
                    {
                        foreach (var vpe in g.ToList())
                        {
                            vpe.FactoryID = virFacility.FactoryID;
                        }
                    }
                }

                //按工厂生产simulation对象
                var gsFacility = this.LstVirtualFacility.GroupBy(x => x.FactoryID);
                foreach (var g in gsFacility)
                {
                    if (!DicSimulation.ContainsKey(g.Key))
                    {
                        //Simulation sim = this.CreateSimulation(g.Key, lstVirProdEvent.Where(x => x.FactoryID == g.Key).ToList());
                        var lstVirProdEventFinded = lstVirProdEvent.Where(x => x.FactoryID == g.Key);
                        if (lstVirProdEventFinded != null && lstVirProdEventFinded.Count() > 0)
                        {
                            Simulation sim = this.CreateSimulation(g.Key, lstVirProdEventFinded.ToList());
                            DicSimulation.Add(g.Key, sim);
                        }
                    }
                }
            }
            #endregion

        }

        private Simulation CreateSimulation(int factoryID, List<AMOData.VirtualProdEvent> lstEvent)
        {
            //if (lstEvent == null || lstEvent.Count == 0)
            //    return;

            Simulation sim = new Simulation(false, lstEvent.Min(p => p.StartTime.Date), lstEvent.Max(p => p.EndTime.Date));

            string strFacilityIDs = string.Join(",", (from item in lstEvent select item.FacilityID.ToString()).Distinct().ToArray());
            Filter filter = new Filter();
            filter.Add("ID", strFacilityIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            sim.LoadFacility(filter, true);

            sim.ShowFactoryID = factoryID;
            if (sim.ShowFactoryID > 0)
                sim.LoadCalendar(true, false);
            sim.LoadLnCurveData();

            foreach (var vpe in lstEvent)
            {
                var lnApp = sim.LearningCurve.GetInLncApply(vpe);
                if (lnApp != null)
                {
                    vpe.LncTemplateID = lnApp.LnCurveTemplateID;
                }
            }
            return sim;
        }

        private List<VirtualProdEvent> CreateVirtualProdEvent(List<ProductionEvent> lstProdEvent, List<ProductionEventDetail> lstProdDetail)
        {
            List<AMOData.VirtualProdEvent> lstVirProdEvent = new List<AMOData.VirtualProdEvent>();
            var pedGroups = lstProdDetail.GroupBy(ped => ped.ProductionEventID).ToList().ToDictionary(x => x.Key);
            foreach (AMOData.ProductionEvent pe in lstProdEvent)
            {
                if (!pedGroups.ContainsKey(pe.ID))
                    continue;
                AMOData.VirtualProdEvent vp = new AMOData.VirtualProdEvent(pe, DateTime.Now);

                if (pedGroups.ContainsKey(pe.ID))
                {
                    List<AMOData.ProductionEventDetail> lstDetail = pedGroups[pe.ID].ToList();// lstAllProdDetail.Where(x => x.ProductionEventID == pe.ID).ToList();
                    foreach (AMOData.ProductionEventDetail xDetail in lstDetail)
                        vp.AddDetail(new AMOData.VirtualProdEventDetail(xDetail, "", "", ""));
                }
                //vp.FactoryID = (dicFacility.ContainsKey(vp.FacilityID)) ? dicFacility[vp.FacilityID].FactoryID : -1;
                lstVirProdEvent.Add(vp);
            }

            return lstVirProdEvent;
        }
        /// <summary>
        /// 报表数据
        /// </summary>
        /// <param name="factoryID">工厂id</param>
        /// <param name="poCode">生产单号</param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable ReporDataList(int factoryID, string poCode, DateTime startDate, DateTime endDate)
        {
            InitData(factoryID, poCode, startDate, endDate, -1, "");

            DataTable tb = new DataTable();
            tb = SettingTable.Copy();
            tb.Rows.Clear();

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Title", "滚动排产");//CText.TextReportRounRoll);

            int i = 1;
            DataRow newRow = this.GetTableRow();

            var groupName = LstVirtualFacility.GroupBy(m => m.GroupName).ToList();//按车间分组
            foreach (var fg in groupName)//1.遍历车间组
            {
                List<ProductionEvent> lstProductionEvent = new List<ProductionEvent>();
                foreach (VirtualFacility vfa in fg.ToList()) //2.遍历生产线
                {
                    if (!this.DicFaProdEvent.ContainsKey(vfa.ID))
                        continue;
                    lstProductionEvent = this.DicFaProdEvent[vfa.ID].ToList();
                    i = 1;

                    //计算是否需要 产能空隙颜色提示
                    //CalcProdCapacityAlert(vfa, lstProductionEvent);
                    List<ProductionEvent> pes = lstProductionEvent.ToList();//生产计划
                    foreach (AMOData.ProductionEvent pe in pes)//3.遍历生产计划，一条生产线上有多个生产计划
                    {
                        if (!this.DicProdEventDetail.ContainsKey(pe.ID))
                            continue;
                        var details = this.DicProdEventDetail[pe.ID];// this.LstProdEventDetail.Where(x => x.ProductionEventID == pe.ID).ToList();

                        if (details.Count() <= 0)
                            continue;

                        //calc plan daily plan qty
                        //AMOHB - 3684 增加区间排单数、区间产值
                        int areaQty = 0;
                        double areaOutput = 0;
                        double totalOutputValue = 0;

                        List<FacilityWorkerNumber> lstFwn = this.DicFwn.ContainsKey(vfa.ID) ? this.DicFwn[vfa.ID].ToList() : new List<FacilityWorkerNumber>();
                        DataRow dr = this.GetTableRow();
                        RoundRollData oRoundRoll = new RoundRollData(pe, vfa, lstFwn, this.DicPO, this.LstPODetail, details,
                            this.DicAllCustomers, this.LstPOMaterialRequest, false, groupIdCodes, this.LstReportSetting, this.DicProdFinishDates, this.DicPOEvents, dr, vfa.Name, i, details, this.DicProductType, areaQty, areaOutput, totalOutputValue);

                        var row = tb.NewRow();
                        row.ItemArray = dr.ItemArray;
                        tb.Rows.Add(row);//这是加入的是第一行
                        i++;

                    }//生产计划遍历结束

                }//生产线遍历线束

            }//遍历车间组结束

            return tb;

        }

        private DataRow GetTableRow()
        {
            return this.SettingTable.NewRow();
        }


        /// <summary>
        /// 将参数值设置为“0”
        /// </summary>
        /// <param name="totalOtHrs"></param>
        /// <param name="otdayCount"></param>
        /// <param name="workDayCount"></param>
        /// <param name="dayOff"></param>
        public void InitValue(ref double totalOtHrs, ref int otdayCount, ref double workDayCount, ref int dayOff)
        {
            totalOtHrs = 0;
            otdayCount = 0;
            workDayCount = 0;
            dayOff = 0;
        }

        //初始化报表列设置
        private void InitLstReportSetting()
        {
            this.LstReportSetting = ReportSetting.GetList("AMO.Reports.CtrlRoundRollReport", "zh-CN");
            //重新构造表结构
            this.BuildTableStructure();
        }
        // 构建数据集的结构
        private void BuildTableStructure()
        {
            this.SettingTable = new DataTable();
            foreach (ReportSetting rptSet in this.LstReportSetting.OrderBy(e => e.SEQ))
            {
                switch (rptSet.DataType)
                {
                    case "string":
                        this.SettingTable.Columns.Add(rptSet.ColumnName);
                        break;
                    case "int":
                        this.SettingTable.Columns.Add(rptSet.ColumnName, typeof(int));
                        break;
                    case "double":
                        this.SettingTable.Columns.Add(rptSet.ColumnName, typeof(double));
                        break;
                    case "DateTime":
                        this.SettingTable.Columns.Add(rptSet.ColumnName, typeof(DateTime));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}