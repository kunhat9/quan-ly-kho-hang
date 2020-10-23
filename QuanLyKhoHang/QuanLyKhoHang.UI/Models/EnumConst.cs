using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public static class EnumStatus
    {
        public const int INACTIVE = 2;
        public const int ACTIVE = 1;
        public const int FINISH = 3;
        public const int CANCEL = 4;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case INACTIVE:
                    return "Chờ duyệt";
                case ACTIVE:
                    return "Kích hoạt";
                case FINISH:
                    return "Hoàn thành";
                case CANCEL:
                    return "Hủy";
                default:
                    return "";
            }
        }
    }
    public static class EnumInventoryStatus
    {
        public const int DAY_DU = 1;
        public const int DU_THUA = 2;
        public const int KHONG_DAY_DU = 3;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case DAY_DU:
                    return "Đầy đủ";
                case DU_THUA:
                    return "Dư thừa";
                case KHONG_DAY_DU:
                    return "Thiếu sản phẩm";
                default:
                    return "";
            }
        }
    }

    public static class EnumInvetoryStatus
    {
        public const int KHONG_XAC_DINH = 0;
        public const int SU_DUNG_DUOC = 1;
        public const int KHONG_SU_DUNG = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case SU_DUNG_DUOC:
                    return "Đầy đủ";
                case KHONG_SU_DUNG:
                    return "Dư thừa";
                case KHONG_XAC_DINH:
                    return "Không xác định";
                default:
                    return "";
            }
        }
    }
    public static class EnumReportType
    {
        public const int GIAO_DICH_SAN_PHAM = 1;
        public const int SAN_PHAM_TRONG_KHO = 2;
        public const int SAN_PHAM_HET_HAN = 3;
        public const int SAN_PHAM_HET_HANG = 4;
        public const int SAN_PHAM_SAP_HET_HANG = 5;
        public const int SAN_PHAM_SAP_HET_HAN = 6;
    }
    public static class EnumOrderStatus
    {
        public const int DANG_SU_DUNG = 1;
        public const int HET_HANG = 2;
        public const int KHONG_SU_DUNG = 3;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case DANG_SU_DUNG:
                    return "Sử dụng";
                case HET_HANG:
                    return "Đã hết hàng";
                case KHONG_SU_DUNG:
                    return "Không sử dụng";
                default:
                    return "";
            }
        }
    }
    public static class EnumUserType
    {
        public const int ADMIN = 0;
        public const int SUB_ADMIN = 1;
        public const int MEMBER = 2;
        public const int ACCOUNTANT = 3;
        public const int INVENTORY = 4;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case ADMIN:
                    return "Quản trị hệ thống";
                case SUB_ADMIN:
                    return "Quản trị";
                case MEMBER:
                    return "Nhân viên";
                case ACCOUNTANT:
                    return "Kế toán";
                case INVENTORY:
                    return "Người kiểm kê";
                default:
                    return "";
            }
        }
    }
    public static class EnumOrderType
    {
        public const int NHAP = 1;
        public const int XUAT = 2;
        public static string ToString(int? value)
        {
            switch (value)
            {
                case NHAP:
                    return "Hóa đơn nhập";
                case XUAT:
                    return "Hóa đơn xuất";
                default:
                    return "";
            }
        }
    }
}