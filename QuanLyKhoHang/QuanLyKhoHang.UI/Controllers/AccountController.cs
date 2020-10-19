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
    public class AccountController : Controller
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("login")]
        public ActionResult Login()
        {
            UserInfo nd = (UserInfo)Session["NguoiDung"];
            if (nd != null)
                return RedirectToAction("Mainpage", "Dashboard", new { area = "QuanTri" });
            var logoLogin = db.AppConfigs.FirstOrDefault().ImageLogin;
            ViewBag.Logo = string.IsNullOrEmpty("") ? "" : logoLogin;
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
            return RedirectToAction("MainPage", "Home", new { area = "" });
        }
    }
}