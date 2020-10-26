using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class CompareProduct
    {
        public TB_OrderDetails OrderDetails { get; set; }
        public TB_Orders Order { get; set; }
        public TB_Products Product { get; set; }
        public TB_Providers Provider { get; set; }

        public TB_InventoryDetails InventoryDetails { get; set; }
        public int TotalRemain { get; set; }
        public int TotalRemainNow { get; set; }
        public int Total { get; set; }
        public int TotalNow { get; set; }
        public int TotalUse { get; set; } 

    }
}