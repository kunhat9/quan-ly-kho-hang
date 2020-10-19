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
}