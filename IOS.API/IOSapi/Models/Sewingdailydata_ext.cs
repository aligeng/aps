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
    
    public partial class Sewingdailydata_ext
    {
        public int ID { get; set; }
        public Nullable<int> Poid { get; set; }
        public Nullable<int> facilityid { get; set; }
        public string facilityname { get; set; }
        public Nullable<int> Processid { get; set; }
        public string Processname { get; set; }
        public Nullable<System.DateTime> Processdate { get; set; }
        public Nullable<System.DateTime> Startdate { get; set; }
        public Nullable<System.DateTime> Enddate { get; set; }
        public Nullable<int> planqty { get; set; }
        public Nullable<int> Dayqty { get; set; }
        public Nullable<int> Totalqty { get; set; }
        public Nullable<double> Duration { get; set; }
        public Nullable<double> Planduration { get; set; }
        public Nullable<int> Troubletype { get; set; }
        public Nullable<double> Schedulepercent { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<System.DateTime> updatetime { get; set; }
        public Nullable<int> factoryid { get; set; }
    }
}
