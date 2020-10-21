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

            return View();
        }
    }
}