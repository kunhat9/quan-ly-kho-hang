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
    
    public partial class TB_OrderDetails
    {
        public int DetailId { get; set; }
        public Nullable<int> DetailNumber { get; set; }
        public Nullable<decimal> DetailPrice { get; set; }
        public Nullable<System.DateTime> DetailValueDate { get; set; }
        public Nullable<System.DateTime> DetailExpiredDate { get; set; }
        public Nullable<int> DetailOrderId { get; set; }
        public Nullable<int> DetailProductId { get; set; }
    
        public virtual TB_Orders TB_Orders { get; set; }
        public virtual TB_Products TB_Products { get; set; }
    }
}