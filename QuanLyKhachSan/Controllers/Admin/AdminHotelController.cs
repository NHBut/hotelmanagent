using QuanLyKhachSan.Controllers.Auth;
using QuanLyKhachSan.Daos;
using QuanLyKhachSan.DTO;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhachSan.Controllers.Admin
{
    [CustomAuthorize(1)]
    public class AdminHotelController : Controller
    {
       HotelDao hotelDAO = new HotelDao();
        QuanLyKhachSanDBContext db = new QuanLyKhachSanDBContext();
        // GET: AdminHotel
        public ActionResult Index(string msg)
        {
            ViewBag.Msg = msg;
            ViewBag.List = hotelDAO.GetHotels();
            ViewBag.listCity = db.cities.ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Hotel hotel)
        {
            var file = Request.Files["file"];
            string reName = DateTime.Now.Ticks.ToString() + file.FileName;
            file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            hotel.ImageUrl = reName;

            hotelDAO.add(hotel);
            return RedirectToAction("Index", new { msg = "1" });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Hotel hotel)
        {
            string reName = "";
            var objCourse = hotelDAO.GetDetail(hotel.HotelId);
            var file = Request.Files["file"];
            if (file.FileName == "" || file.FileName == null)
            {
                reName = hotel.ImageUrl;
            }
            else
            {
                reName = DateTime.Now.Ticks.ToString() + file.FileName;
                file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            }
            hotel.ImageUrl = reName;
            hotelDAO.update(hotel);
            return RedirectToAction("Index", new { msg = "1" });
        }
        [HttpPost]
        public ActionResult Delete(Hotel hotel)
        {
            var check = hotelDAO.getCheck(hotel.HotelId);
            if (check.Count == 0)
            {
                hotelDAO.delete(hotel.HotelId);
                return RedirectToAction("Index", new { msg = "1" });
            }
            else
            {
                return RedirectToAction("Index", new { msg = "2" });
            }
        }

    }
}