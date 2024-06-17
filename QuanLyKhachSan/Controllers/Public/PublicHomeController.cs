using QuanLyKhachSan.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhachSan.Controllers.Public
{
    public class PublicHomeController : Controller
    {
        RoomDao roomDao = new RoomDao();
        ServiceDao serviceDao = new ServiceDao();
        TypeDao typeDao = new TypeDao();
        CityDao cityDao = new CityDao();
        
        // GET: PublicHome
        public ActionResult Index()
        {
            ViewBag.ListCity = cityDao.GetCities();
            ViewBag.ListRoomTop5 = roomDao.GetRoomTop5();
            ViewBag.ListRoomDiscount = roomDao.GetRoomDiscount();
            ViewBag.ListService = serviceDao.GetServicesTop5();
            ViewBag.ListType = typeDao.GetTypes();
            ViewBag.active = "home";
            return View();
        }


        public ActionResult AboutUs()
        {
            ViewBag.active = "aboutus";
            return View();
        }

        public ActionResult Introduce()
        {
            ViewBag.active = "introduce";
            return View();
        }
        
    }
}