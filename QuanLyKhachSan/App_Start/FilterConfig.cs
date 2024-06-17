using QuanLyKhachSan.Controllers.Auth;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhachSan
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           /* filters.Add(new AdminSessionAuthorizeAttribute()); // Kiểm tra session toàn cầu*/
        }
    }
}
