using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IOSapi.BLL
{
    /// <summary>
    /// 系统数据
    /// </summary>
    public static class StaticData
    {

        private static string _OthProcessAddType;

        /// <summary>
        /// 非排产工序录入类型（0不区分车间生产线，1按车间录入，2按产线录入）
        /// </summary>
        public static string OthProcessAddType 
        {
            get
            {
                if (string.IsNullOrEmpty(_OthProcessAddType))
                {
                    APSEntities db = new APSEntities();

                    var data = db.MODELSETTINGS.Where(c => c.NAME == "WorkshopInOtherProcess" || c.NAME == "FacilityInOtherProcess").Select(p => new { p.NAME, p.VALUE }).ToList();
                    var item = data.Where(c => c.NAME == "WorkshopInOtherProcess").FirstOrDefault();
                    if (item != null)
                    {
                        if (item.VALUE == "1")
                        {
                            _OthProcessAddType = "1";
                        }
                        else
                        {
                            var line = data.Where(c => c.NAME == "FacilityInOtherProcess").FirstOrDefault();
                            if (line != null && line.VALUE == "1")
                            {
                                _OthProcessAddType = "2";
                            }
                        }
                    }

                    _OthProcessAddType = "0";
                }
                
                return _OthProcessAddType;
            }
        }


        private static int _CuttingId;

        public static int CuttingId
        {
            get
            {
                if (_CuttingId == 0)
                {
                    _CuttingId = Convert.ToInt32(ConfigurationManager.AppSettings["CuttingId"]);
                }

                return _CuttingId;
            }
        }
    }
}