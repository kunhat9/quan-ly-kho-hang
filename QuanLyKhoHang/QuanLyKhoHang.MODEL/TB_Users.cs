//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyKhoHang.MODEL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_Users
    {
        public TB_Users()
        {
            this.TB_Orders = new HashSet<TB_Orders>();
        }
    
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserFullName { get; set; }
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
        public Nullable<System.DateTime> UserDateCreated { get; set; }
        public string UserPassword { get; set; }
        public Nullable<int> UserType { get; set; }
        public Nullable<int> UserStatus { get; set; }
        public string UserNote { get; set; }
        public string Avatar { get; set; }
    
        public virtual ICollection<TB_Orders> TB_Orders { get; set; }
    }
}
