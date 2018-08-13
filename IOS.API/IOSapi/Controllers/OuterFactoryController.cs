using IOSapi.BLL;
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
    /// 外发工厂接口
    /// </summary>
    public class OuterFactoryController : BaseApiController
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
        /// 生产单查询列表(包括未排产的生产单)
        /// </summary>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="star">交期查询开始时间</param>
        /// <param name="end">交期查询结束时间</param>
        /// <param name="productTypeIds">产品大类id集</param>
        /// <param name="processid">工序id集</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="code">生产单号/款号</param>
        /// <param name="customer">客户</param>
        /// <returns></returns>
        [HttpGet, Route("api/OuterFactory/GetPoes")]
        public List<Po> GetPoes(int pageIndex, int pageSize, DateTime? star = null, DateTime? end = null, string productTypeIds = "", string processid = "", string fids = "", string code = "", string customer = "")
        {
            //产品类型查询
            List<int> TypeLs = Commons.GetProductTypeList(productTypeIds);

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

            //客户查询
            List<int> cusIds = new List<int>();
            if (!string.IsNullOrEmpty(customer))
            {
                cusIds = db.CUSTOMERs.Where(c => c.NAME.Contains(customer)).Select(s => s.ID).ToList();
            }

            var ls = db.POes.Where(c => c.STATUS == 0 && (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code) || c.CUSTOMERSTYLENO.Contains(code))) &&
                ((star != null && end != null) == true ? (c.DELIVERYDATE >= star && c.DELIVERYDATE <= end) : true) && (string.IsNullOrEmpty(customer) ? true : cusIds.Contains(c.CUSTOMERID.Value)) &&
                (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)) && (string.IsNullOrEmpty(processid) ? true : routeIds.Contains(c.PROCESSROUTEID.Value)) &&
                fidLs.Contains(c.FACTORYID.Value)).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
            var datas = ls.Select(p => new ViewModels.Po
            {
                id = p.ID,
                code = p.CODE,
                customerid = p.CUSTOMERID.Value,
                customername = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault().NAME,
                deliverydate = p.DELIVERYDATE,
                amount = p.AMOUNT,
                pattern = p.PATTERN,
                customerPattern = p.CUSTOMERSTYLENO,
                priority = p.PRIORITY.Value,
                producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                CuttingAmount = (int)db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID && c.PROCESSID == StaticData.CuttingId).Select(s => s.AMOUNT.Value).DefaultIfEmpty(0).Sum()
                //CuttingAmount = p.ACTCUTQTY.Value < 0 ? 0 : p.ACTCUTQTY.Value
            });
            return datas.ToList();
        }



    }
}
