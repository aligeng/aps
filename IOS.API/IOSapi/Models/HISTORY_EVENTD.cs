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
    
    public partial class HISTORY_EVENTD
    {
        public int ID { get; set; }
        public Nullable<int> MID { get; set; }
        public int LOGTYPE { get; set; }
        public int LOGCAUSE { get; set; }
        public Nullable<int> PRODEVENTID { get; set; }
        public Nullable<int> PRODEVENTDETAILID { get; set; }
        public Nullable<int> POID { get; set; }
        public Nullable<int> SORTORDER { get; set; }
        public string FIELDNAME { get; set; }
        public Nullable<int> FIELDTYPE { get; set; }
        public string OLDVALUE { get; set; }
        public string NEWVALUE { get; set; }
        public string DESCRIPTION { get; set; }
        public string FIELDVALUE { get; set; }
    
        public virtual HISTORY_EVENTM HISTORY_EVENTM { get; set; }
    }
}
