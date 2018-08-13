using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 返回类型
    /// </summary>
    public class ResultModel
    {
        public ResultModel()
        {
            IsSuccess = 0;
            ErrMessage = new List<LangMessage>();
        }

        /// <summary>
        /// 成功标志：1成功；0失败
        /// </summary>
        public int IsSuccess { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public List<LangMessage> ErrMessage { get; set; }
    }

    /// <summary>
    /// 语言提示
    /// </summary>
    public class LangMessage
    {
        public LangMessage(string lang, string message)
        {
            Lang = lang;
            Message = message;
        }

        /// <summary>
        /// 语言
        /// </summary>
        public string Lang { get; set; }
        /// <summary>
        /// 信息内容
        /// </summary>
        public string Message { get; set; }
    }
}