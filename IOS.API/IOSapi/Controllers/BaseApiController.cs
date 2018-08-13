using IOSapi.App_Start;
using IOSapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IOSapi.Controllers
{
    
    //[RequestAuthorize]//权限控制  先关闭
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BaseApiController : ApiController
    {
        APSEntities db = new APSEntities();

        /// <summary>
        /// 获取用户可见工厂id集字符串
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetAllowedFactory(int userId = 0)
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
        /// <summary>
        /// 获取用户可见工厂id集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int[] GetAllowedFactoryIDs(int userId = 0)
        {

            if (userId == 0)
            {
                return db.FACTORies.ToList().Select(c => c.ID).ToArray();
            }
            else
            {
                return db.VIEWDATAPERMISSIONs.Where(c => c.USERID == userId).Select(c => c.FACTORYID).ToArray();
            }
        }
    }
}
