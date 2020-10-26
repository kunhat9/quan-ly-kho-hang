using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace QuanLyKhoHang.UI.Models
{
    public static class ExcelHelper
    {
        public static void AddValue(this Worksheet ws, string begin, string end, dynamic value, int fontSize, bool merge = false
          , XlHAlign align = XlHAlign.xlHAlignCenter, bool bold = true, int ColumnWidth = -1, string fontName = "Times New Roman")
        {
            Range range = ws.get_Range(begin, end);
            if (merge) range.Merge();
            range.Font.Size = fontSize;
            range.Font.Name = fontName;
            range.Font.Bold = bold;
            range.Cells.HorizontalAlignment = align;
            range.Value2 = value;
            if (ColumnWidth > -1) range.ColumnWidth = ColumnWidth;
        }

        //Hàm kẻ khung cho Excel
        public static void SetBorderAround(this Range range)
        {
            Borders borders = range.Borders;
            borders.Color = Color.Black;
            //Viền (khung) ngoài
            borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            //Viền trong
            borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;//Dọc
            borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;//Ngang
            borders[XlBordersIndex.xlDiagonalUp].LineStyle = XlLineStyle.xlLineStyleNone;//Chéo lên
            borders[XlBordersIndex.xlDiagonalDown].LineStyle = XlLineStyle.xlLineStyleNone;//Chéo xuống
        }

        //Hàm thu hồi bộ nhớ cho COM Excel
        public static void ReleaseObject(this object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                obj = null;
            }
            finally
            { GC.Collect(); }
        }
    }
}