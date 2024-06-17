using QuanLyKhachSan.Controllers.Auth;
using QuanLyKhachSan.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace QuanLyKhachSan.Controllers.Admin
{
    [CustomAuthorize(1, 2)]
    public class AdminServiceController : Controller
    {
        ServiceDao serviceDao = new ServiceDao();
        HotelDao hotelDao = new HotelDao();
        // GET: Adminservice
        public ActionResult Index(string msg)
        {
            ViewBag.Msg = msg;
            ViewBag.List = serviceDao.GetServicesByRoomId();
            ViewBag.listHotel = hotelDao.GetHotels();
            return View();
        }
        public ActionResult Add(QuanLyKhachSan.Models.Service service)
        {
            serviceDao.add(service);
            return RedirectToAction("Index", new { msg = "1" });
        }

        public ActionResult Update(QuanLyKhachSan.Models.Service service)
        {
            serviceDao.update(service);
            return RedirectToAction("Index", new { msg = "1" });
        }

        public ActionResult Delete(QuanLyKhachSan.Models.Service service)
        {
            var check = serviceDao.getCheck(service.idService);
            if (check.Count == 0)
            {
                serviceDao.delete(service.idService);
                return RedirectToAction("Index", new { msg = "1" });
            }
            else
            {
                return RedirectToAction("Index", new { msg = "2" });
            }
        }
    }
}