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
    public class AccountController : BaseController
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        
        
        public ActionResult MainPage()
        {
            return RedirectToAction("Login");
        }
        [Route("login")]
        public ActionResult Login()
        {
            UserInfo nd = (UserInfo)Session["NguoiDung"];
            if (nd != null)
                return RedirectToAction("Mainpage", "Dashboard", new { area = "QuanTri" });
            var config = db.AppConfigs.FirstOrDefault();
            ViewBag.Config = config;
            return View();
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(string u = "", string p = "")
        {
            p = p.Encode();
            var nd_dv = (from a in db.TB_Users
                         where (a.Username == u) && a.UserPassword == p && a.UserStatus == EnumStatus.ACTIVE && (a.UserType == EnumUserType.ADMIN || a.UserType == EnumUserType.SUB_ADMIN)
                         select new UserInfo()
                         {
                             User = a,
                         }).FirstOrDefault();
            if (nd_dv == null)
                return Json(new { kq = "err", msg = "Đăng nhập thất bại!" }, JsonRequestBehavior.AllowGet);

            Session["NguoiDung"] = nd_dv;
            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
        // GET: User
        [Route("logout")]
        public ActionResult Logout()
        {
            Session["NguoiDung"] = null;
            Session["User_Member"] = null;
            return RedirectToAction("MainPage", "Account", new { area = "" });
        }


        [Route("change-password")]
        public ActionResult Change_Password()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("change-password")]
        public ActionResult Change_Password(string OldPassword = "", string NewPassword = "")
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null)
                return RedirectToAction("MainPage", "Home", new { });

            var user = (from a in db.TB_Users.ToList()
                        where nd_dv.User.UserId == a.UserId
                        select new UserInfo()
                        {
                            User = a,
                        }).FirstOrDefault();

            OldPassword = OldPassword.Encode();

            if (user.User.UserPassword != OldPassword)
                return Json(new { kq = "err", msg = "Mật khẩu cũ không đúng!" }, JsonRequestBehavior.AllowGet);

            NewPassword = NewPassword.Encode();
            user.User.UserPassword = NewPassword;
            db.SaveChanges();
            Session["NguoiDung"] = user;
            return Json(new { kq = "ok", data = nd_dv.User.UserType, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
    }
}