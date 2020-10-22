using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class OrderDetailsInfo
    {
        public TB_OrderDetails OrderDetails { get; set; }
        public ProductInfo ProductInfo { get; set; }
    }
}