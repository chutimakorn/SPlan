using OfficeOpenXml;

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
    }
}
