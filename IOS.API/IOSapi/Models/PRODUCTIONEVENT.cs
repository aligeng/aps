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
    
    public partial class PRODUCTIONEVENT
    {
        public PRODUCTIONEVENT()
        {
            this.PRODUCTIONEVENTDETAILs = new HashSet<PRODUCTIONEVENTDETAIL>();
        }
    
        public int ID { get; set; }
        public int POID { get; set; }
        public int FACILITYID { get; set; }
        public Nullable<double> CAPACITY { get; set; }
        public Nullable<double> EFFICIENCY { get; set; }
        public Nullable<int> WORKERNUMBER { get; set; }
        public Nullable<System.DateTime> MAKEDAYSTARTTIME { get; set; }
        public System.DateTime STARTTIME { get; set; }
        public System.DateTime ENDTIME { get; set; }
        public double DURATION { get; set; }
        public Nullable<int> LOCKED { get; set; }
        public Nullable<int> ISFIRMORDER { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<int> ISMERGEGTM { get; set; }
        public string GROUPID { get; set; }
        public Nullable<int> ISCOMPELEARLIST { get; set; }
        public Nullable<int> LNCSERIALNUMBER { get; set; }
        public Nullable<int> CHILDFACILITYNO { get; set; }
        public string UPDATEUSER { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public Nullable<int> BaseDataID { get; set; }
        public Nullable<int> SWITCHTYPE { get; set; }
        public Nullable<int> SWITCHTIME { get; set; }
    
        public virtual FACILITY FACILITY { get; set; }
        public virtual ICollection<PRODUCTIONEVENTDETAIL> PRODUCTIONEVENTDETAILs { get; set; }
    }
}
