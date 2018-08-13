using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 生产的跟催报表
    /// </summary>
    public class ReportPoProgressController : BaseApiController
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

        #region 生产单跟催表

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="star">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="dateType">日期类型：1交期；2生产单确认日期</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号</param>
        /// <returns></returns>

        [HttpGet, Route("api/ReportPoProgress/GetPlanPoes")]

        public List<PlanPo> GetPlanPoes(int pageIndex, int pageSize, DateTime star, DateTime end, int dateType = -1, string fids = "", string wsids = "", string code = "")
        {
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

            if (star == null || end == null || star > end)
            {
                dateType = -1;
            }

            DateTime thisDate = DateTime.Now.Date;

            //生产计划中包含多个生产单
            //var miutiPoPlan = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.POID == -1 && c.PRODUCTIONEVENT.STATUS == 0 && c.PO.STATUS == 0).Select(c => c.POID).Distinct();
            var miutiPoPlan = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.POID == -1 && c.PRODUCTIONEVENT.STATUS == 0 && c.PO.STATUS == 0).GroupBy(x =>
            new { x.PROCESSROUTEID, x.POID }).Select(x => new
            {
                ID = x.Key.POID,
                FACTORYID = x.FirstOrDefault().FACTORYID,
                NAME = x.FirstOrDefault().PRODUCTIONEVENT.FACILITY.NAME,
                STARTTIME = x.FirstOrDefault().PRODUCTIONEVENT.STARTTIME
            }).ToList();

            var dd = miutiPoPlan.Count;

            if (miutiPoPlan != null && miutiPoPlan.Count > 0)
            {
                var datas = ((from a in db.PRODUCTIONEVENTs.Where(c => c.POID != -1 && c.STATUS == 0)
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) && c.STATUS == 0 &&
                              fidLs.Contains(c.FACTORYID.Value) && (dateType == 1 ? (c.DELIVERYDATE > star && c.DELIVERYDATE < end) : true) &&
                              (dateType == 2 ? (c.PLACEORDERDATE > star && c.PLACEORDERDATE < end) : true))
                              on a.POID equals p.ID
                              select new ViewModels.PlanPo
                              {
                                  ProductionEventID = a.ID,
                                  LineID = a.FACILITYID,
                                  LineName = a.FACILITY.NAME,
                                  StartTime = a.STARTTIME,
                                  id = p.ID,
                                  code = p.CODE,
                                  customerid = p.CUSTOMERID.Value,
                                  customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                  deliverydate = p.DELIVERYDATE,
                                  amount = p.AMOUNT,
                                  pattern = p.PATTERN,
                                  priority = p.PRIORITY.Value,
                                  producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                  merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME
                              }).Union(from a in miutiPoPlan
                                       join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) && c.STATUS == 0 &&
                                      fidLs.Contains(c.FACTORYID.Value) && (dateType == 1 ? (c.DELIVERYDATE > star && c.DELIVERYDATE < end) : true) &&
                                      (dateType == 2 ? (c.PLACEORDERDATE > star && c.PLACEORDERDATE < end) : true)) on a.ID equals p.ID
                                       select new ViewModels.PlanPo
                                       {
                                           ProductionEventID = a.ID,
                                           LineID = a.FACTORYID.Value,
                                           LineName = a.NAME,
                                           StartTime = a.STARTTIME,
                                           id = p.ID,
                                           code = p.CODE,
                                           customerid = p.CUSTOMERID.Value,
                                           customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                           deliverydate = p.DELIVERYDATE,
                                           amount = p.AMOUNT,
                                           pattern = p.PATTERN,
                                           priority = p.PRIORITY.Value,
                                           producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                           merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME
                                       })).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                return datas.ToList();
            }
            else
            {

                var datas = ((from a in db.PRODUCTIONEVENTs.Where(c => c.POID != -1 && c.STATUS == 0)
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) && c.STATUS == 0 &&
                              fidLs.Contains(c.FACTORYID.Value) && (dateType == 1 ? (c.DELIVERYDATE > star && c.DELIVERYDATE < end) : true) &&
                              (dateType == 2 ? (c.PLACEORDERDATE > star && c.PLACEORDERDATE < end) : true))
                              on a.POID equals p.ID
                              select new ViewModels.PlanPo
                              {
                                  ProductionEventID = a.ID,
                                  LineID = a.FACILITYID,
                                  LineName = a.FACILITY.NAME,
                                  StartTime = a.STARTTIME,
                                  id = p.ID,
                                  code = p.CODE,
                                  customerid = p.CUSTOMERID.Value,
                                  customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                                  deliverydate = p.DELIVERYDATE,
                                  amount = p.AMOUNT,
                                  pattern = p.PATTERN,
                                  priority = p.PRIORITY.Value,
                                  producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                  merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME
                              })).OrderBy(c => c.id).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
                return datas.ToList();
            }

        }

        /// <summary>
        /// 获取颜色尺码数据
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <param name="planId">排产计划id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPoProgress/GetPlanScheduleData")]
        public List<PoColorSizeCompleted> GetPlanScheduleData(int poid, int planId)
        {
            List<PoColorSizeCompleted> ls = new List<PoColorSizeCompleted>();
            try
            {


                ls = db.POSIZEDETAILs.Where(c => c.POID == poid).Select(x => new PoColorSizeCompleted()
                {
                    Color = x.PODETAIL.POCOLOR,
                    Size = x.POSIZE,
                    BillAmount = x.AMOUNT,
                    PlanAmount = db.PRODUCTIONEVENTDETAILs.Where(p => p.PRODUCTIONEVENTID == planId && p.POID == poid && p.POCOLOR == x.PODETAIL.POCOLOR && p.POSIZE == x.POSIZE).Sum(c => c.AMOUNT),
                    CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(m => m.PRODUCTIONEVENTID == planId && m.POID == poid && m.POCOLOR == x.PODETAIL.POCOLOR && m.POSIZE == x.POSIZE).Sum(c => c.AMOUNT.Value)
                }).ToList();


                //ls = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENTID == planId && c.POID == poid).Select(x => new PoColorSizeCompleted()
                //{
                //    Color = x.POCOLOR,
                //    Size = x.POSIZE,
                //    PlanAmount = x.AMOUNT,
                //    CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(m => m.PRODUCTIONEVENTID == planId && m.POID == poid && m.POCOLOR == x.POCOLOR && m.POSIZE == x.POSIZE).Sum(c => c.AMOUNT.Value)
                //}).ToList();
            }
            catch (Exception)
            {

            }

            return ls;
        }


        /// <summary>
        /// 获取生产单所有关键事件
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPoProgress/GetPoEvents")]
        public List<PoEvent> GetPoEvents(int poId)
        {
            var ls = db.POEVENTs.Where(c => c.POID == poId).Select(p => new PoEvent
            {
                id = p.ID,
                description = p.DESCRIPTION,
                duration = p.DURATION.Value,
                enddate = p.ENDDATE == null ? DateTime.MinValue : p.ENDDATE.Value,
                eventflownodeid = p.EVENTFLOWNODEID,
                eventflownodeName = p.EVENTFLOWNODE.CRITICALEVENT.NAME,
                isaffectplan = p.ISAFFECTPLAN.Value,
                standardtime = p.STANDARDTIME == null ? DateTime.MinValue : p.STANDARDTIME.Value,
                startdate = p.STARTDATE == null ? DateTime.MinValue : p.STARTDATE.Value,
            });

            foreach (var item in ls)
            {
                if (item.enddate != DateTime.MinValue && item.startdate != DateTime.MinValue)
                {
                    TimeSpan days = item.startdate.Value.Subtract(item.enddate.Value);
                    item.diffDays = days.Days;
                }
            }

            return ls.ToList();
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="star">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="dateType">日期类型：1交期；2生产单确认日期</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号</param>
        /// <param name="isreal">是否实单：0非实单；1实单</param>
        /// <param name="iscolor">是否区分颜色：0不区分；1区分</param>
        /// <returns></returns>

        [HttpGet, Route("api/ReportPoProgress/GetPoes")]
        private List<ViewModels.Po> GetPoes(int pageIndex, int pageSize, DateTime star, DateTime end, int dateType = -1, string fids = "", string wsids = "", string code = "", int isreal=0,int iscolor=0)
        {
            List<ViewModels.Po> ls = new List<ViewModels.Po>();

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

            if (star == null || end == null || star > end)
            {
                dateType = -1;
            }

            DateTime thisDate = DateTime.Now.Date;

            var query = db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) &&
            fidLs.Contains(c.FACTORYID.Value) && 
            (dateType == 1 ? (c.DELIVERYDATE > star && c.DELIVERYDATE < end) : true) &&
            (dateType == 2 ? (c.PLACEORDERDATE > star && c.PLACEORDERDATE < end) : true) &&
            c.ISFIRMORDER == isreal).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));

            ls = query.Select(p => new ViewModels.Po
            {
                id = p.ID,
                code = p.CODE,
                factoryid=p.FACTORYID.Value,
                customerid = p.CUSTOMERID.Value,
                customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                deliverydate = p.DELIVERYDATE,
                amount = p.AMOUNT,
                pattern = p.PATTERN,
                priority = p.PRIORITY.Value,
                producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME
            }).ToList();

            return ls;
        }

        /// <summary>
        /// 生产单工序节点完成情况
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPoProgress/GetPoProcessState")]
        private List<ViewModels.ProdProcess> GetPoProcessState(int poId)
        {
            List<ViewModels.ProdProcess> ls = new List<ViewModels.ProdProcess>();
            var po = db.POes.Where(c => c.ID == poId).FirstOrDefault();

            ls = db.PROCESSROUTENODEs.Where(c => c.PROCESSROUTEID == po.PROCESSROUTEID).Select(p => new ViewModels.ProdProcess
            {
                processid = p.PROCESSID,
                processname = p.PROCESS.NAME,
                amount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == po.ID && c.PROCESSID == p.PROCESSID).Sum(b => b.AMOUNT.Value),
                startdate = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == po.ID && c.PROCESSID == p.PROCESSID).Min(c => c.PRODUCTIONDATE)
            }).ToList();

            //ls= db.POes.Where(c => c.ID == poId).FirstOrDefault()
            return ls;
        }

        /// <summary>
        /// 生产单关键事件完成情况
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPoProgress/GetPoEventState")]
        private List<ViewModels.ProdEvent> GetPoEventState(int poId)
        {
            var po = db.POes.Where(c => c.ID == poId).FirstOrDefault();

            var eflow = db.EVENTFLOWs.Where(c => c.ID == po.EVENTFLOWID).FirstOrDefault();

            return eflow.EVENTFLOWNODEs.Where(c => c.CRITICALEVENTID > 0).Select(p => new ViewModels.ProdEvent
            {
                evtid = p.CRITICALEVENT.ID,
                evtname = p.CRITICALEVENT.NAME,
            }).ToList();
        }

        #endregion
    }
}
