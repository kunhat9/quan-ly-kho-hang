using QuanLyKhoHang.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public class ProductInfo
    {
        public TB_Products Product { get; set; }
        public TB_Categories Categories { get; set; }
        public TB_Providers Provider { get; set; }
         
    }
}