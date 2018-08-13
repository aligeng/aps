using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.Models
{
    public class UserInfo
    {
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool bRes { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 加密的票据信息Ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 允许访问的工厂id集
        /// </summary>
        public string AllowFactoryIDs { get; set; }
    }

    public class LoginInfo
    {
        /// <summary>
        /// 成功标志：1成功；0失败
        /// </summary>
        public int IsSuccess { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string ErrMessage { get; set; }
        /// <summary>
        /// 加密的票据信息Ticket
        /// </summary>
        public string Ticket { get; set; }
    }

}