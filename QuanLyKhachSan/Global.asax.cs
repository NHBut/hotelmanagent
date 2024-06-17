using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace QuanLyKhachSan
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    try
                    {
                        var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        if (authTicket != null)
                        {
                            var userData = authTicket.UserData;
                            var userRole = new QuanLyKhachSan.Daos.UserDao().getInfor(int.Parse(userData));

                            string[] roles = new string[] { userRole.ToString() };
                            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                        }
                    }
                    catch (CryptographicException ex)
                    {
                        // Log the cryptographic exception
                        System.Diagnostics.Trace.TraceError("CryptographicException: " + ex.Message);
                        // Sign out the user to clear the invalid cookie
                        FormsAuthentication.SignOut();
                    }
                    catch (Exception ex)
                    {
                        // Log any other exceptions
                        System.Diagnostics.Trace.TraceError("Exception: " + ex.Message);
                        // Optionally, sign out the user if any other exception occurs
                        FormsAuthentication.SignOut();
                    }
                }
            }
        }
    }
}
