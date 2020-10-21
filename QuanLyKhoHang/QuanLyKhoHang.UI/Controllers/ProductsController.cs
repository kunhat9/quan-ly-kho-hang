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
    public class ProductsController : BaseController
    {
        private QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("product/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
            var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in provider)
            {
                cbxProvider += string.Format("<option value=\"{0}\">{1}</option>", item.ProviderId, item.ProviderName);
            }
            ViewBag.cbxProvider = cbxProvider;
            string cbxCategory = "<option value=\"\">Chọn danh mục...</option>";
            var categories = db.TB_Categories.Where(x => x.CategoriesStatus == EnumStatus.ACTIVE).ToList();
            foreach (var item in categories)
            {
                cbxCategory += string.Format("<option value=\"{0}\">{1}</option>", item.CategoriesId, item.CategoriesName);
            }
            ViewBag.cbxCategory = cbxCategory;


            return View();
        }
        [Route("product/danh-sach")]
        public ActionResult List(string keyword = "", int status = EnumStatus.ACTIVE, int? category = null, int? provider = null, int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();


            var list = (from a in db.TB_Products.ToList()
                        join b in db.TB_Providers on a.ProductProviderId equals b.ProviderId into b1
                        from b in b1.DefaultIfEmpty()
                        join c in db.TB_Categories on a.ProductCategoriesId equals c.CategoriesId into c1
                        from c in c1.DefaultIfEmpty()
                        where (a != null && (keyword == "" || (keyword != "" && (a.ProductCode.ToLower().Contains(keyword) || a.ProductName.BoDauTiengViet().ToLower().Contains(keyword)
                        || a.ProductNote.BoDauTiengViet().ToLower().Contains(keyword)))))
                        && a.ProductStatus == status
                        && provider == null ? true : a.ProductProviderId == provider
                        && category == null ? true : a.ProductCategoriesId == category
                        select new ProductInfo()
                        {
                            Product = a,
                            Provider = b,
                            Categories = c
                        })

                   .OrderByDescending(x => x.Product.ProductId).ToList();

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

    [Route("product/update")]
    public ActionResult Update(int? id = null)
    {
        UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
        if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
            return RedirectToAction("MainPage", "Account", new { area = "" });
        var product = db.TB_Products.FirstOrDefault(x => x.ProductId == id);
        product = product == null ? new TB_Products() : product;
        string cbxProvider = "<option value=\"\">Chọn nhà cung cấp...</option>";
        var provider = db.TB_Providers.Where(x => x.ProviderStatus == EnumStatus.ACTIVE).ToList();
        foreach (var item in provider)
        {
            cbxProvider += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.ProviderId, item.ProviderName, product.ProductProviderId == item.ProviderId ? "selected" : "");
        }
        ViewBag.cbxProvider = cbxProvider;
        string cbxCategory = "<option value=\"\">Chọn danh mục...</option>";
        var categories = db.TB_Categories.Where(x => x.CategoriesStatus == EnumStatus.ACTIVE).ToList();
        foreach (var item in categories)
        {
            cbxCategory += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.CategoriesId, item.CategoriesName, product.ProductCategoriesId == item.CategoriesId ? "selected" : "");
        }
        ViewBag.cbxCategory = cbxCategory;

        return PartialView(product);
    }
    [Route("product/update")]
    [HttpPost, ValidateInput(false)]
    public ActionResult Update(TB_Products product, HttpPostedFileBase _Avatar = null)
    {
        UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
        if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
            return RedirectToAction("MainPage", "Account", new { area = "" });


        if (product.ProductId == 0)
        {
            // tao moi

            if (_Avatar != null)
            {
                string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                product.ProductImage = fileImage[1];
            }
            product.ProductCode = Functions.CreateCode();
            db.TB_Products.Add(product);
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        else
        {
            // up date
            var productOld = db.TB_Products.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (productOld == null)
                return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);
            string avatar = "";
            if (_Avatar != null)
            {
                string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                avatar = fileImage[1];
            }
            productOld.ProductName = product.ProductName;
            productOld.ProductNote = product.ProductNote;
            productOld.ProductStatus = product.ProductStatus;
            productOld.ProductProviderId = product.ProductProviderId;
            productOld.ProductCategoriesId = product.ProductCategoriesId;
            productOld.ProductImage = avatar;
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
    }


    [Route("product/change-status")]
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

    [Route("product/delete")]
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