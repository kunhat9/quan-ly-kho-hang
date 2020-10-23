using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class ProductInfoOrder
    {
        public TB_Products Product { get; set; }
        public TB_Categories Categories { get; set; }
        public TB_Providers Provider { get; set; }
        public TB_Orders Order { get; set; }
        public TB_OrderDetails OrderDetails { get; set; }
    }
}