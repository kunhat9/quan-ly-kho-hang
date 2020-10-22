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
        public int TotalRemain { get; set; }
    }
}