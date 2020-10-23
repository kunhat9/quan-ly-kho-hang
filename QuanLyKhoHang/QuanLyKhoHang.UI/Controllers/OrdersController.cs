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
    public class OrdersController : BaseController
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("order/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            string cbxProduct = "<option value=\"\">Chọn nhà sản phẩm...</option>";
            var product = db.TB_Products.Where(x => x.ProductStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in product)
            {
                cbxProduct += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.ProductName);
            }
            ViewBag.cbxProduct = cbxProduct;
            return View();
        }
        [Route("order/danh-sach")]
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

        [Route("order/update")]
        public ActionResult Update(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            var test = (from a in db.TB_Orders.ToList()
                        join b in db.TB_OrderDetails on a.OrderId equals b.DetailOrderId into b1
                        from b in b1.DefaultIfEmpty()
                        join c in db.TB_Products on b.DetailProductId equals c.ProductId into c1
                        from c in c1.DefaultIfEmpty()
                        join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                        from d in d1.DefaultIfEmpty()
                        join e in db.TB_Categories on c.ProductCategoriesId equals e.CategoriesId into e1
                        from e in e1.DefaultIfEmpty()
                        where a.OrderId == id
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
            var order = test.GroupBy(x => x.Orders.OrderId).Select(t => new OrderInfoView
            {
                Orders = t.FirstOrDefault(y => y.Orders.OrderId == t.Key).Orders,
                OrderDetails = t.Where(y => y.Orders.OrderId == t.Key).Select(k => new OrderDetailsInfo
                {
                    OrderDetails = k.OrderDetails,
                    ProductInfo = k.ProductInfo
                }).ToList(),
            }).FirstOrDefault();
            order = order == null ? new OrderInfoView() : order;
            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.ProviderId, item.ProviderName, id != null ? order.Orders.OrderProviderId == item.ProviderId ? "selected" : "" : "");
            }
            ViewBag.cbxProvider = cbxProvider;
            return View(order);
        }
        [Route("order/update")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(TB_Orders order, List<TB_OrderDetails> list)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            if (order.OrderType == EnumOrderType.XUAT)
            {
                foreach (var item in list)
                {
                    List<CompareProduct> listCheck = CompareProduct(item.DetailProductId, item.DetailsOrderProductId);
                    foreach (var check in listCheck)
                    {
                        if (item.DetailNumber > check.TotalRemain)
                        {
                            return Json(new { kq = "err", msg = "Hóa đơn " + check.Order.OrderCode + " đang còn " + check.TotalRemain + " sản phẩm" }, JsonRequestBehavior.AllowGet);
                        }
                    }


                }
            }

            if (order.OrderId == 0)
            {
                // tao moi
                order.OrderCode = Functions.CreateCode();
                order.OrderDate = DateTime.Now;
                order.OrderStatus = EnumOrderStatus.DANG_SU_DUNG;
                order.OrderPrice = list.Sum(x => x.DetailPrice * x.DetailNumber);
                int orderId = 0;
                using (var context = new QuanLyKhoHangEntities())
                {
                    context.TB_Orders.Add(order);
                    context.SaveChanges();

                    orderId = order.OrderId; // Yes it's here
                }
                list.ForEach(x => x.DetailOrderId = orderId);
                db.TB_OrderDetails.AddRange(list);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                // up date
                var orderOld = db.TB_Orders.FirstOrDefault(x => x.OrderId == order.OrderId);
                if (orderOld == null)
                    return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);
                // tim thang order details cua thang kia roi remove di
                var orderDetailsOld = db.TB_OrderDetails.Where(x => x.DetailOrderId == orderOld.OrderId).ToList();
                db.TB_OrderDetails.RemoveRange(orderDetailsOld);
                // check so luong san pham con lai trong kho xem co du khong





                list.ForEach(x => x.DetailOrderId = orderOld.OrderId);
                db.TB_OrderDetails.AddRange(list);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Route("order/change-status")]
        public ActionResult Change_Status(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var product = db.TB_Products.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);

            // kiem tra xem san pham con dang su dung hay khong
            var checkProduct = db.TB_OrderDetails.Where(x => x.DetailProductId == id).ToList();
            if (checkProduct.Count > 0)
                return Json(new { kq = "err", msg = "Sản phẩm đang được lưu trong kho. Không thể thay sản phẩm!" }, JsonRequestBehavior.AllowGet);

            if (product.ProductStatus == EnumStatus.ACTIVE)
                product.ProductStatus = EnumStatus.INACTIVE;
            else
                product.ProductStatus = EnumStatus.ACTIVE;
            db.SaveChanges();
            return Json(new { kq = "ok", data = product.ProductStatus, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("order/delete")]
        public ActionResult Delete(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            var checkProduct = db.TB_OrderDetails.Where(x => x.DetailProductId == id).ToList();
            if (checkProduct.Count > 0)
                return Json(new { kq = "err", msg = "Sản phẩm đang được sử dụng! Vui long cập nhật sản phẩm rồi xóa!" }, JsonRequestBehavior.AllowGet);
            var product = db.TB_Products.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);
            db.TB_Products.Remove(product);
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }


        [Route("order/get-product-provider")]
        public ActionResult GetProductByProvider(int? id)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            string cbxProduct = "";
            var product = db.TB_Products.Where(x => x.ProductProviderId == id).ToList();
            foreach (var item in product)
            {
                cbxProduct += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.ProductName);
            }
            return Json(new { kq = "ok", data = cbxProduct, msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("order/get-order-by-product")]
        public ActionResult GetOrderXuatByProduct(int? id)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            string cbxProvider = "";
            var orderNhap = (from a in db.TB_OrderDetails
                             join b in db.TB_Orders on a.DetailOrderId equals b.OrderId into b1
                             from b in b1.DefaultIfEmpty()
                             where a.DetailProductId == id
                             select new
                             {
                                 Order = b,
                                 OrderDetails = a
                             }
                           ).Where(x => x.Order.OrderStatus == EnumOrderStatus.DANG_SU_DUNG && x.Order.OrderType == EnumOrderType.NHAP).ToList();

            var orderXuat = (from a in db.TB_OrderDetails
                             join b in db.TB_Orders on a.DetailOrderId equals b.OrderId into b1
                             from b in b1.DefaultIfEmpty()
                             where a.DetailProductId == id
                             select new
                             {
                                 Order = b,
                                 OrderDetails = a
                             }
                           ).Where(x => x.Order.OrderStatus == EnumOrderStatus.DANG_SU_DUNG && x.Order.OrderType == EnumOrderType.XUAT).ToList();
            List<CompareProduct> list = CompareProduct(id);
            list = list.Where(x => x.TotalRemain > 0).ToList();
            foreach (var item in list)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.Order.OrderId, item.Order.OrderCode + " ( " + item.TotalRemain + " sản phẩm )");
            }
            var obj = new
            {
                data = cbxProvider,
                list = list
            };
            return Json(new { kq = "ok", data = obj, msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }

       
        [Route("order/check-product-order")]
        public ActionResult CheckProductInOrder(int? product = null, int? order = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if (product == null)
                return Json(new { kq = "err", msg = "Sản phẩm không tìm thấy" }, JsonRequestBehavior.AllowGet);
            if (order == null)
                return Json(new { kq = "err", msg = "Không tìm thấy hóa đơn nhập sản phẩm" }, JsonRequestBehavior.AllowGet);

            var total = (from a in db.TB_OrderDetails
                         join b in db.TB_Orders on a.DetailOrderId equals b.OrderId into b1
                         from b in b1.DefaultIfEmpty()
                         where a.DetailProductId == product
                         && a.DetailOrderId == order
                         select b).Count(x => x.OrderStatus == EnumOrderStatus.DANG_SU_DUNG && x.OrderType == EnumOrderType.NHAP);
            return Json(new { kq = "ok", data = total, msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}