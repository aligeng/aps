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
    
    public partial class USERPREFERENCE
    {
        public int USERID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string CATEGORY { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<System.DateTime> UPDATEDATE { get; set; }
    
        public virtual USER USER { get; set; }
    }
}
