using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class OrderInfoView
    {
        public TB_Orders Orders { get; set; }
        public List<OrderDetailsInfo> OrderDetails { get; set; }
    }
}