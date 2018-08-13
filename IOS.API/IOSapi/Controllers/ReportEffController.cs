using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IOS.DBUtility;
using System.Data;

namespace IOSapi.Controllers
{
    /// <summary>
    /// 生产效率统计表
    /// </summary>
    public class ReportEffController : ApiController
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
        /// 生产单效率日统计表
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="facId">工厂id</param>
        /// <param name="lineId">生产线id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportEff/GetPoEff")]
        public List<PoEff> GetPoEff(DateTime starTime, DateTime endTime, int facId, int lineId)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {
               new System.Data.SqlClient.SqlParameter("@starTime",starTime),
               new System.Data.SqlClient.SqlParameter("@endTime",endTime),
               new System.Data.SqlClient.SqlParameter("@facId",facId),
               new System.Data.SqlClient.SqlParameter("@lineId",lineId),};

            string sql = @"SELECT ps.ProductionDate,p.code PoCode,ps.Amount FinishedAmount,p.capacity Capacity,ps.Workers,ps.Duration,
( case (ps.Workers * ps.Duration) WHEN 0 THEN 0 ELSE  (round((ps.Amount * p.capacity /60 ) / (ps.Workers * ps.Duration),8))  END) Eff
 FROM Productionschedule ps 
 LEFT JOIN PO p ON ps.poid=p.id 
 LEFT JOIN Process pc ON ps.processid=pc.id AND pc.isprimary=1 
 JOIN ProductionEvent pe ON ps.Productioneventid=pe.id
 --JOIN Facility fa ON pe.facilityid=fa.id 
  where  ps.ProductionDate >= cast(@starTime as datetime) 
  AND ps.ProductionDate < cast(@endTime as datetime) 
  AND p.FactoryID = @facId
  AND pe.facilityid =@lineId";

            var slt = db.Database.SqlQuery<PoEff>(sql, parameters).ToList();
            return slt;
        }


        /// <summary>
        /// 生产线效率日统计表
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="facId">工厂id</param>
        /// <returns></returns>
        [HttpGet, Route("api/ReportEff/GetLineEff")]
        public List<LineEff> GetLineEff(DateTime starTime, DateTime endTime, int facId)
        {
            string sql = string.Format("exec app_GetLineEff {0},'{1}','{2}' ", facId, starTime, endTime);
            DataTable dt= DBHelper.ExecuteDataset(DBHelper.CurrentDatabase.ConnectionString, CommandType.Text, sql).Tables[0];
            List<string> lines = (from dt1 in dt.AsEnumerable() orderby dt1.Field<String>("FacilityName") select dt1.Field<String>("FacilityName")).Distinct().ToList();
            var q1 = from dt1 in dt.AsEnumerable()//查询
                     orderby dt1.Field<DateTime>("ProductionDate") descending//排序
                     group dt1 by new { t1 = dt1.Field<DateTime>("ProductionDate") } into m
                     select new LineEff()
                     {
                         ProductionDate = m.Key.t1,
                         Datas = (from le in lines
                                  join p in m.DefaultIfEmpty() on le equals p.Field<String>("FacilityName")
                                  into Joinedtb
                                  from joi in Joinedtb.DefaultIfEmpty()
                                  select new LineDayEff()
                                  {
                                      LineName = le,
                                      Eff = joi == null ? 0 : joi.Field<Double>("Eff")
                                  }).ToList()
                     };

            return q1.ToList();
        }
    }
}
