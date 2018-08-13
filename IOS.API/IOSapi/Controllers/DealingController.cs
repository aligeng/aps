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
    /// 首页延误处理图表
    /// </summary>
    public class DealingController : ApiController
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
        /// 延误订单统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/PoDeliveryDelay")]
        public List<DealPoint> PoDeliveryDelay()
        {
            List<DealPoint> ls = new List<ViewModels.DealPoint>();

            DateTime thisTime = DateTime.Now;
            var query = (from a in db.POes.Where(c => c.ISFIRMORDER == 1 && c.STATUS == 0 && c.DELIVERYDATE < thisTime)
                        join b in db.ExceptionHandle_ext.Where(c =>c.ProcessID == null) on a.ID equals b.PoID into jointb

                        from jtb in jointb.DefaultIfEmpty()
                        select new PoDeal()
                        {
                            PRIORITY = a.PRIORITY.Value,
                            Status = jtb == null ? 0 : jtb.Status
                        }).ToList();

            query.Add(new ViewModels.PoDeal() { PRIORITY = 6, Status = 0 });
            query.Add(new ViewModels.PoDeal() { PRIORITY = 8, Status = 0 });
            query.Add(new ViewModels.PoDeal() { PRIORITY = 10, Status = 0 });


            List<DelayDeal> ddls = query.GroupBy(c => c.PRIORITY).OrderBy(c=>c.Key).Select(p => new DelayDeal()
            {
                DealedAmount = p.Count(c => c.Status == 1),
                Amount = p.Count(),
                Priority = p.Key
            }).ToList();


            foreach (var item in ddls)
            {
                DealPoint dp = new DealPoint();
                dp.DealedAmount = item.DealedAmount;
                dp.NotDealedAmount = item.Amount - item.DealedAmount;
                dp.KeyId = item.Priority;
                switch (item.Priority)
                {
                    case 6:
                        dp.LableText = "普通";
                        break;
                    case 8:
                        dp.LableText = "重要";
                        break;
                    case 10:
                        dp.LableText = "紧急";
                        break;
                    default:
                        dp.LableText = "普通";
                        break;
                };

                ls.Add(dp);
            }

            return ls;
        }

        /// <summary>
        /// 延误订单列表
        /// </summary>
        /// <param name="status">状态：1已处理；0未处理</param>
        /// <param name="keyId">节点id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页记录数</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/GetPoes")]
        public List<PoAndDeal> GetPoes(int status,int keyId,int pageIndex, int pageSize)
        {
            DateTime thisTime = DateTime.Now;
            var ls = (from p in db.POes.Where(c =>c.PRIORITY== keyId && c.ISFIRMORDER == 1 && c.STATUS == 0 && c.DELIVERYDATE < thisTime)
                      join b in db.ExceptionHandle_ext.Where(c => c.Status == status && c.ProcessID == null) on p.ID equals b.PoID into jointb
                      from jtb in jointb.DefaultIfEmpty()
                      select new PoAndDeal()
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
                          keyId=keyId,
                          dealStatus=status
                      }).ToList();

            return ls;
        }

        /// <summary>
        /// 获取延误单详细信息
        /// </summary>
        /// <param name="Type">延迟类型：0订单延误；1事件延误；2工序延误</param>
        /// <param name="poId">生产单id</param>
        /// <param name="keyId">节点id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/GetPo")]
        public ViewModels.PoAndDeal GetPo(int Type, int poId, int keyId)
        {
            var p = db.POes.Where(c => c.ID == poId).FirstOrDefault();
            var vpo = new ViewModels.PoAndDeal();
            vpo.id = p.ID;
            vpo.code = p.CODE;
            vpo.customerid = p.CUSTOMERID.Value;
            if (p.CUSTOMERID != null)
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

            if (Type == 0)
            {
                var eh = db.ExceptionHandle_ext.Where(c => c.PoID == poId && c.ProcessID == null).FirstOrDefault();
                if (eh != null)
                {
                    vpo.dealStatus = eh.Status;
                    vpo.dealRemark = eh.Remark;
                }
            }
            else
            {
                var eh = db.ExceptionHandle_ext.Where(c => c.PoID == poId && c.ProcessID == keyId && c.Type == Type).FirstOrDefault();
                if (eh != null)
                {
                    vpo.dealStatus = eh.Status;
                    vpo.dealRemark = eh.Remark;
                }
            }


            return vpo;
        }

        /// <summary>
        /// 延误事件统计
        /// </summary>
        /// <param name="facId">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/PoEventDelay")]
        public List<DealPoint> PoEventDelay(int facId)
        {
          
            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@facId",facId)
                                                              };

            var ds = this.db.Database.SqlQuery<DealPoint>("exec app_chart_EventDeal @facId ", parameters).ToList();

            return ds.ToList();

            //List<DealPoint> ls = new List<ViewModels.DealPoint>();
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 2,
            //    LableText = "产前板",
            //    NotDealedAmount = 5
            //});
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 0,
            //    LableText = "纸样",
            //    NotDealedAmount = 4
            //});
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 1,
            //    LableText = "工艺制单",
            //    NotDealedAmount = 3
            //});

            //return ls;
        }

        /// <summary>
        /// 延误工序统计
        /// </summary>
        /// <param name="facId">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/PoProceDelay")]
        public List<DealPoint> PoProceDelay(int facId)
        {

            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@facId",facId)
                                                              };

            var ds = this.db.Database.SqlQuery<DealPoint>("exec app_chart_ProcessDeal @facId ", parameters).ToList();

            return ds.ToList();

            //List<DealPoint> ls = new List<ViewModels.DealPoint>();
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 2,
            //    LableText = "裁床",
            //    NotDealedAmount = 8
            //});
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 3,
            //    LableText = "车缝",
            //    NotDealedAmount = 4
            //});
            //ls.Add(new DealPoint()
            //{
            //    DealedAmount = 2,
            //    LableText = "洗水",
            //    NotDealedAmount = 3
            //});
            //return ls;
        }


        /// <summary>
        /// 事件延误订单列表
        /// </summary>
        /// <param name="status">状态：1已处理；0未处理</param>
        /// <param name="keyId">节点id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页记录数</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/GetEventDelayPoes")]
        public List<PoAndDeal> GetEventDelayPoes(int status, int keyId, int pageIndex, int pageSize)
        {
            DateTime thisTime = DateTime.Now;

            var ls = (from p in db.POes.Where(c => c.ISFIRMORDER == 1 && c.STATUS == 0)
                      join q in db.Processdailydata_ext.Where(c => c.Type == 1 && c.Processdate == db.Processdailydata_ext.OrderByDescending(b => b.Processdate).FirstOrDefault().Processdate)
                      on p.ID equals q.Poid
                      join b in db.ExceptionHandle_ext.Where(c => c.Status == status && c.ProcessID == keyId && c.Type == 1) on q.ID equals b.PoID into jointb
                      from jtb in jointb.DefaultIfEmpty()
                      select new PoAndDeal()
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
                          keyId = keyId,
                          dealStatus = status
                      }).ToList();

            return ls;
        }


        /// <summary>
        /// 工序延误订单列表
        /// </summary>
        /// <param name="status">状态：1已处理；0未处理</param>
        /// <param name="keyId">节点id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页记录数</param>
        /// <returns></returns>
        [HttpGet, Route("api/Dealing/GetProcessDelayPoes")]
        public List<PoAndDeal> GetProcessDelayPoes(int status, int keyId, int pageIndex, int pageSize)
        {
            DateTime thisTime = DateTime.Now;

            var ls = (from p in db.POes.Where(c => c.ISFIRMORDER == 1 && c.STATUS == 0)
                      join q in db.Processdailydata_ext.Where(c => c.Processid==keyId && c.Type == 2 && c.Processdate == db.Processdailydata_ext.OrderByDescending(b => b.Processdate).FirstOrDefault().Processdate)
                      on p.ID equals q.Poid
                      join b in db.ExceptionHandle_ext.Where(c => c.Status == status && c.ProcessID == keyId && c.Type == 2) on q.ID equals b.PoID into jointb
                      from jtb in jointb.DefaultIfEmpty()
                      select new PoAndDeal()
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
                          keyId = keyId,
                          dealStatus = status
                      }).OrderBy(c=>c.code).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

            return ls;
        }


        /// <summary>
        /// 添加处理记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Dealing/AddDealData")]
        public ResultModel AddDealData(DealData data)
        {
            ResultModel rm = new ResultModel();
            try
            {
                ExceptionHandle_ext eh = new Models.ExceptionHandle_ext();
                eh.PoID = data.PoId;
                if (data.Type != 0)
                {
                    eh.ProcessID = data.KeyId;
                }
                eh.Remark = data.Remark;
                eh.Status = 1;

                db.ExceptionHandle_ext.Add(eh);

                db.SaveChanges().ToString();

                rm.IsSuccess = 1;
                
            }
            catch (Exception)
            {
                rm.ErrMessage = "";
            }

            return rm;

        }



    }


}
