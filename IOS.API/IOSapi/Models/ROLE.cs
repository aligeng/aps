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
    
    public partial class ROLE
    {
        public ROLE()
        {
            this.ROLEFRAMEPOPEDOMS = new HashSet<ROLEFRAMEPOPEDOM>();
            this.ROLEJOBPOPEDOMS = new HashSet<ROLEJOBPOPEDOM>();
            this.USERROLES = new HashSet<USERROLE>();
        }
    
        public int ID { get; set; }
        public string ROLENAME { get; set; }
        public string REMARK { get; set; }
    
        public virtual ICollection<ROLEFRAMEPOPEDOM> ROLEFRAMEPOPEDOMS { get; set; }
        public virtual ICollection<ROLEJOBPOPEDOM> ROLEJOBPOPEDOMS { get; set; }
        public virtual ICollection<USERROLE> USERROLES { get; set; }
    }
}
