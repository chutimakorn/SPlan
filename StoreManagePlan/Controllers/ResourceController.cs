using Microsoft.AspNetCore.Mvc;

namespace StoreManagePlan.Controllers
{
    public class ResourceController : Controller
    {
        public string ConvertDate(string date)
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

        public string CreateDate()
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyyMMdd");
            return formattedDate;
        }
    }
}
