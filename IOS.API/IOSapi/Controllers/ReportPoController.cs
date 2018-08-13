using AMOData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AMO.Model;
using IOSapi.Models;
using Swashbuckle.Swagger;
using IOSapi.ViewModels;
using System.Data.Entity;

namespace IOSapi.Controllers
{
    //test code:D20170113-02,123
    //poid:18822

    /// <summary>
    /// 生产单明细分析
    /// </summary>
    [ControllerGroup("报表统计","info")]
    public class ReportPoController : ApiController
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

        #region 生产单详细报表

        // GET api/<controller>/5
        /// <summary>
        /// 生产单明细报表(细分)
        /// </summary>
        /// <param name="poid">生产单ID</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetPoDetaile")]
        public List<ViewModels.PoDetaile> GetPoDetaile(int poid)
        {
            var po = db.POes.FirstOrDefault(c => c.ID == poid);
            if (po != null)
            {
                return po.PODETAILs.Select(p => new ViewModels.PoDetaile
                {
                    id = p.ID,
                    amount = p.AMOUNT,
                    deliverydate = p.DELIVERYDATE,
                    description = p.DESCRIPTION,
                    excountry = p.EXCOUNTRY,
                    extype = p.EXCOUNTRY,
                    pocolor = p.POCOLOR,
                    posize = "",
                    receiveDate = p.DELIVERYDATE.AddDays(-10)
                }).ToList();
            }
            else
                return null;

        }

        /// <summary>
        /// 获取生产单工序节点
        /// </summary>
        /// <param name="poId">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetPoProcess")]
        public List<SelectModel> GetPoProcess(int poId)
        {
            List<SelectModel> ls = new List<ViewModels.SelectModel>();
            try
            {
                var po = db.POes.Where(t => t.ID == poId).FirstOrDefault();
                if (po != null)
                {
                    ls = db.PROCESSROUTENODEs.Where(c => c.PROCESSROUTEID == po.PROCESSROUTEID).Select(p => new SelectModel()
                    {
                        itemId = p.PROCESSID,
                        itemName = p.PROCESS.NAME
                    }).ToList();
                }

            }
            catch (Exception)
            {
                return null;
            }


            return ls;
        }


        /// <summary>
        /// 获取生产单工序每日进度报表
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <param name="pcid">工序id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetProductionSchedules")]
        public List<ViewModels.ProdSchedule> GetProductionSchedules(int poid, int pcid)
        {

            var query = from p in db.PRODUCTIONSCHEDULEs
                        where p.POID == poid
                        where p.PROCESSID == pcid
                        group p by new { OperateDate = DbFunctions.TruncateTime(p.PRODUCTIONDATE) } into g
                        select new ViewModels.ProdSchedule
                        {
                            productiondate = g.Key.OperateDate.Value,
                            amount = g.Sum(m => m.AMOUNT).Value
                        };

            return query.OrderBy(c=>c.productiondate).ToList();
        }

        /// <summary>
        /// 生产单关键事件报表
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetPoEvent")]
        public List<ViewModels.PoEvent> GetPoEvent(int poid)
        {
            var ls = db.POEVENTs.Where(c => c.POID == poid);

            return ls.Select(p => new ViewModels.PoEvent
            {
                id = p.ID,
                description = p.DESCRIPTION,
                duration = p.DURATION.Value,
                enddate = p.ENDDATE.Value,
                eventflownodeid = p.EVENTFLOWNODEID,
                eventflownodeName = p.EVENTFLOWNODE.EVENTFLOW.NAME,
                isaffectplan = p.ISAFFECTPLAN.Value,
                standardtime = p.STANDARDTIME.Value,
                startdate = p.STARTDATE.Value

            }).ToList();
        }

        #endregion

        /// <summary>
        /// 生产单计划
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetProPlan")]
        public List<PoProPlan> GetProPlan(int poid)
        {
            List<PoProPlan> ls = new List<ViewModels.PoProPlan>();

            var query = db.PRODUCTIONEVENTDETAILs.Where(c => c.PRODUCTIONEVENT.STATUS == 0 && c.POID == poid).GroupBy(x =>
                   new { x.PRODUCTIONEVENTID }).Select(p => new PoProPlan()
                   {

                       Duration = p.First().PRODUCTIONEVENT.DURATION,
                       EndTime = p.First().PRODUCTIONEVENT.ENDTIME,
                       StarTime = p.First().PRODUCTIONEVENT.STARTTIME,
                       LineName = p.First().PRODUCTIONEVENT.FACILITY.NAME,
                       PlanAmount = p.Sum(c => c.PLANAMOUNT),
                       FinishAmount = p.Sum(c => c.FINISHEDAMOUNT.Value),
                       PlanId = p.Key.PRODUCTIONEVENTID,
                       Remark = p.First().PRODUCTIONEVENT.DESCRIPTION,
                       Status = 0
                   });

            ls = query.ToList();

            //PoProPlan pp = new ViewModels.PoProPlan()
            //{
            //    Duration = 6,
            //    EndTime = Convert.ToDateTime("2018-03-10"),
            //    StarTime = Convert.ToDateTime("2017-12-10"),
            //    LineName = "02组",
            //    PlanAmount = 850,
            //    FinishAmount=200,
            //    PlanId = 261,
            //    Remark = "test",
            //    Status = "生产中"
            //};
            //pp.FinishEff = Math.Round(pp.FinishAmount / (double)pp.PlanAmount, 2);
            //ls.Add(pp);


            //PoProPlan pp2 = new ViewModels.PoProPlan()
            //{
            //    Duration = 6,
            //    EndTime = Convert.ToDateTime("2018-02-05"),
            //    StarTime = Convert.ToDateTime("2017-11-10"),
            //    LineName = "05组",
            //    PlanAmount = 2300,
            //    FinishAmount = 1200,
            //    PlanId = 183,
            //    Remark = "test2",
            //    Status = "生产中"
            //};
            //pp2.FinishEff = Math.Round(pp2.FinishAmount / (double)pp2.PlanAmount, 2);
            //ls.Add(pp2);

            return ls;

        }


        /// <summary>
        /// 生产单物料
        /// </summary>
        /// <param name="poid">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportPo/GetPoMateriel")]
        public List<PoMateriel> GetPoMateriel(int poid)
        {
            List<PoMateriel> ls = new List<ViewModels.PoMateriel>();

            PoMateriel pp = new ViewModels.PoMateriel()
            {
                Remark = "test",
                Status = "已到货",
                ArrivedAmount = 165,
                ArrivedTime = Convert.ToDateTime("2017-12-09"),
                BookTime = Convert.ToDateTime("2017-12-01"),
                ExpectTime = Convert.ToDateTime("2017-12-11"),
                Color = "粉红色",
                Dosage = 15,
                MatCode = "面料",
                MatName = "罗缎弹力",
                PurchCode = "EPMP1000116",
                Standard = "142CM",
                SupplierName = "test",
                Unit = "M"
            };
            ls.Add(pp);


            PoMateriel pp2 = new ViewModels.PoMateriel()
            {
                Remark = "test",
                Status = "已到货",
                ArrivedAmount = 165,
                ArrivedTime = Convert.ToDateTime("2017-10-25"),
                BookTime = Convert.ToDateTime("2017-10-22"),
                ExpectTime = Convert.ToDateTime("2017-10-25"),
                Color = "米白",
                Dosage = 15,
                MatCode = "橡筋",
                MatName = "弹力橡筋",
                PurchCode = "EPMP1000117",
                Standard = "15CM",
                SupplierName = "test",
                Unit = "M"
            };
            ls.Add(pp2);

            return ls;

        }


    }
}