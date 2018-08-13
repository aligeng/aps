using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using IOSapi.ViewModels;
using IOSapi.BLL;
using System.Data;
using AMO.Logic;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 生产进度数据录入（包括外协厂）
    /// </summary>
    public class ScheduleController : BaseApiController
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

        /// <summary>
        ///  根据生产单ID获取所在排产产线
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetLineByPo")]
        public List<SelectModel> GetLineByPo(int poId)
        {
            List<SelectModel> ls = new List<SelectModel>();
            ls = db.PRODUCTIONEVENTs.Where(c => c.PRODUCTIONEVENTDETAILs.Where(p => p.POID == poId).Count() > 0).Select(p => new SelectModel()
            {
                itemId = p.FACILITYID,
                itemName = p.FACILITY.NAME
            }).Distinct().ToList();

            return ls;
        }

        /// <summary>
        /// 获取生产单所有款式颜色
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPoColors")]
        public List<SelectModel> GetPoColors(int poId)
        {
            List<SelectModel> ls = new List<SelectModel>();

            ls = db.PODETAILs.Where(c => c.POID == poId).OrderBy(c => c.POCOLOR).Select(p => new SelectModel()
            {
                itemId = p.ID,
                itemName = p.POCOLOR

            }).Distinct().ToList();

            return ls;
        }

        /// <summary>
        /// 获取生产单所有尺码
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPoSizes")]
        public List<SelectModel> GetPoSizes(int poId)
        {
            List<SelectModel> ls = new List<SelectModel>();

            ls = db.POSIZEDETAILs.Where(c => c.POID == poId).OrderBy(c => c.POSIZE).Select(p => new SelectModel()
            {
                itemId = 0,
                itemName = p.POSIZE
            }).Distinct().ToList();

            return ls;
        }

        #region 关键事件

        /// <summary>
        /// 关键事件录入查询列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="star">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="dateType">日期类型：0车缝开始日期；1交货期；2生产单确认日期</param>
        /// <param name="eventid">事件id集</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号</param>
        /// <returns></returns>

        [HttpGet, Route("api/Schedule/GetEventPoes")]
        public List<Po> GetEventPoes(int pageIndex, int pageSize, DateTime star, DateTime end, int dateType = -1, string eventid = "", string fids = "", string wsids = "", string code = "")
        {

           // LogTest.Write("[code]内容:"+  System.Web.HttpUtility.UrlDecode(strUrl, System.Text.Encoding.GetEncoding("UTF-8"))Server.UrlDecode(Name) System.Text.Encoding. code)+"    ");


            //工厂查询
            List<int> fidLs = new List<int>();
            if (!string.IsNullOrEmpty(fids))
            {
                string[] fidsArry = fids.Split(',');
                foreach (var item in fidsArry)
                {
                    fidLs.Add(Convert.ToInt32(item));
                }
            }
            else
            {
                int[] fidsArry = GetAllowedFactoryIDs();
                foreach (var item in fidsArry)
                {
                    fidLs.Add(item);
                }
            }

            //关键事件查询
            List<int> eventIds = new List<int>();
            if (!string.IsNullOrEmpty(eventid))
            {
                string[] fidsArry = eventid.Split(',');
                foreach (var item in fidsArry)
                {
                    eventIds.Add(Convert.ToInt32(item));
                }
            }

            List<Po> ls = new List<Po>();
            if (star == null || end == null || star > end)
            {
                dateType = -1;
            }

            if (dateType == 0)
            {
                //var datas = (from a in db.POes
                //             join b in db.PRODUCTIONEVENTs on a.ID equals b.POID
                //             where (string.IsNullOrEmpty(code) ? true : (a.CODE.Contains(code) || a.PATTERN.Contains(code))) && a.STATUS == 0 &&
                //                 fidLs.Contains(a.FACTORYID.Value) && (b.STARTTIME >= star && b.STARTTIME <= end)
                //             select new ViewModels.Po
                //             {
                //                 id = a.ID,
                //                 code = a.CODE,
                //                 customerid = a.CUSTOMERID.Value,
                //                 customername = db.CUSTOMERs.Where(c => c.ID == a.CUSTOMERID).FirstOrDefault().NAME,
                //                 customerPattern=a.CUSTOMERSTYLENO,
                //                 deliverydate = a.DELIVERYDATE,
                //                 confirmdate = a.PLACEORDERDATE,
                //                 initDeliveryDate=a.INITDELIVERYDATE,
                //                 amount = a.AMOUNT,
                //                 pattern = a.PATTERN,
                //                 priority = a.PRIORITY.Value,
                //                 producttype = db.PRODUCTTYPEs.Where(c => c.ID == a.PRODUCTTYPEID).FirstOrDefault().NAME,
                //                 merchandiser = db.USERS.Where(c => c.ID == a.MERCHANDISER).FirstOrDefault().USERNAME,//c => ((int?)c.AMOUNT) ?? 0).GetValueOrDefault(0m
                //                 CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                //                 StarDate =b.STARTTIME,
                //                 EndDate=b.ENDTIME,
                //                 MainMaterielArriveDate = a.MASTERMATERIALRECEIVEDDATE,
                //                 OthMaterielArriveDate = a.MATERIALRECEIVEDDATE,
                //                 EventFlowName=db.EVENTFLOWs.Where(c=>c.ID==a.EVENTFLOWID.Value).Select(p=>p.NAME).FirstOrDefault()
                //             }).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));             
                end = end.AddDays(1).Date;
                var evens = (from a in db.PRODUCTIONEVENTDETAILs where a.PRODUCTIONEVENT.STATUS==0 && a.PRODUCTIONEVENT.STARTTIME >= star && a.PRODUCTIONEVENT.STARTTIME < end group a by a.POID into g select new { poid = g.Key, evenTime = g.Min(p => p.PRODUCTIONEVENT.STARTTIME) }).ToList();
                List<int> poids = evens.Select(c => c.poid).ToList();
                if (poids != null || poids.Count > 0)
                {

                    ls = (from a in db.POes
                          where (string.IsNullOrEmpty(code) ? true : (a.CODE.Contains(code) || a.PATTERN.Contains(code))) && a.STATUS == 0 &&
                              fidLs.Contains(a.FACTORYID.Value) && poids.Contains(a.ID) && (string.IsNullOrEmpty(eventid) ? true : a.POEVENTs.Where(c => eventIds.Contains(c.EVENTFLOWNODE.CRITICALEVENTID.Value)).Count() > 0)
                          select new ViewModels.Po
                          {
                              id = a.ID,
                              code = a.CODE,
                              customerid = a.CUSTOMERID.Value,
                              customername = db.CUSTOMERs.Where(c => c.ID == a.CUSTOMERID).FirstOrDefault().NAME,
                              customerPattern = a.CUSTOMERSTYLENO,
                              deliverydate = a.DELIVERYDATE,
                              confirmdate = a.PLACEORDERDATE,
                              initDeliveryDate = a.INITDELIVERYDATE,
                              amount = a.AMOUNT,
                              pattern = a.PATTERN,
                              priority = a.PRIORITY.Value,
                              producttype = db.PRODUCTTYPEs.Where(c => c.ID == a.PRODUCTTYPEID).FirstOrDefault().NAME,
                              merchandiser = db.USERS.Where(c => c.ID == a.MERCHANDISER).FirstOrDefault().USERNAME,//c => ((int?)c.AMOUNT) ?? 0).GetValueOrDefault(0m
                              //CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                              //StarDate =evens.Where(m=>m.poid==a.ID).Select(c=>c.evenTime).FirstOrDefault(),
                              //EndDate = b.ENDTIME,
                              MainMaterielArriveDate = a.MASTERMATERIALRECEIVEDDATE,
                              OthMaterielArriveDate = a.MATERIALRECEIVEDDATE,
                              EventFlowName = db.EVENTFLOWs.Where(c => c.ID == a.EVENTFLOWID.Value).Select(p => p.NAME).FirstOrDefault()
                          }).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

                    foreach (var item in ls)
                    {
                        item.StarDate = evens.Where(m => m.poid == item.id).Select(c => c.evenTime).FirstOrDefault();
                    }

                }

            }
            else
            {
                var datas = db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code))) && c.STATUS == 0 &&
fidLs.Contains(c.FACTORYID.Value) && (dateType == 1 ? (c.DELIVERYDATE >= star && c.DELIVERYDATE <= end) : true) && 
(string.IsNullOrEmpty(eventid) ? true : c.POEVENTs.Where(s => eventIds.Contains(s.EVENTFLOWNODE.CRITICALEVENTID.Value)).Count() > 0) &&
(dateType == 2 ? (c.PLACEORDERDATE >= star && c.PLACEORDERDATE <= end) : true)).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                ls = datas.Select(p => new ViewModels.Po
                {
                    id = p.ID,
                    code = p.CODE,
                    customerid = p.CUSTOMERID.Value,
                    customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                    customerPattern = p.CUSTOMERSTYLENO,
                    deliverydate = p.DELIVERYDATE,
                    confirmdate = p.PLACEORDERDATE,
                    initDeliveryDate = p.INITDELIVERYDATE,
                    amount = p.AMOUNT,
                    pattern = p.PATTERN,
                    priority = p.PRIORITY.Value,
                    producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                    merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                    StarDate = db.PRODUCTIONEVENTDETAILs.Where(c => c.POID == p.ID).Select(m => m.PRODUCTIONEVENT.STARTTIME).DefaultIfEmpty().Min(),
                    MainMaterielArriveDate = p.MASTERMATERIALRECEIVEDDATE,
                    OthMaterielArriveDate = p.MATERIALRECEIVEDDATE,
                    EventFlowName = db.EVENTFLOWs.Where(c => c.ID == p.EVENTFLOWID.Value).Select(m => m.NAME).FirstOrDefault()
                }).ToList();

            }


            return ls;

        }

        /// <summary>
        /// 获取生产单所有关键事件
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPoEvents")]
        public List<PoEvent> GetPoEvents(int poId)
        {
            //var ls = db.POEVENTs.Where(c => c.POID == poId).Select(p => new PoEvent
            //{
            //    id = p.ID,
            //    description = p.DESCRIPTION,
            //    duration = p.DURATION.Value,
            //    enddate = p.ENDDATE == null ? DateTime.MinValue : p.ENDDATE.Value,
            //    eventflownodeid = p.EVENTFLOWNODEID,
            //    eventflownodeName = p.EVENTFLOWNODE.CRITICALEVENT.NAME,
            //    isaffectplan = p.ISAFFECTPLAN.Value,
            //    standardtime = p.STANDARDTIME == null ? DateTime.MinValue : p.STANDARDTIME.Value,
            //    startdate = p.STARTDATE == null ? DateTime.MinValue : p.STARTDATE.Value,
            //    recommendeddate = DateTime.MinValue,
            //});

            List<PoEvent> ls = new List<PoEvent>();
            string strSql = string.Format(@" SELECT tbA.nodeId, tbA.eventId,tbA.evenName,tbA.PREVNODEID,(CASE WHEN tbB.DURATION>0 THEN tbB.DURATION ELSE tbA.DURATION END) DURATION ,
 tbB.[ID],tbB.[POID],tbB.[EVENTFLOWNODEID],tbB.[STARTDATE],tbB.[ENDDATE],tbB.[DESCRIPTION],tbB.[ISAFFECTPLAN],tbB.[STANDARDTIME],tbB.[STARTDATEUPDATEDATE],
 tbB.[UPDATEUSER],tbB.[UPDATEDATE] ,tbB.[KPIDate] ,tbB.[BaseDataID],tbB.[DelayFinishDateCauseBaseDataID] ,tbB.[InitKPIDate]  FROM 
(SELECT a.ID nodeId, b.ID eventId,b.NAME evenName,a.PREVNODEID,a.DURATION FROM dbo.EVENTFLOWNODE a INNER JOIN dbo.CRITICALEVENT b ON a.CRITICALEVENTID=b.ID
  WHERE a.CRITICALEVENTID>0 and a.EVENTFLOWID=(SELECT p.EVENTFLOWID FROM dbo.PO p WHERE  p.ID={0})) tbA
LEFT JOIN (SELECT * FROM dbo.POEVENT WHERE POID={0}) tbB ON tbA.nodeId=tbB.EVENTFLOWNODEID ORDER BY tbA.PREVNODEID", poId);

            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
                CalcCriticalEventDate calcCriticalEventDate = new CalcCriticalEventDate(new List<int>() { poId });
                var calctb = calcCriticalEventDate.Caculation();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        PoEvent poEvent = new PoEvent();
                        poEvent.id = Convert.ToInt32(row["eventId"]);
                        poEvent.description = row["DESCRIPTION"].ToString();
                        poEvent.duration = Convert.ToDouble(row["DURATION"]);
                        poEvent.eventflownodeid = Convert.ToInt32(row["nodeId"]);
                        poEvent.eventflownodeName = row["evenName"].ToString();
                        poEvent.isaffectplan = Convert.ToInt32(row["ISAFFECTPLAN"]);
                        poEvent.standardtime = row["STANDARDTIME"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["STANDARDTIME"].ToString());
                        poEvent.startdate = row["STARTDATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["STARTDATE"].ToString());
                        poEvent.enddate = row["ENDDATE"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["ENDDATE"].ToString());
                        //poEvent.recommendeddate = DateTime.MinValue;

                        ls.Add(poEvent);
                    }
                }
                if (calctb != null)
                {
                    foreach (DataRow row in calctb.Rows)
                    {
                        var item = ls.FirstOrDefault(c => c.id == Convert.ToInt32(row["CriticalEventID"]));
                        item.recommendeddate = row["RecommendEndDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["RecommendEndDate"].ToString());
                    }
                }

            }
            catch (Exception)
            {
                return null;
            }


            return ls.OrderBy(c => c.recommendeddate).ToList();
        }

        // POST api/values
        /// <summary>
        /// 修改单个关键事件完成日期（完成日期）
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <param name="eventflownodeId">关键事件流程节点id</param>
        /// <param name="expectTime">预计完成日期</param>
        /// <param name="endDate">实际完成日期</param>
        [HttpPost, Route("api/Schedule/UpdateEventDate")]
        public ResultModel UpdateEventDate(int poId, int eventflownodeId, DateTime expectTime, DateTime endDate)
        {
            ResultModel rm = new ResultModel();

            try
            {
                var ev = db.POEVENTs.Where(c => c.POID == poId && c.EVENTFLOWNODEID == eventflownodeId).FirstOrDefault();
                if (ev != null)
                {
                    ev.STARTDATE = expectTime;
                    ev.ENDDATE = endDate;
                }
                else
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "该生产单不存在或没有关键事件"));
                    rm.ErrMessage.Add(new LangMessage("en", "The production list does not exist or there are no critical incidents."));
                    return rm;
                }

                db.SaveChanges();
                rm.IsSuccess = 1;
            }
            catch (Exception ex)
            {
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
            }

            return rm;
        }

        /// <summary>
        /// 批量修改关键事件完成时间
        /// </summary>
        /// <param name="evns"></param>
        [HttpPost, Route("api/Schedule/UpdateEventsDate")]
        public ResultModel UpdateEventsDate(PoEvents evns)
        {
            ResultModel rm = new ResultModel();
            try
            {
                var ls = db.POEVENTs.Where(c => c.POID == evns.PoId);
                if (ls != null && ls.Count() > 0)
                {
                    foreach (var item in evns.Events)
                    {
                        var ev = ls.Where(c => c.EVENTFLOWNODEID == item.EventflownodeId).FirstOrDefault();
                        if (ev != null)
                        {
                            if (!string.IsNullOrEmpty(item.ExpectTime))
                            {
                                ev.STARTDATE = Convert.ToDateTime(item.ExpectTime);
                            }
                            if (!string.IsNullOrEmpty(item.EndTime))
                            {
                                 ev.ENDDATE = Convert.ToDateTime(item.EndTime);
                            }
                        }
                    }

                    db.SaveChanges();
                }
                else
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "该生产单不存在或没有关键事件"));
                    rm.ErrMessage.Add(new LangMessage("en", "The production list does not exist or there are no critical incidents."));
                    return rm;
                }

            }
            catch (Exception ex)
            {
                rm.IsSuccess = 0;
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
                return rm;
            }

            rm.IsSuccess = 1;
            //rm.ErrMessage = Newtonsoft.Json.JsonConvert.SerializeObject(evns);
            return rm;
        }

        #endregion

        #region 非排产工序

        /// <summary>
        /// 非排产工序进度录入查询列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="star">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="dateType">日期类型：0车缝开始日期；1交期；2生产单确认日期</param>
        /// <param name="processid">工序id集</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetProcessPoes")]
        public List<Po> GetProcessPoes(int pageIndex, int pageSize, DateTime star, DateTime end, int dateType = -1, string processid = "", string fids = "", string wsids = "", string code = "")
        {
            //工厂查询
            List<int> fidLs = new List<int>();
            if (!string.IsNullOrEmpty(fids))
            {
                string[] fidsArry = fids.Split(',');
                foreach (var item in fidsArry)
                {
                    fidLs.Add(Convert.ToInt32(item));
                }
            }
            else
            {
                int[] fidsArry = GetAllowedFactoryIDs();
                foreach (var item in fidsArry)
                {
                    fidLs.Add(item);
                }
            }

            //工序查询
            List<int> routeIds = new List<int>();
            if (!string.IsNullOrEmpty(processid))
            {
                string[] fidsArry = processid.Split(',');
                foreach (var item in fidsArry)
                {
                    routeIds.Add(Convert.ToInt32(item));
                }
                routeIds = db.PROCESSROUTENODEs.Where(c => c.PROCESS.ISPRIMARY == 0 && routeIds.Contains(c.PROCESSID)).Select(p => p.PROCESSROUTEID).Distinct().ToList();
            }


            List<Po> ls = new List<Po>();

            if (star == null || end == null || star > end)
            {
                dateType = -1;
            }

            if (dateType == 0)  //按车缝开始日期
            {
                star = star.Date;
                end = end.AddDays(1).Date;
                var evens = (from a in db.PRODUCTIONEVENTDETAILs where a.PRODUCTIONEVENT.STATUS==0 && a.PRODUCTIONEVENT.STARTTIME >= star && a.PRODUCTIONEVENT.STARTTIME < end group a by a.POID into g select new { poid = g.Key, evenTime = g.Min(p => p.PRODUCTIONEVENT.STARTTIME) }).ToList();
                List<int> poids = evens.Select(c => c.poid).ToList();
                //List<int> poids = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.STARTTIME >= star && c.PRODUCTIONEVENT.ENDTIME <= end).Select(p => p.POID).Distinct().ToList();
                if (poids != null || poids.Count > 0)
                {
                    //var ls = (from a in db.POes
                    //             join b in db.PRODUCTIONEVENTs on a.ID equals b.POID
                    //             where (string.IsNullOrEmpty(code) ? true : (a.CODE.Contains(code) || a.PATTERN.Contains(code))) && a.STATUS == 0 &&
                    //                 fidLs.Contains(a.FACTORYID.Value) && (b.STARTTIME >= star && b.STARTTIME <= end)
                    //             select new ViewModels.Po
                    //             {
                    //                 id = a.ID,
                    //                 code = a.CODE,
                    //                 customerid = a.CUSTOMERID.Value,
                    //                 customername = db.CUSTOMERs.Where(c => c.ID == a.CUSTOMERID).FirstOrDefault().NAME,
                    //                 deliverydate = a.DELIVERYDATE,
                    //                 amount = a.AMOUNT,
                    //                 pattern = a.PATTERN,
                    //                 priority = a.PRIORITY.Value,
                    //                 producttype = db.PRODUCTTYPEs.Where(c => c.ID == a.PRODUCTTYPEID).FirstOrDefault().NAME,
                    //                 customerPattern = a.CUSTOMERSTYLENO,
                    //                 merchandiser = db.USERS.Where(c => c.ID == a.MERCHANDISER).FirstOrDefault().USERNAME,//c => ((int?)c.AMOUNT) ?? 0).GetValueOrDefault(0m
                    //                 //CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                    //                 StarDate = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID).Min(c => c.PRODUCTIONDATE),
                    //                 confirmdate = a.PLACEORDERDATE
                    //             }).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

                    ls = (from a in db.POes
                          where ( string.IsNullOrEmpty(code) ? true : (a.CODE.Contains(code) || a.PATTERN.Contains(code))) && a.STATUS == 0 &&
                              fidLs.Contains(a.FACTORYID.Value) && poids.Contains(a.ID) && (string.IsNullOrEmpty(processid) ? true : routeIds.Contains(a.PROCESSROUTEID.Value))
                          select new ViewModels.Po
                          {
                              id = a.ID,
                              code = a.CODE,
                              customerid = a.CUSTOMERID.Value,
                              customername = db.CUSTOMERs.Where(c => c.ID == a.CUSTOMERID).FirstOrDefault().NAME,
                              deliverydate = a.DELIVERYDATE,
                              amount = a.AMOUNT,
                              pattern = a.PATTERN,
                              priority = a.PRIORITY.Value,
                              producttype = db.PRODUCTTYPEs.Where(c => c.ID == a.PRODUCTTYPEID).FirstOrDefault().NAME,
                              customerPattern = a.CUSTOMERSTYLENO,
                              merchandiser = db.USERS.Where(c => c.ID == a.MERCHANDISER).FirstOrDefault().USERNAME,//c => ((int?)c.AMOUNT) ?? 0).GetValueOrDefault(0m
                              //CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                              //StarDate = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == a.ID).Min(c => c.PRODUCTIONDATE),
                              //StarDate = evens.Where(m => m.poid == a.ID).Select(c => c.evenTime).FirstOrDefault(),
                              confirmdate = a.PLACEORDERDATE
                          }).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

                    foreach (var item in ls)
                    {
                        item.StarDate = evens.Where(m => m.poid == item.id).Select(c => c.evenTime).FirstOrDefault();
                    }
                }
            }
            else
            {
                var datas = db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code))) && c.STATUS == 0 &&
fidLs.Contains(c.FACTORYID.Value) && (dateType == 1 ? (c.DELIVERYDATE >= star && c.DELIVERYDATE <= end) : true) && (string.IsNullOrEmpty(processid) ? true : routeIds.Contains(c.PROCESSROUTEID.Value)) &&
(dateType == 2 ? (c.PLACEORDERDATE >= star && c.PLACEORDERDATE <= end) : true)).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                ls = datas.Select(p => new ViewModels.Po
                {
                    id = p.ID,
                    code = p.CODE,
                    customerid = p.CUSTOMERID.Value,
                    customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                    deliverydate = p.DELIVERYDATE,
                    amount = p.AMOUNT,
                    pattern = p.PATTERN,
                    priority = p.PRIORITY.Value,
                    producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                    customerPattern = p.CUSTOMERSTYLENO,
                    merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,//c => ((int?)c.AMOUNT) ?? 0).GetValueOrDefault(0m
                    //CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                    //StarDate = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID).Min(c => c.PRODUCTIONDATE),
                    confirmdate = p.PLACEORDERDATE
                }).ToList();
            }

            //if (ls != null && ls.Count > 0)
            //{
            //    foreach (var item in ls)
            //    {
            //        item.Processes = GetPoProcess(item.id);
            //    }
            //}

            return ls;

        }

        /// <summary>
        /// 生产单所有非排产工序节点
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPoProcess")]
        public List<SelectModel> GetPoProcess(int poId)
        {
            List<SelectModel> ls = new List<SelectModel>();
            try
            {
                ls = db.POes.Where(c => c.ID == poId).FirstOrDefault().PROCESSROUTE.PROCESSROUTENODEs.Where(c => c.PROCESS.ISPRIMARY == 0).Select(p => new ViewModels.SelectModel
                {
                    itemId = p.PROCESSID,
                    itemName = p.PROCESS.NAME
                }).ToList();
            }
            catch (Exception)
            {

            }

            return ls;
        }

        /// <summary>
        /// 获取非排产颜色尺码的录入数据
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <param name="processId">工序id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetScheduleData")]
        public ProDataCompletedNP GetScheduleData(int poid, int processId)
        {         
            ProDataCompletedNP pnp = new ProDataCompletedNP();
            List<ProDataCompleted> ls = new List<ProDataCompleted>();
            try
            {
                ls = db.PODETAILs.Where(c => c.POID == poid).Select(p => new ProDataCompleted()
                {
                    Color = p.POCOLOR,
                    SizeCompletes = p.POSIZEDETAILs.Select(t => new ProSizeCompleted()
                    {
                        Size = t.POSIZE,
                        PlanAmount = t.AMOUNT,
                        CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(m => m.POID == poid && m.POCOLOR == p.POCOLOR &&
                        m.POSIZE == t.POSIZE && m.PROCESSID == processId).Select(c => c.AMOUNT).DefaultIfEmpty(0).Sum(),
                    }).ToList()
                }).ToList();

                pnp.ProDataCompletedList = ls;
                pnp.ProcessId = processId;
                pnp.CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(m => m.POID == poid && m.PROCESSID == processId).Select(c => c.AMOUNT).DefaultIfEmpty(0).Sum();

                return pnp;
            }
            catch (Exception e)
            {
            }

            return pnp;
        }

        /// <summary>
        /// 非排产工序录入选车间/产线选项数据
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetScheduleWorkshop")]
        public ScheduleWorkshop GetScheduleWorkshop(int poid)
        {
            ScheduleWorkshop swList = new ScheduleWorkshop();

            swList.ScheduleWorkshopType = StaticData.OthProcessAddType;

            string strSql = string.Format(@" SELECT d.ID groupId,d.GROUPNAME,c.ID lineId,c.NAME lineName FROM dbo.FACILITY c LEFT JOIN dbo.WORKSHOP d ON c.GROUPNAME=d.GROUPNAME WHERE c.ID IN(
SELECT DISTINCT a.FACILITYID FROM  [dbo].[PRODUCTIONEVENT] a WHERE a.ID IN(
SELECT DISTINCT  b.PRODUCTIONEVENTID FROM dbo.PRODUCTIONEVENTDETAIL b 
WHERE b.POID={0}))", poid);

            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
                if (table != null && table.Rows.Count > 0)
                {
                    var groups = from c in table.AsEnumerable() group c by new { groupId = c.Field<int>("groupId"), groupName = c.Field<string>("GROUPNAME") } into g select new { g.Key.groupId, g.Key.groupName, g };

                    if (groups.Count() > 0)
                    {
                        swList.Shops = new List<Workshop>();
                        foreach (var item in groups)
                        {
                            Workshop workshop = new Workshop();
                            workshop.WId = item.groupId;
                            workshop.ShopName = item.groupName;
                            workshop.Lines = item.g.AsEnumerable().Select(p => new Line() { LId = p.Field<int>("lineId"), LineName = p.Field<string>("lineName") }).DefaultIfEmpty().ToList();
                            swList.Shops.Add(workshop);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return swList;
            }

            return swList;
        }

        /// <summary>
        /// 添加非排序更新生产日进度(按颜色尺码)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Schedule/AddScheduleDaily")]
        public ResultModel AddScheduleDaily(ProScheduleAdd data)
        {
            ResultModel rm = new ResultModel();
            try
            {
                string lotNO = DateTime.Now.ToString("MMddHHmmss") + (new Random().Next(1, 999)).ToString();

                int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", data.ProDatas.Count());
                if (dataId <= 1)
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "创建表ID失败"));
                    rm.ErrMessage.Add(new LangMessage("en", "Create table ID failure"));
                    return rm;
                }
                var ls = new List<Models.PRODUCTIONSCHEDULE>();
                foreach (var item in data.ProDatas)
                {
                    Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
                    schedule.ID = dataId;
                    schedule.POID = data.PoId;
                    schedule.PRODUCTIONEVENTID = 0;
                    schedule.PROCESSID = data.ProceesId;
                    schedule.PRODUCTIONDATE = Convert.ToDateTime(data.ProDate);
                    schedule.AMOUNT = item.Amount;
                    schedule.POCOLOR = item.Color;
                    schedule.POSIZE = item.Size;
                    schedule.WORKSHOPID = -1;
                    schedule.DESCRIPTION = "app add";
                    schedule.LOTNO = lotNO;

                    ls.Add(schedule);

                    dataId++;
                }
                db.PRODUCTIONSCHEDULEs.AddRange(ls);
                db.SaveChanges();
                rm.IsSuccess = 1;


            }
            catch (Exception ex)
            {
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
            }

            return rm;
        }

        /// <summary>
        /// 添加非排序更新生产日进度（按生产单）
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <param name="processId">工序ID</param>
        /// <param name="proDate">生产日期</param>
        /// <param name="amount">数量</param>
        [HttpPost, Route("api/Schedule/AddScheduleByPo")]
        public ResultModel AddScheduleByPo(int poId, int processId, DateTime proDate, int amount)
        {
            ResultModel rm = new ResultModel();

            try
            {
                string lotNO = DateTime.Now.ToString("MMddHHmmss") + (new Random().Next(1, 999)).ToString();

                int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", 1);
                if (dataId <= 1)
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "创建表ID失败"));
                    rm.ErrMessage.Add(new LangMessage("en", "Create table ID failure"));
                    return rm;
                }

                Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
                schedule.ID = dataId;
                schedule.POID = poId;
                schedule.PRODUCTIONEVENTID = 0;
                schedule.PROCESSID = processId;
                schedule.PRODUCTIONDATE = proDate;
                schedule.AMOUNT = amount;
                schedule.WORKSHOPID = -1;
                schedule.DESCRIPTION = "app add";
                schedule.LOTNO = lotNO;
                db.PRODUCTIONSCHEDULEs.Add(schedule);

                db.SaveChanges();
                rm.IsSuccess = 1;
            }
            catch (Exception ex)
            {
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
            }


            return rm;
        }

        #endregion

        #region 排产工序

        /// <summary>
        /// 获取产线默认工时
        /// </summary>
        /// <param name="lineId">产线id</param>
        /// <param name="proDate">生产日期</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetLineDefaultDuration")]
        public LineDefaultDuration GetLineDefaultDuration(int lineId, DateTime proDate)
        {
            LineDefaultDuration ldd = new LineDefaultDuration();

            AMOData.VirtualFacility vf = new AMOData.VirtualFacility(AMO.BLL.Facility.GetFromID(lineId));
            vf.VirtualCalendar = new AMOData.VirtualCalendar();
            vf.VirtualCalendar.LoadCalendar(new List<AMOData.VirtualFacility>() { vf });
            DateTime startD = Convert.ToDateTime(proDate).Date;
            DateTime endD = startD.AddDays(1).AddMinutes(-1);

            ldd.LineId = lineId;
            ldd.ProDate = proDate;
            ldd.WorkHours = vf.CalculateWorkHours(startD, endD);

            return ldd;
        }

        /// <summary>
        /// 排产工序进度录入查询列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="planTime">计划区间包含日期</param>
        /// <param name="productTypeIds">产品大类id集</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPlanPoes")]
        public List<PlanPo> GetPlanPoes(int pageIndex, int pageSize, string planTime, string productTypeIds = "", string fids = "", string wsids = "", string code = "")
        {
            List<int> TypeLs = Commons.GetProductTypeList(productTypeIds);

            List<int> fidLs = new List<int>();
            List<int> lineList = new List<int>();

            //是否按车间查询
            bool isWorkShop = false;
            if (!string.IsNullOrEmpty(wsids)) //车间查询
            {
                isWorkShop = true;
                string[] idsArry = wsids.Split(',');
                foreach (var item in idsArry)
                {
                    fidLs.Add(Convert.ToInt32(item));
                }

                //车间下的生产线id
                lineList = (from a in db.FACILITies join b in db.WORKSHOPs on a.GROUPNAME equals b.GROUPNAME where fidLs.Contains(b.ID) select a.ID).ToList();
            }
            else //工厂查询
            {
                if (!string.IsNullOrEmpty(fids))
                {
                    string[] fidsArry = fids.Split(',');
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(Convert.ToInt32(item));
                    }
                }
                else
                {
                    int[] fidsArry = GetAllowedFactoryIDs();
                    foreach (var item in fidsArry)
                    {
                        fidLs.Add(item);
                    }
                }
            }

            bool isTime = false;
            DateTime thisDate = DateTime.Now.Date;
            if (DateTime.TryParse(planTime, out thisDate))
            {
                isTime = true;
            }
            DateTime endDate = thisDate.AddDays(1);

            //生产计划中包含多个生产单
            //var miutiPoPlan = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.POID == -1 && c.PRODUCTIONEVENT.STATUS == 0 && c.PO.STATUS == 0).Select(c => c.POID).Distinct();
            var miutiPoPlan = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.POID == -1 && c.PRODUCTIONEVENT.STATUS == 0 && c.PO.STATUS == 0 &&
                (isWorkShop ? fidLs.Contains(c.PRODUCTIONEVENT.FACILITYID) : true) &&
                (isTime ? (c.PRODUCTIONEVENT.STARTTIME < endDate && thisDate <= c.PRODUCTIONEVENT.ENDTIME) : true)).GroupBy(x =>
            new { x.PROCESSROUTEID, x.POID }).Select(x => new
            {
                ID = x.Key.POID,
                FACTORYID = x.FirstOrDefault().FACTORYID,
                NAME = x.FirstOrDefault().PRODUCTIONEVENT.FACILITY.NAME,
                STARTTIME = x.FirstOrDefault().PRODUCTIONEVENT.STARTTIME,
                ENDTIME = x.FirstOrDefault().PRODUCTIONEVENT.ENDTIME,
            }).ToList();

            var dd = miutiPoPlan.Count;

            if (miutiPoPlan != null && miutiPoPlan.Count > 0)
            {
                var datas = ((from a in db.PRODUCTIONEVENTs.Where(c => c.POID != -1 && c.STATUS == 0 && (isTime ? (thisDate <= c.ENDTIME && endDate > c.STARTTIME) : true) && (isWorkShop ? lineList.Contains(c.FACILITYID) : true))
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code))) && c.STATUS == 0 &&
                              (isWorkShop ? true : fidLs.Contains(c.FACTORYID.Value)) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)))
                              on a.POID equals p.ID
                              select new ViewModels.PlanPo
                              {
                                  factoryid = p.FACTORYID.Value,
                                  ProductionEventID = a.ID,
                                  LineID = a.FACILITYID,
                                  LineName = a.FACILITY.NAME,
                                  StartTime = a.STARTTIME,
                                  id = p.ID,
                                  code = p.CODE,
                                  customerid = p.CUSTOMERID.Value,
                                  customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                  deliverydate = p.DELIVERYDATE,
                                  workshopDays = p.FACTORYVIEWDATEDAYS == null ? 0 : p.FACTORYVIEWDATEDAYS.Value,
                                  amount = p.AMOUNT,
                                  planAmount = db.PRODUCTIONEVENTDETAILs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID == a.ID).Select(g => g.PLANAMOUNT).DefaultIfEmpty().Sum(),
                                  pattern = p.PATTERN,
                                  priority = p.PRIORITY.Value,
                                  producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                  merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                                  StarDate = a.STARTTIME,
                                  EndDate = a.ENDTIME,
                                  customerPattern = p.CUSTOMERSTYLENO,
                                  CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                              }).Union(from a in miutiPoPlan
                                       join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) && c.STATUS == 0 &&
                                       fidLs.Contains(c.FACTORYID.Value) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value))) on a.ID equals p.ID
                                       select new ViewModels.PlanPo
                                       {
                                           factoryid = p.FACTORYID.Value,
                                           ProductionEventID = a.ID,
                                           LineID = a.FACTORYID.Value,
                                           LineName = a.NAME,
                                           StartTime = a.STARTTIME,
                                           id = p.ID,
                                           code = p.CODE,
                                           customerid = p.CUSTOMERID.Value,
                                           customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                           deliverydate = p.DELIVERYDATE,
                                           workshopDays = p.FACTORYVIEWDATEDAYS == null ? 0 : p.FACTORYVIEWDATEDAYS.Value,
                                           amount = p.AMOUNT,
                                           planAmount = db.PRODUCTIONEVENTDETAILs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID == a.ID).Select(g => g.PLANAMOUNT).DefaultIfEmpty().Sum(),
                                           pattern = p.PATTERN,
                                           priority = p.PRIORITY.Value,
                                           producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                           merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                                           StarDate = a.STARTTIME,
                                           EndDate = a.ENDTIME,
                                           customerPattern = p.CUSTOMERSTYLENO,
                                           CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                                       })).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                return datas.ToList();
            }
            else
            {

                var datas = ((from a in db.PRODUCTIONEVENTs.Where(c => c.POID != -1 && c.STATUS == 0 && (isTime ? (thisDate <= c.ENDTIME && endDate > c.STARTTIME) : true) && (isWorkShop ? lineList.Contains(c.FACILITYID) : true))
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code))) && c.STATUS == 0 &&
                              (isWorkShop ? true : fidLs.Contains(c.FACTORYID.Value)) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)))
                              on a.POID equals p.ID
                              select new ViewModels.PlanPo
                              {
                                  factoryid = p.FACTORYID.Value,
                                  ProductionEventID = a.ID,
                                  LineID = a.FACILITYID,
                                  LineName = a.FACILITY.NAME,
                                  StartTime = a.STARTTIME,
                                  id = p.ID,
                                  code = p.CODE,
                                  customerid = p.CUSTOMERID.Value,
                                  customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                  deliverydate = p.DELIVERYDATE,
                                  workshopDays = p.FACTORYVIEWDATEDAYS == null ? 0 : p.FACTORYVIEWDATEDAYS.Value,
                                  amount = p.AMOUNT,
                                  planAmount = db.PRODUCTIONEVENTDETAILs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID == a.ID).Select(g => g.PLANAMOUNT).DefaultIfEmpty().Sum(),
                                  pattern = p.PATTERN,
                                  priority = p.PRIORITY.Value,
                                  producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                  merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                                  StarDate = a.STARTTIME,
                                  EndDate = a.ENDTIME,
                                  customerPattern = p.CUSTOMERSTYLENO,
                                  CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(o => o.POID == p.ID && o.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                              })).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                return datas.ToList();
            }
        }

        private DateTime GetWorkerDate(AMOData.VirtualProdEvent pe)
        {
            AMOData.VirtualFacility vf = new AMOData.VirtualFacility(AMO.BLL.Facility.GetFromID(pe.FacilityID));
            vf.VirtualCalendar = new AMOData.VirtualCalendar();
            vf.VirtualCalendar.LoadCalendar(new List<AMOData.VirtualFacility>() { vf });

            for (var i = 1; i < 10; i++)
            {
                var prevWorkerDate = DateTime.Now.Date.AddDays(-i);
                DateTime startD = prevWorkerDate.Date;
                DateTime endD = startD.AddDays(1).AddMinutes(-1);
                var duration = vf.CalculateWorkHours(startD, endD);
                if (duration > 0)
                {
                    return prevWorkerDate;
                }
            }
            return DateTime.Now.Date.AddDays(-1);
        }

        /// <summary>
        /// 获取每日进度颜色尺码数据
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <param name="planId">排产计划id</param>
        /// <param name="lineId">产线id</param>
        /// <param name="proTime">生产日期</param>
        /// <returns></returns>
        [HttpGet, Route("api/Schedule/GetPlanScheduleData")]
        public ProDataCompletedNP GetPlanScheduleData(int poid, int planId, int lineId, DateTime? proTime = null)
        {
            ProDataCompletedNP pnp = new ProDataCompletedNP();
            pnp.ProDataCompletedList = new List<ProDataCompleted>();
            pnp.CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == poid && c.PRODUCTIONEVENTID == planId).Select(p => p.AMOUNT).DefaultIfEmpty(0).Sum();

            try
            {
                if (proTime==null)
                {
                    var ProdEvent = AMO.BLL.ProductionEvent.GetFromID(planId);
                    AMOData.VirtualProdEvent vpe = new AMOData.VirtualProdEvent(ProdEvent, DateTime.Now);
                    DateTime dt = GetWorkerDate(vpe);
                    pnp.ProTime = dt;
                }
                else
                {
                    pnp.ProTime = proTime.Value;
                }

                AMOData.VirtualFacility vf = new AMOData.VirtualFacility(AMO.BLL.Facility.GetFromID(lineId));
                vf.VirtualCalendar = new AMOData.VirtualCalendar();
                vf.VirtualCalendar.LoadCalendar(new List<AMOData.VirtualFacility>() { vf });
                DateTime startD = pnp.ProTime.Date;
                DateTime endD = startD.AddDays(1).AddMinutes(-1);

                pnp.WorkHours = vf.CalculateWorkHours(startD, endD);

                var CFacility = AMO.BLL.Facility.GetFromID(lineId);
                pnp.WorkerAmount = CFacility.WorkerNumber;



                pnp.ProDataCompletedList = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENTID == planId && c.POID == poid).GroupBy(x =>
                    new { x.POCOLOR }).Select(x => new ProDataCompleted()
                    {
                        Color = x.Key.POCOLOR,
                        SizeCompletes = x.Select(t => new ProSizeCompleted()
                        {
                            Size = t.POSIZE,
                            PlanAmount = t.PLANAMOUNT,
                            CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(m => m.PRODUCTIONEVENTID == planId && m.POID == poid && m.POCOLOR == t.POCOLOR && m.POSIZE == t.POSIZE).Select(c => c.AMOUNT).DefaultIfEmpty(0).Sum()
                        }).ToList()
                    }).ToList();
            }
            catch (Exception)
            {
                return null;
            }

            return pnp;
        }

        /// <summary>
        /// 添加每日进度数(按颜色尺码)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Schedule/AddPlanScheduleDaily")]
        public ResultModel AddPlanScheduleDaily(ProPlanScheduleAdd data)
        {
            ResultModel rm = new ResultModel();
            try
            {
                string lotNO = DateTime.Now.ToString("MMddHHmmss") + (new Random().Next(1, 999)).ToString();

                if (data.WorkHours <= 0)
                {
                    AMOData.VirtualFacility vf = new AMOData.VirtualFacility(AMO.BLL.Facility.GetFromID(data.LineId));
                    vf.VirtualCalendar = new AMOData.VirtualCalendar();
                    vf.VirtualCalendar.LoadCalendar(new List<AMOData.VirtualFacility>() { vf });
                    DateTime startD = Convert.ToDateTime(data.ProDate).Date;
                    DateTime endD = startD.AddDays(1).AddMinutes(-1);

                    data.WorkHours = vf.CalculateWorkHours(startD, endD);
                }
                if (data.WorkerAmount <= 0)
                {
                    var CFacility = AMO.BLL.Facility.GetFromID(data.LineId);
                    data.WorkerAmount = CFacility.WorkerNumber;
                }
                if (data.FPY <= 0)
                {
                    data.FPY = 100;
                }

                int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", data.ProDatas.Count());
                if (dataId <= 1)
                {
                    rm.ErrMessage.Add( new  LangMessage("cn", "创建表ID失败"));
                    rm.ErrMessage.Add(new LangMessage("en", "Create table ID failure"));
                    return rm;
                }

                var proEvent = db.PRODUCTIONEVENTs.Where(c => c.FACILITYID == data.LineId && c.PRODUCTIONEVENTDETAILs.Where(p => p.POID == data.PoId).Count() > 0);
                if (proEvent.Count() == 0)
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "创建表ID缺少对应的排产计划数据"));
                    rm.ErrMessage.Add(new LangMessage("en", "Lack of corresponding production planning data"));
                    return rm;
                }

                if (data.ProceesId == 0)
                {
                    data.ProceesId = db.PROCESSes.Where(c => c.ISPRIMARY == 1).FirstOrDefault().ID;
                }

                var addls = new List<Models.PRODUCTIONSCHEDULE>();
                foreach (var item in data.ProDatas)
                {
                    if (item.Amount>0)
                    {
                        Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
                        schedule.ID = dataId;
                        schedule.POID = data.PoId;
                        schedule.PRODUCTIONEVENTID = data.ProductionEventId;
                        schedule.PROCESSID = data.ProceesId;
                        schedule.PRODUCTIONDATE = Convert.ToDateTime(data.ProDate);
                        schedule.AMOUNT = item.Amount;
                        schedule.POCOLOR = item.Color;
                        schedule.POSIZE = item.Size;
                        schedule.DESCRIPTION = "app add";
                        schedule.UPDATEDATE = DateTime.Now;
                        schedule.DURATION = data.WorkHours;
                        schedule.WORKERS = data.WorkerAmount;
                        schedule.LOTNO = lotNO;
                        schedule.OVERTIME = 0;
                        schedule.DECWORKERS = 0;
                        //schedule.UPDATEUSER = 0;
                        schedule.FPY = data.FPY;

                        addls.Add(schedule);
                        dataId++;
                        
                    }

                }

                db.PRODUCTIONSCHEDULEs.AddRange(addls);

                //设置生产单是否生产完成
                if (data.IsProCompleted==1)
                {
                    var thisEvent = proEvent.Where(c => c.ID == data.ProductionEventId).FirstOrDefault();

                    var sumComleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.PRODUCTIONEVENTID == data.ProductionEventId && c.POID == data.PoId).Select(s => s.AMOUNT).DefaultIfEmpty().Sum();
                    if (thisEvent != null && (addls.Count>0 || sumComleteAmount > 0))
                    {
                        thisEvent.STATUS = 1;
                        thisEvent.UPDATEDATE = DateTime.Now;


                        if (db.PRODUCTIONEVENTs.Where(c => c.POID == data.PoId && c.ID != data.ProductionEventId && c.STATUS == 0).Any() == false)
                        {
                            var po = db.POes.FirstOrDefault(c => c.ID == data.PoId);
                            po.STATUS = 1;
                            po.UPDATEDATE = DateTime.Now;
                        }
                    }
                    else
                    {
                        rm.IsSuccess = 0;
                        rm.ErrMessage.Add(new LangMessage("cn", "生产日进度数量为必须大于0才能设置完成状态"));
                        rm.ErrMessage.Add(new LangMessage("en", "Daily producess quality  must be greater than 0 to be completed settings"));
                        return rm;
                    }



                }

                db.SaveChanges();
                rm.IsSuccess = 1;
            }
            catch (Exception ex)
            {
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
            }


            return rm;
        }

        /// <summary>
        /// 添加每日进度数（按生产单）
        /// </summary>
        /// <param name="ProductionEventId">排产计划ID</param>
        /// <param name="lineId">生产线ID</param>
        /// <param name="poId">生产单id</param>
        /// <param name="processId">工序ID</param>
        /// <param name="proDate">生产日期</param>
        /// <param name="amount">数量</param>
        /// <param name="isProCompleted">计划完成状态：1完成，0进行中</param>
        /// <param name="WorkerAmount">人工数</param>
        /// <param name="WorkHours">人工时</param>
        /// <param name="FPY">合格率（%）</param>
        [HttpPost, Route("api/Schedule/AddPlanScheduleByPo")]
        public ResultModel AddPlanScheduleByPo(int ProductionEventId, int lineId, int poId, int isProCompleted, DateTime? proDate = null, int amount = 0, int WorkerAmount = 0, double WorkHours = 0, double FPY = 100, int processId = 0)
        {
            ResultModel rm = new ResultModel();
            try
            {
                if (proDate == null)
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "请输入日期及完成数量"));
                    rm.ErrMessage.Add(new LangMessage("en", "Please enter the date and complete the number"));

                    return rm;
                }
                else if (amount > 0)
                {
                    if (processId == 0)
                    {
                        processId = db.PROCESSes.Where(c => c.ISPRIMARY == 1).FirstOrDefault().ID;
                    }

                    int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", 1);
                    if (dataId <= 1)
                    {
                        rm.ErrMessage.Add(new LangMessage("cn", "创建表ID失败"));
                        rm.ErrMessage.Add(new LangMessage("en", "Create table ID failure"));
                        return rm;
                    }

                    string lotNO = DateTime.Now.ToString("MMddHHmmss") + (new Random().Next(1, 999)).ToString();

                    if (WorkHours <= 0)
                    {
                        AMOData.VirtualFacility vf = new AMOData.VirtualFacility(AMO.BLL.Facility.GetFromID(lineId));
                        vf.VirtualCalendar = new AMOData.VirtualCalendar();
                        vf.VirtualCalendar.LoadCalendar(new List<AMOData.VirtualFacility>() { vf });
                        DateTime startD = Convert.ToDateTime(proDate).Date;
                        DateTime endD = startD.AddDays(1).AddMinutes(-1);

                        WorkHours = vf.CalculateWorkHours(startD, endD);
                    }
                    if (WorkerAmount <= 0)
                    {
                        var CFacility = AMO.BLL.Facility.GetFromID(lineId);
                        WorkerAmount = CFacility.WorkerNumber;
                    }
                    if (FPY <= 0)
                    {
                        FPY = 100;
                    }

                    Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
                    schedule.ID = dataId;
                    schedule.POID = poId;
                    schedule.PRODUCTIONEVENTID = ProductionEventId;
                    schedule.PROCESSID = processId;
                    schedule.PRODUCTIONDATE = proDate.Value;
                    schedule.AMOUNT = amount;
                    schedule.DURATION = WorkHours;
                    schedule.WORKERS = WorkerAmount;
                    schedule.FPY = FPY;
                    schedule.DESCRIPTION = "app add";
                    schedule.UPDATEDATE = proDate.Value;
                    schedule.LOTNO = lotNO;
                    schedule.OVERTIME = 0;
                    schedule.DECWORKERS = 0;

                    db.PRODUCTIONSCHEDULEs.Add(schedule);
                }

                if (isProCompleted == 1)
                {
                    var proEvent = db.PRODUCTIONEVENTs.Where(c => c.ID == ProductionEventId).FirstOrDefault();
                    var sumComleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.PRODUCTIONEVENTID == ProductionEventId && c.POID == poId).Select(s => s.AMOUNT).DefaultIfEmpty().Sum();
                    if (proEvent != null && (amount>0 || sumComleteAmount > 0))
                    {
                        proEvent.STATUS = 1;
                        proEvent.UPDATEDATE = proDate.Value;

                        if (db.PRODUCTIONEVENTs.Where(c => c.POID == poId && c.ID != ProductionEventId && c.STATUS == 0).Any()==false)
                        {
                            var po = db.POes.FirstOrDefault(c => c.ID == poId);
                            po.STATUS = 1;
                            po.UPDATEDATE = DateTime.Now;
                        }
                    }
                    else
                    {
                        rm.IsSuccess = 0;
                        rm.ErrMessage.Add(new LangMessage("cn", "生产日进度数量为必须大于0才能设置完成状态"));
                        rm.ErrMessage.Add(new LangMessage("en", "Before to set is completed, the number of production plans must be more than 0."));
                        return rm;
                    }
                }


                db.SaveChanges();
                rm.IsSuccess = 1;

            }
            catch (Exception ex)
            {
                rm.ErrMessage.Add(new LangMessage("cn", "操作失败：" + ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", "Err：" + ex.Message));
            }


            return rm;
        }


        ///// <summary>
        ///// 外协厂录入每日完成数量（按生产单）
        ///// </summary>
        ///// <param name="lineId">生产线ID</param>
        ///// <param name="poId">生产单id</param>
        ///// <param name="proDate">生产日期</param>
        ///// <param name="amount">数量</param>
        ///// <returns></returns>
        //[HttpPost, Route("api/Schedule/AddOutFactoryDailyByPo")]
        //public ResultModel AddOutFactoryDailyByPo(int lineId, int poId, DateTime proDate, int amount)
        //{
        //    ResultModel rm = new ResultModel();

        //    try
        //    {
        //        var process = db.PROCESSes.Where(c => c.ISPRIMARY == 1).FirstOrDefault();//产工序
        //        var proEvent = db.PRODUCTIONEVENTs.Where(c => c.FACILITYID == lineId && c.PRODUCTIONEVENTDETAILs.Where(p => p.POID == poId).Count() > 0);

        //        int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", 1);
        //        if (dataId <= 1)
        //        {
        //            rm.ErrMessage = "创建记录ID失败";
        //            return rm;
        //        }


        //        if (proEvent.Count() > 0)
        //        {
        //            Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
        //            schedule.ID = dataId;
        //            schedule.POID = poId;
        //            schedule.PRODUCTIONEVENTID = proEvent.First().ID;
        //            schedule.PROCESSID = process.ID;
        //            schedule.PRODUCTIONDATE = proDate;
        //            schedule.AMOUNT = amount;

        //            db.PRODUCTIONSCHEDULEs.Add(schedule);

        //            db.SaveChanges();
        //            rm.IsSuccess = 1;
        //        }
        //        else
        //        {
        //            rm.ErrMessage = "缺少对应的排产计划数据";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.ErrMessage = "操作失败：" + ex.Message;
        //    }


        //    return rm;
        //}

        ///// <summary>
        ///// 录入外协厂每日成品完成数(按颜色尺码)
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //[HttpPost, Route("api/Schedule/AddOutFactoryDaily")]
        //public ResultModel AddOutFactoryDaily(OutProScheduleAdd data)
        //{
        //    ResultModel rm = new ResultModel();

        //    try
        //    {
        //        var process = db.PROCESSes.Where(c => c.ISPRIMARY == 1).FirstOrDefault();//排产工序
        //        var proEvent = db.PRODUCTIONEVENTs.Where(c => c.FACILITYID == data.LineId && c.PRODUCTIONEVENTDETAILs.Where(p => p.POID == data.PoId).Count() > 0);
        //        if (proEvent.Count() == 0)
        //        {
        //            rm.ErrMessage = "缺少对应的排产计划数据";
        //            return rm;
        //        }

        //        int dataId = new BLL.ScheduleComm().CreateTbID("PRODUCTIONSCHEDULE", proEvent.Count());
        //        if (dataId <= 1)
        //        {
        //            rm.ErrMessage = "创建记录ID失败";
        //            return rm;
        //        }

        //        foreach (var item in data.ProDatas)
        //        {
        //            Models.PRODUCTIONSCHEDULE schedule = new Models.PRODUCTIONSCHEDULE();
        //            schedule.ID = dataId;
        //            schedule.POID = data.PoId;
        //            schedule.PRODUCTIONEVENTID = proEvent.First().ID;
        //            schedule.PROCESSID = process.ID;
        //            schedule.PRODUCTIONDATE = data.ProDate;
        //            schedule.AMOUNT = item.Amount;

        //            db.PRODUCTIONSCHEDULEs.Add(schedule);

        //            dataId++;
        //        }

        //        db.SaveChanges();
        //        rm.IsSuccess = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.ErrMessage = "操作失败：" + ex.Message;
        //    }
        //    return rm;
        //}



        #endregion
    }
}
