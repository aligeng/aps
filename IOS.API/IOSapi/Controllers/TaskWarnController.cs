using AMO.Model;
using IOS.DBUtility;
using IOSapi.BLL;
using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 提醒通知
    /// </summary>
    public class TaskWarnController : BaseApiController
    {
        APSEntities db = new APSEntities();

        /// <summary>
        /// 跨域处理
        /// </summary>
        /// <returns></returns>
        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }

        #region 异常事件提醒

        /// <summary>
        /// 获取N天设置参数
        /// </summary>
        /// <returns></returns>
         [HttpGet, Route("api/TaskWarn/GetAlertDays")]
        public Parameter GetAlertDays()
        {
            Parameter p = new Parameter();
            p.name = "N天设置参数值";
            p.value = AMOData.Settings.SysSetting.JA_JobAlertDays.ToString();

            return p;
        }


        #region 统计类型个数
        /// <summary>
        /// 获取异常事件提醒的统计数据
        /// </summary>
        /// <param name="jobCodes">对应jobcode,比如: 'NotPlan,NewAdd,PlanChange'</param>
        /// <param name="allowFactoryIDs">用户可访问工厂IDs，比如：'1,2,3'</param>
        /// <param name="jobAlertDays">对应参数JA_JobAlertDays</param>
        /// <param name="ottoffset">对应参数PoOTTOffset</param>
        /// <param name="notmaintaindays">对应参数ScheduleNotMaintain</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetUserDelayAlertCountList")]
        private List<WarnData> GetUserDelayAlertCountList(string jobCodes, string allowFactoryIDs, int jobAlertDays, int ottoffset, int notmaintaindays)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {   
                                                              new System.Data.SqlClient.SqlParameter("@p_jobkeys",jobCodes),  
                                                              new System.Data.SqlClient.SqlParameter("@p_factoryids",allowFactoryIDs),  
                                                              new System.Data.SqlClient.SqlParameter("@p_ottoffset", ottoffset),
                                                              new System.Data.SqlClient.SqlParameter("@p_jobalertdays", jobAlertDays),
                                                              new System.Data.SqlClient.SqlParameter("@p_notmaintaindays", notmaintaindays),
                                                              new System.Data.SqlClient.SqlParameter("@p_currentdate",DateTime.Now)  
                                                              };

            var ds = this.db.Database.SqlQuery<WarnData>("exec app_gen_jobalert_delay @p_jobkeys,@p_factoryids,@p_ottoffset,@p_jobalertdays,@p_notmaintaindays,@p_currentdate ", parameters).ToList();

            return ds;
        }


        /// <summary>
        /// 获取 异常事件提醒的统计数据
        /// </summary>
        /// <returns> 
        /// 返回数据说明：
        /// DeliveryDelay:生产单交期延误个数
        /// ProgressDelay :生产计划进度落后个数
        /// StartDelay 生产计划开工期延后个数
        /// MatDelay 物料未按生产计划到货个数
        /// EventDelay 关键事件未按生产计划完成个数
        /// ScheduleNotMaintain  n天内未维护生产日进度的生产计划个数
        /// </returns>
        [HttpGet, Route("api/TaskWarn/GetUserDelayAlertCount")]
        public ViewModels.UserDelayCount GetUserDelayAlertCount()
        {
            string jobCodes = "DeliveryDelay,StartDelay,MatDelay,EventDelay,ProcessDelay";
            string allowFactoryIDs = new TaskWarn().GetAllowedFactory();
            if (string.IsNullOrEmpty(allowFactoryIDs))
                return null;


            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            int notmaintaindays = AMOData.Settings.SysSetting.JA_ScheduleNotMaintainDays;
            int ottoffset = AMOData.Settings.SysSetting.PoOTTOffset;

            DataTable dt = AMOData.JobAlertNew.GetUserDelayAlertCount(jobCodes, allowFactoryIDs, jobAlertDays, ottoffset, notmaintaindays);
            long tick = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                //计算 生产计划开工期延误
                if (jobCodes.Contains("StartDelay") && dt.Columns.Contains("StartDelay"))
                {
                    try
                    {
                        tick = DateTime.Now.Ticks;
                        var ls = CalcStartDelay(allowFactoryIDs);
                        tick = DateTime.Now.Ticks - tick;
                        int startDelayCount = ls == null ? 0 : ls.Count;
                        dt.Rows[0]["StartDelay"] = startDelayCount;
                    }
                    catch
                    {
                        dt.Rows[0]["StartDelay"] = -1;
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ViewModels.UserDelayCount model = new ViewModels.UserDelayCount();
                model.DeliveryDelay = dt.Rows[0]["DeliveryDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["DeliveryDelay"]);
                model.EventDelay = dt.Rows[0]["EventDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["EventDelay"]);
                model.MatDelay = dt.Rows[0]["MatDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["MatDelay"]);
                model.ProcessDelay = dt.Rows[0]["ProcessDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ProcessDelay"]);
                model.StartDelay = dt.Rows[0]["StartDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["StartDelay"]);

                //model.ProgressDelay = dt.Rows[0]["ProgressDelay"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ProgressDelay"]);
                //model.ScheduleNotMaintain = dt.Rows[0]["ScheduleNotMaintain"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ScheduleNotMaintain"]);
                //DateTime pdate = DateTime.Now.AddDays(-notmaintaindays);
                //var count = db.Processdailydata_ext.Where(c => c.Processdate > pdate && c.Type == 2 && (c.Troubletype == 2 || c.Troubletype == 3)).Count();
                //model.ProcessDelay = count;
                return model;
            }

            return null;
        }

        #endregion

        #region 明细

        ///// <summary>
        ///// 生产单交期延误数据集
        ///// </summary>
        ///// <param name="priority">优先级,-1全部,6-普通，8-重要,10-紧急</param>
        ///// <param name="code">模糊搜索：生产单号或款号</param>
        ///// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        ///// <param name="pageIndex">第几页</param>
        ///// <param name="pageSize">分页大小</param>
        ///// <returns></returns>
        //public List<Po> GetPoesDeliveryDelay(int pageIndex, int pageSize, int priority = -1, string code = "", string fids = "")
        //{
        //    List<int> fidLs = new List<int>();
        //    if (!string.IsNullOrEmpty(fids))
        //    {
        //        string[] fidsArry = fids.Split(',');
        //        foreach (var item in fidsArry)
        //        {
        //            fidLs.Add(Convert.ToInt32(item));
        //        }
        //    }
        //    else
        //    {
        //        int[] fidsArry = GetAllowedFactoryIDs();
        //        foreach (var item in fidsArry)
        //        {
        //            fidLs.Add(item);
        //        }
        //    }


        //    var ls = db.POes.Where(c => string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code) && fidLs.Contains(c.FACTORYID.Value) && 
        //    priority < 0 ? true : c.PRIORITY == priority).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
        //    var datas = ls.Select(p => new ViewModels.Po
        //    {
        //        id = p.ID,
        //        code = p.CODE,
        //        customerid = p.CUSTOMERID.Value,
        //        customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
        //        deliverydate = p.DELIVERYDATE,
        //        amount = p.AMOUNT,
        //        pattern = p.PATTERN,
        //        priority = p.PRIORITY.Value,
        //        producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
        //        merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME
        //    });
        //    return datas.ToList();
        //}


        /// <summary>
        /// {0}个生产计划交期延误 (明细数据)
        /// </summary>
        /// <param name="priority">优先级,-1全部,6-普通，8-重要,10-紧急</param>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailDeliveryDelay")]
        public List<JaDeliveryDelay> GetDetailDeliveryDelay(int pageIndex, int pageSize, int priority = -1, string code = "", string fids = "")
        {
            List<JaDeliveryDelay> ls = new List<ViewModels.JaDeliveryDelay>();

            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }
            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            DataTable dt = TaskWarn.GetDetailDeliveryDelay(fids, jobAlertDays);//, factoryviewDays
            ls = dt.AsEnumerable().Where(c => (priority < 0 ? true : c.Field<int>("Priority") == priority) &&
            (string.IsNullOrEmpty(code) ? true : (c.Field<string>("PoCode").Contains(code)) || c.Field<string>("Pattern").Contains(code))).OrderByDescending(c =>
            c.Field<int>("DelayDays")).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).Select(p =>
            new JaDeliveryDelay()
            {
                StartTime = p.Field<DateTime>("StartTime") == null ? "" : p.Field<DateTime>("StartTime").ToString("yyyy-MM-dd"),
                FacilityName = p.Field<string>("FacilityName"),
                FactoryID = p.Field<int>("FactoryID"),
                PoCode = p.Field<string>("PoCode"),
                CustomerName = p.Field<string>("CustomerName"),
                CustomerStyleNO = p.Field<string>("CustomerStyleNO"),
                Pattern = p.Field<string>("Pattern"),
                ProductTypeName = p.Field<string>("ProductTypeName"),
                DeliveryDate = p.Field<DateTime>("DeliveryDate") == null ? "" : p.Field<DateTime>("DeliveryDate").ToString("yyyy-MM-dd"),
                FactoryDelivery = p.Field<DateTime>("FactoryDelivery") == null ? "" : p.Field<DateTime>("FactoryDelivery").ToString("yyyy-MM-dd"),
                Merchandiser = p.Field<string>("Merchandiser"),
                Description = p.Field<string>("Description"),
                DelayDays = p.Field<int>("DelayDays"),
                PlanAmount = p.Field<double>("PlanAmount"),
                Amount = p.Field<double>("Amount"),
                Priority = p.Field<int>("Priority"),
                DelayGrade = p.Field<int>("DelayDays") < AMOData.Settings.UserSetting.HardDelayDays ? 1 : 2,
                CAOTD = p.Field<DateTime>("CAOTD"),
                EarlinessStartDate = p.Field<DateTime>("EARLINESSSTARTDATE"),
                InitDeliveryDate = p.Field<DateTime>("INITDELIVERYDATE"),
                MainMaterielArriveDate = p.Field<DateTime>("MASTERMATERIALRECEIVEDDATE"),
                Capacity = p.Field<double>("CAPACITY"),
            }).ToList();

            return ls;

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        JaDeliveryDelay model = new ViewModels.JaDeliveryDelay();
            //        model.StartTime = row["StartTime"].ToString();
            //        model.FacilityName = row["FacilityName"].ToString();
            //        model.FactoryID = row["FactoryID"].ToString();
            //        model.PoCode = row["PoCode"].ToString();
            //        model.CustomerName = row["CustomerName"].ToString();
            //        model.CustomerStyleNO = row["CustomerStyleNO"].ToString();
            //        model.Pattern = row["Pattern"].ToString();
            //        model.ProductTypeName = row["ProductTypeName"].ToString();
            //        model.DeliveryDate = row["DeliveryDate"].ToString();
            //        model.FactoryDelivery = row["FactoryDelivery"].ToString();
            //        model.Merchandiser = row["Merchandiser"].ToString();
            //        model.Description = row["Description"].ToString();
            //        model.DelayDays = row["DelayDays"].ToString();
            //        model.PlanAmount = row["PlanAmount"].ToString();
            //        model.Amount = row["Amount"].ToString();
            //        ls.Add(model);
            //    }
            //}
            //return ls;

        }

        /// <summary>
        /// 工序延误明细（监控中心的工序延误一致）
        /// </summary>
        /// <param name="processId">工艺节点id,-1全部</param>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailProcessDelay")]
        public List<JaProcessDelay> GetDetailProcessDelay(int pageIndex, int pageSize, int processId = -1, string code = "", string fids = "")
        {
            List<JaProcessDelay> ls = new List<ViewModels.JaProcessDelay>();

            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }
            string[] fidsArry = fids.Split(',');
            List<int> fidLs = new List<int>();
            foreach (var item in fidsArry)
            {
                fidLs.Add(Convert.ToInt32(item));
            }

            var query = from a in db.Processdailydata_ext.Where(c => (processId > -1 ? c.Processid == processId : true) && c.Type == 2 &&
                        (c.Troubletype == 2 || c.Troubletype == 3) && c.Processdate.Value == db.Processdailydata_ext.Max(q => q.Processdate) && fidLs.Contains(c.factoryid.Value))
                        join b in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code)))) on a.Poid equals b.ID
                        //join d in db.FACILITies on b.FACTORYID equals d.ID
                        //join e in db.CUSTOMERs on b.CUSTOMERID equals e.ID
                        select new JaProcessDelay()
                        {
                            Dayqty = a.Dayqty,
                            Duration = a.Duration,
                            EndDate = a.Enddate,
                            Factoryid = a.factoryid,
                            CustomerName = b.CUSTOMER.NAME,
                            LineName = b.FACTORY.NAME,
                            Planduration = a.Planduration,
                            Planqty = a.planqty,
                            PoCode = b.CODE,
                            Pattern = b.PATTERN,
                            CustomerStyleNO = b.CUSTOMERSTYLENO,
                            PoId = a.Poid.Value,
                            ProcessDate = a.Processdate,
                            ProcessId = a.Processid.Value,
                            ProcessName = a.Processname,
                            Schedulepercent = a.Schedulepercent,
                            StartDate = a.Startdate,
                            Totalqty = a.Totalqty,
                            Troubletype = a.Troubletype,
                            DeliveryDate = b.DELIVERYDATE,
                            DelayDays = EntityFunctions.DiffDays(a.Enddate.Value, DateTime.Now).Value,
                            Capacity = b.CAPACITY.Value,
                            EarlinessStartDate = b.EARLINESSSTARTDATE,
                            MainMaterielArriveDate = b.MASTERMATERIALRECEIVEDDATE,
                            MaterielArriveDate=b.MATERIALRECEIVEDDATE,
                            ProductTypeName = b.PRODUCTTYPE.NAME,
                            RouteName = b.PROCESSROUTE.NAME
                        };

            ls = query.OrderByDescending(c => c.DelayDays).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

            return ls;

        }

        /// <summary>
        /// {0}个生产计划进度落后 (明细数据)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailProgressDelay")]//添加方法修饰属性返回类型说明
        private List<IOSapi.ViewModels.JaProgressDelay> GetDetailProgressDelay()
        {
            string factoryIDs = new TaskWarn().GetAllowedFactory();

            List<IOSapi.ViewModels.JaProgressDelay> lstProgressDelay = new List<IOSapi.ViewModels.JaProgressDelay>();

            long tick = DateTime.Now.Ticks;
            DateTime dYesterday = AMOData.AMOTime.Now.AddDays(-1).Date;

            //ProductionEvent
            Filter filFacility = new Filter();
            filFacility.Add("State", 0, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);//生产线在用
            filFacility.Add("FactoryID", factoryIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            string distFacilityIDs = DBHelper.GetDistinctSql("Facility", "ID", filFacility);

            Filter filter = new Filter();
            filter.Add("Status", 1, RELEATTION_TYPE.NOTEQUAL, LOGIC_TYPE.AND);//计划未完成
            filter.Add("StartTime", dYesterday.Date.AddDays(1), RELEATTION_TYPE.LESS, LOGIC_TYPE.AND);
            filter.Add("FacilityID", distFacilityIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filter.Add("POID", "(select ID from PO WHERE STATUS=0)", RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<AMOData.ProductionEvent> lstAllProdEvent = AMOData.ProductionEvent.GetList(filter);

            //ProductionEventDetail
            string distProdEventIDs = DBHelper.GetDistinctSql("ProductionEvent", "ID", filter);
            Filter filDetail = new Filter();
            filDetail.Add("ProductionEventID", distProdEventIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<AMOData.ProductionEventDetail> lstAllProdDetail = AMOData.ProductionEventDetail.GetList(filDetail);
            if (lstAllProdEvent.Count == 0 || lstAllProdDetail.Count == 0)
            {
                return lstProgressDelay;
            }

            //ProductionSchedule
            int primaryProcessID = AMO.BLL.Process.GetPrimaryProcessID();
            Filter filSchedule = new Filter();
            filSchedule.Add("ProductionEventID", distProdEventIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filSchedule.Add("ProcessID", primaryProcessID, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            List<AMOData.ProductionSchedule> lstAllSchedule = AMOData.ProductionSchedule.GetList(filSchedule);

            //PO
            string distinctPOID = DBHelper.GetDistinctSql("ProductionEventDetail", "POID", filDetail);
            Filter filterPO = new Filter();
            filterPO.Add("ID", distinctPOID, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            Dictionary<int, AMOData.PO> dicPO = AMOData.PO.GetDictionary(filterPO);

            Dictionary<int, AMOData.Customer> dicCustomer = AMOData.Customer.GetDictionary(null);
            Dictionary<int, AMOData.ProductType> dicProductType = AMOData.ProductType.GetDictionary(null);
            Dictionary<int, AMOData.Users> dicUser = AMOData.Users.GetDictionary(null);
            Dictionary<int, AMOData.Facility> dicFacility = AMOData.Facility.GetDictionary(null);
            tick = DateTime.Now.Ticks - tick;
            //VirtualProdEvent
            tick = DateTime.Now.Ticks;
            List<AMOData.VirtualProdEvent> lstVirProdEvent = new List<AMOData.VirtualProdEvent>();
            //foreach (AMOData.ProductionEvent pe in lstAllProdEvent)
            //{
            //    AMOData.VirtualProdEvent vp = new AMOData.VirtualProdEvent(pe, DateTime.Now);
            //    List<AMOData.ProductionEventDetail> lstDetail = lstAllProdDetail.Where(x => x.ProductionEventID == pe.ID).ToList();
            //    foreach (AMOData.ProductionEventDetail xDetail in lstDetail)
            //        vp.AddDetail(new AMOData.VirtualProdEventDetail(xDetail, "", "", ""));
            //    vp.FactoryID = (dicFacility.ContainsKey(vp.FacilityID)) ? dicFacility[vp.FacilityID].FactoryID : -1;
            //    lstVirProdEvent.Add(vp);
            //}

            var pedGroups = lstAllProdDetail.GroupBy(ped => ped.ProductionEventID).ToList().ToDictionary(x => x.Key);
            //var dicProd = lstAllProdEvent.ToDictionary(x => x.ID);
            foreach (AMOData.ProductionEvent pe in lstAllProdEvent)
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
                vp.FactoryID = (dicFacility.ContainsKey(vp.FacilityID)) ? dicFacility[vp.FacilityID].FactoryID : -1;
                lstVirProdEvent.Add(vp);
            }


            tick = DateTime.Now.Ticks - tick;
            //get plan progress info
            AMO.BLL.CalculatePlanSchedule calcObj = new AMO.BLL.CalculatePlanSchedule(lstVirProdEvent);
            Dictionary<int, Dictionary<DateTime, int>> dicPlanProgress = calcObj.GetBatchPlanProgress();

            tick = DateTime.Now.Ticks;
            //double delayWarn = AMOData.Settings.SysSetting.JA_ProgressDelayPercent;// 0.1;
            double delayWarn = 0.8;// 0.1;
            var dicSchedule = lstAllSchedule.Where(x => x.ProductionDate.Date <= dYesterday.Date).ToList().GroupBy(x => x.ProductionEventID).ToDictionary(x => x.Key);
            foreach (AMOData.VirtualProdEvent virPE in lstVirProdEvent)
            {
                int peid = virPE.ID;// in dicPlanProgress.Keys
                if (!dicPlanProgress.ContainsKey(peid))
                    continue;

                double planShouldFinish = 0, actualFinish = 0;

                var q = (from p in dicPlanProgress[peid].ToList()
                         where p.Key.Date <= dYesterday.Date
                         select p.Value).ToList();
                if (q.Count() > 0)
                    planShouldFinish = q.Sum();//计划应完成数

                if (planShouldFinish <= 0)
                    continue;

                List<AMOData.ProductionSchedule> qSchedule = (dicSchedule.ContainsKey(peid)) ? dicSchedule[peid].ToList() : new List<AMOData.ProductionSchedule>();// lstAllSchedule.Where(x => x.ProductionEventID == peid && x.ProductionDate.Date <= dYesterday.Date).ToList();
                if (qSchedule.Count() > 0)
                    actualFinish = qSchedule.Sum(x => x.Amount);//实际完成数

                if (actualFinish < planShouldFinish)
                {
                    double actualProgress = 0, progressDelay = 0;
                    actualProgress = actualFinish / planShouldFinish;
                    progressDelay = 1 - actualProgress;
                    if (progressDelay >= delayWarn)
                    {
                        IOSapi.ViewModels.JaProgressDelay ja = new IOSapi.ViewModels.JaProgressDelay();
                        ja.ProductionEventID = peid;
                        ja.PlanShouldFinish = planShouldFinish;
                        ja.ActualFinish = actualFinish;
                        ja.PlanAmount = virPE.PlanAmount;//
                        ja.PlanStart = virPE.StartTime;
                        ja.ActualStart = qSchedule.Count() > 0 ? qSchedule.Min(x => x.ProductionDate).Date : DateTime.MinValue;
                        ja.ActualProgress = actualProgress.ToString("P0");
                        ja.ProgressDelay = progressDelay.ToString("P0");
                        ja.LineName = dicFacility.ContainsKey(virPE.FacilityID) ? dicFacility[virPE.FacilityID].Name : "";
                        ja.FactoryID = dicFacility.ContainsKey(virPE.FacilityID) ? dicFacility[virPE.FacilityID].FactoryID : 0;

                        #region append po info

                        ja.PoCode = "";
                        ja.CustomerStyleNO = "";
                        ja.Pattern = "";
                        ja.DeliveryDate = "";
                        ja.Amount = "";
                        ja.CustomerName = "";
                        ja.ProductTypeName = "";
                        ja.Merchandiser = "";


                        List<int> lstPOID = (from p in virPE.AllDetails select p.POID).Distinct().ToList();
                        foreach (int poid in lstPOID)
                        {

                            if (dicPO.ContainsKey(poid))//生产单
                            {
                                ja.PoCode += dicPO[poid].Code + ", ";
                                ja.CustomerStyleNO += dicPO[poid].CustomerStyleNo + ", ";
                                ja.Pattern += dicPO[poid].Pattern + ", ";
                                ja.DeliveryDate += dicPO[poid].DeliveryDate.ToString("yyyy-MM-dd") + ", ";
                                ja.Amount += dicPO[poid].Amount.ToString() + ", ";
                                ja.CustomerName += (dicCustomer.ContainsKey(dicPO[poid].CustomerID) ? dicCustomer[dicPO[poid].CustomerID].Name : "") + ", ";
                                ja.ProductTypeName += (dicProductType.ContainsKey(dicPO[poid].ProductTypeID) ? dicProductType[dicPO[poid].ProductTypeID].Name : "") + ", ";
                                ja.Merchandiser += (dicUser.ContainsKey(dicPO[poid].Merchandiser) ? dicUser[dicPO[poid].Merchandiser].UserName : "") + ", ";
                            }
                        }

                        ja.PoCode = ProcessDuplication(ja.PoCode);
                        ja.CustomerStyleNO = ProcessDuplication(ja.CustomerStyleNO);
                        ja.Pattern = ProcessDuplication(ja.Pattern);
                        ja.DeliveryDate = ProcessDuplication(ja.DeliveryDate);
                        //ja.Amount = ProcessDuplication(ja.Amount);
                        if (ja.Amount.Length > 0)
                            ja.Amount = ja.Amount.TrimEnd(' ').TrimEnd(',');

                        ja.CustomerName = ProcessDuplication(ja.CustomerName);
                        ja.ProductTypeName = ProcessDuplication(ja.ProductTypeName);
                        ja.Merchandiser = ProcessDuplication(ja.Merchandiser);
                        #endregion

                        lstProgressDelay.Add(ja);
                    }

                }//进度落后
            }//foreach
            tick = DateTime.Now.Ticks - tick;

            return lstProgressDelay;
        }

        /// <summary>
        /// 开工期延误 计算
        /// </summary>
        /// <param name="factoryIDs">工厂IDs</param>
        private List<IOSapi.ViewModels.JaStartDelay> CalcStartDelay(string factoryIDs)
        {

            List<IOSapi.ViewModels.JaStartDelay> lst = new List<IOSapi.ViewModels.JaStartDelay>();

            //所有未完成的生产计划
            Filter filFacility = new Filter();







            filFacility.Add("FactoryID", factoryIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            string distFacilitySQL = DBHelper.GetDistinctSql("Facility", "ID", filFacility);

            Filter filter = new Filter();
            filter.Add("Status", 0, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            filter.Add("FacilityID", distFacilitySQL, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filter.Add("POID", "(select ID from PO WHERE STATUS=0)", RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<AMOData.ProductionEvent> lstPE = AMOData.ProductionEvent.GetList(filter);

            Dictionary<int, AMOData.Facility> dicFacility = AMOData.Facility.GetDictionary(null);

            //计算计划 最早可开工期
            AMO.Logic.EarliestStart es = new AMO.Logic.EarliestStart(lstPE);
            Dictionary<int, DateTime> dicES = es.CacuESFromProdEvent();
            foreach (AMOData.ProductionEvent pe in lstPE)
            {
                //最早可开工期>计划开始日期
                DateTime dateES = dicES.ContainsKey(pe.ID) ? dicES[pe.ID] : DateTime.MinValue;
                if (dateES > pe.StartTime.Date)
                {
                    IOSapi.ViewModels.JaStartDelay ja = new IOSapi.ViewModels.JaStartDelay();
                    ja.ProductionEventID = pe.ID;
                    ja.PlanStart = pe.StartTime;
                    ja.EarliestStartDate = dateES;
                    ja.DelayDays = Convert.ToInt32((dateES.Date - pe.StartTime.Date).TotalDays);
                    ja.LineName = dicFacility.ContainsKey(pe.FacilityID) ? dicFacility[pe.FacilityID].Name : "";
                    ja.FactoryID = dicFacility.ContainsKey(pe.FacilityID) ? dicFacility[pe.FacilityID].FactoryID : -1;
                    //其他取数放在弹出窗口处理
                    lst.Add(ja);
                }
            }

            return lst;

        }

        /// <summary>
        /// {0}个生产计划开工期延误 (明细数据)
        /// </summary>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailStartDelay")]
        public List<IOSapi.ViewModels.JaStartDelay> GetDetailStartDelay(int pageIndex, int pageSize, string code = "", string fids = "")
        {
            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }

            List<IOSapi.ViewModels.JaStartDelay> LstJaStartDelay = CalcStartDelay(fids);
            if (LstJaStartDelay == null || LstJaStartDelay.Count == 0)
                return new List<ViewModels.JaStartDelay>();

            string distinctProdEventID = string.Join(",", (from p in LstJaStartDelay select p.ProductionEventID.ToString()).ToArray());
            List<AMO.Model.PlanPOMapping> lstPlanPO = AMOData.JobAlertNew.GetPlanPOMapping(distinctProdEventID);

            //Filter filter = new Filter();
            //filter.Add("ProductionEventID", distinctProdEventID, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            Filter cf = AMOData.AMODataHelper.GetFilterFromString("ProductionEventID", distinctProdEventID, 800);

            string distinctPOID = DBHelper.GetDistinctSql("ProductionEventDetail", "POID", cf);

            Filter filterPO = new Filter();
            filterPO.Add("ID", distinctPOID, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            Dictionary<int, AMOData.PO> dicPO = AMOData.PO.GetDictionary(filterPO);

            Dictionary<int, AMOData.Customer> dicCustomer = AMOData.Customer.GetDictionary(null);
            Dictionary<int, AMOData.ProductType> dicProductType = AMOData.ProductType.GetDictionary(null);
            Dictionary<int, AMOData.Users> dicUser = AMOData.Users.GetDictionary(null);

            foreach (IOSapi.ViewModels.JaStartDelay ja in LstJaStartDelay)
            {
                ja.PoCode = "";
                ja.CustomerStyleNO = "";
                ja.Pattern = "";
                //ja.LineName = "";
                ja.DeliveryDate = "";
                ja.Amount = "";
                ja.CustomerName = "";
                ja.ProductTypeName = "";
                ja.Merchandiser = "";

                ja.MasterMatFinishDate = "";
                ja.NotMasterMatFinishDate = "";
                ja.PreProductionEventDate = "";

                List<AMO.Model.PlanPOMapping> lstFilterPlanPO = lstPlanPO.Where(x => x.ProductionEventID == ja.ProductionEventID).ToList();
                foreach (AMO.Model.PlanPOMapping pp in lstFilterPlanPO)
                {
                    if (dicPO.ContainsKey(pp.POID))
                    {
                        ja.PoCode += dicPO[pp.POID].Code + ", ";
                        ja.CustomerStyleNO += dicPO[pp.POID].CustomerStyleNo + ", ";
                        ja.Pattern += dicPO[pp.POID].Pattern + ", ";
                        //ja.LineName += dicFacility[pp.].Name + ", ";

                        ja.DeliveryDate += dicPO[pp.POID].DeliveryDate.ToString("yyyy-MM-dd") + ", ";
                        ja.Amount += dicPO[pp.POID].Amount.ToString() + ", ";
                        ja.CustomerName += (dicCustomer.ContainsKey(dicPO[pp.POID].CustomerID) ? dicCustomer[dicPO[pp.POID].CustomerID].Name : "") + ", ";
                        ja.ProductTypeName += (dicProductType.ContainsKey(dicPO[pp.POID].ProductTypeID) ? dicProductType[dicPO[pp.POID].ProductTypeID].Name : "") + ", ";
                        ja.Merchandiser += (dicUser.ContainsKey(dicPO[pp.POID].Merchandiser) ? dicUser[dicPO[pp.POID].Merchandiser].UserName : "") + ", ";

                        ja.MasterMatFinishDate += dicPO[pp.POID].MasterMaterialReceivedDate.ToString("yyyy-MM-dd") + ", ";
                        ja.NotMasterMatFinishDate += dicPO[pp.POID].MaterialReceivedDate.ToString("yyyy-MM-dd") + ", ";
                        ja.PreProductionEventDate += dicPO[pp.POID].PreProductionEventDate.ToString("yyyy-MM-dd") + ", ";
                    }
                }

                ja.PoCode = ProcessDuplication(ja.PoCode);
                ja.CustomerStyleNO = ProcessDuplication(ja.CustomerStyleNO);
                ja.Pattern = ProcessDuplication(ja.Pattern);
                //ja.LineName = ProcessDuplication(ja.LineName);
                ja.DeliveryDate = ProcessDuplication(ja.DeliveryDate);
                //ja.Amount = ProcessDuplication(ja.Amount);
                if (ja.Amount.Length > 0)
                    ja.Amount = ja.Amount.TrimEnd(' ').TrimEnd(',');

                ja.CustomerName = ProcessDuplication(ja.CustomerName);
                ja.ProductTypeName = ProcessDuplication(ja.ProductTypeName);
                ja.Merchandiser = ProcessDuplication(ja.Merchandiser);

                ja.MasterMatFinishDate =  Convert.ToDateTime( ja.MasterMatFinishDate) <new DateTime(2000,01,01)? "" : ProcessDuplication(ja.MasterMatFinishDate);
                ja.NotMasterMatFinishDate = Convert.ToDateTime(ja.NotMasterMatFinishDate) < new DateTime(2000, 01, 01) ? "" : ProcessDuplication(ja.NotMasterMatFinishDate);
                ja.PreProductionEventDate = Convert.ToDateTime(ja.PreProductionEventDate) < new DateTime(2000, 01, 01) ? "" : ProcessDuplication(ja.PreProductionEventDate);
            }

            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }
            string[] fidsArry = fids.Split(',');
            List<int> fidLs = new List<int>();
            foreach (var item in fidsArry)
            {
                fidLs.Add(Convert.ToInt32(item));
            }

            var ls = LstJaStartDelay.Where(c => fidLs.Contains(c.FactoryID) &&
            string.IsNullOrEmpty(code) ? true : (c.PoCode.Contains(code) || c.Pattern.Contains(code))).OrderByDescending(c =>
            c.DelayDays).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();


            return ls;
        }


        /// <summary>
        /// {0}个关键事件延误明细数据（与监控中心的关键事件延误一致）
        /// </summary>
        /// <param name="eventId">工艺节点id,-1全部</param>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailEventDelay")]
        public List<JaEventDelay> GetDetailEventDelay(int pageIndex, int pageSize, int eventId = -1, string code = "", string fids = "")
        {
            #region 与监控中心一致

            List<JaEventDelay> ls = new List<ViewModels.JaEventDelay>();
            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }
            string[] fidsArry = fids.Split(',');
            List<int> fidLs = new List<int>();
            foreach (var item in fidsArry)
            {
                fidLs.Add(Convert.ToInt32(item));
            }

            DateTime oTime = new DateTime(2000, 01, 01);
            var query = from a in db.Processdailydata_ext.Where(c => (eventId > -1 ? c.Processid == eventId : true) && c.Type == 1 &&
                        (c.Troubletype == 2 || c.Troubletype == 3) && c.Processdate.Value == db.Processdailydata_ext.Max(q => q.Processdate) && fidLs.Contains(c.factoryid.Value))
                        join b in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code)))) on a.Poid equals b.ID
                        //join d in db.FACILITies on b.FACTORYID equals d.ID
                        //join e in db.CUSTOMERs on b.CUSTOMERID equals e.ID
                        select new JaEventDelay()
                        {
                            FactoryID = a.factoryid.Value,
                            PoCode = b.CODE,
                            CustomerName = b.CUSTOMER.NAME,
                            CustomerStyleNO = b.CUSTOMERSTYLENO,
                            Pattern = b.PATTERN,
                            ProductTypeName = b.PRODUCTTYPE.NAME,
                            DeliveryDate = b.DELIVERYDATE,
                            EventId = a.Processid.Value,
                            EventName = a.Processname,
                            EventMan=  (from o in db.CRITICALEVENTMen.Where(c=>(c.FACTORYID==a.factoryid || c.FACTORYID==-1) && c.CUSTOMERID==b.CUSTOMERID && c.CRITICALEVENTID== a.Processid) join p in  db.USERS on o.USERID equals p.ID orderby o.FACTORYID descending select p.USERNAME).FirstOrDefault(),
                            PlanStarDate=a.Startdate,
                            PlanFinishDate = a.Enddate,
                            Merchandiser = b.MERCHANDISER == null ? "" : db.USERS.Where(q => q.ID == b.MERCHANDISER.Value).Select(k => k.USERNAME).FirstOrDefault(),
                            DelayDays = a.Enddate > oTime ? EntityFunctions.DiffDays(a.Enddate.Value, DateTime.Now).Value : EntityFunctions.DiffDays(a.Startdate.Value, DateTime.Now).Value
                        };

            ls = query.OrderByDescending(c => c.DelayDays).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

            return ls;


            #endregion




            //#region 与APS提醒一致

            //List<JaEventDelay> ls = new List<ViewModels.JaEventDelay>();
            //if (string.IsNullOrEmpty(fids))
            //{
            //    fids = new TaskWarn().GetAllowedFactory();
            //}
            //DataTable dt = TaskWarn.GetDetailEventDelay(fids);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        JaEventDelay model = new ViewModels.JaEventDelay();

            //        model.FactoryID = row["FactoryID"].ToString();
            //        model.PoCode = row["PoCode"].ToString();
            //        model.CustomerName = row["CustomerName"].ToString();
            //        model.CustomerStyleNO = row["CustomerStyleNO"].ToString();
            //        model.Pattern = row["Pattern"].ToString();
            //        model.ProductTypeName = row["ProductTypeName"].ToString();
            //        model.DeliveryDate = row["DeliveryDate"].ToString();
            //        model.PlanStart = row["PlanStart"].ToString();
            //        model.EventId = Convert.ToInt32(row["EventId"]);
            //        model.EventName = row["EventName"].ToString();
            //        model.ExpectFinishDate = row["ExpectFinishDate"] == null ? "" : Convert.ToDateTime(row["ExpectFinishDate"]).ToString("yyyy-MM-dd");
            //        model.DelayDays = row["DelayDays"].ToString();
            //        model.Merchandiser = row["Merchandiser"].ToString();
            //        model.CriticalEventManID = row["CriticalEventManID"].ToString();
            //        ls.Add(model);
            //    }
            //}
            //return ls.Where(c => (eventId > -1 ? c.EventId == eventId : true) && (string.IsNullOrEmpty(code) ? true : (c.PoCode.Contains(code) || c.Pattern.Contains(c.Pattern)))).OrderByDescending(c =>
            //             c.DelayDays).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();



            //#endregion

        }


        /// <summary>
        /// {0}个物料未按生产计划到货 (明细数据)
        /// </summary>
        /// <param name="type">布料类型：-1全部；0辅料；1面料</param>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailMatDelay")]
        public List<JaDetailMatDelay> GetDetailMatDelay(int pageIndex, int pageSize, int type = -1, string code = "", string fids = "")
        {
            List<JaDetailMatDelay> ls = new List<ViewModels.JaDetailMatDelay>();

            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }

            int poOTTOffset = AMOData.Settings.SysSetting.PoOTTOffset;
            DataTable dt = TaskWarn.GetDetailMatDelay(type, fids, poOTTOffset);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    JaDetailMatDelay model = new ViewModels.JaDetailMatDelay();
                    model.POMaterialRequestID = row["POMaterialRequestID"].ToString();
                    model.MPNO = row["MPNO"].ToString();
                    model.Supplier = row["Supplier"].ToString();
                    model.MPDate =Convert.ToDateTime(row["MPDate"]).ToString("yyyy-MM-dd");
                    model.ExpectedReciveDate = row["ExpectedReciveDate"] == DBNull.Value ? "" : Convert.ToDateTime(row["ExpectedReciveDate"]).ToString("yyyy-MM-dd");
                    model.MaterialCode = row["MaterialCode"].ToString();
                    model.MaterialName = row["MaterialName"].ToString();
                    model.MaterialColor = row["MaterialColor"].ToString();
                    model.MaterialSize = row["MaterialSize"].ToString();
                    model.Unit = row["Unit"].ToString();
                    model.Amount = row["Amount"].ToString();
                    model.NeedDate = Convert.ToDateTime(row["NeedDate"]).ToString("yyyy-MM-dd");
                    model.DelayDays = row["DelayDays"].ToString();
                    model.MaterialStyle = row["MaterialStyle"].ToString();
                    model.PoCode = row["PoCode"].ToString();
                    model.Pattern = row["Pattern"].ToString();
                    model.CustomerStyleNo = row["CustomerStyleNo"].ToString();
                    model.CustomerName = row["CustomerName"].ToString();
                    
                    ls.Add(model);
                }
            }

            return ls.Where(c => (string.IsNullOrEmpty(code) ? true : (c.PoCode.Contains(code) || c.Pattern.Contains(code)))).OrderByDescending(c =>
                  c.DelayDays).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

            //else
            //{
            //    //string ss = string.Empty;
            //    //foreach (DataColumn item in dt.Columns)
            //    //{
            //    //    ss += string.Format("model.{0}=row['{0}'].ToString();", item.ColumnName);
            //    //}
            //    return ls;
            //}
        }


        #endregion

        #endregion

        #region 计划与进度提醒

        #region 统计数据
        /// <summary>
        /// 获取 计划与进度提醒的统计数据
        /// </summary>
        /// <returns>
        /// DayStart在7天内开始的生产计划个数,
        /// DayEnd在7天内结束的生产单个数,
        /// Producting正在生产中的生产计划个数(未做)，
        /// EffAnalysis n天内生产线效率分析(未做)，
        /// ProgressTrack 生产单7天内进度跟踪个数
        /// </returns>
        [HttpGet, Route("api/TaskWarn/GetUserPlanAlertCount")]
        public ViewModels.UserPlanCount GetUserPlanAlertCount()
        {
            string jobCodes = "ProgressTrack";
            //string jobCodes = "DayStart,DayEnd,Producting,EffAnalysis,ProgressTrack";
            string allowFactoryIDs = new TaskWarn().GetAllowedFactory();
            if (string.IsNullOrEmpty(allowFactoryIDs))
                return null;

            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            DataTable dt = AMOData.JobAlertNew.GetUserPlanAlertCount(jobCodes, allowFactoryIDs, jobAlertDays);
            if (dt.Rows.Count > 0)
            {
                ViewModels.UserPlanCount model = new ViewModels.UserPlanCount();
                //model.DayStart = dt.Rows[0]["DayStart"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["DayStart"]);
                //model.DayEnd = dt.Rows[0]["DayEnd"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["DayEnd"]);
                //model.Producting = dt.Rows[0]["Producting"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["Producting"]);
                //model.EffAnalysis = dt.Rows[0]["EffAnalysis"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["EffAnalysis"]);
                model.ProgressTrack = dt.Rows[0]["ProgressTrack"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ProgressTrack"]);

                return model;
            }
            else
            {
                return null;
            }
        }
        #endregion


        #region 明细

        /// <summary>
        /// {0} 个生产计划在 {1} 天内开始 (明细数据)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailDayStart")]
        public List<IOSapi.ViewModels.JaDayStartDayEnd> GetDetailDayStart()
        {
            string factoryIDs = new TaskWarn().GetAllowedFactory();
            AMO.Model.BindCollection<AMO.Model.JaDayStartDayEnd> datas = AMO.BLL.JobAlert.GetDetailDayStart(factoryIDs);

            List<IOSapi.ViewModels.JaDayStartDayEnd> ls = new List<ViewModels.JaDayStartDayEnd>();
            foreach (var item in datas)
            {
                ls.Add(new ViewModels.JaDayStartDayEnd()
                {
                    CustomerName = item.CustomerName,
                    CustomerStyleNO = item.CustomerStyleNO,
                    DeliveryDate = item.DeliveryDate,
                    Description = item.Description,
                    EarlinessStartDate = item.EarlinessStartDate,
                    FactoryID = item.FactoryID,
                    LineName = item.LineName,
                    Merchandiser = item.Merchandiser,
                    Pattern = item.Pattern,
                    PlanAmount = item.PlanAmount,
                    PlanEnd = item.PlanEnd,
                    PlanStart = item.PlanStart,
                    POAmount = item.POAmount,
                    PoCode = item.PoCode,
                    ProductionEventID = item.ProductionEventID,
                    ProductTypeName = item.ProductTypeName,
                });
            }

            return ls;
        }

        /// <summary>
        /// {0} 个生产计划在 {1} 天内结束 (明细数据)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailDayEnd")]
        public List<IOSapi.ViewModels.JaDayStartDayEnd> GetDetailDayEnd()
        {
            string factoryIDs = new TaskWarn().GetAllowedFactory();
            AMO.Model.BindCollection<AMO.Model.JaDayStartDayEnd> datas = AMO.BLL.JobAlert.GetDetailDayEnd(factoryIDs);

            List<IOSapi.ViewModels.JaDayStartDayEnd> ls = new List<ViewModels.JaDayStartDayEnd>();
            foreach (var item in datas)
            {
                ls.Add(new ViewModels.JaDayStartDayEnd()
                {
                    CustomerName = item.CustomerName,
                    CustomerStyleNO = item.CustomerStyleNO,
                    DeliveryDate = item.DeliveryDate,
                    Description = item.Description,
                    EarlinessStartDate = item.EarlinessStartDate,
                    FactoryID = item.FactoryID,
                    LineName = item.LineName,
                    Merchandiser = item.Merchandiser,
                    Pattern = item.Pattern,
                    PlanAmount = item.PlanAmount,
                    PlanEnd = item.PlanEnd,
                    PlanStart = item.PlanStart,
                    POAmount = item.POAmount,
                    PoCode = item.PoCode,
                    ProductionEventID = item.ProductionEventID,
                    ProductTypeName = item.ProductTypeName,
                });
            }

            return ls;
        }


        #endregion

        #endregion

        #region 物料与关键事件提醒

        #region 统计数据
        /// <summary>
        /// 获取 物料与关键事件提醒的统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetUserMatEventAlertCount")]
        public ViewModels.UserMatEventCount GetUserMatEventAlertCount()
        {
            string jobCodes = "MatReceive,EventFinish,ProgressTrack";
            string allowFactoryIDs = new TaskWarn().GetAllowedFactory();
            if (string.IsNullOrEmpty(allowFactoryIDs))
                return null;

            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            int ottoffset = AMOData.Settings.SysSetting.PoOTTOffset;

            ViewModels.UserMatEventCount model = new TaskWarn().GetUserMatEventAlertCount(jobCodes, allowFactoryIDs, jobAlertDays, ottoffset);



            ////计算 XX个物料需在YY内天到货
            //if (jobCodes.Contains("MatReceive") && model!=null)
            //{
            //    try
            //    {
            //        AMO.Model.BindCollection<AMO.Model.JaMaterial> LstJaMaterialReceive = AMOData.JobAlertNew.GetDetailMatReceive(allowFactoryIDs, 10, ottoffset);
            //        int materialReceiveCount = LstJaMaterialReceive == null ? 0 : LstJaMaterialReceive.Count;
            //        model.MatReceive = materialReceiveCount;
            //    }
            //    catch { }
            //}

            return model;

            //DataTable dt = AMOData.JobAlertNew.GetUserMatEventAlertCount(jobCodes, allowFactoryIDs, jobAlertDays, ottoffset);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    //计算 XX个物料需在YY内天到货
            //    if (jobCodes.Contains("MatReceive") && dt.Columns.Contains("MatReceive"))
            //    {
            //        try
            //        {
            //            AMO.Model.BindCollection<AMO.Model.JaMaterial> LstJaMaterialReceive = AMOData.JobAlertNew.GetDetailMatReceive(allowFactoryIDs, 10, ottoffset);
            //            int materialReceiveCount = LstJaMaterialReceive == null ? 0 : LstJaMaterialReceive.Count;
            //            dt.Rows[0]["MatReceive"] = materialReceiveCount;
            //        }
            //        catch { }
            //    }
            //    ////计算  N 天内开始的生产计划 XX 个关键事件未维护
            //    //if (jobCodes.Contains("EventNotMaintain") && dt.Columns.Contains("EventNotMaintain"))
            //    //{
            //    //    try
            //    //    {
            //    //        var results = AMO.BLL.JobAlert.GetDetailEventNotMaintain(allowFactoryIDs);
            //    //        dt.Rows[0]["EventNotMaintain"] = results == null ? 0 : results.Count();
            //    //    }
            //    //    catch { }
            //    //}
            //}
            //if (dt.Rows.Count > 0)
            //{
            //    ViewModels.UserMatEventCount model = new ViewModels.UserMatEventCount();
            //    model.MatReceive = dt.Rows[0]["MatReceive"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["MatReceive"]);
            //    model.EventFinish = dt.Rows[0]["EventFinish"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["EventFinish"]);
            //    model.ProgressTrack = dt.Rows[0]["ProgressTrack"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ProgressTrack"]);
            //    return model;
            //}
            //else
            //{
            //    return null;
            //}
        }
        #endregion

        #region 明细

        /// <summary>
        /// XX个物料需在YY天内到货(明细数据)(未区分工厂)
        /// </summary>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="type">主辅料：-1全部；0主料；1辅料</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailMatReceive")]
        public List<IOSapi.ViewModels.JaMaterial> GetDetailMatReceive(int pageIndex, int pageSize, int type = -1, string code = "", string fids = "")
        {
            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }

            return TaskWarn.GetDetailMatReceive(pageIndex, pageSize, type, code, fids);

            //AMO.Model.BindCollection<AMO.Model.JaMaterial> datas = AMO.BLL.JobAlert.GetDetailMatReceive();
            //AMO.Model.BindCollection<AMO.Model.JaMaterial> datas = TaskWarn.GetDetailMatReceive(fids);
            //List<IOSapi.ViewModels.JaMaterial> ls = new List<ViewModels.JaMaterial>();
            //try
            //{
            //    //var ddd=
            //    if (datas!=null && datas.Count>0)
            //    {
            //        List<AMO.Model.JaMaterial> jLis = datas.Where(c => string.IsNullOrEmpty(code) ? true : (code.Contains(c.PoCode) || code.Contains(c.Pattern))).OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
            //        foreach (var item in jLis)
            //        {
            //            ls.Add(new ViewModels.JaMaterial()
            //            {
            //                CustomerStyleNo = item.CustomerStyleNo,
            //                DelayDays = item.DelayDays,
            //                ExpectedReciveDate = item.ExpectedReciveDate,
            //                MaterialCode = item.MaterialCode,
            //                MaterialColor = item.MaterialColor,
            //                MaterialName = item.MaterialName,
            //                MaterialSize = item.MaterialSize,
            //                MaterialStyle = item.MaterialStyle,
            //                MPAmount = item.MPAmount,
            //                MPDate = item.MPDate,
            //                MPNO = item.MPNO,
            //                MPUnit = item.MPUnit,
            //                NeedDate = item.NeedDate,
            //                Pattern = item.Pattern,
            //                PoCode = item.PoCode,
            //                Supplier = item.Supplier
            //            });
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //}
            //return ls;
        }

        /// <summary>
        /// XX个关键事件需在YY内天完成 (明细数据)
        /// </summary>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailEventNotMaintain")]
        public List<ViewModels.JaEventNotMaintain> GetDetailEventNotMaintain(int pageIndex, int pageSize, string code = "", string fids = "")
        {
            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }

            List<ViewModels.JaEventNotMaintain> ls = new List<ViewModels.JaEventNotMaintain>();
            try
            {
                int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
                DataTable dtPO = AMOData.JobAlertNew.GetDetailEventFinish(fids, jobAlertDays);
                if (dtPO == null || dtPO.Rows.Count == 0)
                    return ls;


                ls = dtPO.AsEnumerable().Where(c => string.IsNullOrEmpty(code) ? true : (c.Field<string>("PoCode").Contains(code) || c.Field<string>("Pattern").Contains(code))).OrderBy(c =>
                    c.Field<string>("PoCode")).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).Select(p => new ViewModels.JaEventNotMaintain()
                {
                    FactoryID = p.Field<int>("FactoryID"),
                    Code = p.Field<string>("PoCode"),
                    CriticalEventManID = p.Field<int>("CriticalEventManID"),
                    DeliveryDate = p.Field<DateTime>("DeliveryDate"),
                    ExpectFinishDate = p.Field<DateTime>("ExpectFinishDate"),
                    PlanStart = p.Field<DateTime>("PlanStart"),
                    EventName = p.Field<string>("EventName"),
                    ProductTypeName = p.Field<string>("ProductTypeName"),
                    CustomerName = p.Field<string>("CustomerName"),
                    Merchandiser = p.Field<string>("Merchandiser"),
                    Pattern = p.Field<string>("Pattern"),
                }).ToList();


                // List<AMO.Model.JaEventNotMaintain> jLis = bls.Where(c => string.IsNullOrEmpty(code) ? true : (code.Contains(c.PoCode) || code.Contains(c.Pattern))).OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
                //if (bls != null)
                //{
                //    foreach (var item in jLis)
                //    {
                //        ViewModels.JaEventNotMaintain model = new ViewModels.JaEventNotMaintain()
                //        {
                //            Code = item.Code,
                //            CriticalEventManID = item.CriticalEventManID,
                //            CustomerID = item.CustomerID,
                //            EndDate = item.EndDate,
                //            EventFlowID = item.EventFlowID,
                //            FacilityID = item.FacilityID,
                //            Index = item.Index,
                //            MakeDayStartTime = item.MakeDayStartTime,
                //            PoEventID = item.PoEventID,
                //            POID = item.POID,
                //            ProductTypeID = item.ProductTypeID,
                //            RecommendEndDate = item.RecommendEndDate,
                //            StandardTime = item.StandardTime,
                //            StartDate = item.StartDate,
                //            CustomerName = item.CustomerName,
                //            Merchandiser = item.Merchandiser,
                //            Pattern = item.Pattern
                //        };
                //        ls.Add(model);
                //    }
                //}


                return ls;

            }
            catch (Exception)
            {
                return null;
            }

            return ls;

        }



        /// <summary>
        /// {0} 个生产单 {1} 天内进度跟踪 (明细数据)
        /// </summary>
        /// <param name="code">模糊搜索：生产单号或款号</param>
        /// <param name="fids">工厂id字符串（如1,5,12,15）</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/TaskWarn/GetDetailProgressTrack")]
        public List<JaProgressTrack> GetDetailProgressTrack(int pageIndex, int pageSize, string code = "", string fids = "")
        {
            if (string.IsNullOrEmpty(fids))
            {
                fids = new TaskWarn().GetAllowedFactory();
            }

            List<JaProgressTrack> ls = new List<ViewModels.JaProgressTrack>();

            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            DataTable dtPO = AMOData.JobAlertNew.GetDetailProgressTrackPO(fids, jobAlertDays);
            if (dtPO == null || dtPO.Rows.Count == 0)
                return ls;

            try
            {
                ls = dtPO.AsEnumerable().Where(c => (string.IsNullOrEmpty(code) ? true : c.Field<string>("PoCode").Contains(code) || c.Field<string>("Pattern").Contains(code))).OrderBy(c => c.Field<string>("PoCode")).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).Select(p => new JaProgressTrack()
                {
                    Amount = p.Field<double>("Amount"),
                    CustomerName = p.Field<string>("CustomerName"),
                    CustomerStyleNO = p.Field<string>("CustomerStyleNO"),
                    Pattern = p.Field<string>("Pattern"),
                    PoCode = p.Field<string>("PoCode"),
                    POID = p.Field<int>("POID"),
                    ProductTypeName = p.Field<string>("ProductTypeName")

                }).ToList();

                if (ls != null && ls.Count > 0)
                {
                    DateTime dt = DateTime.Now.AddDays(-7);
                    List<int> poids = ls.Select(m => m.POID).ToList();
                    List<PRODUCTIONSCHEDULE> schedules = db.PRODUCTIONSCHEDULEs.Where(c => c.PRODUCTIONDATE >= dt && poids.Contains(c.POID)).ToList();
                    var procceses = db.PROCESSes.OrderBy(c => c.ID).ToList();
                    foreach (var po in ls)
                    {
                        DateTime prodDate = DateTime.MinValue;
                        var qPO = schedules.Where(x => x.POID == po.POID).ToList();
                        if (qPO.Count() > 0)
                            prodDate = qPO.Max(x => x.PRODUCTIONDATE);

                        if (prodDate.Date <= DBHelper.MinDate.Date)
                            continue;

                        po.LastProDate = prodDate;
                        po.JaProcessList = new List<JaProcess>();
                        foreach (var item in procceses)
                        {
                            var sumAmount = qPO.Where(c => c.PROCESSID == item.ID).Sum(q => q.AMOUNT);
                            if (sumAmount != null && sumAmount > 0)
                            {
                                JaProcess jp = new JaProcess();
                                jp.TotalAmount = sumAmount.Value;
                                jp.Amount = qPO.Where(c => c.PROCESSID == item.ID && c.PRODUCTIONDATE == prodDate).Sum(q => q.AMOUNT);
                                jp.ProcessId = item.ID;
                                jp.ProcessName = item.NAME;

                                po.JaProcessList.Add(jp);
                            }
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                return null;
            }



            //string factoryIDs = new TaskWarn().GetAllowedFactory();
            //DataTable dt = AMO.BLL.JobAlert.GetDetailProgressTrack(fids);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        JaProgressTrack model = new ViewModels.JaProgressTrack();


            //        ls.Add(model);
            //    }


            //    return ls;
            //}

            return ls;
        }


        #endregion

        #endregion

        /// <summary>
        /// 合并掉重复的信息(只要有一个不一样就不合并)
        /// </summary>
        /// <returns></returns>
        private string ProcessDuplication(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            str = str.TrimEnd(' ').TrimEnd(',');
            if (!str.Contains(','))
                return str;
            if (str.Trim() == ",")
                return "";

            List<string> lstStr = new List<string>();
            List<string> lst = str.Split(',').ToList();
            foreach (string s in lst)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                lstStr.Add(s.Trim());
            }

            if (lstStr.Count == 0)
                return "";

            IEnumerable<string> ieStr = lstStr.Distinct();
            if (ieStr.Count() == 1)
            {
                return ieStr.First();
            }
            else
            {
                return str;
            }
        }

    }
}
