using QuanLyKhachSan.Controllers.Auth;
using QuanLyKhachSan.Daos;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;   
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhachSan.Controllers.Admin
{
    [CustomAuthorize(1,2)]
    public class AdminHomeController : Controller
    {
        BookingDao bookingDao = new BookingDao();
        HotelDao hotelDao = new HotelDao();
        // GET: AdminHome
        
        public ActionResult Index(int hotelId = 0, int year = 0)

        {
            ViewBag.Hotels = hotelDao.GetHotels();
            ViewBag.SelectedHotelId = hotelId;
            ViewBag.SelectedYear = year;

            if (hotelId > 0 && year > 0)
            {
                var revenueData = bookingDao.GetRevenueStatistics(hotelId, year);

                ViewBag.RevenueData = revenueData;
                ViewBag.TotalRevenue = revenueData.Sum();
                ViewBag.TotalBookings = bookingDao.GetTotalBookings(hotelId, year);
                ViewBag.CancelledBookings = bookingDao.GetCancelledBookings(hotelId, year);
            }
            else
            {
                ViewBag.RevenueData = new List<decimal>();
            }
            return View();
        }
/*
        [HttpGet]
        public ActionResult GetRevenueStatistics(int hotelId, int year)
        {
            *//*var revenueData = bookingDao.GetRevenueStatistics(hotelId, year);*//*
            var revenueData = new List<decimal>
    {
        1000, 1500, 1200, 1800, 2000, 1700, 1600, 1900, 2100, 2200, 2300, 2400
    };
            ViewBag.RevenueData = revenueData;
            ViewBag.SelectedYear = year;
            ViewBag.Hotels = hotelDao.GetHotels(); // To refill the dropdown
            return View("Index");
        }
*/
        
    }
}