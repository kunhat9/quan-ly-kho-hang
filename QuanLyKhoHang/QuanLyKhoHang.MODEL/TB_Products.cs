//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLyKhoHang.MODEL
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_Products
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductNote { get; set; }
        public Nullable<int> ProductStatus { get; set; }
        public Nullable<int> ProductProviderId { get; set; }
        public Nullable<int> ProductCategoriesId { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
    }
}
