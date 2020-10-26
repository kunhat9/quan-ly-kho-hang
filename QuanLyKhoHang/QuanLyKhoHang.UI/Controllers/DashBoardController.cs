using BASICAUTHORIZE.ATCAPITAL.HETHONGGOPCOPHAN;
using QuanLyKhoHang.MODEL;
using QuanLyKhoHang.UI.Controllers;
using QuanLyKhoHang.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoHang.UI.Areas.QuanTri.Controller
{
    [RouteArea("QuanTri", AreaPrefix = "admin")]
    [Route("{action}")]
    public class DashBoardController : BaseController
    {
        private QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("dashboard")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

          
            var checkImport = (from a in db.TB_Orders
                        where a.OrderType == EnumOrderType.NHAP
                        select a)                       
                        .ToList();
            var checkExport = (from a in db.TB_Orders
                               where a.OrderType == EnumOrderType.XUAT
                               select a)
                       .ToList();
            var totalSystemImport = checkImport.Count();
            var totalSystemExport = checkExport.Count();
            ViewBag.SystemImport = totalSystemImport;
            ViewBag.SystemExport = totalSystemImport;
            var now = DateTime.Now.ToString("dd/MM/yyyy");
            var totalTranImport = checkImport.Where(x => x.OrderDate.Value.ToString("dd/MM/yyyy").Equals(now)).ToList().Count;
            var totalTranExport = checkExport.Where(x => x.OrderDate.Value.ToString("dd/MM/yyyy").Equals(now)).ToList().Count;
            var test = CompareProduct();
            test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
            var totalRemain = test.Sum(x => x.TotalRemain); // con trong kho
            var totalExpire = test.Where(x => x.OrderDetails.DetailExpiredDate <= DateTime.Today).ToList().Count; // het han
            var totalNotHave = test.Where(x => x.TotalRemain == 0).ToList().Count;  // het hang
            var totalExpireNow = test.Where(x => x.OrderDetails.DetailExpiredDate <= DateTime.Today)
                .Where(item => item.OrderDetails.DetailExpiredDate >= DateTime.Today
                            && CompareDateTime(item.OrderDetails.DetailExpiredDate.Value, DateTime.Today) >= 7)
                .ToList().Count; // sap het han
            var totalNotHaveNow = test.Where(x => x.TotalRemain > 0 && x.TotalRemain < 20)
                .Where(item => item.OrderDetails.DetailExpiredDate >= DateTime.Today)
                .ToList().Count;


            ViewBag.TotalTranImport = totalTranImport;
            ViewBag.TotalTranExport = totalTranExport;
            ViewBag.TotalRemain = totalRemain;
            ViewBag.TotalExpire = totalExpire;
            ViewBag.TotalNotHave = totalNotHave;
            ViewBag.TotalExpireNow = totalExpireNow;
            ViewBag.TotalNotHaveNow = totalNotHaveNow;
            return View();
        }



        [Route("dashboard/danh-sach")]
        public ActionResult List(string keyword = "", int status = EnumStatus.ACTIVE, int? product = null, int? type = null, int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();


            var test = (from a in db.TB_Orders.ToList()
                        join b in db.TB_OrderDetails on a.OrderId equals b.DetailOrderId into b1
                        from b in b1.DefaultIfEmpty()
                        join c in db.TB_Products on b.DetailProductId equals c.ProductId into c1
                        from c in c1.DefaultIfEmpty()
                        join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                        from d in d1.DefaultIfEmpty()
                        join e in db.TB_Categories on c.ProductCategoriesId equals e.CategoriesId into e1
                        from e in e1.DefaultIfEmpty()
                        where (keyword == "" || (keyword != "" && (c.ProductCode.ToLower().Contains(keyword) || c.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                        || c.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)))
                        || d.ProviderName.BoDauTiengViet().ToLower().Contains(keyword)
                        || e.CategoriesName.BoDauTiengViet().ToLower().Contains(keyword)
                        || d.ProviderName.BoDauTiengViet().ToLower().Contains(keyword)
                        || d.ProviderNote.BoDauTiengViet().ToLower().Contains(keyword)
                        )
                        && a.OrderStatus == status
                        && product == null ? true : b.DetailProductId == product
                        && type == null ? true : a.OrderType == type
                        select new OrderInfo()
                        {
                            Orders = a,
                            OrderDetails = b,
                            ProductInfo = new ProductInfo()
                            {
                                Product = c,
                                Provider = d,
                                Categories = e
                            }
                        })
                   .OrderByDescending(x => x.Orders.OrderDate).ToList();
            var list = test.GroupBy(x => x.Orders.OrderId).Select(t => new OrderInfoView
            {
                Orders = t.FirstOrDefault(y => y.Orders.OrderId == t.Key).Orders,
                OrderDetails = t.Where(y => y.Orders.OrderId == t.Key).Select(k => new OrderDetailsInfo
                {
                    OrderDetails = k.OrderDetails,
                    ProductInfo = k.ProductInfo
                }).ToList(),
            }).ToList();
            int tongso = list.Count();
            sotrang = sotrang <= 0 ? 1 : sotrang;
            tongsodong = tongsodong <= 0 ? 10 : tongsodong;
            int tongsotrang = tongso % tongsodong > 0 ? tongso / tongsodong + 1 : tongso / tongsodong;
            tongsotrang = tongsotrang <= 0 ? 1 : tongsotrang;
            sotrang = sotrang > tongsotrang ? tongsotrang - 1 : sotrang - 1;
            ViewBag.sotrang = sotrang + 1;
            ViewBag.tongsotrang = tongsotrang;
            ViewBag.tongso = tongso;
            return PartialView(list == null ? list : list.Skip(sotrang * tongsodong).Take(tongsodong));
        }
    }
}