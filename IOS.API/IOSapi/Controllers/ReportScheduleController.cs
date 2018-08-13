using AMO.Reports;
using AMOData;
using IOS.DBUtility;
using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 滚动生产排期表
    /// </summary>
    public class ReportScheduleController : BaseApiController
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
        /// 产线(列表)
        /// </summary>
        /// <param name="starDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="fid">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportSchedule/GetLineSchedule")]
        private List<ViewModels.LineSchedule> GetLineSchedule(DateTime starDate, DateTime endDate, int fid)
        {
            var ls = db.PRODUCTIONEVENTDETAILs.Where(c => c.FACTORYID == fid && c.DELIVERYDATE >= starDate && c.DELIVERYDATE <= endDate);

            List<ViewModels.LineSchedule> datas = new List<ViewModels.LineSchedule>();

            datas = (from a in db.PRODUCTIONEVENTDETAILs.Where(c => c.FACTORYID == fid && c.DELIVERYDATE >= starDate && c.DELIVERYDATE <= endDate)
                     group a by new { a.FACTORYID, a.FACTORY.NAME, a.PRODUCTIONEVENT.FACILITYID } into g
                     select new ViewModels.LineSchedule()
                     {
                         amount = g.Sum(c => c.AMOUNT),
                         planAmount = g.Sum(c => c.PLANAMOUNT),
                         factoryName = g.Key.NAME,
                         lineName = db.FACILITies.Where(c => c.ID == g.Key.FACILITYID).FirstOrDefault().NAME,
                         fid = g.Key.FACTORYID.Value
                     }).ToList();

            return datas;

        }

        /// <summary>
        /// 滚动生产排期表数据（按生产单排产计划划分）
        /// </summary>
        /// <param name="factoryid">工厂id</param>
        /// <param name="pocode">生产单号</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportSchedule/GetPoSchedule")]
        public RollPlanReportData GetPoSchedule(int factoryid,string pocode = "")
        {
            RollPlanReportData datas = new ViewModels.RollPlanReportData();

            var po = db.POes.Where(c => c.CODE == pocode).FirstOrDefault();
            if (po!=null)
            {
                DateTime startDate = DateTime.Now.AddMonths(-5);
                DateTime endDate = DateTime.Now.AddMonths(5);
                BLL.PoSchedule PoSchedule = new BLL.PoSchedule();

                datas.dt = PoSchedule.ReporDataList(factoryid, pocode, startDate, endDate);
                datas.total = datas.dt.Rows.Count;
            }


            return datas;
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

        [HttpGet, Route("api/ReportSchedule/GetPoes")]
        public List<ViewModels.Po> GetPoes(int pageIndex, int pageSize, DateTime star, DateTime end, int dateType = -1, string fids = "", string wsids = "", string code = "", int isreal = 0, int iscolor = 0)
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
                factoryid = p.FACTORYID.Value,
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




    }
}
