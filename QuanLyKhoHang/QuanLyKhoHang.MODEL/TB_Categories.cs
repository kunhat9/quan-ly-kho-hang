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
    
    public partial class TB_Categories
    {
        public TB_Categories()
        {
            this.TB_Products = new HashSet<TB_Products>();
        }
    
        public int CategoriesId { get; set; }
        public string CategoriesName { get; set; }
        public string CategoriesNote { get; set; }
        public Nullable<int> CategoriesStatus { get; set; }
    
        public virtual ICollection<TB_Products> TB_Products { get; set; }
    }
}