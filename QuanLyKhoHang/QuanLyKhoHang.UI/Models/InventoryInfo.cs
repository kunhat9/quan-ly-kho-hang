using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class InventoryInfo
    {
        public TB_Inventory Inventory { get; set; }
        public List<TB_InventoryDetails> ListInventoryDetails { get; set; }
        public TB_Users Users { get; set; }
    }
}