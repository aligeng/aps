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
    /// 日计划产量报表
    /// </summary>
    public class ReportDailyProdController : BaseApiController
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
        /// 工厂/产线日计划产量报表
        /// </summary>
        /// <param name="starDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="facid">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportDailyProd/GetDailyProd")]
        private List<DailyProdSum> GetDailyProd(DateTime starDate, DateTime endDate, int facid = -1)
        {
            List<DailyProdSum> ls = new List<DailyProdSum>();

            if (facid < 0)//按工厂统计
            {
                //var query = from a in db.POes
                //            group a by new { a.FACTORYID } into g
                //            select new DailyProdSum()
                //            {
                //                itemId = g.Key.FACTORYID.Value,
                //                amount = g.Sum(c => (from d in c.PRODUCTIONSCHEDULEs where d.PRODUCTIONDATE >= starDate && d.PRODUCTIONDATE < endDate select d.AMOUNT.Value).Sum()),
                //                name = g.Max(c => c.FACTORY.NAME)
                //            };

                var query = db.FACTORies.Select(c => new DailyProdSum()
                {
                    itemId = c.ID,
                    name = c.NAME,
                    amount = (from a in db.PRODUCTIONSCHEDULEs join b in db.PROCESSes on a.PROCESSID equals b.ID where b.ISPRIMARY == 1 && a.PO.FACTORYID == c.ID && a.PRODUCTIONDATE >= starDate && a.PRODUCTIONDATE < endDate select a.AMOUNT.Value).Sum()
                });

                ls = query.ToList();
            }
            else //按产线统计
            {
                var query = db.FACILITies.Where(c=>c.FACTORYID==facid).Select(c => new DailyProdSum()
                {
                    itemId = c.ID,
                    name = c.NAME,
                    amount = (from a in db.PRODUCTIONSCHEDULEs.Where(m => m.PROCESSID == 1 && m.PO.FACTORYID==facid && m.PRODUCTIONDATE >= starDate && m.PRODUCTIONDATE < endDate)
                              join o in db.PRODUCTIONEVENTs on a.PRODUCTIONEVENTID equals o.ID
                              where o.FACILITYID == c.ID 
                              select a.AMOUNT).Sum()
                    //amount = (from a in db.PRODUCTIONSCHEDULEs.Where(m=>m.PROCESSID==1)
                    //          join b in db.PROCESSes.Where(i=>i.ISPRIMARY==1) on a.PROCESSID equals b.ID
                    //          join o in db.PRODUCTIONEVENTs on a.PRODUCTIONEVENTID equals o.ID
                    //          where b.ISPRIMARY == 1 && a.PO.FACTORYID == facid && a.PRODUCTIONDATE >= starDate && o.FACILITYID==c.ID && a.PRODUCTIONDATE < endDate
                    //          select a.AMOUNT.Value).Sum()
                });

                ls = query.OrderByDescending(c=>c.amount).ToList();
            }
            return ls;
        }

        /// <summary>
        /// 生产计划日产量数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="starDate">车缝开始日期</param>
        /// <param name="endDate">车缝结束日期</param>
        /// <param name="fids">工厂id</param>
        /// <param name="wsids">车间id</param>
        /// <param name="lineids">产线id</param>
        /// <param name="code">生产单编号</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportDailyProd/GetPoPlans")]
        public List<PoDailyProd> GetPoPlans(int pageIndex, int pageSize, DateTime starDate, DateTime endDate, string fids = "", string wsids = "", string lineids = "", string code = "")
        {

            List<ViewModels.PoDailyProd> PlanPoLs = new List<PoDailyProd>();

            try
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


                var miutiPoPlan = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.POID == -1 && c.UPDATEDATE > starDate && c.UPDATEDATE < endDate).GroupBy(x =>
               new { x.PROCESSROUTEID, x.POID }).Select(x => x.First()).ToList();

                PlanPoLs = ((from a in db.PRODUCTIONEVENTs.Where(c => c.POID != -1 && c.STARTTIME > starDate && c.ENDTIME < endDate)
                             join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) &&
                             fidLs.Contains(c.FACTORYID.Value)) on a.POID equals p.ID
                             select new ViewModels.PoDailyProd
                             {
                                 productionEventID = a.ID,
                                 lineID = a.FACILITYID,
                                 lineName = a.FACILITY.NAME,
                                 poId = p.ID,
                                 pocode = p.CODE,
                                 patternNo = p.PATTERN,
                                 amount = p.AMOUNT,
                                 DeliveryDate = p.DELIVERYDATE,
                                 customer = p.CUSTOMER.NAME,
                                 dailyAmount = p.PRODUCTIONSCHEDULEs.Where(c => c.PROCESS.ISPRIMARY == 1 && c.PRODUCTIONDATE >= starDate
                                   && c.PRODUCTIONDATE < endDate).GroupBy(c => c.PRODUCTIONDATE).Select(o => new DailyAmount()
                                   {
                                       amount = o.Sum(c => c.AMOUNT.Value),
                                       Date = o.Key
                                   }).ToList()
                             }).Union(from a in miutiPoPlan
                                      join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) &&
                                     fidLs.Contains(c.FACTORYID.Value)) on a.POID equals p.ID
                                      select new ViewModels.PoDailyProd
                                      {
                                          productionEventID = a.ID,
                                          lineID = a.FACTORYID.Value,
                                          lineName = a.PRODUCTIONEVENT.FACILITY.NAME,
                                          poId = p.ID,
                                          pocode = p.CODE,
                                          patternNo = p.PATTERN,
                                          amount = p.AMOUNT,
                                          DeliveryDate = p.DELIVERYDATE,
                                          customer = p.CUSTOMER.NAME,
                                          dailyAmount = p.PRODUCTIONSCHEDULEs.Where(c => c.PROCESS.ISPRIMARY == 1 && c.PRODUCTIONDATE >= starDate
                                            && c.PRODUCTIONDATE < endDate).GroupBy(c => c.PRODUCTIONDATE).Select(o => new DailyAmount()
                                            {
                                                amount = o.Sum(c => c.AMOUNT.Value),
                                                Date = o.Key
                                            }).ToList()

                                      })).OrderBy(c => c.productionEventID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
            }
            catch (Exception)
            {

            }
         
            return PlanPoLs;
        }


    }
}
