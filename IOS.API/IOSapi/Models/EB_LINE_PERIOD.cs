//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace IOSapi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EB_LINE_PERIOD
    {
        public int id { get; set; }
        public System.DateTime productiondate { get; set; }
        public int lineid { get; set; }
        public string periodname { get; set; }
        public decimal starthour { get; set; }
        public decimal endhour { get; set; }
        public System.DateTime starttime { get; set; }
        public System.DateTime endtime { get; set; }
        public Nullable<int> isot { get; set; }
        public Nullable<int> scheduleid { get; set; }
        public Nullable<decimal> schedulehours { get; set; }
        public Nullable<int> active { get; set; }
        public Nullable<int> workers { get; set; }
        public Nullable<int> targetqty { get; set; }
        public Nullable<int> finishedqty { get; set; }
        public Nullable<int> efficiency { get; set; }
    }
}
