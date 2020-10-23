using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class InventoryInfoDetails
    {
        public TB_Inventory Inventory { get; set; }
        public TB_InventoryDetails InventoryDetails { get; set; }
        public TB_Products Product { get; set; }
        public TB_Providers Provider { get; set; }
        public TB_Categories Category { get; set; }
        public TB_Users Users { get; set; }

    }
}