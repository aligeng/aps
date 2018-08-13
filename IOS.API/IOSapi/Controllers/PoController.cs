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
    /// 生产单信息
    /// </summary>
    public class PoController : ApiController
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
        /// 模糊搜索
        /// </summary>
        /// <param name="code">生产单号或款号</param>
        /// <param name="type">指定code搜索类型：0生产单号或款号；1生产单号；2款号</param>
        /// <returns></returns>
        public List<ViewModels.PoSearch> GetPoSearch(string code,int type=0)
        {
            List<PoSearch> ls = new List<PoSearch>();
            switch (type)
            {
                case 1:
                    ls = db.POes.Where(c => c.CODE.Contains(code)).Take(10).Select(p => new PoSearch() { PoId = p.ID, PoCode = p.CODE, PatternNo = p.PATTERN }).ToList();
                    break;
                case 2:
                    ls = db.POes.Where(c => c.PATTERN.Contains(code)).Take(10).Select(p => new PoSearch() { PoId = p.ID, PoCode = p.CODE, PatternNo = p.PATTERN }).ToList();
                    break;
                default:
                    ls = db.POes.Where(c => c.CODE.Contains(code) || c.PATTERN.Contains(code)).Take(10).Select(p => new PoSearch() { PoId = p.ID, PoCode = p.CODE, PatternNo = p.PATTERN }).ToList();
                    break;
            }
            return ls;
        }




        /// <summary>
        /// 获取单个生产单信息
        /// </summary>
        /// <param name="id">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Po/GetPo")]
        public ViewModels.Po GetPo(int id)
        {
            var p = db.POes.Where(c => c.ID == id).FirstOrDefault();
            var vpo = new ViewModels.Po();
            vpo.id = p.ID;
            vpo.code = p.CODE;
            vpo.customerid = p.CUSTOMERID.Value;
            if (p.CUSTOMERID!=null)
            {
                var cus = db.CUSTOMERs.Where(c => c.ID == p.CUSTOMERID).FirstOrDefault();
                vpo.customername = cus == null ? "" : cus.NAME;
            }
            vpo.deliverydate = p.DELIVERYDATE;
            vpo.initDeliveryDate = p.INITDELIVERYDATE;
            vpo.amount = p.AMOUNT;
            vpo.pattern = p.PATTERN;
            vpo.priority = p.PRIORITY.Value;
            if (p.PRODUCTTYPEID != null)
            {
                var cus = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault();
                vpo.producttype = cus == null ? "" : cus.NAME;
            }
            if (p.MERCHANDISER != null)
            {
                var cus = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault();
                vpo.merchandiser = cus == null ? "" : cus.USERNAME;
            }
            return vpo;
        }


        /// <summary>
        /// 生产单列表
        /// </summary>
        /// <param name="fid">工厂id</param>
        /// <param name="code">生产单编号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <returns></returns>
        [HttpGet, Route("api/Po/GetPoes")]
        public List<Po> GetPoes(int pageIndex, int pageSize, int fid = -1, string code = "")
        {
            var ls = db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code)) && c.FACTORYID == fid).OrderBy(c => c.ID).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1));
            var datas = ls.Select(p => new ViewModels.Po
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
                merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                confirmdate = p.PLACEORDERDATE,
                CuttingAmount = p.ACTCUTQTY == null ? 0 : (p.ACTCUTQTY.Value < 0 ? 0 : p.ACTCUTQTY.Value),
                customerPattern = p.CUSTOMERSTYLENO,
                factoryid = p.FACTORYID == null ? 0 : p.FACTORYID.Value,
                initDeliveryDate = p.INITDELIVERYDATE,
                OthMaterielArriveDate = p.MATERIALDATE,
                MainMaterielArriveDate = p.MATERIALRECEIVEDDATE,
            });
            return datas.ToList();
        }




    }
}
