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
    /// 主界面接口
    /// </summary>
    public class MainController : BaseApiController
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
        /// 获取生产单交期延误单个数 id:6-普通，8-重要，10-紧急
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Main/GetDeliveryDelayCount")]
        public WarnTips GetDeliveryDelayCount()
        {
            WarnTips wt = new WarnTips();

            string allowFactoryIDs = GetAllowedFactory();
            if (string.IsNullOrEmpty(allowFactoryIDs))
                return wt;

            int jobAlertDays = AMOData.Settings.SysSetting.JA_JobAlertDays;
            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@p_factoryids",allowFactoryIDs),
                                                              new System.Data.SqlClient.SqlParameter("@p_jobalertdays", jobAlertDays),
                                                              new System.Data.SqlClient.SqlParameter("@p_currentdate",DateTime.Now.Date)
                                                              };
            wt.TipsCountList = this.db.Database.SqlQuery<ViewModels.TipsCount>("exec app_poDelivery_delay @p_factoryids,@p_jobalertdays,@p_currentdate ", parameters).ToList();


            if (wt.TipsCountList == null)
            {
                wt.TipsCountList = new List<TipsCount>();
            }
            if (!wt.TipsCountList.Exists(c => c.Id == 6))
            {
                wt.TipsCountList.Add(new TipsCount() { Id = 6, Name = "", Amount = 0 });
            }
            if (!wt.TipsCountList.Exists(c => c.Id == 8))
            {
                wt.TipsCountList.Add(new TipsCount() { Id = 8, Name = "", Amount = 0 });
            }
            if (!wt.TipsCountList.Exists(c => c.Id == 10))
            {
                wt.TipsCountList.Add(new TipsCount() { Id = 10, Name = "", Amount = 0 });
            }
            wt.TipsCountList = wt.TipsCountList.OrderBy(c => c.Id).ToList();
            return wt;
        }


        /// <summary>
        /// 获取关键事件延误单个数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Main/GetEventsDelayCount")]
        public WarnTips GetEventsDelayCount()
        {
            WarnTips wt = new WarnTips();
            var ls = this.db.Database.SqlQuery<TipsCount>("exec app_main_EventDelayCount").ToList();

            wt.TipsCountList = ls.Where(c => c.Amount > 0).ToList();

            //wt.TipsCountList.Add(new TipsCount() { Id = 1, Name = "PP办", Amount = 19 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 2, Name = "工艺制单", Amount = 4 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 3, Name = "纸样", Amount = 16 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 4, Name = "排麦架", Amount = 18 });

            return wt;
        }

        /// <summary>
        /// 获取工序延误单个数 
        /// </summary> 
        /// <returns></returns>
        [HttpGet, Route("api/Main/GetProcessDelayCount")]
        public WarnTips GetProcessDelayCount()
        {
            WarnTips wt = new WarnTips();

            //var ls = this.db.Database.SqlQuery<TipsCount>("exec app_main_ProcessDelayCount").ToList();

            var ls = db.PROCESSes.Select(p => new TipsCount()
            {
                Id = p.ID,
                Name = p.NAME,
                Amount = db.Processdailydata_ext.Count(c => c.Processid == p.ID && c.Type == 2 && (c.Troubletype == 2 || c.Troubletype == 3) &&
                        c.Processdate.Value == db.Processdailydata_ext.OrderByDescending(d => d.Processdate).FirstOrDefault().Processdate)
            }).OrderBy(c=>c.Id).ToList();

            if (ls != null)
            {
                wt.TipsCountList = ls.Where(c => c.Amount > 0).ToList();
            }
            else
            {
                ls = new List<TipsCount>();
            }

            return wt;

            //wt.TipsCountList.Add(new TipsCount() { Id = 1, Name = "裁剪", Count = 9 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 2, Name = "装篮", Count = 14 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 3, Name = "钉纽", Count = 23 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 4, Name = "车缝", Count = 8 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 5, Name = "包装", Count = 21 });
            //wt.TipsCountList.Add(new TipsCount() { Id = 6, Name = "入库", Count = 27 });
            //return wt;


        }

        /// <summary>
        /// 获取物料延误单个数 0辅料；1面料
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Main/GetMaterialDelayCount")]
        public WarnTips GetMaterialDelayCount()
        {
            WarnTips wt = new WarnTips();
            wt.TipsCountList = new List<TipsCount>();

            string allowFactoryIDs = new TaskWarn().GetAllowedFactory();
            if (string.IsNullOrEmpty(allowFactoryIDs))
                return null;
            int ottoffset = AMOData.Settings.SysSetting.PoOTTOffset;

            System.Data.SqlClient.SqlParameter[] parameters = {
                                                              new System.Data.SqlClient.SqlParameter("@p_factoryids",allowFactoryIDs),
                                                              new System.Data.SqlClient.SqlParameter("@p_ottoffset", ottoffset),
                                                              new System.Data.SqlClient.SqlParameter("@p_currentdate",DateTime.Now)
                                                              };

            wt.TipsCountList = this.db.Database.SqlQuery<TipsCount>("exec app_main_mat_delay @p_factoryids,@p_ottoffset,@p_currentdate ", parameters).ToList();

            return wt;
        }


        /// <summary>
        /// 获取今日工作
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/Main/GetTodayCount")]
        public WarnTips GetTodayCount()
        {
            WarnTips wt = new WarnTips();
            wt.TipsCountList = new List<TipsCount>();

            wt.TipsCountList.Add(new TipsCount() { Id = 1, Name = "关键事件", Amount = 18 });
            wt.TipsCountList.Add(new TipsCount() { Id = 2, Name = "生产", Amount = 23 });
            wt.TipsCountList.Add(new TipsCount() { Id = 3, Name = "物料采购", Amount = 52 });

            return wt;
        }



    }
}
