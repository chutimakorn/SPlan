using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Server;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StoreManagePlan.Repository;
using System.Globalization;

namespace StoreManagePlan.Repository
{
    public class Utility:IUtility
    {
        public static readonly string FormateInput = "d/M/yyyy H:mm:ss";
        public static readonly string FormatOutput = "yyyyMMdd";
        public static readonly CultureInfo culture = new CultureInfo("en-US");
        public string ConvertDate(string? date)
        {

            var fulldate = "";

            if (date != null && date.Length == 8)
            {
                var dd = date.Substring(6, 2);
                var mm = date.Substring(4, 2);
                var yy = date.Substring(0, 4);
                fulldate = dd + "/" + mm + "/" + yy;
            }



            return fulldate;
        }

        public string ConvertDateFormExcel(string? date)
        {

            if (date != null && date.Length >= 8)
            {
                DateTime result = DateTime.ParseExact(date, FormateInput, culture);
                string dateString = result.ToString(FormatOutput, culture);

                return dateString;
            }
            return "";
        }

        public string CreateDate()
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString(FormatOutput, culture);
            return formattedDate;
        }

        public string? GetString(ExcelRangeBase cell)
        {
            if (cell.Value != null)
            {
                return cell.Value.ToString();
            }
            return null;
        }
        public int? GetInt(ExcelRangeBase cell)
        {
            if (cell.Value != null)
            {
                return Convert.ToInt32(cell.Value.ToString());
            }
            return null;
        }
        public double? GetDecimal(ExcelRangeBase cell)
        {
            if (cell.Value != null)
            {
                return Convert.ToDouble(cell.Value.ToString());
            }
            return null;
        }
        public void SaveExcelFile(ExcelPackage package, string filePath)
        {
            // Ensure the directory exists before saving
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Save the Excel package to the specified path
            package.SaveAs(new FileInfo(filePath));
        }
        public bool CheckInt(ExcelRangeBase cell)
        {
            
            if (cell.Value != null)
            {
                return decimal.TryParse(cell.Value.ToString(), out decimal result);
            }
            return false;
        }
        public void MergeRowspanHeaders(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn)
        {
            using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
            {
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        public void MergeColspanHeaders(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn)
        {
            using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
            {
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }
    }
}
