using IOSapi.BLL;
using IOSapi.Models;
using IOSapi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace IOSapi.Controllers
{
    /// <summary>
    /// APS用户
    /// </summary>
    public class UserController : ApiController
    {
        APSEntities db = new APSEntities();

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="strUser">账户名</param>
        /// <param name="strPwd">密码</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public LoginInfo Login(string strUser, string strPwd)
        {
            LoginInfo rm = new LoginInfo();
            try
            {
                strPwd = Commons.Decode(strPwd, "iosapsio");

                var users = db.USERS.Where(c => c.ACCOUNT == strUser).ToList();
                if (users == null || users.Count == 0)
                {
                    rm.ErrMessage = "用户不存在";
                    return rm;
                }

                string usePass = IOS.Encryptor.EncryptorFactory.CreateEncrypter(0).Encrypt(strPwd.Trim());   /*密码加密一下*/ ;

                var user = users.Find(c => c.ACCOUNT == strUser && c.PASS_WORD == usePass);
                if (user == null)
                {
                     rm.ErrMessage = "用户密码错误";
                    return rm;
                }

                AMOData.Settings.SysSetting.Load();
                AMOData.Settings.UserSetting.Load(0);//加载用户参数

                rm.IsSuccess = 1;
                return rm;

                //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, strUser, DateTime.Now,
                //DateTime.Now.AddHours(1), true, string.Format("{0}&{1}", strUser, strPwd),
                //FormsAuthentication.FormsCookiePath);
                ////返回登录结果、用户信息、用户验证票据信息
                //var oUser = new UserInfo { bRes = true, UserName = strUser, Password = strPwd, Ticket = FormsAuthentication.Encrypt(ticket) };
                ////将身份信息保存在session中，验证当前请求是否是有效请求
                ////HttpContext.Current.Session[strUser] = oUser;

            }
            catch (Exception ex)
            {
                rm.IsSuccess = 0;
                rm.ErrMessage = ex.Message;
                return rm;
            }

        }







        //校验用户名密码（正式环境中应该是数据库校验）
        private bool ValidateUser(string strUser, string strPwd)
        {
            if (strUser == "admin" && strPwd == "123456")
            {
                return true;
            }
            else
            {
                return false;
            }
        }








    }
}
