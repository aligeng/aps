using AMOData;
using IOS.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using AMO.Logic;
using System.Text;
using AMO;

namespace IOSapi.BLL
{
    /// <summary>
    /// 日计划产量报表
    /// </summary>
    public class DailyProduction
    {

        private Dictionary<int, ProcessRoute> DicAllProcessRoute = null;
        private Dictionary<int, Users> DicAllUsers = null;
        private Dictionary<int, Factory> DicAllFactory = null;
        private Dictionary<int, Customer> DicAllCustomer = null;
        private Dictionary<int, ProductType> DicAllProductType = null;
        private Dictionary<int, List<FacilityWorkerNumber>> DicFacilityWorkerNumber = null;
        private Dictionary<int, PO> DicPO = null;
        private Dictionary<int, ProductionEvent> DicProdEvent = null;
        private List<PODetail> PODetail = null;
        /// <summary>
        /// key ProductionEventID
        /// </summary>
        private Dictionary<int, List<ProductionEventDetail>> DicProdEventDetails = null;

        /// <summary>
        /// 数据库当前日期
        /// </summary>
        private DateTime DateNow = AMOData.AMOTime.Now.Date;


        List<DailyProdAmount> lstActualAmount = new List<DailyProdAmount>();


        /// <summary>
        /// 报表设置
        /// </summary>
        private List<ReportSetting> LstReportSetting = new List<ReportSetting>();




        /// <summary>
        /// 生成报表数据
        /// </summary>
        /// <returns></returns>
        private DataTable CreateData(string strFacilityIDs, DateTime dFrom, DateTime dTo, string code, string pattern, bool showByPlan, SortOrders sortOrder)
        {
            #region get data

            Filter filter1 = new Filter();
            filter1.Add("StartTime", dTo.Date.AddDays(1), RELEATTION_TYPE.LESSEQUAL, LOGIC_TYPE.AND);
            filter1.Add("EndTime", dFrom.Date, RELEATTION_TYPE.GREATEREQUAL, LOGIC_TYPE.AND);

            //拿取指定的生产线
            //string strFacilityIDs = this.GetFacilityID();
            List<Facility> lstFacility = AMO.BLL.Facility.GetList(strFacilityIDs);

            //ProductionEvent
            Filter filter = new Filter();
            filter.Add("FacilityID", strFacilityIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filter.Add(filter1, LOGIC_TYPE.AND);


            List<ProductionEvent> lstProductionEvent = ProductionEvent.GetList(filter);
            DicProdEvent = lstProductionEvent.ToDictionary(x => x.ID);

            ////拿取指定时间段内指定生产线的生产计划
            string ProductionEventIDSql = DBHelper.GetDistinctSql("ProductionEvent", "ID", filter);

            //ProductionEventDetail
            filter = new Filter();
            filter.Add("ProductionEventID", ProductionEventIDSql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<ProductionEventDetail> lstProductionEventDetail = ProductionEventDetail.GetList(filter);
            DicProdEventDetails = new Dictionary<int, List<ProductionEventDetail>>();
            var gsPED = lstProductionEventDetail.GroupBy(x => x.ProductionEventID);
            foreach (var g in gsPED)
                DicProdEventDetails.Add(g.Key, g.ToList());

            //PO
            string distPOIDSql = DBHelper.GetDistinctSql("ProductionEventDetail", "POID", filter);

            //增加生产单&&款号过滤
            if (!string.IsNullOrEmpty(code))
            {
                if (code.Length >= AMOData.Settings.SysSetting.PoCodeLength)
                    distPOIDSql += string.Format(@" and code = '{0}' ", code);
                else
                    distPOIDSql += string.Format(@" and code like '%{0}%' ", code);
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                if (pattern.Length >= AMOData.Settings.SysSetting.PatternLength)
                    distPOIDSql += string.Format(@" and PATTERN ='{0}' ", pattern);
                else
                    distPOIDSql += string.Format(@" and PATTERN like '%{0}%' ", pattern);
            }

            filter = new Filter();

            filter.Add("ID", distPOIDSql, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);

            DicPO = PO.GetDictionary(filter);

            //ProductionSchedule
            Filter filterProcess = new Filter();
            filterProcess.Add("IsPrimary", 1, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            string distProcessSQL = DBHelper.GetDistinctSql("Process", "ID", filterProcess);

            Filter filterPS = new Filter();
            filterPS.Add("ProductionDate", dFrom.Date, RELEATTION_TYPE.GREATEREQUAL, LOGIC_TYPE.AND);
            filterPS.Add("ProductionDate", dTo.Date, RELEATTION_TYPE.LESSEQUAL, LOGIC_TYPE.AND);
            filterPS.Add("ProcessID", distProcessSQL, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<AMOData.ProductionSchedule> lstProdSchedule = ProductionSchedule.GetList(filterPS);
            //var qPS = from g in lstProdSchedule
            //          group g by g.POID;

            //生产线的动态人数
            Filter dyFilter = new Filter();
            dyFilter.Add("FacilityID", strFacilityIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            var lstDyWrorkers = FacilityWorkerNumber.GetList(dyFilter);
            if (lstDyWrorkers.Count > 0)
                lstDyWrorkers = lstDyWrorkers.OrderByDescending(x => x.BeginDate).ToList();//开始日期大到小排序

            DicFacilityWorkerNumber = new Dictionary<int, List<FacilityWorkerNumber>>();
            var gs = lstDyWrorkers.GroupBy(x => x.FacilityID).ToList();
            foreach (var g in gs)
            {
                DicFacilityWorkerNumber[g.Key] = g.ToList();
            }
            //
            #endregion

            #region create data structure

            lstActualAmount.Clear();

            List<DailyProdAmount> lstPlanAmount = new List<DailyProdAmount>();

            List<LineRate> lstLineRate = new List<LineRate>();

            //结果表
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("SpecLineID", typeof(int));//生产线ID
            foreach (ReportSetting rptSet in LstReportSetting)
            {
                //switch (rptSet.DataType.ToLower())
                //{
                //    case "int":
                //        dtResult.Columns.Add(rptSet.HeaderText, typeof(int));
                //        break;
                //    case "datetime":
                //        dtResult.Columns.Add(rptSet.HeaderText, typeof(DateTime));
                //        break;
                //    case "string":
                //    default:
                dtResult.Columns.Add(rptSet.HeaderText);
                //        break;
                //}
            }

            dtResult.Columns.Add("SpecCol", typeof(string));//特殊列

            DateTime dayDate = dFrom.Date;
            while (dayDate <= dTo.Date)
            {
                DataColumn dc = new DataColumn(dayDate.ToString("MM-dd"), typeof(string));
                dc.DefaultValue = 0;
                dtResult.Columns.Add(dc);
                dayDate = dayDate.AddDays(1);
            }

            DataColumn dcRowSum = new DataColumn("RowSum", typeof(string));
            dcRowSum.DefaultValue = 0;
            dtResult.Columns.Add(dcRowSum);//行 sum 列

            DataColumn dcRowSpan = new DataColumn("RowSpan", typeof(int));
            dcRowSpan.DefaultValue = 1;
            dtResult.Columns.Add(dcRowSpan);

            #endregion

            #region put data to lstPlanAmount

            Simulation oSimulation = new Simulation(true);
            oSimulation.LoadSettings();
            oSimulation.LoadFacility(true);
            oSimulation.LoadCalendar(false, true);
            oSimulation.LoadLnCurveData();
            //外发不排产需过滤外发线
            if (!AMOData.Settings.ImpSetting.IsOutsourcedScheduling)
                lstFacility = lstFacility.Where(m => m.IsSubCon == false).ToList();
            //join lstProductionEvent & lstFacility
            var query = (from xEvent in lstProductionEvent
                         join xFacility in lstFacility
                         on xEvent.FacilityID equals xFacility.ID
                         orderby xFacility.Name, xEvent.StartTime
                         select new { xEvent, xFacility }).ToList();//

            foreach (var obj in query)//遍历每条生产线上每个计划
            {
                VirtualProdEvent vProdEvent = new VirtualProdEvent(obj.xEvent, oSimulation.DateNow);
                List<int> lstPOIDs = null;
                if (DicProdEventDetails.ContainsKey(obj.xEvent.ID))
                {
                    List<ProductionEventDetail> queryDetail = DicProdEventDetails[obj.xEvent.ID];
                    foreach (ProductionEventDetail detail in queryDetail)
                    {
                        VirtualProdEventDetail vDetail = new VirtualProdEventDetail(detail, string.Empty, string.Empty, string.Empty);
                        vProdEvent.AddDetail(vDetail);
                    }
                    lstPOIDs = queryDetail.Select(x => x.POID).Distinct().ToList();
                }

                if (lstPOIDs == null)
                    lstPOIDs = new List<int>();

                //学习曲线
                LnCurveApply oLnCurveApply = oSimulation.LearningCurve.GetInLncApply(vProdEvent);
                if (oLnCurveApply != null)
                {
                    vProdEvent.LncApplyID = oLnCurveApply.ID;
                    vProdEvent.LncTemplateID = oLnCurveApply.LnCurveTemplateID;
                }

                foreach (int pid in lstPOIDs)
                {
                    if (DicPO.ContainsKey(pid))
                    {
                        // double poqty = vProdEvent.AllDetails.Where(p => p.POID == pid).Sum(p => p.PlanAmount);//PO排单总数
                        //
                        Dictionary<DateTime, int> dicAmount = vProdEvent.CalculateCanFinishedQty(oSimulation, true, -1, pid);//计算某个计划每天应该完成的数量
                        dayDate = dFrom.Date;
                        while (dayDate <= dTo.Date)
                        {
                            int amount = 0;
                            if (dicAmount.ContainsKey(dayDate.Date))
                                amount = dicAmount[dayDate.Date];

                            DailyProdAmount pAmount = new DailyProdAmount();
                            pAmount.LineID = obj.xFacility.ID;
                            pAmount.ProdEventID = obj.xEvent.ID;
                            pAmount.POID = pid;//po.ID;
                            pAmount.ProdDate = dayDate;
                            pAmount.Amount = amount;// (poqty == vpqty ? amount : (int)Math.Round(amount * poqty / vpqty, 0));//考虑合单情况
                            lstPlanAmount.Add(pAmount);

                            dayDate = dayDate.AddDays(1);
                        }
                    }
                }

            }

            #endregion

            #region put data to lstActualAmount

            //join lstProdSchedule ,lstProductionEvent, lstFacility
            var query2 = (from xPS in lstProdSchedule
                          join xProdEv in lstProductionEvent on xPS.ProductionEventID equals xProdEv.ID
                          join xFacility in lstFacility on xProdEv.FacilityID equals xFacility.ID
                          orderby xFacility.ID
                          select new { xPS, xFacility }).ToList();//

            foreach (var obj in query2)//遍历每条生产线上每个计划 实际产量 
            {
                DailyProdAmount aAmount = new DailyProdAmount();
                aAmount.LineID = obj.xFacility.ID;
                aAmount.ProdEventID = obj.xPS.ProductionEventID;
                aAmount.POID = obj.xPS.POID;
                aAmount.ProdDate = obj.xPS.ProductionDate;
                if (obj.xPS.Amount == 0)
                    aAmount.HasZeroAmount = true;
                if (!DicPO.Keys.Contains(aAmount.POID))
                    continue;
                aAmount.PoCode = DicPO[aAmount.POID].Code;
                aAmount.Amount = Convert.ToInt32(obj.xPS.Amount);//Ben 等于0时表示有进度，但数量为0

                lstActualAmount.Add(aAmount);
            }
            #endregion

            #region put data to lstLineRate
            var lineIDs = lstPlanAmount.Select(p => p.LineID).Distinct();
            List<int> prodtingFaciltyids = DicProdEvent.Values.Select(x => x.FacilityID).Distinct().ToList();
            foreach (int lineid in lineIDs)
            {
                if (!prodtingFaciltyids.Contains(lineid))
                    continue;
                int pAmount = lstPlanAmount.Where(p => p.LineID == lineid).ToList().Sum(p => p.Amount);
                if (pAmount > 0)
                {
                    int aAmount = lstActualAmount.Where(p => p.LineID == lineid).ToList().Sum(p => p.Amount);
                    LineRate lr = new LineRate();
                    lr.LineID = lineid;
                    lr.CompleteRate = Math.Round(((double)aAmount / pAmount), 3);
                    Facility facility = lstFacility.First(m => m.ID == lineid);
                    lr.FacilityName = facility.Name;
                    lr.GroupName = facility.GroupName;
                    lstLineRate.Add(lr);
                }
            }
            if (sortOrder == SortOrders.ByRanking)
            {
                lstLineRate = lstLineRate.OrderByDescending(p => p.CompleteRate).ToList();
            }
            else
            {//生产线，排序顺序同排产器
                lstLineRate = lstLineRate.OrderBy(p => p.GroupName).ThenBy(m => m.FacilityName).ToList();
            }

            #endregion

            #region put data to dtResult

            DataRow drTempTotalPlan = dtResult.NewRow();//最后一行（总计行）
            DataRow drTempTotalActual = dtResult.NewRow();
            int tempRowTotalPlan = 0, tempRowTotalActual = 0;//总计列累计
            int mc = 0;//名次

            #region insert 数据行&小计行
            foreach (LineRate lr in lstLineRate)//按完成率倒序
            {
                Facility oFacility = lstFacility.Find(f => f.ID == lr.LineID);
                if (oFacility == null)
                    continue;
                int lineid = lr.LineID;

                #region insert 生产线分组行
                DataRow drLine = dtResult.NewRow();
                drLine["SpecLineID"] = -1;

                string strSpecialCol = string.Empty;
                string leader = string.Empty;
                if (string.IsNullOrEmpty(oFacility.Leader) == false)
                {
                    leader = string.Format(",{0}", oFacility.Leader);
                }
                //if (sortOrder == SortOrders.ByRanking)
                //{

                //    strSpecialCol = string.Format("{0} {1} ( {2}: {3} )", oFacility.Name, leader, CText.TextOrderName, ++mc);
                //}
                //else
                //{
                    strSpecialCol = string.Format("{0} {1} ", oFacility.Name, leader);
                //}
                //增加code&pattern过滤条件的显示
                if (!string.IsNullOrEmpty(code.Trim()))
                    strSpecialCol += " , " + code;
                if (!string.IsNullOrEmpty(pattern.Trim()))
                    strSpecialCol += " , " + pattern;

                drLine["SpecCol"] = strSpecialCol;
                dtResult.Rows.Add(drLine);//insert facility summary row
                #endregion

                #region insert 数据行

                //生产线最后一行（小计行）
                DataRow drTempPlanSum = dtResult.NewRow();
                DataRow drTempActualSum = dtResult.NewRow();
                int tempRowSumPlan = 0, tempRowSumActual = 0;//小计列累计

                var linePlanIDs = lstPlanAmount.Where(p => p.LineID == lineid).ToList().Select(p => p.ProdEventID).Distinct();
                foreach (int peid in linePlanIDs)//循环产线上的每一个计划
                {
                    if (!showByPlan)
                    {
                        SetContentRow(dtResult, peid, lineid, lstPlanAmount, oFacility, dFrom, dTo,
                                      ref tempRowSumPlan, ref tempRowSumActual, ref tempRowTotalPlan, ref tempRowTotalActual,
                                      drTempPlanSum, drTempActualSum, drTempTotalPlan, drTempTotalActual);
                    }
                    else
                    {
                        SetContentRow22(dtResult, peid, lineid, lstPlanAmount, oFacility, dFrom, dTo,
                                     ref tempRowSumPlan, ref tempRowSumActual, ref tempRowTotalPlan, ref tempRowTotalActual,
                                     drTempPlanSum, drTempActualSum, drTempTotalPlan, drTempTotalActual);
                    }

                }//end foreach plans

                #endregion

                #region insert 小计行
                DataRow drLineTotal = dtResult.NewRow();
                drLineTotal["SpecLineID"] = -2;
                drLineTotal["SpecCol"] = "目标达成（小计）";// "目标达成（小计）";

                dayDate = dFrom.Date;
                while (dayDate <= dTo.Date)
                {
                    string colName = dayDate.ToString("MM-dd");
                    int planAmountSum = Convert.ToInt32(drTempPlanSum[colName]);
                    if (planAmountSum == 0)
                        drLineTotal[colName] = "";
                    else
                        drLineTotal[colName] = (Convert.ToDouble(drTempActualSum[colName]) / planAmountSum).ToString("p1");
                    dayDate = dayDate.AddDays(1);
                }

                if (tempRowSumPlan == 0)
                    drLineTotal["RowSum"] = "";
                else
                    drLineTotal["RowSum"] = (Convert.ToDouble(tempRowSumActual) / tempRowSumPlan).ToString("p1");

                drTempActualSum = null;
                drTempPlanSum = null;
                dtResult.Rows.Add(drLineTotal);//insert 小计行
                #endregion
            }
            #endregion

            #region insert 总计行
            if (dtResult.Rows.Count > 0)
            {
                DataRow drAllTotal = dtResult.NewRow();
                drAllTotal["SpecLineID"] = -3;
                drAllTotal["SpecCol"] = "目标达成（总计）";// "目标达成（总计）";

                dayDate = dFrom.Date;
                while (dayDate <= dTo.Date)
                {
                    string colName = dayDate.ToString("MM-dd");
                    int planAmountTotal = Convert.ToInt32(drTempTotalPlan[colName]);
                    if (planAmountTotal == 0)
                        drAllTotal[colName] = "";
                    else
                        drAllTotal[colName] = (Convert.ToDouble(drTempTotalActual[colName]) / planAmountTotal).ToString("p1");
                    dayDate = dayDate.AddDays(1);
                }

                if (tempRowTotalPlan == 0)
                    drAllTotal["RowSum"] = "";
                else
                    drAllTotal["RowSum"] = (Convert.ToDouble(tempRowTotalActual) / tempRowTotalPlan).ToString("p1");

                drTempTotalActual = null;
                drTempTotalPlan = null;
                dtResult.Rows.Add(drAllTotal);//insert 总计行

                #region 汇总所有数量

                DataRow drSumPlan = dtResult.NewRow();
                drSumPlan["SpecLineID"] = -1;
                drSumPlan["SpecCol"] = "汇总计划产量 : " + tempRowTotalPlan;//汇总计划产量
                dtResult.Rows.Add(drSumPlan);//insert 汇总计划数量

                DataRow drSumActual = dtResult.NewRow();
                drSumActual["SpecLineID"] = -1;
                drSumActual["SpecCol"] = "汇总实际产量 : " + tempRowTotalActual;//汇总实际产量
                dtResult.Rows.Add(drSumActual);//insert 汇总实际产量
                #endregion

            }
            #endregion

            #endregion

            return dtResult;
        }



        private void SetContentRow(DataTable dtResult, int peid, int lineid,
    List<DailyProdAmount> lstPlanAmount, Facility oFacility, DateTime dFrom, DateTime dTo,
    ref int tempRowSumPlan, ref int tempRowSumActual, ref int tempRowTotalPlan, ref int tempRowTotalActual,
    DataRow drTempPlanSum, DataRow drTempActualSum, DataRow drTempTotalPlan, DataRow drTempTotalActual)
        {
            DateTime dayDate = dFrom.Date;
            var planPOIDs = lstPlanAmount.Where(p => p.ProdEventID == peid).Select(p => p.POID).Distinct();
            foreach (int poid in planPOIDs)//foreach plans' po 循环每一个PO
            {
                PO po = null;
                if (DicPO.ContainsKey(poid))
                    po = DicPO[poid];
                else
                    po = new PO();

                DataRow drPlan = dtResult.NewRow();
                drPlan["SpecLineID"] = lineid;
                drPlan["SpecCol"] = "计划产量";// "计划产量";
                drPlan["RowSpan"] = 3;

                ProductionEvent prodEvent = null;
                if (DicProdEvent.ContainsKey(peid))
                    prodEvent = DicProdEvent[peid];

                List<ProductionEventDetail> lstDetails = null;
                if (DicProdEventDetails.ContainsKey(peid))
                    lstDetails = DicProdEventDetails[peid];

                #region insert PO信息及计划产量 row

                this.SetPOValues(drPlan, prodEvent, lstDetails, po, oFacility);

                var qPA = lstPlanAmount.Where(p => p.ProdEventID == peid && p.POID == poid).ToList();
                if (qPA != null && qPA.Count > 0)
                {
                    int sumPlan = 0;
                    dayDate = dFrom.Date;
                    while (dayDate <= dTo.Date)
                    {
                        var oPA = qPA.Find(p => p.ProdDate.Date == dayDate.Date);
                        if (oPA != null)
                        {
                            string colName = dayDate.ToString("MM-dd");
                            drPlan[colName] = oPA.Amount;
                            drTempPlanSum[colName] = Convert.ToInt32(drTempPlanSum[colName]) + oPA.Amount;
                            drTempTotalPlan[colName] = Convert.ToInt32(drTempTotalPlan[colName]) + oPA.Amount;
                            sumPlan += oPA.Amount;
                        }

                        dayDate = dayDate.AddDays(1);
                    }
                    drPlan["RowSum"] = sumPlan;
                    tempRowSumPlan += sumPlan;
                    tempRowTotalPlan += sumPlan;
                }

                dtResult.Rows.Add(drPlan);//*insert 计划产量 row
                #endregion

                #region insert 实际产量 row

                DataRow drActual = dtResult.NewRow();
                drActual["SpecLineID"] = lineid;
                drActual["SpecCol"] = "实际产量";// "实际产量";

                var qAA = lstActualAmount.Where(p => p.ProdEventID == peid && p.POID == poid).ToList();
                if (qAA != null && qAA.Count > 0)
                {
                    int sumActual = 0;
                    dayDate = dFrom.Date;
                    while (dayDate <= dTo.Date)
                    {
                        var qa = qAA.Where(p => p.ProdDate.Date == dayDate.Date);
                        if (qa != null)
                        {
                            int iAmount = qa.Sum(p => p.Amount);
                            string colName = dayDate.ToString("MM-dd");
                            drActual[colName] = iAmount;

                            drTempActualSum[colName] = Convert.ToInt32(drTempActualSum[colName]) + iAmount;
                            drTempTotalActual[colName] = Convert.ToInt32(drTempTotalActual[colName]) + iAmount;

                            sumActual += iAmount;
                        }

                        dayDate = dayDate.AddDays(1);
                    }

                    drActual["RowSum"] = sumActual;
                    tempRowSumActual += sumActual;
                    tempRowTotalActual += sumActual;
                }

                dtResult.Rows.Add(drActual);//*insert 实际产量 row
                #endregion

                #region insert 差异 row
                DataRow drDiff = dtResult.NewRow();
                drDiff["SpecLineID"] = lineid;
                drDiff["SpecCol"] = "差异";// "差异";

                int sumDiff = 0;
                dayDate = dFrom.Date;
                while (dayDate <= dTo.Date)
                {
                    string colName = dayDate.ToString("MM-dd");
                    var scAmount = Convert.ToInt32(drPlan[colName]);
                    int iDiff = Convert.ToInt32(drActual[colName]) - scAmount;
                    drDiff[colName] = iDiff;
                    sumDiff += iDiff;
                    dayDate = dayDate.AddDays(1);
                }

                drDiff["RowSum"] = sumDiff;
                dtResult.Rows.Add(drDiff);//insert 差异 row

                #endregion

            }//end foreach plans' po

        }

        private void SetContentRow22(DataTable dtResult, int peid, int lineid,
            List<DailyProdAmount> lstPlanAmount, Facility oFacility, DateTime dFrom, DateTime dTo,
            ref int tempRowSumPlan, ref int tempRowSumActual, ref int tempRowTotalPlan, ref int tempRowTotalActual,
            DataRow drTempPlanSum, DataRow drTempActualSum, DataRow drTempTotalPlan, DataRow drTempTotalActual)
        {
            DateTime dayDate = dFrom.Date;
            DataRow drPlan = dtResult.NewRow();
            var planPOIDs = lstPlanAmount.Where(p => p.ProdEventID == peid).Select(p => p.POID).Distinct();
            foreach (int poid in planPOIDs)//foreach plans' po 循环每一个PO
            {
                PO po = null;
                if (DicPO.ContainsKey(poid))
                    po = DicPO[poid];
                else
                    po = new PO();

                drPlan["SpecLineID"] = lineid;
                drPlan["SpecCol"] = "计划产量";// "计划产量";
                drPlan["RowSpan"] = 3;

                ProductionEvent prodEvent = null;
                if (DicProdEvent.ContainsKey(peid))
                    prodEvent = DicProdEvent[peid];

                List<ProductionEventDetail> lstDetails = null;
                if (DicProdEventDetails.ContainsKey(peid))
                    lstDetails = DicProdEventDetails[peid];

                this.SetPOValues(drPlan, prodEvent, lstDetails, po, oFacility);

            }//end foreach plans' po

            MergeData(drPlan);

            #region insert 计划数 row
            var qPA = lstPlanAmount.Where(p => p.ProdEventID == peid).ToList();
            if (qPA != null && qPA.Any())
            {
                int sumPlan = 0;
                dayDate = dFrom.Date;
                while (dayDate <= dTo.Date)
                {
                    var oPA = qPA.Where(p => p.ProdDate.Date == dayDate.Date).ToList();
                    string colName = dayDate.ToString("MM-dd");
                    int sumAmount = oPA.Sum(x => x.Amount);
                    drPlan[colName] = sumAmount;
                    drTempPlanSum[colName] = Convert.ToInt32(drTempPlanSum[colName]) + sumAmount;
                    drTempTotalPlan[colName] = Convert.ToInt32(drTempTotalPlan[colName]) + sumAmount;
                    sumPlan += sumAmount;

                    dayDate = dayDate.AddDays(1);
                }
                drPlan["RowSum"] = sumPlan;
                tempRowSumPlan += sumPlan;
                tempRowTotalPlan += sumPlan;
            }

            dtResult.Rows.Add(drPlan);//*insert 计划产量 row

            #endregion

            #region insert 实际产量 row
            DataRow drActual = dtResult.NewRow();
            drActual["SpecLineID"] = lineid;
            drActual["SpecCol"] = "实际产量";// "实际产量";

            var qAA = lstActualAmount.Where(p => p.ProdEventID == peid).ToList();
            if (qAA != null && qAA.Count > 0)
            {
                int sumActual = 0;
                dayDate = dFrom.Date;
                while (dayDate <= dTo.Date)
                {
                    var qa = qAA.Where(p => p.ProdDate.Date == dayDate.Date);
                    if (qa != null)
                    {
                        int iAmount = qa.Sum(p => p.Amount);
                        string colName = dayDate.ToString("MM-dd");
                        drActual[colName] = iAmount;

                        drTempActualSum[colName] = Convert.ToInt32(drTempActualSum[colName]) + iAmount;
                        drTempTotalActual[colName] = Convert.ToInt32(drTempTotalActual[colName]) + iAmount;

                        sumActual += iAmount;
                    }

                    dayDate = dayDate.AddDays(1);
                }

                drActual["RowSum"] = sumActual;
                tempRowSumActual += sumActual;
                tempRowTotalActual += sumActual;
            }

            dtResult.Rows.Add(drActual);//*insert 实际产量 row
            #endregion

            #region insert 差异 row
            DataRow drDiff = dtResult.NewRow();
            drDiff["SpecLineID"] = lineid;
            drDiff["SpecCol"] = "差异";// "差异";

            int sumDiff = 0;
            dayDate = dFrom.Date;
            while (dayDate <= dTo.Date)
            {
                string colName = dayDate.ToString("MM-dd");
                var scAmount = Convert.ToInt32(drPlan[colName]);
                int iDiff = Convert.ToInt32(drActual[colName]) - scAmount;
                drDiff[colName] = iDiff;
                sumDiff += iDiff;
                dayDate = dayDate.AddDays(1);
            }

            drDiff["RowSum"] = sumDiff;
            dtResult.Rows.Add(drDiff);//insert 差异 row

            #endregion
        }


        private void SetPOValues(DataRow drPlanRow, ProductionEvent prodEvent, List<ProductionEventDetail> lstDetails, AMOData.PO po, AMOData.Facility oFacility)
        {
            if (prodEvent == null)
                return;

            foreach (ReportSetting rptSet in LstReportSetting)
            {
                switch (rptSet.ColumnName)
                {
                    case "WorkerNumber"://生产人数
                        if (prodEvent.WorkerNumber > 0)
                            drPlanRow[rptSet.HeaderText] = prodEvent.WorkerNumber;
                        else
                        {
                            if (!DicFacilityWorkerNumber.ContainsKey(oFacility.ID))//不包含动态人数
                                drPlanRow[rptSet.HeaderText] = oFacility.WorkerNumber;
                            else
                            {
                                drPlanRow[rptSet.HeaderText] = AMO.BLL.FacilityWorkerNumber.GetProdEventWorkerNumber(prodEvent.StartTime, prodEvent.EndTime, oFacility.WorkerNumber, DicFacilityWorkerNumber[oFacility.ID]);
                            }
                        }
                        break;
                    case "Code"://生产单号
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.Code);
                        break;
                    case "Amount"://订单数
                        SetRowValue(drPlanRow, rptSet.HeaderText, Convert.ToInt32(po.Amount).ToString("###,###"));
                        break;
                    case "PlanAmount"://排单数 
                        if (lstDetails == null || !lstDetails.Any())
                            SetRowValue(drPlanRow, rptSet.HeaderText, "");
                        else
                        {
                            double pa = lstDetails.Sum(p => p.PlanAmount);
                            SetRowValue(drPlanRow, rptSet.HeaderText, pa.ToString("###,###"));
                        }
                        break;
                    case "EndTime"://完工期                        
                        drPlanRow[rptSet.HeaderText] = prodEvent.EndTime.ToStringEx();
                        break;
                    case "Merchandiser"://跟单员
                        string strUser = DicAllUsers.ContainsKey(po.Merchandiser) ? DicAllUsers[po.Merchandiser].UserName : string.Empty;
                        SetRowValue(drPlanRow, rptSet.HeaderText, strUser);
                        break;
                    case "Pattern"://款号
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.Pattern);
                        break;
                    case "PatternDesc"://款式描述
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.PatternDesc);
                        break;
                    case "DeliveryDate"://交货期
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.DeliveryDate.ToStringEx());
                        break;
                    case "Customer"://客户
                        string custName = DicAllCustomer.ContainsKey(po.CustomerID) ? DicAllCustomer[po.CustomerID].Code : string.Empty;
                        SetRowValue(drPlanRow, rptSet.HeaderText, custName);
                        break;
                    case "CustomerOrderID"://客单号
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.CustomerOrderID);
                        break;
                    case "Factory"://工厂
                        string facName = DicAllFactory.ContainsKey(po.FactoryID) ? DicAllFactory[po.FactoryID].Name : string.Empty;
                        //SetRowValue(row, rptSet.HeaderText, facName);
                        drPlanRow[rptSet.HeaderText] = facName;
                        break;
                    case "ProcessRoute"://工艺路线
                        string routeName = DicAllProcessRoute.ContainsKey(po.ProcessRouteID) ? DicAllProcessRoute[po.ProcessRouteID].Name : string.Empty;
                        SetRowValue(drPlanRow, rptSet.HeaderText, routeName);
                        break;
                    case "ProductType"://产品分类
                        string productTypeName = DicAllProductType.ContainsKey(po.ProductTypeID) ? DicAllProductType[po.ProductTypeID].Name : string.Empty;
                        SetRowValue(drPlanRow, rptSet.HeaderText, productTypeName);
                        break;
                    case "SalesOrderNO"://销售号
                        SetRowValue(drPlanRow, rptSet.HeaderText, po.SalesOrderNO);
                        break;
                    case "PoDetailDesc"://细分备注
                        PODetail = AMO.BLL.PO.GetPODetailList(po.ID.ToString());
                        StringBuilder PoEventName = new StringBuilder();
                        foreach (var item in PODetail)
                        {
                            if (item.Description != "")
                                PoEventName.Append(item.Description + ";");

                        }
                        SetRowValue(drPlanRow, rptSet.HeaderText, PoEventName.ToString());
                        break;
                    default:
                        break;
                }
            }


        }

        private void MergeData(DataRow drPlan)
        {
            foreach (ReportSetting rptSet in LstReportSetting)
            {
                if (rptSet.ColumnName.ToUpper() == "AMOUNT")
                {
                    string str = drPlan[rptSet.HeaderText].ToString();
                    string[] arr = str.Trim().Split(';');
                    if (arr.Length == 1)
                        drPlan[rptSet.HeaderText] = arr[0];
                    else if (arr.Length > 1)
                    {
                        int amount = 0;
                        for (int i = 0; i < arr.Length; i++)
                        {
                            try
                            {
                                amount += Convert.ToInt32(arr[i]);
                            }
                            catch { }
                        }
                        drPlan[rptSet.HeaderText] = amount.ToString();
                    }
                }
                else if (rptSet.ColumnName.ToUpper() == "PLANAMOUNT")
                {
                    string str = drPlan[rptSet.HeaderText].ToString();
                    string[] arr = str.Trim().Split(';');
                    if (arr.Length >= 1)
                        drPlan[rptSet.HeaderText] = arr[0];
                }
                else
                {
                    string str = drPlan[rptSet.HeaderText].ToString();
                    string[] arr = str.Trim().Split(';');
                    if (arr.Length == 1)
                        drPlan[rptSet.HeaderText] = arr[0];
                    else if (arr.Length > 1)
                        drPlan[rptSet.HeaderText] = string.Join("; ", arr.ToList().Select(x => x.Trim()).Distinct().ToArray());
                }
            }
        }

        /// <summary>
        /// 设置行指定列的值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        private void SetRowValue(DataRow row, string columnName, string columnValue)
        {
            if (row[columnName] == DBNull.Value)
                row[columnName] = columnValue;
            else
                row[columnName] = row[columnName].ToString() + "; " + columnValue;
        }

    }

    #region sub class
    /// <summary>
    /// 计划产量、实际产量
    /// </summary>
    public class DailyProdAmount
    {
        public int LineID { get; set; }
        public int ProdEventID { get; set; }
        public int POID { get; set; }
        public DateTime ProdDate { get; set; }
        public int Amount { get; set; }

        public string PoCode { get; set; }

        /*Ben 添加是否有为0的记录，因为显示方式不同，所以要添加属性记录*/
        public bool HasZeroAmount { get; set; }
    }

    /// <summary>
    /// 生产线达成率
    /// </summary>
    public class LineRate
    {
        public int LineID { get; set; }
        /// <summary>
        /// 三位小数
        /// </summary>
        public double CompleteRate { get; set; }
        /// <summary>
        /// 车间名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string FacilityName { get; set; }
    }

    #endregion

    /// <summary>
    /// 排序类别
    /// </summary>
    public enum SortOrders
    {
        /// <summary>
        /// 按名次
        /// </summary>
        ByRanking,
        /// <summary>
        /// 按生产线
        /// </summary>
        ByProductionLine
    }
}