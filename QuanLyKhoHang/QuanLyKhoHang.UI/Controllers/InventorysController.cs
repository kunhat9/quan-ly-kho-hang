using BASICAUTHORIZE.ATCAPITAL.HETHONGGOPCOPHAN;
using Microsoft.Office.Interop.Excel;
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
        public ActionResult List(int ?status = EnumInventoryStatus.DAY_DU, int? product = null, string startDate = "", string endDate = "", int sotrang = 1, int tongsodong = 10)
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
                cbxUser += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.UserId, item.UserFullName, inventory.Inventory != null ? inventory.Inventory.UserId == item.UserId?"selected":"":"");
            }
            ViewBag.cbxUser = cbxUser;

            return View(inventory);
        }
        [Route("inventory/update")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Update(TB_Inventory inventory, List<TB_InventoryDetails> list)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            if (inventory.Id == 0)
            {
                // tao moi
                inventory.CreatedDate = DateTime.Now;
                inventory.Code = Functions.CreateCode();
                inventory.StatusID = EnumInventoryStatus.DAY_DU;
                int inventoryId = 0;
                using (var context = new QuanLyKhoHangEntities())
                {
                    context.TB_Inventory.Add(inventory);
                    context.SaveChanges();

                    inventoryId = inventory.Id; // Yes it's here
                }
                list.ForEach(x => x.InventoryId = inventoryId);
                db.TB_InventoryDetails.AddRange(list);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                // up date
                var inventoryOld = db.TB_Inventory.FirstOrDefault(x => x.Id == inventory.Id);
                if (inventoryOld == null)
                    return Json(new { kq = "err", msg = "Thông tin không xác định!" }, JsonRequestBehavior.AllowGet);
                inventoryOld.Note = inventory.Note;
                inventoryOld.StatusID = inventory.StatusID;
                // tim thang order details cua thang kia roi remove di
                var invenDetails = db.TB_InventoryDetails.Where(x => x.InventoryId == inventory.Id).ToList();
                db.TB_InventoryDetails.RemoveRange(invenDetails);
                // check so luong san pham con lai trong kho xem co du khong
                list.ForEach(x => x.InventoryId = inventoryOld.Id);
                db.TB_InventoryDetails.AddRange(list);
                db.SaveChanges();
                return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("inventory/danh-sach-san-pham-kiem-ke")]
        public ActionResult ListProduct(List<int> listProduct, int? Id = null, string startDate ="", string endDate ="")
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("MainPage", "Account", new { area = "" });
            List<CompareProduct> listCompare = new List<Models.CompareProduct>();
            List<CompareProduct> list = new List<CompareProduct>();
            ViewBag.Id = Id == null ? 0 : Id;
            if(listProduct.Count == 0)
            {
                var product = db.TB_Products.Where(x => x.ProductStatus == EnumStatus.ACTIVE).ToList();
                foreach (var item in product)
                {
                    List<CompareProduct> temp = CompareProduct(item.ProductId,null,startDate,endDate);
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
                    List<CompareProduct> temp = CompareProduct(item,null,startDate,endDate);
                    if (temp.Count > 0)
                    {
                        temp.ForEach(x => listCompare.Add(x));
                    }
                }
            }
            
            if(Id != 0)
            {
                // lay ra danh sach lan kiem ke dau tien
                var listInvenDetails = db.TB_InventoryDetails.Where(x => x.InventoryId == Id).ToList();
                foreach(var item in listCompare)
                {
                    foreach(var value in listInvenDetails)
                    {
                        if(value.OrderId == item.Order.OrderId && value.ProductId == item.Product.ProductId)
                        {
                            item.Total = value.Total == null?0:value.Total.Value;
                            item.TotalNow = value.TotalNow == null ? 0 : value.TotalNow.Value;
                            item.TotalRemain = value.TotalRemaining == null ? 0 : value.TotalRemaining.Value;
                            item.TotalRemainNow = value.TotalRemainNow == null ? 0 : value.TotalRemainNow.Value;
                            item.TotalUse = value.TotalUsed == null ? 0 : value.TotalUsed.Value;
                            item.InventoryDetails = value;
                            list.Add(item);
                        }
                    }                
                }

            }else
            {
                list = listCompare;
               
                
            }
            
            
            return PartialView(list);
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
        public ActionResult ChangeStatus(int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            var inventory = db.TB_Inventory.FirstOrDefault(x => x.Id == id);
            if(inventory == null)
                return Json(new { kq = "err", msg = "Kiểm kê không hợp lệ" }, JsonRequestBehavior.AllowGet);
            return PartialView(inventory);
        }

        [HttpPost]
        [Route("inventory/change-status-inventory")]
        public ActionResult ChangeStatus(int? status = null, int? id = null)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            var inventory = db.TB_Inventory.FirstOrDefault(x => x.Id == id);
            if (inventory == null)
                return Json(new { kq = "err", msg = "Kiểm kê không hợp lệ" }, JsonRequestBehavior.AllowGet);
            inventory.StatusID = status;
            db.SaveChanges();

            return Json(new { kq = "ok", msg = "Success!" }, JsonRequestBehavior.AllowGet);
        }

        [Route("inventory/change-note-inventory")]
        public ActionResult ChangeNote(int index)
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.Index = index;
            return PartialView();
        }

        [HttpPost]
        [Route("inventory/change-note-inventory")]
        public ActionResult ChangeNote(string descriptiopn = "")
        {
            UserInfo nd_dv = (UserInfo)Session["NguoiDung"];
            if (nd_dv == null || (nd_dv.User.UserType != EnumUserType.ADMIN && nd_dv.User.UserType != EnumUserType.SUB_ADMIN))
                return RedirectToAction("Index", "Home", new { area = "" });

            return Json(new { kq = "ok", data = descriptiopn, msg = "Success!" }, JsonRequestBehavior.AllowGet);
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

        [Route("inventory/export-excel")]
        public ActionResult ExportExcel(int? id = null)
        {
          
            Application xlApp = new Application();
            if (xlApp == null)
            {
                return Json(new { kq = "err", msg = "Lỗi không thể sử dụng được thư viện EXCEL" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                
                var test = (from a in db.TB_Inventory.ToList()
                            join b in db.TB_InventoryDetails on a.Id equals b.InventoryId into b1
                            from b in b1.DefaultIfEmpty()
                            join c in db.TB_Products on b.ProductId equals c.ProductId into c1
                            from c in c1.DefaultIfEmpty()
                            join d in db.TB_Providers on c.ProductProviderId equals d.ProviderId into d1
                            from d in d1.DefaultIfEmpty()
                            join e in db.TB_Users on a.UserId equals e.UserId into e1
                            from e in e1.DefaultIfEmpty()
                            select new
                            {
                                Inventory = a,
                                InventoryDetails = b,
                                Product = c,
                                Provider = d,
                                Users = e
                            }).Where(x => x.Inventory.Id == id).ToList();
                var data = test.GroupBy(x => x.Inventory.Id).Select(t => new InventoryInfo
                {

                    Inventory = t.FirstOrDefault(y => y.Inventory.Id == t.Key).Inventory,
                    ListInventoryDetails = t.Select(y => y.InventoryDetails).Where(s => s.InventoryId == t.Key).ToList(),
                    Users = t.FirstOrDefault(y => y.Inventory.Id == t.Key).Users
                })
                .OrderByDescending(x => x.Inventory.CreatedDate)
                .FirstOrDefault();
                if (data == null)
                {
                    return Json(new { kq = "err", msg = "Lỗi không có bản kiểm kê nào" }, JsonRequestBehavior.AllowGet);
                }

                string fileName = "\\Export\\KiemKe" + data.Inventory.Code+".xls";
                string filePath = HttpContext.Server.MapPath("~" + fileName);

                xlApp.DisplayAlerts = false;
                xlApp.Visible = false;
                object missing = Type.Missing;
                Workbook wb = xlApp.Workbooks.Add(missing);
                Worksheet ws = (Worksheet)wb.Worksheets[1];

                int fontSizeTieuDe = 18;
                int fontSizeTenTruong = 14;
                int fontSizeNoiDung = 12;
                //Info
                ws.AddValue("A1", "B1", "Đơn vị :", fontSizeTieuDe, false, XlHAlign.xlHAlignCenter, false, 20);
                ws.AddValue("A2", "B2", "Bộ phận :", fontSizeTenTruong, false, XlHAlign.xlHAlignCenter, false);

                ws.AddValue("E5", "K5", "BIÊN BẢN KIỂM KÊ HÀNG HÓA, SẢN PHẨM", fontSizeTieuDe, true, XlHAlign.xlHAlignCenter, false, 20);
                ws.AddValue("A7", "E7", "Thời điểm kiểm kê :"+ data.Inventory.CreatedDate.Value.ToString("dd/MM/yyyy HH:ss:mm"), fontSizeTieuDe, true, XlHAlign.xlHAlignLeft, false, 20);
                ws.AddValue("A8", "B8", "Ban kiểm kê gồm :", fontSizeTieuDe, true, XlHAlign.xlHAlignLeft, false, 20);

                ws.AddValue("A9", "K9", "Ông (bà) :"+ data.Users.UserFullName+ "   Chức vụ :"+EnumUserType.ToString(data.Users.UserType)+" Trưởng ban", fontSizeTieuDe, true, XlHAlign.xlHAlignLeft, false);
                ws.AddValue("A10", "K10", "Ông (bà) :...........................      Chức vụ :............................ Ủy viên", fontSizeTieuDe, true, XlHAlign.xlHAlignLeft, false);
                ws.AddValue("A12", "E12", "Đã kiểm kê những mặt hàng dưới đây : ", fontSizeTieuDe, true, XlHAlign.xlHAlignLeft, false);


                int rowStart = 13, rowIndex = 13;
                //Header
                ws.AddValue("A" + rowIndex, "A" + rowIndex, "STT", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("B" + rowIndex, "B" + rowIndex, "Mã sản phẩm", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("C" + rowIndex, "C" + rowIndex, "Tên sản phẩm", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("D" + rowIndex, "D" + rowIndex, "Mã hóa đơn", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("E" + rowIndex, "E" + rowIndex, "Ngày nhập", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("F" + rowIndex, "F" + rowIndex, "Ngày hết hạn", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("G" + rowIndex, "G" + rowIndex, "Đơn vị tính", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("H" + rowIndex, "H" + rowIndex, "Đơn giá", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("I" + rowIndex, "I" + rowIndex, "Tổng số sản phẩm", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("J" + rowIndex, "J" + rowIndex, "Tổng sản phẩm nhập thực tế", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("K" + rowIndex, "K" + rowIndex, "Tổng số đã sử dụng ", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("L" + rowIndex, "L" + rowIndex, "Tông số còn lại", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("M" + rowIndex, "M" + rowIndex, "Tổng số còn lại thực tế", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("N" + rowIndex, "N" + rowIndex, "Trạng thái", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                ws.AddValue("O" + rowIndex, "O" + rowIndex, "Mô tả", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 12);
                rowIndex += 1;
                int total = 0;
                int totalNow = 0;
                int totalUsed = 0;
                int totalRemain = 0;
                int totalRemainNow = 0;



                //Body

                for (int i = 0; i < data.ListInventoryDetails.Count; i++)
                {
                    var item = data.ListInventoryDetails[i];
                    var stt = i + 1;
                    var product = db.TB_Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                    var order = db.TB_Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                    var orderDetails = db.TB_OrderDetails.Where(x => x.DetailOrderId == item.OrderId && x.DetailProductId == item.ProductId).FirstOrDefault();
                    dynamic[] val = { stt,product.ProductCode,product.ProductName,order.OrderCode,order.OrderDate, orderDetails.DetailExpiredDate,item.Unit,orderDetails.DetailPrice,item.Total,item.TotalNow,item.TotalUsed,item.TotalRemaining,item.TotalRemainNow,EnumInventoryStatus.ToString(item.StatusID),item.Note.Trim()};
                    ws.AddValue("A" + rowIndex, "O" + rowIndex, val, fontSizeNoiDung, false, XlHAlign.xlHAlignLeft, false);
                    total += item.Total.Value;
                    totalNow += item.Total.Value;
                    totalUsed += item.TotalUsed.Value;
                    totalRemain += item.TotalRemaining.Value;
                    totalRemainNow += item.TotalRemainNow.Value;
                    rowIndex += 1;
                }
                //End
                ws.AddValue("H" + rowIndex, "H" + rowIndex, "TỔNG CỘNG", fontSizeTenTruong, true, XlHAlign.xlHAlignCenter, true, 18);
                ws.AddValue("I" + rowIndex, "I" + rowIndex, total.ToString("#,###.###"), fontSizeTenTruong);
                ws.AddValue("J" + rowIndex, "J" + rowIndex, totalNow.ToString("#,###.###"), fontSizeTenTruong);
                ws.AddValue("K" + rowIndex, "K" + rowIndex, totalUsed.ToString("#,###.###"), fontSizeTenTruong);
                ws.AddValue("L" + rowIndex, "L" + rowIndex, totalRemain.ToString("#,###.###"), fontSizeTenTruong);
                ws.AddValue("M" + rowIndex, "M" + rowIndex, totalRemainNow.ToString("#,###.###"), fontSizeTenTruong);
                //Border
                ws.get_Range("D" + rowStart, "U" + rowIndex).SetBorderAround();

                //Save
                wb.SaveAs(filePath);
                wb.SaveAs(filePath, XlFileFormat.xlOpenXMLWorkbook, missing, missing, false, false
                    , XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlUserResolution
                    , true, missing, missing, missing);
                wb.Saved = true;
                wb.Close(true, missing, missing);
                //wb.Close();

                //thoát và thu hồi bộ nhớ cho COM
                ws.ReleaseObject();
                wb.ReleaseObject();

                return Json(new { kq = "ok", msg = "Thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { kq = "err", msg = "Đã xảy ra lỗi khi lưu dữ liệu. Kiểm tra File" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (xlApp != null)
                {
                    xlApp.Quit();
                }
                xlApp.ReleaseObject();
            }
        }


    }
}