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
    
    public partial class OPDWFPatternInfo
    {
        public int ID { get; set; }
        public Nullable<int> FACTORYID { get; set; }
        public string FACTORYNAME { get; set; }
        public Nullable<int> FACILITYID { get; set; }
        public string FACILITYNAME { get; set; }
        public Nullable<int> CUSTOMERID { get; set; }
        public string CUSTOMERNAME { get; set; }
        public Nullable<int> POID { get; set; }
        public string PATTERN { get; set; }
        public string PATTERNDESC { get; set; }
        public string POCOLOR { get; set; }
        public string POSIZE { get; set; }
        public System.DateTime DELIVERYDATE { get; set; }
        public Nullable<double> AMOUNT { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<System.DateTime> STARTTIME { get; set; }
        public Nullable<System.DateTime> UPDATETIME { get; set; }
    }
}
