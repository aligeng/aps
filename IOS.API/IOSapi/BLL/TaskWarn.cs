using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IOS.DBUtility;
using IOSapi.BLL;
using System.Data;
using AMO.BLL;

namespace IOSapi.BLL
{
    /// <summary>
    /// 提醒通知
    /// </summary>
    public class TaskWarn
    {
        APSEntities db = new APSEntities();

        /// <summary>
        /// 获取用户可见工厂id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetAllowedFactory(int userId=0)
        {
            List<FACTORY> ls = new List<Models.FACTORY>();
            string ids = string.Empty;

            if (userId == 0)
            {
                ids = string.Join(",", db.FACTORies.ToList().Select(c => c.ID.ToString()).ToArray());
            }
            else
            {
                ids = string.Join(",", db.VIEWDATAPERMISSIONs.Where(c => c.USERID == userId).Select(c => c.FACTORYID.ToString()).ToArray());
            }

            return ids;
        }



        #region 异常事件提醒

        #region 统计类型个数
        /// <summary>
        /// 获取异常事件提醒的统计数据
        /// </summary>
        /// <param name="jobCodes">对应jobcode,比如: 'NotPlan,NewAdd,PlanChange'</param>
        /// <param name="allowFactoryIDs">用户可访问工厂IDs，比如：'1,2,3'</param>
        /// <param name="jobAlertDays">对应参数JA_JobAlertDays</param>
        /// <param name="ottoffset">对应参数PoOTTOffset</param>
        /// <param name="notmaintaindays">对应参数ScheduleNotMaintain</param>
        /// <returns></returns>
        private List<WarnData> GetUserDelayAlertCountList(string jobCodes, string allowFactoryIDs, int jobAlertDays, int ottoffset, int notmaintaindays)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@p_jobkeys",jobCodes),
                                                              new System.Data.SqlClient.SqlParameter("@p_factoryids",allowFactoryIDs),
                                                              new System.Data.SqlClient.SqlParameter("@p_ottoffset", ottoffset),
                                                              new System.Data.SqlClient.SqlParameter("@p_jobalertdays", jobAlertDays),
                                                              new System.Data.SqlClient.SqlParameter("@p_notmaintaindays", notmaintaindays),
                                                              new System.Data.SqlClient.SqlParameter("@p_currentdate",DateTime.Now)
                                                              };

            var ds = this.db.Database.SqlQuery<WarnData>("exec app_jobalert_delay @p_jobkeys,@p_factoryids,@p_ottoffset,@p_jobalertdays,@p_notmaintaindays,@p_currentdate ", parameters).ToList();

            return ds;
            //slt.ToList();
            //int AllCount = Int32.Parse(parameters[2].Value.ToString()); 

        }


        /// <summary>
        /// 获取 异常事件提醒的统计数据<br/>
        /// 返回数据说明：
        /// DeliveryDelay:生产单交期延误个数
        /// ProgressDelay :生产计划开工期延后个数
        /// StartDelay 生产计划开工期延后个数
        /// MatDelay 物料未按生产计划到货个数
        /// EventDelay 关键事件未按生产计划完成个数
        /// ScheduleNotMaintain  n天内未维护生产日进度的生产计划个数
        /// </summary>
        /// <returns> </returns>
        public DataTable GetUserDelayAlertCount()
        {
            string jobCodes = "DeliveryDelay,ProgressDelay,StartDelay,MatDelay,EventDelay,ScheduleNotMaintain";
            AMOData.Users user = new AMOData.Users();
            user.ID =22;

            return JobAlert.GetUserDelayAlertCount(jobCodes, user);
        }
        #endregion

        #region 明细

        /// <summary>
        /// {0}个生产计划交期延误 (明细数据)
        /// </summary>
        /// <returns></returns>
        public DataTable GetDetailDeliveryDelay()
        {
            string factoryIDs = new TaskWarn().GetAllowedFactory();

            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            //int factoryviewDays = AMOData.Settings.SysSetting.FactoryViewDateDays;//no used
            return AMOData.JobAlertNew.GetDetailDeliveryDelay(factoryIDs, jobAlertDays);//, factoryviewDays
        }




        /// <summary>
        /// 开工期延误 计算
        /// </summary>
        /// <param name="factoryIDs">工厂IDs</param>
        private AMO.Model.BindCollection<AMO.Model.JaStartDelay> CalcStartDelay(string factoryIDs)
        {

            AMO.Model.BindCollection<AMO.Model.JaStartDelay> lst = new AMO.Model.BindCollection<AMO.Model.JaStartDelay>();

            //所有未完成的生产计划
            Filter filFacility = new Filter();
            filFacility.Add("FactoryID", factoryIDs, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            string distFacilitySQL = DBHelper.GetDistinctSql("Facility", "ID", filFacility);

            Filter filter = new Filter();
            filter.Add("Status", 0, RELEATTION_TYPE.EQUAL, LOGIC_TYPE.AND);
            filter.Add("FacilityID", distFacilitySQL, RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            filter.Add("POID", "(select ID from PO WHERE STATUS=0)", RELEATTION_TYPE.IN, LOGIC_TYPE.AND);
            List<AMOData.ProductionEvent> lstPE = AMOData.ProductionEvent.GetList(filter);

            Dictionary<int, AMOData.Facility> dicFacility = AMOData.Facility.GetDictionary(null);

            //计算计划 最早可开工期
            AMO.Logic.EarliestStart es = new AMO.Logic.EarliestStart(lstPE);
            Dictionary<int, DateTime> dicES = es.CacuESFromProdEvent();
            foreach (AMOData.ProductionEvent pe in lstPE)
            {
                //最早可开工期>计划开始日期
                DateTime dateES = dicES.ContainsKey(pe.ID) ? dicES[pe.ID] : DateTime.MinValue;
                if (dateES > pe.StartTime.Date)
                {
                    AMO.Model.JaStartDelay ja = new AMO.Model.JaStartDelay();
                    ja.ProductionEventID = pe.ID;
                    ja.PlanStart = pe.StartTime;
                    ja.EarliestStartDate = dateES;
                    ja.DelayDays = Convert.ToInt32((dateES.Date - pe.StartTime.Date).TotalDays);
                    ja.LineName = dicFacility.ContainsKey(pe.FacilityID) ? dicFacility[pe.FacilityID].Name : "";
                    ja.FactoryID = dicFacility.ContainsKey(pe.FacilityID) ? dicFacility[pe.FacilityID].FactoryID : -1;
                    //其他取数放在弹出窗口处理
                    lst.Add(ja);
                }
            }

            return lst;
        }



        #endregion

        #endregion

        #region 物料与关键事件提醒

        /// <summary>
        /// 获取异常事件提醒的统计数据
        /// </summary>
        /// <param name="jobCodes">对应jobcode,比如: 'NotPlan,NewAdd,PlanChange'</param>
        /// <param name="allowFactoryIDs">用户可访问工厂IDs，比如：'1,2,3'</param>
        /// <param name="jobAlertDays">对应参数JA_JobAlertDays</param>
        /// <param name="ottoffset">对应参数PoOTTOffset</param>
        /// <returns></returns>
        public ViewModels.UserMatEventCount GetUserMatEventAlertCount(string jobCodes, string allowFactoryIDs, int jobAlertDays, int ottoffset)
        {
            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@p_jobkeys",jobCodes),
                                                              new System.Data.SqlClient.SqlParameter("@p_factoryids",allowFactoryIDs),
                                                              new System.Data.SqlClient.SqlParameter("@p_ottoffset", ottoffset),
                                                              new System.Data.SqlClient.SqlParameter("@p_jobalertdays", jobAlertDays),
                                                              new System.Data.SqlClient.SqlParameter("@p_currentdate",DateTime.Now)
                                                              };

            ViewModels.UserMatEventCount ds = this.db.Database.SqlQuery<ViewModels.UserMatEventCount>("exec app_jobalert_mat_event @p_jobkeys,@p_factoryids,@p_ottoffset,@p_jobalertdays,@p_currentdate ", parameters).FirstOrDefault();

            return ds;
        }

        /// <summary>
        /// {0}个生产计划交期延误 (明细数据)
        /// </summary>
        /// <param name="factoryIDs">工厂IDs</param>
        /// <param name="jobAlertDays">对应参数JA_JobAlertDays</param>
        /// <returns></returns>
        public static DataTable GetDetailDeliveryDelay(string factoryIDs, int jobAlertDays)//, int factoryviewDays
        {
            //DelayDays 排产器的延误天数差1天，所以+1
            #region sql
            string sqlStr = string.Empty;
            if (DBHelper.CurrentDatabase.DbType == DB_TYPE.Oracle)
            {
                sqlStr = @"SELECT T3.StartTime,T4.NAME FacilityName,T4.FactoryID,T5.CODE PoCode,T6.NAME CustomerName,T5.CustomerStyleNO,T5.Pattern,T7.NAME ProductTypeName,
                                  T1.DeliveryDate,(T3.ENDTIME+T5.FACTORYVIEWDATEDAYS) FactoryDelivery,T8.USERNAME Merchandiser,T5.Description,
                                  (ROUND(TO_NUMBER(T3.ENDTIME+T5.FACTORYVIEWDATEDAYS -T1.DELIVERYDATE))+1) DelayDays,T1.PlanAmount,T5.Amount
                                FROM (
                                  SELECT PRODUCTIONEVENTID,POID,DELIVERYDATE,SUM(PLANAMOUNT) PlanAmount,SUM(AMOUNT) Amount
                                  FROM PRODUCTIONEVENTDETAIL
                                  GROUP BY PRODUCTIONEVENTID,POID,DELIVERYDATE
                                ) T1
                                JOIN (
                                  SELECT DISTINCT PRODUCTIONEVENTID,p.FACTORYVIEWDATEDAYS 
                                  FROM PRODUCTIONEVENTDETAIL  a JOIN PO p ON a.POID=P.ID
                                ) T2 ON T2.PRODUCTIONEVENTID = T1.PRODUCTIONEVENTID
                                JOIN PRODUCTIONEVENT T3 ON T3.ID = T1.PRODUCTIONEVENTID
                                JOIN FACILITY T4 ON T4.ID = T3.FACILITYID
                                JOIN PO T5 ON T5.ID = T1.POID
                                LEFT JOIN CUSTOMER T6 ON T6.ID = T5.CUSTOMERID
                                LEFT JOIN PRODUCTTYPE T7 ON T7.ID = T5.PRODUCTTYPEID
                                LEFT JOIN USERS T8 ON T8.ID= T5.MERCHANDISER
                                WHERE {0}
                                  AND ROUND(TO_NUMBER(T3.ENDTIME+T5.FACTORYVIEWDATEDAYS -T1.DELIVERYDATE)) > 0
                                  AND T4.FACTORYID IN({1}) AND T5.STATUS=0 ORDER BY T5.CODE,T1.DELIVERYDATE";
            }
            else
            {
                sqlStr = @"SELECT  min(StartTime) as StartTime,MAX(FacilityName) as FacilityName,max(FactoryID) as FactoryID ,max(PoCode) as PoCode,MAX(CustomerName) as CustomerName,
MAX(CustomerStyleNO) as CustomerStyleNO ,max(Pattern) as Pattern,max(Priority) as Priority,MAX(ProductTypeName) as ProductTypeName,MAX(CAPACITY) as CAPACITY,
	                                max(DeliveryDate) as DeliveryDate,MAX(FactoryDelivery) as FactoryDelivery,MAX(Merchandiser) Merchandiser,MAX(Description) as Description,
	                                MAX(DelayDays) as DelayDays,SUM(PlanAmount) as PlanAmount,max(Amount) as Amount,MAX(INITDELIVERYDATE) INITDELIVERYDATE,
									MAX(EARLINESSSTARTDATE) EARLINESSSTARTDATE,MAX(CAOTD) CAOTD,MAX(MASTERMATERIALRECEIVEDDATE) MASTERMATERIALRECEIVEDDATE
	                                  FROM (
    SELECT T3.StartTime,T4.NAME FacilityName,T4.FactoryID,T5.CODE PoCode,T6.NAME CustomerName,T5.CustomerStyleNO,T5.Pattern,T5.Priority,T7.NAME ProductTypeName,
	                                T1.DeliveryDate,(T3.ENDTIME+T5.FACTORYVIEWDATEDAYS) FactoryDelivery,T8.USERNAME Merchandiser,T5.Description,
	                                (DATEDIFF(D, T1.DELIVERYDATE, T3.ENDTIME+T5.FACTORYVIEWDATEDAYS))+1 DelayDays,T1.PlanAmount,T5.Amount,
                                    T5.ID AS POID,T4.ID AS FACILITYID,T5.INITDELIVERYDATE,T5.EARLINESSSTARTDATE,T5.CAOTD,T5.MASTERMATERIALRECEIVEDDATE,T5.CAPACITY
                                FROM (
	                                SELECT PRODUCTIONEVENTID,POID,DELIVERYDATE,SUM(PLANAMOUNT) PlanAmount,SUM(AMOUNT) Amount
	                                FROM dbo.PRODUCTIONEVENTDETAIL
	                                GROUP BY PRODUCTIONEVENTID,POID,DELIVERYDATE
                                ) T1
                                JOIN (
	                                SELECT DISTINCT PRODUCTIONEVENTID,p.FACTORYVIEWDATEDAYS 
	                                FROM dbo.PRODUCTIONEVENTDETAIL  a JOIN PO p ON a.POID=P.ID
                                ) T2 ON T2.PRODUCTIONEVENTID = T1.PRODUCTIONEVENTID
                                JOIN dbo.PRODUCTIONEVENT T3 ON T3.ID = T1.PRODUCTIONEVENTID
                                JOIN dbo.FACILITY T4 ON T4.ID = T3.FACILITYID
                                JOIN dbo.PO T5 ON T5.ID = T1.POID
                                LEFT JOIN dbo.CUSTOMER T6 ON T6.ID = T5.CUSTOMERID
                                LEFT JOIN dbo.PRODUCTTYPE T7 ON T7.ID = T5.PRODUCTTYPEID
                                LEFT JOIN dbo.USERS T8 ON T8.ID= T5.MERCHANDISER
                                WHERE {0}
	                                AND DATEDIFF(D, T1.DELIVERYDATE, T3.ENDTIME+T5.FACTORYVIEWDATEDAYS) > 0
	                                AND T4.FACTORYID IN({1}) AND T5.STATUS=0 ) a
	                                	group by POID,FACTORYID,FACILITYID
	                                ORDER BY PoCode,DELIVERYDATE";
            }

            DateTime dNow = AMOData.AMOTime.Now.Date;
            Filter filter = new Filter();
            filter.Add("T1.DELIVERYDATE", dNow.Date, RELEATTION_TYPE.GREATEREQUAL, LOGIC_TYPE.AND);
            filter.Add("T1.DELIVERYDATE", dNow.AddDays(jobAlertDays).Date, RELEATTION_TYPE.LESSEQUAL, LOGIC_TYPE.AND);

            string sqlWhere = filter.GetFilter(false, false);

            sqlStr = string.Format(sqlStr, sqlWhere, factoryIDs); //factoryviewDays,
            #endregion

            DataSet ds = DBHelper.ExecuteDataset(DBHelper.CurrentDatabase.ConnectionString, CommandType.Text, sqlStr);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }


        /// <summary>
        /// N天内的生产计划有多少个物料未到货
        /// </summary>
        /// <param name="factoryIDs">工厂IDs</param>
        /// <param name="ottoffset">参数OTT偏移量PoOTTOffset</param>
        /// <returns></returns>
        public static DataTable GetDetailMatDelay(int type, string factoryIDs, int ottoffset)
        {
            #region sql
            string strW = string.Empty;
            if (type > -1)
            {
                strW = "m.MaterialStyle = " + type;
            }
            else
            {
                strW = "(m.MaterialStyle =0 or m.MaterialStyle =1 )";
            }

            string sqlStr = string.Empty;
            if (DBHelper.CurrentDatabase.DbType == DB_TYPE.Oracle)
            {

            
                sqlStr = @"select m.ID POMaterialRequestID, m.MPNO, m.Supplier,

 (CASE  WHEN  m.MPDate <= to_timestamp('1901-01-01','yyyy-mm-dd hh24:mi:ss') then null else m.mpdate end) MPDate,
(CASE  WHEN   m.ExpectedReciveDate <= to_timestamp('1901-01-01','yyyy-mm-dd hh24:mi:ss') then null else  m.ExpectedReciveDate end)  ExpectedReciveDate,

m.MaterialCode, m.MaterialName, m.MaterialColor, m.MaterialSize, m.Unit,m.Amount,
 ps.NeedDate, round( to_number(({2}+0)-ps.NeedDate)) DelayDays, m.MaterialStyle, 
p.Code PoCode,p.Pattern,p.CustomerStyleNo from PoMaterialRequest m 
inner join po p on m.POID =p.ID
inner join (
	select POID, MinStartTime-{1} NeedDate from (
		select p.ID POID, min(pe.StartTime) MinStartTime from po p 
		inner join ProductionEventDetail ped on ped.poid=p.id
		inner join ProductionEvent pe on pe.id=ped.ProductionEventID
		and p.FactoryID in ({0}) 
		group by p.ID
	) t where MinStartTime-{1}<{2}
) ps on ps.POID=p.ID
where (m.FinishedDate is null or m.FinishedDate<to_timestamp('1901-01-01','yyyy-mm-dd hh24:mi:ss'))
and round(to_number(({2}+0) - ps.NeedDate))>0
and p.STATUS=0 and " + strW;
            }
            else
            {
                sqlStr = @"select m.ID POMaterialRequestID, m.MPNO, m.Supplier, 
 (CASE  WHEN  m.MPDate <= '1901-1-1' then null else m.mpdate end) MPDate,
 (CASE  WHEN  m.ExpectedReciveDate <= '1901-1-1' then null else m.ExpectedReciveDate end) ExpectedReciveDate,
m.MaterialCode, m.MaterialName, m.MaterialColor, m.MaterialSize, m.Unit,m.Amount,
 ps.NeedDate, datediff(d,ps.NeedDate,{2}) DelayDays, m.MaterialStyle, 
p.Code PoCode,p.Pattern,p.CustomerStyleNo,cu.NAME CustomerName  from PoMaterialRequest m 
inner join po p on m.POID =p.ID
LEFT JOIN dbo.CUSTOMER cu ON cu.ID=p.CUSTOMERID
inner join (
	select POID, dateadd(d,-{1},MinStartTime) NeedDate from (
		select p.ID POID, min(pe.StartTime) MinStartTime from po p 
		inner join ProductionEventDetail ped on ped.poid=p.id
		inner join ProductionEvent pe on pe.id=ped.ProductionEventID
		and p.FactoryID in ({0}) 
		group by p.ID
	) t where convert(varchar(100),dateadd(d,-{1},MinStartTime),23)<{2}
) ps on ps.POID=p.ID
where (m.FinishedDate is null or m.FinishedDate<'1901-1-1')
and datediff(d,ps.NeedDate,{2})>0
and p.STATUS=0 and " + strW;
            }

            DateTime dNow =DateTime.Now.Date;
            string strNowDate = DBHelper.GetDateTimeSqlPart(dNow);
            sqlStr = string.Format(sqlStr, factoryIDs, ottoffset, strNowDate);
            #endregion

            DataSet ds = DBHelper.ExecuteDataset(DBHelper.CurrentDatabase.ConnectionString, CommandType.Text, sqlStr);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }


        /// <summary>
        /// XX个物料需在YY内天到货 (明细数据)
        /// </summary>
        /// <param name="fids">工厂ID字符</param>
        /// <returns></returns>
        public static List<IOSapi.ViewModels.JaMaterial> GetDetailMatReceive(int pageIndex, int pageSize, int type, string code,string fids)
        {
            try
            {
                int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
                int ottoffset = AMOData.Settings.SysSetting.PoOTTOffset;

                string sqlStr = string.Format(@"select m.MPNO, m.Supplier, m.MPDate,ps.CODE,
m.MaterialCode, m.MaterialName, m.MaterialColor, m.MaterialSize, m.MPUnit, m.MaterialStyle, sum(m.MPAmount) MPAmount,
max(m.ExpectedReciveDate) ExpectedReciveDate,ps.pattern,ps.NeedDate
FROM PoMaterialRequest m
inner join (						
		SELECT p.ID AS POID,p.CODE,p.PATTERN, dateadd(d,-{1},min(pe.StartTime)) AS NeedDate from po p 
		inner join ProductionEventDetail ped on ped.poid=p.id
		inner join ProductionEvent pe on pe.id=ped.ProductionEventID
		WHERE p.FactoryID in ({0})	AND p.ISFIRMORDER=1 AND p.STATUS=0 AND pe.STATUS=0	GROUP BY p.ID,p.CODE,p.PATTERN						
) ps on ps.POID=m.POID
where (m.MaterialStyle=0 or m.MaterialStyle=1) and ( m.ExpectedReciveDate <= cast('{2}' as datetime) 
AND m.ExpectedReciveDate > cast('{3}' as datetime) ) AND (m.FinishedDate is null  OR m.FinishedDate < cast('1901-01-01 00:00:00' as datetime))
group BY ps.CODE, m.MPNO, m.Supplier, m.MPDate,
m.MaterialCode, m.MaterialName, m.MaterialColor, m.MaterialSize, m.MPUnit, m.MaterialStyle,ps.pattern,ps.NeedDate ",
     fids, ottoffset, DateTime.Now.AddDays(jobAlertDays).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));


                DataSet ds = DBHelper.ExecuteDataset(DBHelper.CurrentDatabase.ConnectionString, CommandType.Text, sqlStr);

                var query = ds.Tables[0].AsEnumerable().Where(c => (string.IsNullOrEmpty(code) ? true : (c.Field<string>("CODE").Contains(code) || c.Field<string>("Pattern").Contains(code))) &&
                        (type > -1 ? c.Field<int>("MaterialStyle") == type : (c.Field<int>("MaterialStyle") == 0 || c.Field<int>("MaterialStyle") == 1))).OrderBy(c =>
                        c.Field<DateTime>("ExpectedReciveDate")).Take(pageSize * pageIndex).Skip(pageSize * (pageIndex - 1)).Select(p => new ViewModels.JaMaterial()
                        {
                            //DelayDays = p.Field<int>("DelayDays"),
                            ExpectedReciveDate = p.Field<DateTime>("ExpectedReciveDate"),
                            MaterialCode = p.Field<string>("MaterialCode"),
                            MaterialColor = p.Field<string>("MaterialColor"),
                            MaterialName = p.Field<string>("MaterialName"),
                            MaterialSize = p.Field<string>("MaterialSize"),
                            MaterialStyle = p.Field<int>("MaterialStyle"),
                            MPAmount = p.Field<double>("MPAmount"),
                            MPDate = p.Field<DateTime>("MPDate").ToString("yyyy-MM-dd"),
                            MPNO = p.Field<string>("MPNO"),
                            MPUnit = p.Field<string>("MPUnit"),
                            NeedDate = p.Field<DateTime>("NeedDate").ToString("yyyy-MM-dd"),
                            Pattern = p.Field<string>("Pattern"),
                            PoCode = p.Field<string>("CODE"),
                            Supplier = p.Field<string>("Supplier"),
                            

                        }).ToList();

                return query;
            }
            catch (Exception ex)
            {
            }

            return new List<ViewModels.JaMaterial>();



        }






        #endregion


        /// <summary>
        /// {0}个关键事件未按生产计划完成 (明细数据)
        /// </summary>
        /// <param name="factoryIDs">工厂IDs</param>
        /// <returns></returns>
        public static DataTable GetDetailEventDelay(string factoryIDs)
        {

            #region sql
            string sqlStr = string.Empty;
            if (DBHelper.CurrentDatabase.DbType == DB_TYPE.Oracle)
                sqlStr = @"select p.FactoryID, p.Code PoCode, c.Name CustomerName, p.CustomerStyleNO, p.Pattern, pt.Name ProductTypeName,
p.DeliveryDate,pst.MinStartTime PlanStart,ce.id EventId,ce.Name EventName,pe.StartDate ExpectFinishDate,
round(to_number((({1} + 0 ) -pe.StartDate))) DelayDays, u.UserName Merchandiser,
f_get_criticaleventman(p.FactoryID,p.CustomerID,ce.ID) CriticalEventManID  
from PoEvent pe
inner join PO p on pe.POID=p.ID
inner join (
	select p.ID POID,min(pe.StartTime) MinStartTime from PO p 
	inner join ProductionEventDetail ped on ped.POID=p.ID
	inner join ProductionEvent pe on pe.ID=ped.ProductionEventID
	group by p.ID 
) pst on pst.POID=p.ID
left join EventFlowNode ef on ef.ID=pe.EventFlowNodeID
left join CriticalEvent ce on ce.ID=ef.CriticalEventID
left join Customer c on p.CustomerID=c.ID
left join ProductType pt on p.ProductTypeID=pt.ID
left join Users u on p.Merchandiser=u.ID
where p.status=0 and
(pe.EndDate is null or pe.EndDate<to_timestamp('1901-1-1','yyyy-mm-dd hh24:mi:ss')) and (pe.StartDate is not null) and 
pe.StartDate> to_timestamp('1900-1-1','yyyy-mm-dd hh24:mi:ss') and 
pe.StartDate < {1}
and round(to_number(({1} + 0 ) - pe.StartDate))>0
and p.FactoryID in ({0})";
            else
                sqlStr = @"select p.FactoryID, p.Code PoCode, c.Name CustomerName, p.CustomerStyleNO, p.Pattern, pt.Name ProductTypeName,
p.DeliveryDate,pst.MinStartTime PlanStart,ce.id EventId, ce.Name EventName,pe.StartDate ExpectFinishDate,
datediff(d, pe.StartDate, {1}) DelayDays, u.UserName Merchandiser,
dbo.f_get_criticaleventman(p.FactoryID,p.CustomerID,ce.ID) CriticalEventManID  
from PoEvent pe
inner join PO p on pe.POID=p.ID
inner join (
	select p.ID POID,min(pe.StartTime) MinStartTime from PO p 
	inner join ProductionEventDetail ped on ped.POID=p.ID
	inner join ProductionEvent pe on pe.ID=ped.ProductionEventID
	group by p.ID 
) pst on pst.POID=p.ID
left join EventFlowNode ef on ef.ID=pe.EventFlowNodeID
left join CriticalEvent ce on ce.ID=ef.CriticalEventID
left join Customer c on p.CustomerID=c.ID
left join ProductType pt on p.ProductTypeID=pt.ID
left join Users u on p.Merchandiser=u.ID
where p.status=0 and
(pe.EndDate is null or pe.EndDate<'1901-1-1') and (pe.StartDate is not null) and 
pe.StartDate>'1900-1-1' and 
pe.StartDate < convert(varchar(100),{1},23) 
and datediff(d, pe.StartDate, {1})>0
and p.FactoryID in ({0}) ";

            DateTime dNow =DateTime.Now.Date;
            string strNowDate = DBHelper.GetDateTimeSqlPart(dNow);
            sqlStr = string.Format(sqlStr, factoryIDs, strNowDate);
            #endregion

            DataSet ds = DBHelper.ExecuteDataset(DBHelper.CurrentDatabase.ConnectionString, CommandType.Text, sqlStr);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            return null;
        }



    }
}