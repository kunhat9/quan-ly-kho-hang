using BASICAUTHORIZE.ATCAPITAL.HETHONGGOPCOPHAN;
using QuanLyKhoHang.MODEL;
using QuanLyKhoHang.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoHang.UI.Controllers
{
    [RouteArea("QuanTri", AreaPrefix = "admin")]
    [Route("{action}")]
    public class ReportsController : BaseController
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();

        [Route("report/product-transaction")]
        public ActionResult ProductTransaction()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;


            return View();
        }

        [Route("report/product-transaction-list")]
        public ActionResult ProductTransaction_List(string keyword = "", int? order = null, int? provider = null, int? type = null, string fromDate = "", string toDate = "", int sotrang = 1, int tongsodong = 10, int reportType = EnumReportType.GIAO_DICH_SAN_PHAM)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();
            DateTime? dateFrom = fromDate.ToDateTime();
            DateTime? dateTo = toDate.ToDateTime();
            ViewBag.ReportType = reportType;
            if (reportType == EnumReportType.GIAO_DICH_SAN_PHAM)
            {
                var list = (from c in db.TB_Products.ToList()
                            join b in db.TB_OrderDetails on c.ProductId equals b.DetailProductId into b1
                            from b in b1.DefaultIfEmpty()
                            join a in db.TB_Orders on b.DetailOrderId equals a.OrderId into a1
                            from a in a1.DefaultIfEmpty()
                            join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                            from d in d1.DefaultIfEmpty()
                            where
                            (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (c.ProductCode.ToLower().Contains(keyword) || c.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || c.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || a.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && (string.IsNullOrEmpty(fromDate) ? true : a.OrderDate >= dateFrom)
                            && (string.IsNullOrEmpty(toDate) ? true : a.OrderDate <= dateTo)
                            && (provider == null ? true : d.ProviderId == provider)
                            && (order == null ? true : b.DetailOrderId == order)
                            && (type == null ? true : a.OrderType == type)
                            select new ProductInfoOrder()
                            {
                                Order = a,
                                OrderDetails = b,
                                Product = c,
                                Provider = d

                            })
                  .OrderByDescending(x => x.Order.OrderDate).ToList();
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
            else if (reportType == EnumReportType.SAN_PHAM_TRONG_KHO)
            {
                var test = CompareProduct(null, order).Where(x=>x.TotalRemain >0).ToList();
                test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
                var list = test.Select(x => new ProductInfoOrder()
                {
                    Order = x.Order,
                    OrderDetails = x.OrderDetails,
                    Product = x.Product,
                    Provider = x.Provider
                })
                .Where(item => (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (item.Product.ProductCode.ToLower().Contains(keyword) || item.Product.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Product.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Order.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && (string.IsNullOrEmpty(fromDate) ? true : item.Order.OrderDate >= dateFrom)
                            && (string.IsNullOrEmpty(toDate) ? true : item.Order.OrderDate <= dateTo)
                            && (provider == null ? true : item.Provider.ProviderId == provider)
                            && (order == null ? true : item.OrderDetails.DetailOrderId == order)
                            && (type == null ? true : item.Order.OrderType == type))
                .ToList();
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
            else if (reportType == EnumReportType.SAN_PHAM_HET_HAN)
            {
                var test = CompareProduct(null, order).Where(x => x.TotalRemain > 0).ToList();
                test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
                var list = test.Select(x => new ProductInfoOrder()
                {
                    Order = x.Order,
                    OrderDetails = x.OrderDetails,
                    Product = x.Product,
                    Provider = x.Provider
                })
                .Where(item => (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (item.Product.ProductCode.ToLower().Contains(keyword) || item.Product.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Product.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Order.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && item.OrderDetails.DetailExpiredDate <= DateTime.Today
                            && (provider == null ? true : item.Provider.ProviderId == provider)
                            && (order == null ? true : item.OrderDetails.DetailOrderId == order)
                            && (type == null ? true : item.Order.OrderType == type))
                .ToList();
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
            else if (reportType == EnumReportType.SAN_PHAM_HET_HANG)
            {
                var test = CompareProduct(null, order).Where(x => x.TotalRemain == 0).ToList();
                test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
                if (dateTo == null) dateTo = DateTime.Today;
                if (dateFrom == null) dateFrom = DateTime.Today;
                var list = test.Select(x => new ProductInfoOrder()
                {
                    Order = x.Order,
                    OrderDetails = x.OrderDetails,
                    Product = x.Product,
                    Provider = x.Provider
                })
                .Where(item => (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (item.Product.ProductCode.ToLower().Contains(keyword) || item.Product.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Product.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Order.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && item.OrderDetails.DetailExpiredDate >= dateTo
                            && (provider == null ? true : item.Provider.ProviderId == provider)
                            && (order == null ? true : item.OrderDetails.DetailOrderId == order)
                            && (type == null ? true : item.Order.OrderType == type))
                .ToList();
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
            else if (reportType == EnumReportType.SAN_PHAM_SAP_HET_HAN)
            {
                var test = CompareProduct(null, order).Where(x => x.TotalRemain > 0).ToList();
                test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
                if (dateTo == null) dateTo = DateTime.Today;
                if (dateFrom == null) dateFrom = DateTime.Today;
                var list = test.Select(x => new ProductInfoOrder()
                {
                    Order = x.Order,
                    OrderDetails = x.OrderDetails,
                    Product = x.Product,
                    Provider = x.Provider
                })
                .Where(item => (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (item.Product.ProductCode.ToLower().Contains(keyword) || item.Product.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Product.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Order.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && item.OrderDetails.DetailExpiredDate >= dateTo
                            && CompareDateTime(item.OrderDetails.DetailExpiredDate.Value,dateTo.Value) >=7
                            && (provider == null ? true : item.Provider.ProviderId == provider)
                            && (order == null ? true : item.OrderDetails.DetailOrderId == order)
                            && (type == null ? true : item.Order.OrderType == type))
                .ToList();
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
            else if (reportType == EnumReportType.SAN_PHAM_SAP_HET_HANG)
            {
                var test = CompareProduct(null, order).Where(x => x.TotalRemain > 0 && x.TotalRemain < 20).ToList();
                test.ForEach(x => x.OrderDetails.DetailNumber = x.TotalRemain);
                if (dateTo == null) dateTo = DateTime.Today;
                var list = test.Select(x => new ProductInfoOrder()
                {
                    Order = x.Order,
                    OrderDetails = x.OrderDetails,
                    Product = x.Product,
                    Provider = x.Provider
                })
                .Where(item => (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (item.Product.ProductCode.ToLower().Contains(keyword) || item.Product.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Product.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || item.Order.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && item.OrderDetails.DetailExpiredDate >= dateTo
                            && (provider == null ? true : item.Provider.ProviderId == provider)
                            && (order == null ? true : item.OrderDetails.DetailOrderId == order)
                            && (type == null ? true : item.Order.OrderType == type))
                .ToList();
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
            else
            {
                var list = (from c in db.TB_Products.ToList()
                            join b in db.TB_OrderDetails on c.ProductId equals b.DetailProductId into b1
                            from b in b1.DefaultIfEmpty()
                            join a in db.TB_Orders on b.DetailOrderId equals a.OrderId into a1
                            from a in a1.DefaultIfEmpty()
                            join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                            from d in d1.DefaultIfEmpty()
                            where
                            (string.IsNullOrEmpty(keyword) ? true : (keyword == "" || (keyword != "" && (c.ProductCode.ToLower().Contains(keyword) || c.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                            || c.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)
                            || a.OrderCode.BoDauTiengViet().ToLower().Contains(keyword)))))
                            && (string.IsNullOrEmpty(fromDate) ? true : a.OrderDate >= dateFrom)
                            && (string.IsNullOrEmpty(toDate) ? true : a.OrderDate <= dateTo)
                            && (provider == null ? true : d.ProviderId == provider)
                            && (order == null ? true : b.DetailOrderId == order)
                            && (type == null ? true : a.OrderType == type)
                            select new ProductInfoOrder()
                            {
                                Order = a,
                                OrderDetails = b,
                                Product = c,
                                Provider = d

                            })
                  .OrderByDescending(x => x.Order.OrderDate).ToList();
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



        [Route("report/product-remain")]
        public ActionResult ProductRemain()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            return View();
        }


        [Route("report/product-expired")]
        public ActionResult ProductExpired() // het han
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            return View();
        }

        [Route("report/product-about-expired")]
        public ActionResult ProductAboutExpired() // sap het han
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            return View();
        }
        [Route("report/product-not-have")]
        public ActionResult ProductNotHave() // het hang
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            return View();
        }

        [Route("report/product-about-not-have")]
        public ActionResult ProductAboutNotHave() // sap het hang
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            string cbxOrder = "<option value=''>Chọn hóa đơn ..........</option>";
            var order = db.TB_Orders.ToList();
            foreach (var item in order)
            {
                cbxOrder += string.Format("<option value=\"{0}\">{1}</option>", item.OrderId, item.OrderCode);
            }
            ViewBag.cbxOrder = cbxOrder;

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            return View();
        }



    }
}