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
    
    public partial class PRODUCTIONSCHEDULE
    {
        public int ID { get; set; }
        public int POID { get; set; }
        public Nullable<int> PRODUCTIONEVENTID { get; set; }
        public int PROCESSID { get; set; }
        public System.DateTime PRODUCTIONDATE { get; set; }
        public string POCOLOR { get; set; }
        public string POSIZE { get; set; }
        public Nullable<double> AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> DURATION { get; set; }
        public Nullable<double> FPY { get; set; }
        public Nullable<int> WORKERS { get; set; }
        public string LOTNO { get; set; }
        public Nullable<double> OVERTIME { get; set; }
        public Nullable<double> DECWORKERS { get; set; }
        public string UPDATEUSER { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
        public string BATCH { get; set; }
        public Nullable<int> WORKSHOPID { get; set; }
    
        public virtual PO PO { get; set; }
        public virtual PROCESS PROCESS { get; set; }
    }
}
