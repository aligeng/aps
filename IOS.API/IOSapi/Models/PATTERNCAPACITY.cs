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
    
    public partial class PATTERNCAPACITY
    {
        public int ID { get; set; }
        public string PATTERN { get; set; }
        public int PROCESSID { get; set; }
        public double CAPACITY { get; set; }
        public string UPDATEUSER { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    
        public virtual PROCESS PROCESS { get; set; }
    }
}
