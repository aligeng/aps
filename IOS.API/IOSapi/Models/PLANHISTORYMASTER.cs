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
    
    public partial class PLANHISTORYMASTER
    {
        public PLANHISTORYMASTER()
        {
            this.PLANHISTORYDETAILs = new HashSet<PLANHISTORYDETAIL>();
        }
    
        public int ID { get; set; }
        public Nullable<System.DateTime> SCHEDULESTARTDATE { get; set; }
        public Nullable<System.DateTime> SCHEDULEENDDATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string UPDATEUSER { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    
        public virtual ICollection<PLANHISTORYDETAIL> PLANHISTORYDETAILs { get; set; }
    }
}
