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
    
    public partial class POSIZEDETAIL
    {
        public int POID { get; set; }
        public int PODETAILID { get; set; }
        public string POSIZE { get; set; }
        public double AMOUNT { get; set; }
        public string MARKS { get; set; }
        public string UPDATEUSER { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Nullable<double> InitAmount { get; set; }
        public string SplitFromRcid { get; set; }
        public string ParentRcidSplit { get; set; }
    
        public virtual PO PO { get; set; }
        public virtual PODETAIL PODETAIL { get; set; }
    }
}
