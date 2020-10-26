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
    public class UsersController : BaseController
    {
        protected QuanLyKhoHangEntities db = new QuanLyKhoHangEntities();
        [Route("user/trang-chu")]
        public ActionResult MainPage()
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            return View();
        }
        [Route("user/danh-sach")]
        public ActionResult List(string keyword = "", int status = EnumStatus.ACTIVE,int ? type= EnumUserType.MEMBER, int sotrang = 1, int tongsodong = 10)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.BoDauTiengViet().ToLower();

            var list = (from a in db.TB_Users.ToList()
                        where (keyword == "" || (keyword != "" && (a.UserFullName.BoDauTiengViet().ToLower().Contains(keyword) || a.UserAddress.BoDauTiengViet().ToLower().Contains(keyword)
                        || a.UserPhone.BoDauTiengViet().ToLower().Contains(keyword) || a.UserNote.BoDauTiengViet().ToLower().Contains(keyword))))

                        && a.UserStatus == status
                        && a.UserType == type
                        select a)
                        .Where(x=>x.UserType == type && x.UserStatus == status)
                        .OrderBy(x => x.UserDateCreated);

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

        [Route("user/update")]
        public ActionResult Update(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var user = db.TB_Users.FirstOrDefault(x => x.UserId == id);
            user = user == null ? new TB_Users() : user;
            return PartialView(user);
        }
        [Route("user/update")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(TB_Users users, HttpPostedFileBase _Avatar = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            if (!users.UserPhone.checkSoDienThoai())
                return Json(new { kq = "err", msg = "Số điện thoại không đúng !" }, JsonRequestBehavior.AllowGet);

            if (users.UserId == 0)
            {
                // tao moi
                // check user name 
                var checkExist = db.TB_Users.FirstOrDefault(x => x.Username.Equals(users.Username));
                if (checkExist != null)
                    return Json(new { kq = "err", msg = "Tên đăng nhập đã được sử dụng!" }, JsonRequestBehavior.AllowGet);
                if (_Avatar != null)
                {
                    string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                    string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                    users.Avatar = fileImage[1];
                }
                users.UserDateCreated = DateTime.Now;
                users.UserPassword = "123456".Encode();
                db.TB_Users.Add(users);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // up date
                var userOld = db.TB_Users.FirstOrDefault(x => x.UserId == users.UserId);
                if (userOld == null)
                    return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);
                string avatar = "";
                if (_Avatar != null)
                {
                    string rootPathImage = string.Format("~/Files/image/provider/{0}", DateTime.Now.ToString("ddMMyyyy"));
                    string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                    avatar = fileImage[1];
                }
                userOld.UserFullName = users.UserFullName;
                userOld.UserPhone = users.UserPhone;
                userOld.UserStatus = users.UserStatus;
                userOld.UserNote = users.UserNote;
                userOld.UserType = users.UserType;
                userOld.Avatar = avatar;
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Route("user/change-status")]
        public ActionResult Change_Status(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var users = db.TB_Users.FirstOrDefault(x => x.UserId == id);
            if (users == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);

            // kiem tra xem san pham cua nha cung cap do co dang bi dung o dau khong

            if (users.UserStatus == EnumStatus.ACTIVE)
                users.UserStatus = EnumStatus.INACTIVE;
            else
                users.UserStatus = EnumStatus.ACTIVE;
            db.SaveChanges();
            return Json(new { kq = "ok", data = users.UserStatus, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("user/delete")]
        public ActionResult Delete(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var user = db.TB_Providers.FirstOrDefault(x => x.ProviderId == id);
            if (user == null)
                return Json(new { kq = "err", msg = "Không xác định!" }, JsonRequestBehavior.AllowGet);
            db.TB_Providers.Remove(user);
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
        }
        [Route("user/reset-password")]
        public ActionResult ResetPassword(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var user = db.TB_Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
                return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);

            var newPassword = Functions.CreateCode6Char();
            user.UserPassword = newPassword.Encode();
            db.SaveChanges();
            return Json(new { kq = "ok", data = newPassword, msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("user/info")]
        public ActionResult Info(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

            var user = (from a in db.TB_Users.ToList()
                        where id == a.UserId
                        select new UserInfo()
                        {
                            User = a,
                            
                        }).FirstOrDefault();

            if (user == null)
                user = new UserInfo();

            return View(user);
        }

        [Route("user/info")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Info(TB_Users user, HttpPostedFileBase _Avatar = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });

           
            var appConfigs = db.AppConfigs.FirstOrDefault();

            var userOld = db.TB_Users.FirstOrDefault(x => x.UserId == user.UserId);
            if (userOld == null)
                return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);

            //var checkExist = db.TB_Users.Count(x => x.UserId != user.UserId);
            //if (checkExist > 0)
            //    return Json(new { kq = "err", msg = "Thông tin người dùng đã tồn tại rồi!" }, JsonRequestBehavior.AllowGet);

            string avatar = "";
            if (_Avatar != null)
            {
                string rootPathImage = string.Format("~/Files/user/avatar/{0}/{1}", user.UserId, DateTime.Now.ToString("ddMMyyyy"));
                string filePathImage = System.IO.Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _Avatar.uploadFile(rootPathImage, filePathImage);
                avatar = fileImage[1];
            }

            userOld.UserFullName = user.UserFullName;
            userOld.UserPhone = user.UserPhone;
            userOld.UserNote = user.UserNote;
            userOld.UserAddress = user.UserAddress;
            if (_Avatar != null)
                userOld.Avatar = avatar;
            db.SaveChanges();
            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

    }
}