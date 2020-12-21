using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class OrderInfo
    {
        public TB_Orders Orders { get; set; }
        public TB_OrderDetails OrderDetails { get; set; }
        public ProductInfo ProductInfo { get; set; }
        public TB_Users User { get; set; }
    }
}