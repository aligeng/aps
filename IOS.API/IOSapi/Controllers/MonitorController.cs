using IOSapi.BLL;
using IOSapi.Models;
using IOSapi.MonitorModels;
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
    /// 监控中心
    /// </summary>
    public class MonitorController : BaseApiController
    {
        APSEntities db = new APSEntities();

        private static string _MonitorType;
        /// <summary>
        /// 监控模式 0自动模式；1手动模式
        /// </summary>
        public string MonitorType
        {
            get
            {
                if (string.IsNullOrEmpty(_MonitorType))
                {
                    var data = db.MODELSETTINGS.Where(c => c.NAME == "MonitorType").FirstOrDefault();
                    if (data != null)
                    {
                        _MonitorType = "0";
                    }
                    else
                    {
                        _MonitorType = data.VALUE;
                    }                   
                }

                return _MonitorType;
            }
            set
            {
                _MonitorType = value;
            }
        }

        /// <summary>
        /// 跨域处理
        /// </summary>
        /// <returns></returns>
        public string Options()
        {
            return null; // HTTP 200 response with empty body
        }

        /// <summary>
        /// 获取所有工厂数据
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetFactories")]
        public List<FactoryModel> GetFactories()
        {
            List<FactoryModel> ls = new List<FactoryModel>();

            string strSql = " EXEC [_GetFactory] ";
            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        FactoryModel model = new FactoryModel();
                        model.FId = Convert.ToInt32(row["ID"]);
                        model.Name = row["Name"].ToString();
                        model.City = "";

                        ls.Add(model);
                    }
                }
            }
            catch (Exception )
            {               
                throw;
            }


            return ls;
        }

        /// <summary>
        /// 查询可用于监控的生产单
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="code">生产单编号/本厂款号/客户款号</param>
        /// <param name="customer">客户</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetPlanPoext")]
        public ListData GetPlanPoext(int pageIndex, int pageSize, string fids = "", string code = "", string customer = "")
        {
            ListData listData = new ListData();
            listData.objList = new List<WarnPoData>();

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

                var query = db.Po_ext.Where(c => (string.IsNullOrEmpty(fids) || fidLs.Contains(c.Factoryid.Value)) && (string.IsNullOrEmpty(customer) || c.CustomerName.Contains(customer)) &&
                    (string.IsNullOrEmpty(code) || (c.Code.Contains(code) || c.Pattern.Contains(code) || c.customerstyleno.Contains(code)))).Select(a =>
                                  new WarnPoData()
                                  {
                                      Amount = a.Amount.Value,
                                      Createdate = a.Createdate,
                                      CustomerName = a.CustomerName,
                                      Customerstyleno = a.customerstyleno,
                                      Producttype = a.ProducttypeName,
                                      Deliverydate = a.Deliverydate,
                                      FactoryName = a.FactoryName,
                                      InitDeliverydate = a.InitDeliverydate,
                                      Pattern = a.Pattern,
                                      PoCode = a.Code,
                                      Poid = a.ID,
                                      Status = a.status,
                                      Merchandiser = a.Merchandiser
                                  });

                if (query != null)
                {
                    listData.total = query.Count();
                }

                listData.objList = query.OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
            }
            catch (Exception)
            {
            }

            return listData;

        }


        /// <summary>
        /// 已排产未完成的生产单列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="planTime">计划区间包含日期</param>
        /// <param name="productTypeIds">产品大类id集</param>
        /// <param name="fids">工厂id集</param>
        /// <param name="wsids">车间id集</param>
        /// <param name="code">生产单编号/本厂款号/客户款号</param>
        /// <param name="customer">客户</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetPlanPoes")]
        public List<PlanPo> GetPlanPoes(int pageIndex, int pageSize, string planTime, string productTypeIds = "", string fids = "", string wsids = "", string code = "", string customer = "")
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

            //客户查询
            List<int> cusIds = new List<int>();
            if (!string.IsNullOrEmpty(customer))
            {
                cusIds = db.CUSTOMERs.Where(c => c.NAME.Contains(customer)).Select(s => s.ID).ToList();
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
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code)||c.CUSTOMERSTYLENO.Contains(code))) && c.STATUS == 0 &&
                              (isWorkShop ? true : fidLs.Contains(c.FACTORYID.Value)) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)) && 
                              (string.IsNullOrEmpty(customer) ? true : cusIds.Contains(c.CUSTOMERID.Value)))
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
                                  pattern = p.PATTERN,
                                  priority = p.PRIORITY.Value,
                                  producttype = db.PRODUCTTYPEs.Where(c => c.ID == p.PRODUCTTYPEID).FirstOrDefault().NAME,
                                  merchandiser = db.USERS.Where(c => c.ID == p.MERCHANDISER).FirstOrDefault().USERNAME,
                                  StarDate = a.STARTTIME,
                                  EndDate = a.ENDTIME,
                                  customerPattern = p.CUSTOMERSTYLENO,
                                  CompleteAmount = db.PRODUCTIONSCHEDULEs.Where(c => c.POID == p.ID && c.PRODUCTIONEVENTID > 0).Select(s => s.AMOUNT.Value).DefaultIfEmpty().Sum(),
                              }).Union(from a in miutiPoPlan
                                       join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : c.CODE.Contains(code) || c.PATTERN.Contains(code) || c.CUSTOMERSTYLENO.Contains(code)) && c.STATUS == 0 &&
                                       fidLs.Contains(c.FACTORYID.Value) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)) &&
                                       (string.IsNullOrEmpty(customer) ? true : cusIds.Contains(c.CUSTOMERID.Value))) on a.ID equals p.ID
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
                              join p in db.POes.Where(c => (string.IsNullOrEmpty(code) ? true : (c.CODE.Contains(code) || c.PATTERN.Contains(code) || c.CUSTOMERSTYLENO.Contains(code))) && c.STATUS == 0 &&
                              (isWorkShop ? true : fidLs.Contains(c.FACTORYID.Value)) && (string.IsNullOrEmpty(productTypeIds) ? true : TypeLs.Contains(c.PRODUCTTYPEID.Value)) &&
                              (string.IsNullOrEmpty(customer) ? true : cusIds.Contains(c.CUSTOMERID.Value)))
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

        /// <summary>
        /// 宏观计划
        /// </summary>
        /// <param name="fid">工厂id</param>
        /// <param name="monthes">逗号分隔的月份集（如：201708,201709,201710）</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetMacroplan")]
        public List<MacroplanModel> GetMacroplan(int fid, string monthes)
        {
            List<MacroplanModel> ls = new List<MacroplanModel>();

            string[] months = monthes.Split(',');
            if (months.Length > 0)
            {
                foreach (var item in months)
                {
                    ls.Add(new MacroplanModel
                    {
                        Month = item
                    });
                }

                string strSql = string.Format(" EXEC [_GetMacroplan] '{0}','{1}'", monthes, fid);

                try
                {
                    DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (MacroplanModel item in ls)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                if (item.Month == row["month"].ToString())
                                {
                                    item.Capacity = row["capacity"] == DBNull.Value ? 0 : Convert.ToInt32(row["capacity"].ToString());
                                    item.ArrangedAmount = row["arrangedAmount"] == DBNull.Value ? 0 : Convert.ToInt32(row["arrangedAmount"].ToString());
                                    item.NotArrangedAmount = row["notArrangedAmount"] == DBNull.Value ? 0 : Convert.ToInt32(row["notArrangedAmount"].ToString());
                                    item.ExpectedAmount = row["expectedAmount"] == DBNull.Value ? 0 : Convert.ToInt32(row["expectedAmount"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }

            }

            return ls;
        }

        /// <summary>
        /// 查询生产单号
        /// </summary>
        /// <param name="fid">工厂id</param>
        /// <param name="pocode">生产单号查询条件</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/SearchPoCode")]
        public List<PoSelectModel> SearchPoCode(int fid, string pocode)
        {
            List<PoSelectModel> ls = new List<PoSelectModel>();
            string strSql = string.Format(" EXEC [_SearchOrders] '{0}','{1}' ", fid, pocode);
            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow item in table.Rows)
                    {
                        PoSelectModel model = new PoSelectModel();
                        model.PoId = Convert.ToInt32(item["id"]);
                        model.PoCode = item["code"].ToString();
                        ls.Add(model);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return ls;

        }

        /// <summary>
        /// 按生产单id获取特定日期的生产单信息
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="pid">生单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetPoDetaile")]
        public PoDetaileModel GetPoDetaile(DateTime date, int pid)
        {
            string strSql = string.Format(" EXEC [_GetOrder2] '{0}','{1}' ", pid, date);
            DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

            PoDetaileModel poDetaile = new PoDetaileModel();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                poDetaile.Code = row["Code"].ToString();
                poDetaile.CustomerName = row["CustomerName"].ToString();
                poDetaile.ProducttypeName = row["ProducttypeName"].ToString();
                poDetaile.customerstyleno = row["customerstyleno"].ToString();
                poDetaile.Pattern = row["Pattern"].ToString();
                poDetaile.Amount = row["Amount"] == DBNull.Value ? 0 : Convert.ToInt32(row["Amount"].ToString());
                poDetaile.Sam = row["Sam"] == DBNull.Value ? 0 : Convert.ToDouble(row["Sam"].ToString());
                poDetaile.FactoryName = row["FactoryName"].ToString();
                poDetaile.Startdate = row["Startdate"].ToString();
                poDetaile.Enddate = row["Enddate"].ToString();
                poDetaile.Deliverydate = row["Deliverydate"].ToString();
                poDetaile.Troubletype = row["Troubletype"] == DBNull.Value ? 0 : Convert.ToInt32(row["Troubletype"].ToString());
                poDetaile.Duration = Math.Round(Convert.ToDouble(row["Duration"]) / 8.0, 2);
                poDetaile.lineName = row["lineName"].ToString();
                poDetaile.Groupname = row["Groupname"].ToString();

                strSql = string.Format(" EXEC [_GetOrderDetaile2] '{0}'", pid);
                DataTable tbDetaile = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

                if (tbDetaile.Rows.Count > 0)
                {
                    poDetaile.Status = tbDetaile.Rows[0]["status"] == DBNull.Value ? 0 : Convert.ToInt32(tbDetaile.Rows[0]["status"].ToString());
                    poDetaile.StatusName = tbDetaile.Rows[0]["StatusName"].ToString();
                    poDetaile.InitDeliveryDate =  tbDetaile.Rows[0].Field<DateTime?>("initDeliveryDate");
                    poDetaile.IsFirmOrderName = tbDetaile.Rows[0]["isFirmOrderName"].ToString();
                    poDetaile.Merchandiser = tbDetaile.Rows[0]["username"].ToString();
                    poDetaile.EventFlow = tbDetaile.Rows[0]["eventflowname"].ToString();
                    poDetaile.EarlinessStartDate = tbDetaile.Rows[0].Field<DateTime?>("earlinessstartdate");
                    //if (poDetaile.EarlinessStartDate == "1900-01-01")
                    //{
                    //    poDetaile.EarlinessStartDate = "";
                    //}
                    poDetaile.MasterMaterialDate = tbDetaile.Rows[0].Field<DateTime?>("MasterMaterialDate");
                    poDetaile.MasterMaterialReceivedDate = tbDetaile.Rows[0].Field<DateTime?>("MasterMaterialReceivedDate");
                    poDetaile.MaterialDate = tbDetaile.Rows[0].Field<DateTime?>("MaterialDate");
                    poDetaile.MaterialReceivedDate = tbDetaile.Rows[0].Field<DateTime?>("MaterialReceivedDate");
                    poDetaile.PreProductionEventDate = tbDetaile.Rows[0].Field<DateTime?>("PreProductionEventDate");
                }
            }


            return poDetaile;
        }

        /// <summary>
        /// 获取工厂流程图节点
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetFlowchartNodes")]
        public List<FlowchartNode> GetFlowchartNodes()
        {
            List<FlowchartNode> ls = new List<FlowchartNode>();
            try
            {
                string strSql = " SELECT ID,Type,name,isprimary,matchprocessid FROM [dbo].[process_ext] ORDER BY Type,ID ";
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow item in table.Rows)
                    {
                        FlowchartNode model = new FlowchartNode();
                        model.nodeId = Convert.ToInt32(item["ID"]);
                        model.nodeType = Convert.ToInt32(item["Type"]);
                        model.nodeName = item["name"].ToString();
                        model.relatedId = item["matchprocessid"] == DBNull.Value ? 0 : Convert.ToInt32(item["matchprocessid"]);
                        model.isPrimary = item["isprimary"] == DBNull.Value ? 0 : Convert.ToInt32(item["isprimary"]);
                        model.isMonitor = 1;
                        ls.Add(model);
                    }
                }

            }
            catch (Exception)
            {

                return null;
            }

            return ls;
        }

        /// <summary>
        /// 获取生产的异常数量
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="fid">工厂id，-1为全部工厂</param>
        /// <param name="code">单号/款号查询</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/PoesStatusAmount")]
        public PoesStatusModel PoesStatusAmount(DateTime date, int fid, string code = "")
        {
            PoesStatusModel model = new PoesStatusModel();

            string strSql = string.Format(" EXEC [_GetOrderStatus] '{0}','{1}','{2}',{3}", date, fid, code, MonitorType);

            DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);
            if (table.Rows.Count > 0)
            {
                DataRow dr = table.Rows[0];
                model.sall = dr["sall"] == DBNull.Value ? 0 : Convert.ToInt32(dr["sall"]);
                model.s1 = dr["s1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["s1"]);
                model.s2 = dr["s2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["s2"]);
                model.s3 = dr["s3"] == DBNull.Value ? 0 : Convert.ToInt32(dr["s3"]);
                model.s4 = dr["s4"] == DBNull.Value ? 0 : Convert.ToInt32(dr["s4"]);
            }

            return model;
        }

        /// <summary>
        /// 工厂流程图数据（包含异常单数据）
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="fid">工厂id，-1为全部工厂</param>
        /// <param name="code">单号/款号查询</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetFacFlowData")]
        public List<ProcessDatas> GetFacFlowData(DateTime date, int fid, string code = "")
        {
            string retStr = string.Empty;
            string strSql = string.Format(" EXEC [_GetProcessDatas] '{0}',{1},'{2}',{3}", date, fid, code, MonitorType);
            DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

            List<ProcessDatas> ProcessDataList = (List<ProcessDatas>)DataTableList.ConvertTo<ProcessDatas>(table);
            foreach (var item in ProcessDataList)
            {
                if (item.Abnormal > 0)
                {
                    DataTable tbAbnormal = Commons.SqlQueryForDataTatable(new APSEntities().Database, string.Format(" EXEC [_GetProcessAbnormal] '{0}',{1},{2},{3},'{4}',{5}", date, item.ProceId, item.Type, fid, code, MonitorType));

                    if (tbAbnormal != null && tbAbnormal.Rows.Count > 0)
                    {
                        item.WarnDatas = (List<WarnPoData>)DataTableList.ConvertTo<WarnPoData>(tbAbnormal);
                    }
                }

                //item.ProceId = item.ProceId * 10 + item.Type;
                //if (item.Refid > 0)
                //{
                //    item.Refid = item.Refid * 10 + 2;
                //}
            }
            return ProcessDataList;
        }

        /// <summary>
        /// 工厂流程图数据（包含异常单数据）
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/GetFacFlowDatas")]
        public List<ProcessDatas> GetFacFlowDatas(FlowParam para)
        {
            string retStr = string.Empty;
            string strSql = string.Format(" EXEC [_GetProcessDatas] '{0}',{1}", para.Date.Date, para.Id);
            DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

            List<ProcessDatas> ProcessDataList = (List<ProcessDatas>)DataTableList.ConvertTo<ProcessDatas>(table);
            List<ProcessDatas> ProcessSeleList = new List<ProcessDatas>();
            foreach (var item in ProcessDataList)
            {
                if (item.Abnormal > 0)
                {
                    DataTable tbAbnormal = Commons.SqlQueryForDataTatable(new APSEntities().Database, string.Format(" EXEC [_GetProcessAbnormal] '{0}',{1},{2},{3}", para.Date, item.ProceId, item.Type, para.Id));

                    if (tbAbnormal != null && tbAbnormal.Rows.Count > 0)
                    {
                        item.WarnDatas = (List<WarnPoData>)DataTableList.ConvertTo<WarnPoData>(tbAbnormal);
                    }
                }

                //item.ProceId = item.ProceId * 10 + item.Type;
                //if (item.Refid > 0)
                //{
                //    item.Refid = item.Refid * 10 + 2;
                //}

                if (para.FlowPoints.Where(c => c.ProcessId == item.ProceId && c.Type == item.Type).Count() > 0)
                {
                    ProcessSeleList.Add(item);
                }
            }

            return ProcessSeleList;
        }

        /// <summary>
        /// 生产单流程图数据
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="pid">生产单id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetPoFlowData")]
        public List<PoProcessModel> GetPoFlowData(DateTime date, int pid)
        {
            List<PoProcessModel> ls = new List<PoProcessModel>();

            string strSql = string.Format(" EXEC [_GetPoProcess2] '{0}','{1}'", pid, date);

            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        PoProcessModel model = new PoProcessModel();
                        //model.Active = row["Active"] == DBNull.Value ? 0 : Convert.ToInt32(row["Active"]);
                        model.Dayqty = row["Dayqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Dayqty"]);
                        model.Duration = row["Duration"] == DBNull.Value ? 0 : Convert.ToInt32(row["Duration"]);
                        //model.Processid = Convert.ToInt32(row["Processid"].ToString() + row["Type"].ToString());
                        model.Processid = Convert.ToInt32(row["Processid"].ToString());
                        model.Processname = row["Processname"].ToString();
                        if (row["Refid"] != DBNull.Value)
                        {
                            model.Refid = Convert.ToInt32(row["Refid"].ToString());
                        }
                        model.Schedulepercent = row["Schedulepercent"] == DBNull.Value ? 0 : Math.Round(Convert.ToDouble(row["Schedulepercent"]) * 100, 0);
                        //model.Sort = row["Sort"] == DBNull.Value ? 0 : Convert.ToInt32(row["Sort"]);
                        model.Status = row["Status"] == DBNull.Value ? 0 : Convert.ToInt32(row["Status"]);
                        model.Totalqty = row["Totalqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Totalqty"]);
                        model.Troubletype = row["Troubletype"] == DBNull.Value ? 0 : Convert.ToInt32(row["Troubletype"]);
                        model.Type = row["Type"] == DBNull.Value ? 0 : Convert.ToInt32(row["Type"]);
                        model.Startdate = row["Startdate"] == DBNull.Value ? "" : Convert.ToDateTime(row["Startdate"]).ToString("yyyy-MM-dd");
                        model.Enddate = row["Enddate"] == DBNull.Value ? "" : Convert.ToDateTime(row["Enddate"]).ToString("yyyy-MM-dd");
                        model.Planqty = row["Planqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Planqty"]);
                        model.Active = Convert.ToInt32(row["Active"]);
                        ls.Add(model);
                    }
                }
            }
            catch (Exception)
            {
            }

            return ls;
        }

        /// <summary>
        /// 生产单流程图数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/GetPoFlowDatas")]
        public List<PoProcessModel> GetPoFlowDatas(FlowParam para)
        {
            List<PoProcessModel> ls = new List<PoProcessModel>();

            string strSql = string.Format(" EXEC [_GetPoProcess2] '{0}','{1}'", para.Id, para.Date.Date);

            try
            {
                DataTable table = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {

                        PoProcessModel model = new PoProcessModel();
                        //model.Active = row["Active"] == DBNull.Value ? 0 : Convert.ToInt32(row["Active"]);
                        model.Dayqty = row["Dayqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Dayqty"]);
                        model.Duration = row["Duration"] == DBNull.Value ? 0 : Convert.ToInt32(row["Duration"]);
                        //model.Processid = Convert.ToInt32(row["Processid"].ToString() + row["Type"].ToString());
                        model.Processid = Convert.ToInt32(row["Processid"].ToString());
                        model.Processname = row["Processname"].ToString();
                        if (row["Refid"] != DBNull.Value)
                        {
                            model.Refid = Convert.ToInt32(row["Refid"].ToString());
                        }
                        model.Schedulepercent = row["Schedulepercent"] == DBNull.Value ? 0 : Convert.ToDouble(row["Schedulepercent"]);
                        //model.Sort = row["Sort"] == DBNull.Value ? 0 : Convert.ToInt32(row["Sort"]);
                        model.Status = row["Status"] == DBNull.Value ? 0 : Convert.ToInt32(row["Status"]);
                        model.Totalqty = row["Totalqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Totalqty"]);
                        model.Troubletype = row["Troubletype"] == DBNull.Value ? 0 : Convert.ToInt32(row["Troubletype"]);
                        model.Type = row["Type"] == DBNull.Value ? 0 : Convert.ToInt32(row["Type"]);
                        model.Startdate = row["Startdate"] == DBNull.Value ? "" : Convert.ToDateTime(row["Startdate"]).ToString("yyyy-MM-dd");
                        model.Enddate = row["Enddate"] == DBNull.Value ? "" : Convert.ToDateTime(row["Enddate"]).ToString("yyyy-MM-dd");
                        model.Planqty = row["Planqty"] == DBNull.Value ? 0 : Convert.ToInt32(row["Planqty"]);

                        if (para.FlowPoints.Where(c => c.ProcessId == model.Processid && c.Type == model.Type).Count()>0)
                        {
                            ls.Add(model);
                        }          
                    }
                }

            }
            catch (Exception)
            {
            }

            return ls;
        }

        /// <summary>
        /// 获取工厂的所有车缝产线进度数据
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="fid">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetLineSchedule")]
        public List<SewingLine> GetLineSchedule(DateTime date, int fid)
        {
            List<SewingLine> ls = new List<SewingLine>();

            try
            {
                DataTable tbProgress = Commons.SqlQueryForDataTatable(new APSEntities().Database, string.Format(" EXEC [_GetLineSchedule] '{0}',{1}", date, fid)); 

                if (tbProgress != null && tbProgress.Rows.Count > 0)
                {
                    ls = (List<SewingLine>)DataTableList.ConvertTo<SewingLine>(tbProgress);
                }

            }
            catch (Exception)
            {
            }

            return ls;
        }

        ///// <summary>
        ///// 获取所有异常生产单数据
        ///// </summary>
        ///// <param name="date">日期</param>
        ///// <param name="fid">工厂id（-1为所有工厂）</param>
        ///// <returns></returns>
        //[HttpGet, Route("api/Monitor/GetAllWarnPoData")]
        //public List<WarnPoData> GetAllWarnPoData(DateTime date, int fid = -1, string styleNo = "", string joNo = "", string poNo = "", string customer = "")
        //{
        //    List<WarnPoData> ls = new List<WarnPoData>();
        //    try
        //    {
        //        DataTable tbAbnormal = Commons.SqlQueryForDataTatable(new APSEntities().Database, string.Format(" EXEC [_GetAllAbnormal] '{0}',{1}", date, fid));

        //        if (tbAbnormal != null && tbAbnormal.Rows.Count > 0)
        //        {
        //            ls = (List<WarnPoData>)DataTableList.ConvertTo<WarnPoData>(tbAbnormal);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return ls;
        //}

        /// <summary>
        /// 获取所有异常生产单数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页记录数大小</param>
        /// <param name="date">日期</param>
        /// <param name="fid">工厂id（-1为所有工厂）</param>
        /// <param name="styleNo">本厂款号</param>
        /// <param name="cuStyleNo">客户款号</param>
        /// <param name="code">生产单号</param>
        /// <param name="customer">客户名称</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetAllWarnPoData")]
        public ListData GetAllWarnPoData(int pageIndex, int pageSize, DateTime date, int fid = -1, string styleNo = "", string cuStyleNo = "", string code = "", string customer = "")
        {
            ListData listData = new ListData();
            listData.objList = new List<WarnPoData>();
            try
            {
                DateTime dateTime = date.Date;
                var poids = db.Processdailydata_ext.Where(p => p.Processdate == dateTime && p.Troubletype > 1).GroupBy(c => c.Poid).Select(o => 
                    new { Poid = o.Key, Troubletype = o.Max(a => a.Troubletype) });

                //var query = (from a in db.POes.Where(c => c.STATUS == 0)
                //             join h in db.Po_ext on a.ID equals h.ID
                //             join e in poids on a.ID equals e.Poid
                //             join b in db.Customer_ext on a.CUSTOMERID equals b.ID
                //             join d in db.FACTORies on a.FACTORYID equals d.ID
                //             join f in db.USERS on a.MERCHANDISER equals f.ID
                //             where (fid > 0 ? a.FACTORYID == fid : true) &&
                //             (string.IsNullOrEmpty(styleNo) ? true : a.PATTERN.Contains(styleNo)) &&
                //             (string.IsNullOrEmpty(cuStyleNo) ? true : a.CUSTOMERSTYLENO.Contains(cuStyleNo)) &&
                //             (string.IsNullOrEmpty(customer) ? true : b.Name.Contains(customer))
                //             select new WarnPoData()
                //             {
                //                 Amount = a.AMOUNT,
                //                 CaoTD = a.CAOTD.Value,
                //                 Createdate = a.CREATEDATE,
                //                 CustomerName = b.Name,
                //                 Customerstyleno = a.CUSTOMERSTYLENO,
                //                 Producttype = a.PRODUCTTYPE.NAME,
                //                 Deliverydate = a.DELIVERYDATE,
                //                 FactoryName = d.NAME,
                //                 InitDeliverydate = a.INITDELIVERYDATE,
                //                 Pattern = a.PATTERN,
                //                 PoCode = a.CODE,
                //                 Poid = a.ID,
                //                 Status = a.STATUS,
                //                 VIP = b.VIP == null ? 0 : b.VIP.Value,
                //                 Troubletype = e.Troubletype,
                //                 Merchandiser = f.USERNAME
                //             });
                //listData.total = query.Count();
                //listData.objList = query.OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

                if (MonitorType=="0")
                {
                    var query = (from a in db.Po_ext
                                 join e in poids on a.ID equals e.Poid
                                 where (fid > 0 ? a.Factoryid == fid : true) &&
                                 (string.IsNullOrEmpty(code) ? true : a.Code.Contains(code)) &&
                                 (string.IsNullOrEmpty(styleNo) ? true : a.Pattern.Contains(styleNo)) &&
                                 (string.IsNullOrEmpty(cuStyleNo) ? true : a.customerstyleno.Contains(cuStyleNo)) &&
                                 (string.IsNullOrEmpty(customer) ? true : a.CustomerName.Contains(customer))
                                 select new WarnPoData()
                                 {
                                     Amount = a.Amount.Value,
                                     Createdate = a.Createdate,
                                     CustomerName = a.CustomerName,
                                     Customerstyleno = a.customerstyleno,
                                     Producttype = a.ProducttypeName,
                                     Deliverydate = a.Deliverydate,
                                     FactoryName = a.FactoryName,
                                     InitDeliverydate = a.InitDeliverydate,
                                     Pattern = a.Pattern,
                                     PoCode = a.Code,
                                     Poid = a.ID,
                                     Status = a.status,
                                     //Troubletype = e.Troubletype,
                                     Merchandiser = a.Merchandiser
                                 });


                    listData.total = query.Count();
                    listData.objList = query.OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
                }
                else
                {
                    var query = (from a in db.Po_ext
                                 join e in poids on a.ID equals e.Poid
                                 join f in db.MonitorPo_ext on a.ID equals f.PoID
                                 where (fid > 0 ? a.Factoryid == fid : true) &&
                                 (string.IsNullOrEmpty(code) ? true : a.Code.Contains(code)) &&
                                 (string.IsNullOrEmpty(styleNo) ? true : a.Pattern.Contains(styleNo)) &&
                                 (string.IsNullOrEmpty(cuStyleNo) ? true : a.customerstyleno.Contains(cuStyleNo)) &&
                                 (string.IsNullOrEmpty(customer) ? true : a.CustomerName.Contains(customer))
                                 select new WarnPoData()
                                 {
                                     Amount = a.Amount.Value,
                                     Createdate = a.Createdate,
                                     CustomerName = a.CustomerName,
                                     Customerstyleno = a.customerstyleno,
                                     Producttype = a.ProducttypeName,
                                     Deliverydate = a.Deliverydate,
                                     FactoryName = a.FactoryName,
                                     InitDeliverydate = a.InitDeliverydate,
                                     Pattern = a.Pattern,
                                     PoCode = a.Code,
                                     Poid = a.ID,
                                     Status = a.status,
                                     //Troubletype = e.Troubletype,
                                     Merchandiser = a.Merchandiser
                                 });


                    listData.total = query.Count();
                    listData.objList = query.OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();
                }


            }
            catch (Exception )
            {
                
            }
            return listData;
        }

        /// <summary>
        /// 获取当前监控模式
        /// </summary>
        /// <returns>0自动模式；1手动模式</returns>
        [HttpGet, Route("api/Monitor/GetCurrentMonitorType")]
        public Parameter GetCurrentMonitorType()
        {
            Parameter p = new Parameter();

            var data = db.MODELSETTINGS.Where(c => c.NAME == "MonitorType").FirstOrDefault();
            if (data == null)
            {
                p.value="0";
                p.name = "自动模式";
            }
            else
            {
                p.value = data.VALUE;
                p.name = p.value == "0" ? "自动模式" : "手动设置模式";
            }

            return p;
        }

        /// <summary>
        /// 设置监控模式
        /// </summary>
        /// <param name="type">0自动模式，1手动添加生产单监控模式</param>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/SetMonitorType")]
        public ResultModel SetMonitorType(int type)
        {
            ResultModel rm = new ResultModel();
            try
            {
                var data = db.MODELSETTINGS.Where(c => c.NAME == "MonitorType").FirstOrDefault();
                if (data == null)
                {
                    rm.IsSuccess = 0;
                    rm.ErrMessage.Add(new LangMessage("cn", "参数不存在"));
                    rm.ErrMessage.Add(new LangMessage("en", "Parameter does not exist"));
                    return rm;
                }
                else
                {
                    data.VALUE = type == 0 ? "0" : "1";
                    db.SaveChanges();
                    MonitorType = data.VALUE;

                    rm.IsSuccess = 1;
                }

                rm.IsSuccess = 1;
                return rm;
            }
            catch (Exception ex)
            {
                rm.IsSuccess = 0;
                rm.ErrMessage.Add(new LangMessage("cn", ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", ex.Message));
                return rm;
            }
        }

        /// <summary>
        /// 批量添加需要监控的生产单
        /// </summary>
        /// <param name="poIds">生产单id集合</param>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/AddMonitorPoes")]
        public ResultModel AddMonitorPoes(List<int> poIds)
        {
            ResultModel rm = new ResultModel();
            try
            {

                var ids = db.POes.Where(c => poIds.Contains(c.ID)).Select(s => s.ID).ToList();
                if (ids == null || ids.Count == 0)
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "添加的生产单无效"));
                    rm.ErrMessage.Add(new LangMessage("en", "The added production order is invalid"));
                    return rm;
                }
                var exitIds = db.MonitorPo_ext.Select(c=>c.PoID).DefaultIfEmpty();
                List<MonitorPo_ext> ls = new List<MonitorPo_ext>();
                foreach (var item in ids)
                {
                    if (!exitIds.Contains(item) || exitIds == null)
                    {
                        ls.Add(new MonitorPo_ext() { PoID = item, UpdateTime = DateTime.Now });
                    }
                }

                if (ls.Count>0)
                {
                    db.MonitorPo_ext.AddRange(ls);
                    db.SaveChanges();
                    rm.IsSuccess = 1;
                }
                else
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "添加的生产单已存在"));
                    rm.ErrMessage.Add(new LangMessage("en", "The added production list already exists."));
                }

                return rm;
            }
            catch (Exception ex)
            {
                rm.IsSuccess = 0;
                rm.ErrMessage.Add(new LangMessage("cn", ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", ex.Message));
                return rm;
            }
        }
        /// <summary>
        /// 批量删除监控的生产单
        /// </summary>
        /// <param name="poIds">生产单id集</param>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/DelMonitorPoes")]
        public ResultModel DelMonitorPoes(List<int> poIds)
        {
            ResultModel rm = new ResultModel();
            try
            {
                List<MonitorPo_ext> datas = db.MonitorPo_ext.Where(c => poIds.Contains(c.PoID)).ToList();
                if (datas!=null && datas.Count>0)
                {
                    db.MonitorPo_ext.RemoveRange(datas);
                    db.SaveChanges();
                    rm.IsSuccess = 1;
                }
                else
                {
                    rm.ErrMessage.Add(new LangMessage("cn", "生产单不存在"));
                    rm.ErrMessage.Add(new LangMessage("en", "Production orders do not exist."));
                }

                return rm;
            }
            catch (Exception ex)
            {
                rm.IsSuccess = 0;
                rm.ErrMessage.Add(new LangMessage("cn", ex.Message));
                rm.ErrMessage.Add(new LangMessage("en", ex.Message));
                return rm;
            }
        }

        /// <summary>
        /// 获取已设置监控的生产单
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="fid">工厂id,-1为所有工厂</param>
        /// <param name="code">单号/款号</param>
        /// <param name="customer">客户名称</param>
        /// <returns></returns>
        [HttpPost, Route("api/Monitor/GetMonitorPoes")]
        public ListData GetMonitorPoes(int pageIndex, int pageSize, int fid = -1, string code = "", string customer = "")
        {
            ListData listData = new ListData();
            listData.objList = new List<WarnPoData>();

            try
            {              
                var query = (from a in db.POes
                             join b in db.MonitorPo_ext on a.ID equals b.PoID
                             join c in db.USERS on a.MERCHANDISER equals c.ID into joinA
                             from ja in joinA.DefaultIfEmpty()
                             where (fid == -1 || a.FACTORYID == fid) && (string.IsNullOrEmpty(code) || (a.CODE.Contains(code) || a.PATTERN.Contains(code) || a.CUSTOMERSTYLENO.Contains(code)))
               && (string.IsNullOrEmpty(customer) || a.CUSTOMER.NAME.Contains(customer))
                             select new WarnPoData()
                             {
                                 Amount = a.AMOUNT,
                                 Createdate = a.CREATEDATE,
                                 CustomerName = a.CUSTOMER.NAME,
                                 Customerstyleno = a.CUSTOMERSTYLENO,
                                 Producttype = a.PRODUCTTYPE.NAME,
                                 Deliverydate = a.DELIVERYDATE,
                                 FactoryName = a.FACTORY.NAME,
                                 InitDeliverydate = a.INITDELIVERYDATE,
                                 Pattern = a.PATTERN,
                                 PoCode = a.CODE,
                                 Poid = a.ID,
                                 Status = a.STATUS,
                                 Merchandiser = ja.USERNAME
                             });

                listData.total = query.Count();
                listData.objList = query.OrderBy(c => c.PoCode).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).ToList();

            }
            catch (Exception)
            {
            }

            return listData;
        }

        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="fid">工厂id(-1为全部工厂)</param>
        /// <param name="vrpcode">报表编号：
        ///RPT001 成衣库存占比分析；
        ///RPT002 交期延误占比分析；
        ///RPT003 交期金额统计；
        ///RPT004 订单月份准交率；
        ///RPT005 订单月份延误率；
        ///RPT006 产品分类订单统计表；
        ///RPT007 订单客户统计表；
        ///RPT008 订单月份统计表；
        ///RPT009 周生产效率；
        ///RPT010 月份生产效率；
        ///RPT011 车间生产效率；
        ///RPT012 产能/负载分析；
        ///RPT013 产能利用率；
        ///RPT014 订单平均生产周期；
        ///RPT015 订单平均出货周期；
        ///RPT016 主料库存周期；
        ///RPT017 成品库存周期；
        /// </param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetReportData")]
        public object GetReportData(string vrpcode, int fid = -1)
        {
            string strSql = string.Format(" EXEC [_grpt_gendata] '{0}','{1}',{2},{3},'{4}'", "", vrpcode, 0, 1, fid);
            DataTable dt = Commons.SqlQueryForDataTatable(new APSEntities().Database, strSql);

            if (vrpcode == "RPT001" || vrpcode == "RPT002")
            {
                double sum = dt.AsEnumerable().Select(c => c.Field<double>("yvalue")).Sum();

                foreach (DataRow item in dt.Rows)
                {
                    item["yvalue"] = Math.Round(Convert.ToInt32(item["yvalue"]) / sum, 5);
                }
            }

            return dt;
        }


        /// <summary>
        /// 根据图表编号获取图表数据（新增图表）
        /// </summary>
        /// <param name="fid">工厂id(-1为全部工厂)</param>
        /// <param name="vrpcode">报表编号：
        ///RPT100 当日生产线产品类别分布图；
        ///RPT101 当月品牌客户实际生产产量统计图；
        ///RPT102 当月工厂生产效率、产量统计图；
        ///RPT103 工厂每月生产效率汇总；
        ///RPT104 当天生产线效率；
        ///RPT105 工厂每月生产耗时、产量统计；
        /// </param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetChartData")]
        public List<ChartData> GetChartData(string vrpcode, int fid = -1)
        {
            List<ChartData> cls = new List<ChartData>();

            string sql = string.Empty;

            try
            {
                switch (vrpcode)
                {
                    case "RPT100":
                        sql = string.Format(" EXEC [_GetChart] '{0}',{1}", vrpcode, fid);
                        break;
                    case "RPT101":
                        sql = string.Format(" EXEC [_GetChart] '{0}',{1}", vrpcode, fid);
                        break;
                    case "RPT102":
                        sql = string.Format(" EXEC [_grpt_product_efficiency_factory] '{0}'", fid);
                        break;
                    case "RPT103":
                        sql = string.Format(" EXEC [_grpt_product_efficiency_factory_monthes] '{0}'", fid);
                        break;
                    case "RPT104":
                        sql = string.Format(" EXEC [_grpt_product_efficiency_facility] '{0}'", fid);
                        break;
                    case "RPT105":
                        sql = string.Format(" EXEC [_grpt_product_capacity_factory_monthes] '{0}'", fid);
                        break;
                    //case "RPT106":
                    //    sql = string.Format(" EXEC [_grpt_product_sample_factory_monthes] '{0}'", fid);
                    //    break;
                }

                DataTable tbProgress = Commons.SqlQueryForDataTatable(new APSEntities().Database, sql); 

                if (tbProgress != null && tbProgress.Rows.Count > 0)
                {
                    ChartData cd = new ChartData();
                    cd.ChartNodes = new List<ChartNode>();
                    double ysum = 0;
                    foreach (DataRow row in tbProgress.Rows)
                    {
                        ChartNode cn = new ChartNode();
                        cn.xname = row["xname"].ToString();
                        cn.yvalue1 = row["yvalue1"].ToString();
                        cn.yvalue2 = row["yvalue2"].ToString();
                        ysum += Convert.ToDouble(row["yvalue1"]);
                        cd.ChartNodes.Add(cn);
                    }

                    cd.Average = ysum*100 / cd.ChartNodes.Count/100.0;
                    cls.Add(cd);
                }
            }
            catch (Exception)
            {

            }


            return cls;
        }

        /// <summary>
        /// 开发样品统计（TMI）
        /// </summary>
        /// <param name="fid">工厂id，-1默认全部</param>
        /// <param name="customerid">客户id，-1默认全部</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetSampleChartData")]
        public List<ChartData> GetSampleChartData(int fid = -1, int customerid = -1)
        {
            List<ChartData> cls = new List<ChartData>();

            string sql = string.Format(" EXEC [_grpt_product_sample_factory_monthes] '{0}',{1}", fid, customerid);

            DataTable tbProgress = Commons.SqlQueryForDataTatable(new APSEntities().Database, sql);

            if (tbProgress != null && tbProgress.Rows.Count > 0)
            {
                ChartData cd = new ChartData();
                cd.ChartNodes = new List<ChartNode>();
                double ysum = 0;
                foreach (DataRow row in tbProgress.Rows)
                {
                    ChartNode cn = new ChartNode();
                    cn.xname = row["xname"].ToString();
                    cn.yvalue1 = row["yvalue1"].ToString();
                    cn.yvalue2 = row["yvalue2"].ToString();
                    ysum += Convert.ToDouble(row["yvalue1"]);
                    cd.ChartNodes.Add(cn);
                }

                cd.Average = ysum * 100 / cd.ChartNodes.Count / 100.0;
                cls.Add(cd);
            }

            return cls;
        }

        /// <summary>
        /// 订单周期统计
        /// </summary>
        /// <param name="fid">工厂id（-1为所有工厂）</param>
        /// <returns></returns>
        [HttpGet, Route("api/Monitor/GetNChartData")]
        public List<NChartNode> GetNChartData(int fid = -1)
        {
            List<NChartNode> cls = new List<NChartNode>();

            string sql = string.Format(" EXEC [_grpt_product_cycle_monthes] {0}",fid);

            try
            {
                DataSet ds = Commons.SqlQueryForDataSet(new APSEntities().Database, sql);

                int count = ds.Tables.Count;

                foreach (DataTable tb in ds.Tables)
                {
                    count--;
                    NChartNode ncn = new NChartNode();
                    ncn.xname = DateTime.Now.AddMonths(-count).ToString("yyyyMM");
                    ncn.SeriesList = new List<Series>();

                    if (tb != null && tb.Rows.Count > 0)
                    {
                        foreach (DataRow row in tb.Rows)
                        {
                            Series Series = new Series();
                            Series.seriesName = row["LOTNO"].ToString();
                            Series.yvalue1 = row["NormalAmoumt"].ToString();
                            Series.yvalue2 = row["DelayAmoumt"].ToString();
                            ncn.SeriesList.Add(Series);
                        }
                    }

                    cls.Add(ncn);

                }
            }
            catch (Exception)
            {
            }

            return cls;
        }


    }
}
