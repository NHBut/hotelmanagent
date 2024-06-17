using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QuanLyKhachSan.Daos;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers.Public
{
    [AllowAnonymous]
    public class PublicAuthenticationController : Controller
    {
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        UserDao userDao = new UserDao();

        public ActionResult CheckOTP()
        {

            return View();
        }

        public ActionResult ForgotPassword()
        {

            return View();
        }
        public ActionResult Login()
        {
          
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
                    return RedirectToAction("Index", "PublicHome");
                }
                else
                {
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            else
            {
                ViewBag.mess = "Error";
                return View("Login");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "PublicAuthentication");
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

        [HttpPost]
        public ActionResult RePassword(FormCollection form)
        {

            var email = form["email"];
            var check = userDao.getUserByEmail(email);
            if (check != null)
            {
                var idUser = check.idUser;
                /*string html = "Vui lòng nhấn vào link để reset mật khẩu : <a href='https://localhost:44385/PublicAuthentication/ResetPassword/" + idUser + "'>Tại đây</a>";*/
                string html = "Vui lòng nhấn vào link để reset mật khẩu : <a href='http://butnb1234-001-site1.etempurl.com/PublicAuthentication/ResetPassword/" + idUser + "'>Tại đây</a>";
                sendMail(check.email, html);
                ViewBag.mess = "Success";
                return View("ForgotPassword");
            }
            ViewBag.mess = "Error";
            return View("ForgotPassword");
        }
        [HttpPost]
        public ActionResult Register(User user, FormCollection form)
        {
            string rePassword = form["rePassword"];
            bool checkExistUserName = userDao.checkExistUsername(user.userName);
            if (checkExistUserName)
            {
                ViewBag.mess = "ErrorExist";
                return View("Login");
            }
            else
            {
                if (!user.password.Equals(rePassword))
                {
                    ViewBag.mess = "ErrorPassword";
                    return View("Login");
                }
                else
                {
                    user.password = userDao.md5(user.password);
                    user.idRole = 3;
                    var otp = RandomNumber(6);
                    Session.Add("RegisterUser", user);
                    Session.Add("Otp", otp);
                    string html = "Mã xác thực OTP đăng ký của bạn là :  " + otp;
                    sendMail(user.email, html);

                    ViewBag.mess = "Success";
                    return View("CheckOTP");

                }
            }
        }
        //random chuỗi số bất kỳ
        public static string RandomNumber(int numberRD)
        {
            string randomStr = "";
            try
            {

                int[] myIntArray = new int[numberRD];
                int x;
                //that is to create the random # and add it to uour string
                Random autoRand = new Random();
                for (x = 0; x < numberRD; x++)
                {
                    myIntArray[x] = System.Convert.ToInt32(autoRand.Next(0, 9));
                    randomStr += (myIntArray[x].ToString());
                }
            }
            catch (Exception ex)
            {
                randomStr = "error";
            }
            return randomStr;
        }
        public void sendMail(string email, string body)
        {
            var formEmailAddress = ConfigurationManager.AppSettings["FormEmailAddress"].ToString();
            var formEmailDisplayName = ConfigurationManager.AppSettings["FormEmailDisplayName"].ToString();
            var formEmailPassword = ConfigurationManager.AppSettings["FormEmailPassword"].ToString();
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
            var smtpPort = ConfigurationManager.AppSettings["SMTPPost"].ToString();
            bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"].ToString());
            MailMessage message = new MailMessage(new MailAddress(formEmailAddress, formEmailDisplayName), new MailAddress(email));
            message.Subject = "Thông báo";
            message.IsBodyHtml = true;
            message.Body = body;
            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(formEmailAddress, formEmailPassword);
            client.Host = smtpHost;
            client.EnableSsl = enableSsl;
            client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
            client.Send(message);
        }
    }
}
