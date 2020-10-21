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
    public class CategoriesController : BaseController
    {
        protected QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("categories/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            return View();
        }
        [Route("categories/danh-sach")]
        public ActionResult List(string keyword = "", int status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();

            var list = (from a in db.TB_Categories.ToList()
                        where (keyword == "" || (keyword != "" && (a.CategoriesName.BoDauTiengViet().ToLower().Contains(keyword) || a.CategoriesNote.BoDauTiengViet().ToLower().Contains(keyword)
                       )))

                        && a.CategoriesStatus == status
                        select a)
                        .OrderBy(x => x.CategoriesId).ToList();

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

        [Route("categories/update")]
        public ActionResult Update(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var categories = db.TB_Categories.FirstOrDefault(x => x.CategoriesId == id);
            categories = categories == null ? new TB_Categories() : categories;
            return PartialView(categories);
        }
        [Route("categories/update")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(TB_Categories categories, HttpPostedFileBase _Avatar = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

           
            if (categories.CategoriesId == 0)
            {
                db.TB_Categories.Add(categories);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // up date
                var categoriesOld = db.TB_Categories.FirstOrDefault(x => x.CategoriesId == categories.CategoriesId);
                if (categoriesOld == null)
                    return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);

                categoriesOld.CategoriesName = categories.CategoriesName;
                categoriesOld.CategoriesNote = categories.CategoriesNote;
                categoriesOld.CategoriesStatus = categories.CategoriesStatus;
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("categories/change-status")]
        public ActionResult Change_Status(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var categories = db.TB_Categories.FirstOrDefault(x => x.CategoriesId == id);
            if (categories == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);

            // kiem tra xem san pham cua nha cung cap do co dang bi dung o dau khong
            if (categories.CategoriesStatus == EnumStatus.ACTIVE)
                categories.CategoriesStatus = EnumStatus.INACTIVE;
            else
                categories.CategoriesStatus = EnumStatus.ACTIVE;
            db.SaveChanges();
            return Json(new { kq = "ok", data = categories.CategoriesStatus, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("categories/delete")]
        public ActionResult Delete(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var categories = db.TB_Categories.FirstOrDefault(x => x.CategoriesId == id);
            if (categories == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);
            db.TB_Categories.Remove(categories);
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}