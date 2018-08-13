using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOSapi.ViewModels
{
    /// <summary>
    /// 生产单关键事件
    /// </summary>
    public class PoEvent
    {
        /// <summary>
        /// 事件id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 关键事件流程节点id
        /// </summary>
        public int eventflownodeid { get; set; }
        /// <summary>
        /// 关键事件流程节点名称
        /// </summary>
        public string eventflownodeName { get; set; }
        /// <summary>
        /// 事件完成需要天数
        /// </summary>
        public double duration { get; set; }
        /// <summary>
        /// 推荐日期
        /// </summary>
        private DateTime? _recommendeddate;
        /// <summary>
        /// 推荐日期
        /// </summary>
        public DateTime? recommendeddate
        {
            get
            {
                if (_recommendeddate > new DateTime(2000, 01, 01))
                    return _recommendeddate;
                else
                    return null;
            }
            set { _recommendeddate = value; }
        }
        /// <summary>
        /// 预计完成日期(推荐完成日期)
        /// </summary>
        private DateTime? _startdate;
        /// <summary>
        /// 预计完成日期(推荐完成日期)
        /// </summary>
        public DateTime? startdate
        {
            get
            {
                if (_startdate > new DateTime(2000, 01, 01))
                    return _startdate;
                else
                    return null;
            }
            set { _startdate = value; }
        }
        /// <summary>
        /// 实际完成日期
        /// </summary>
        public DateTime? _enddate { get; set; }
        /// <summary>
        /// 实际完成日期
        /// </summary>
        public DateTime? enddate
        {
            get
            {
                if (_enddate > new DateTime(2000, 01, 01))
                    return _enddate;
                else
                    return null;
            }
            set { _enddate = value; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 是否影响生产计划
        /// </summary>
        public int isaffectplan { get; set; }
        /// <summary>
        /// 标准时间
        /// </summary>
        private DateTime? _standardtime;
        /// <summary>
        /// 标准时间
        /// </summary>
        public DateTime? standardtime
        {
            get
            {
                if (_standardtime > new DateTime(2000, 01, 01))
                    return _standardtime;
                else
                    return null;
            }
            set { _standardtime = value; }
        }
        /// <summary>
        /// 偏移天数
        /// </summary>
        public int diffDays { get; set; }
    }
}