using QuanLyKhoHang.MODEL;
using QuanLyKhoHang.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoHang.UI.Controllers
{
    public class BaseController : Controller
    {
        protected List<CompareProduct> CompareProduct(int? product = null, int? order = null)
        {
            QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
            List<CompareProduct> list = new List<CompareProduct>();
            // hoa don nhap cua san pham do
            var orderNhap = (from a in db.TB_OrderDetails
                             join b in db.TB_Orders on a.DetailOrderId equals b.OrderId into b1
                             from b in b1.DefaultIfEmpty()
                             join c in db.TB_Products on a.DetailProductId equals c.ProductId into c1
                             from c in c1.DefaultIfEmpty()
                             join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                             from d in d1.DefaultIfEmpty()
                             where product == null ? true : a.DetailProductId == product
                             && order == null ? true : b.OrderId == order
                             select new
                             {
                                 Order = b,
                                 OrderDetails = a,
                                 Product = c,
                                 Provider = d
                             }
                           ).Where(a => (product == null ? true : a.OrderDetails.DetailProductId == product) && (order == null ? true : a.Order.OrderId == order))
                           .Where(x => x.Order.OrderStatus == EnumOrderStatus.DANG_SU_DUNG && x.Order.OrderType == EnumOrderType.NHAP).ToList();
            // danh sach hoa don xuat dung san pham do
            var orderXuat = (from a in db.TB_OrderDetails
                             join b in db.TB_Orders on a.DetailOrderId equals b.OrderId into b1
                             from b in b1.DefaultIfEmpty()
                             where product == null ? true : a.DetailProductId == product
                             select new
                             {
                                 Order = b,
                                 OrderDetails = a
                             }
                           )
                           .Where(x => x.Order.OrderStatus == EnumOrderStatus.DANG_SU_DUNG && x.Order.OrderType == EnumOrderType.XUAT).ToList();
            foreach (var item in orderNhap)
            {
                CompareProduct p = new Models.CompareProduct();
                p.Order = item.Order;
                p.OrderDetails = item.OrderDetails;
                p.Product = item.Product;
                p.Provider = item.Provider;
                var totalRemain = orderXuat.Where(x => x.OrderDetails.DetailsOrderProductId == item.Order.OrderId && item.Product.ProductId == x.OrderDetails.DetailProductId).Sum(x => x.OrderDetails.DetailNumber);
                p.TotalRemain = totalRemain == null ? item.OrderDetails.DetailNumber.Value : item.OrderDetails.DetailNumber.Value - totalRemain.Value;


                list.Add(p);
            }
            return list;
        }

        protected int CompareDateTime (DateTime dateStart, DateTime dateEnd)
        {
            TimeSpan Time = dateStart - dateEnd;
            int TongSoNgay = Time.Days;
            return TongSoNgay;
        }
    }
}