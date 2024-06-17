using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QuanLyKhachSan.Daos;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers.Admin
{
    [AllowAnonymous]
    public class AdminAuthenticationController : Controller
    {
        UserDao userDao = new UserDao();

        public ActionResult Index()
        {
            var userInformation = GetUserInfoFromCookie();
            
            if (userInformation == null)
            {
                return RedirectToAction("Login", "AdminAuthentication");
            }
            ViewBag.UserInformation = userInformation;
            return View();
        }
 

        public ActionResult Login()
        {
            var userInformation = GetUserInfoFromCookie();
   
            if (userInformation != null)
            {
                if (userInformation.idRole == 3)
                {
                    return RedirectToAction("Index", "PublicHome");
                }
                else
                {
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string userName = form["userName"];
            string password = form["password"];
            string passwordMd5 = userDao.md5(password);

            bool checkLogin = userDao.checkLogin(userName, passwordMd5);
            if (checkLogin)
            {
                var userInformation = userDao.getUserByUserName(userName);
                ViewBag.UserInformation = userInformation;
                string userData = userInformation.idUser.ToString();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    userName,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    userData,
                    FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(authCookie);

                if (userInformation.idRole == 3)
                {
                    ViewBag.mess = "Bạn không có quyền truy cập vào trang quản trị";
                    return View("Login");
                }
                else
                {
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            else
            {
                ViewBag.mess = "Thông tin tài khoản hoặc mật khẩu không chính xác";
                return View("Login");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/AdminAuthentication/Index");
        }

        private User GetUserInfoFromCookie()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket != null && !string.IsNullOrEmpty(ticket.UserData))
                {
                    int userId;
                    if (int.TryParse(ticket.UserData, out userId))
                    {
                        var userDao = new UserDao();
                        return userDao.getInfor(userId);
                    }
                }
            }
            return null;
        }

        public ActionResult Unauthorized()
        {
            return Unauthorized();
        }
    }
}
