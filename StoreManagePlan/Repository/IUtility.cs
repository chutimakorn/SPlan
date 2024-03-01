using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace StoreManagePlan.Repository
{
    public interface IUtility
    {
        string ConvertDate(string? date);
        string ConvertDateFormExcel(string? date);
        string CreateDate();
        string? GetString(ExcelRangeBase cell);
        int? GetInt(ExcelRangeBase cell);
        double? GetDecimal(ExcelRangeBase cell);
        void SaveExcelFile(ExcelPackage package, string filePath);
        bool CheckInt(ExcelRangeBase cell);
        void MergeRowspanHeaders(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn);
        void MergeColspanHeaders(ExcelWorksheet worksheet, int startRow, int startColumn, int endRow, int endColumn);

    }
}
