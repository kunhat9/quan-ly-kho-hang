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
    public class HomeController : Controller
    {
        public QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("config")]
        public ActionResult Config()
        {
            return PartialView();
        }

        [HttpPost]
        [Route("config")]
        public ActionResult Config(AppConfig config, HttpPostedFileBase _imageLogin = null, HttpPostedFileBase _imagePanelLogin = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null)
                return RedirectToAction("MainPage", "Home", new { });

            var appConfig = db.AppConfigs.FirstOrDefault();
            if (_imageLogin != null)
            {
                string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _imageLogin.uploadFile(rootPathImage, filePathImage);
                appConfig.ImageLogin = fileImage[1];
            }
            if (_imagePanelLogin != null)
            {
                string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _imagePanelLogin.uploadFile(rootPathImage, filePathImage);
                appConfig.ImagePanelLogin = fileImage[1];
            }
            db.SaveChanges();
            return Json(new { kq = "ok", data = nd_dv.User.UserType, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }
    }
}