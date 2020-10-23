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
    public class InventorysController : BaseController
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("inventory/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            string cbxProduct = "<option value=\"\">Chọn sản phẩm...</option>";
            var product = db.TB_Products.Where(x => x.ProductStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in product)
            {
                cbxProduct += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.ProductName);
            }
            ViewBag.cbxProduct = cbxProduct;
            return View();
        }
        [Route("inventory/danh-sach")]
        public ActionResult List(int status = EnumInventoryStatus.DAY_DU, int? product = null, string startDate = "", string endDate = "", int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            DateTime? dateFrom, dateTo;
            dateFrom = startDate.ToDateTime();
            dateTo = endDate.ToDateTime();
            var test = (from a in db.TB_Inventory.ToList()
                        join b in db.TB_InventoryDetails on a.Id equals b.InventoryId into b1
                        from b in b1.DefaultIfEmpty()
                        join c in db.TB_Products on b.ProductId equals c.ProductId into c1
                        from c in c1.DefaultIfEmpty()
                        join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                        from d in d1.DefaultIfEmpty()
                        join e in db.TB_Users on a.UserId equals e.UserId into e1
                        from e in e1.DefaultIfEmpty()
                        where a.StatusID == status
                        && product == null ? true : b.ProductId == product
                        select new
                        {
                            Inventory = a,
                            InventoryDetails = b,
                            Product = c,
                            Provider = d,
                            Users = e
                        }).Where(x => (string.IsNullOrEmpty(startDate) ? true : x.Inventory.CreatedDate >= dateFrom) && (string.IsNullOrEmpty(endDate) ? true : x.Inventory.CreatedDate <= dateTo))
                        .ToList();
            var list = test.GroupBy(x => x.Inventory.Id).Select(t => new InventoryInfo
            {

                Inventory = t.FirstOrDefault(y => y.Inventory.Id == t.Key).Inventory,
                ListInventoryDetails = t.Select(y => y.InventoryDetails).Where(s => s.InventoryId == t.Key).ToList(),
                Users = t.FirstOrDefault(y => y.Inventory.Id == t.Key).Users
            })
            .OrderByDescending(x => x.Inventory.CreatedDate)
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

        [Route("inventory/update")]
        public ActionResult Update(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            var inventory = (from a in db.TB_Inventory.ToList()
                        join b in db.TB_InventoryDetails on a.Id equals b.InventoryId into b1
                        from b in b1.DefaultIfEmpty()
                        join c in db.TB_Products on b.ProductId equals c.ProductId into c1
                        from c in c1.DefaultIfEmpty()
                        join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                        from d in d1.DefaultIfEmpty()
                        join e in db.TB_Users on a.UserId equals e.UserId into e1
                        from e in e1.DefaultIfEmpty()
                        join f in db.TB_Categories on c.ProductCategoriesId equals f.CategoriesId into f1
                        from f in f1.DefaultIfEmpty()
                        where a.Id == id
                        select new InventoryInfoDetails
                        {
                            Inventory = a,
                            InventoryDetails = b,
                            Product = c,
                            Provider = d,
                            Category = f,
                            Users = e
                        }).FirstOrDefault();
            inventory = inventory == null ? new InventoryInfoDetails() : inventory;
            string cbxProduct = "<option value=\"\">Tất cả sản phẩm</option>";
            var product = db.TB_Products.Where(x => x.ProductStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in product)
            {
                cbxProduct += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.ProductName);
            }
            ViewBag.cbxProduct = cbxProduct;


            string cbxUser = "<option value=\"\">Chọn người lập biên bản...</option>";
            var users = db.TB_Users.Where(x => x.UserType == EnumUserType.INVENTORY).ToList();
            foreach (var item in users)
            {
                cbxUser += string.Format("<option value=\"{0}\">{1}</option>", item.UserId, item.UserFullName);
            }
            ViewBag.cbxUser = cbxUser;

            return View(inventory);
        }
        [Route("inventory/update")]
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



        [Route("inventory/danh-sach-san-pham-kiem-ke")]
        public ActionResult ListProduct(List<int> listProduct)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            List<CompareProduct> listCompare = new List<Models.CompareProduct>();
            if(listProduct.Count == 0)
            {
                var product = db.TB_Products.Where(x => x.ProductStatus == EnumStatus.ACTIVE).ToList();
                foreach (var item in product)
                {
                    List<CompareProduct> temp = CompareProduct(item.ProductId);
                    if (temp.Count > 0)
                    {
                        temp.ForEach(x => listCompare.Add(x));
                    }
                }
            }
            else
            {
                foreach (var item in listProduct)
                {
                    List<CompareProduct> temp = CompareProduct(item);
                    if (temp.Count > 0)
                    {
                        temp.ForEach(x => listCompare.Add(x));
                    }
                }
            }
            
            
            return PartialView(listCompare);
        }

        [Route("inventory/change-status")]
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


        [Route("inventory/change-status-inventory")]
        public ActionResult ChangeStatus()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            return PartialView();
        }

        [HttpPost]
        [Route("inventory/change-status-inventory")]
        public ActionResult ChangeStatus(int? status = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            
            return Json(new { kq = "ok",data = status, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("inventory/change-note-inventory")]
        public ActionResult ChangeNote()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            return PartialView();
        }

        [HttpPost]
        [Route("inventory/change-note-inventory")]
        public ActionResult ChangeNote(string note="")
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            return Json(new { kq = "ok", data = note, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }


        [Route("inventory/delete")]
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

    }
}