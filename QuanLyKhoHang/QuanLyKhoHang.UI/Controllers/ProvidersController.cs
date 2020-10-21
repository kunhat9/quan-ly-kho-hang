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
    public class ProvidersController : BaseController
    {
        private QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("provider/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            return View();
        }
        [Route("provider/danh-sach")]
        public ActionResult List(string keyword = "", int status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();

            var list = (from a in db.TB_Providers.ToList()
                        where (keyword == "" || (keyword != "" && (a.ProviderName.BoDauTiengViet().ToLower().Contains(keyword) || a.ProviderAddress.BoDauTiengViet().ToLower().Contains(keyword)
                        || a.ProviderNote.BoDauTiengViet().ToLower().Contains(keyword) || a.ProviderPhone.BoDauTiengViet().ToLower().Contains(keyword))))

                        && a.ProviderStatus == status
                        select a)
                        .OrderBy(x => x.ProviderId).ToList();

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

        [Route("provider/update")]
        public ActionResult Update(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var provider = db.TB_Providers.FirstOrDefault(x => x.ProviderId == id);
            provider = provider == null ? new TB_Providers() : provider;
            return PartialView(provider);
        }
        [Route("provider/update")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(TB_Providers provider, HttpPostedFileBase _Avatar = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            if(!provider.ProviderEmail.checkMail())
                return Json(new { kq = "err", msg = "Email không đúng định dạng!" }, JsonRequestBehavior.AllowGet);
            if (!provider.ProviderPhone.checkSoDienThoai())
                return Json(new { kq = "err", msg = "Số điện thoại không đúng !" }, JsonRequestBehavior.AllowGet);
            if (provider.ProviderId == 0)
            {
                // tao moi

                if (_Avatar != null)
                {
                    string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                    string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                    provider.Logo = fileImage[1];
                }
                db.TB_Providers.Add(provider);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // up date
                var providerOld = db.TB_Providers.FirstOrDefault(x => x.ProviderId == provider.ProviderId);
                if (providerOld == null)
                    return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);
                string avatar = "";
                if (_Avatar != null)
                {
                    string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                    string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                    avatar = fileImage[1];
                }
                providerOld.ProviderName = provider.ProviderName;
                providerOld.ProviderAddress = provider.ProviderAddress;
                providerOld.ProviderPhone = provider.ProviderPhone;
                providerOld.ProviderEmail = provider.ProviderEmail;
                providerOld.ProviderNote = provider.ProviderNote;
                providerOld.ProviderStatus = provider.ProviderStatus;
                providerOld.Logo = avatar;
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Route("provider/change-status")]
        public ActionResult Change_Status(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var users = db.TB_Users.FirstOrDefault(x => x.UserId == id);
            if (users == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);

            // kiem tra xem san pham cua nha cung cap do co dang bi dung o dau khong

            //var pro = (from a in db.TB_Providers
            //           join b in db.TB_Products on a.ProviderId equals b.ProductProviderId into b1
            //           from b in b1.DefaultIfEmpty()
            //           join c in db.TB_OrderDetails on b.ProductId equals c.DetailProductId into c1
            //           from c in c1.DefaultIfEmpty()
            //           select a).FirstOrDefault();
            //if(pro != null)
            //    return Json(new { kq = "err", msg = "Nhà cung cấp vẫn đang được sử dụng ở các hóa đơn. Vui lòng kiểm tra lại!" }, JsonRequestBehavior.AllowGet);

            if (users.UserStatus == EnumStatus.ACTIVE)
                users.UserStatus = EnumStatus.INACTIVE;
            else
                users.UserStatus = EnumStatus.ACTIVE;
            db.SaveChanges();
            return Json(new { kq = "ok", data = users.UserStatus, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("provider/delete")]
        public ActionResult Delete(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var user = db.TB_Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);
            db.TB_Users.Remove(user);
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}