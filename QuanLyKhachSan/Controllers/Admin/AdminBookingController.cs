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
    [CustomAuthorize(1,2)]
    public class AdminBookingController : Controller
    {
        BookingDao bookingDao = new BookingDao();
        BookingServiceDao bookingServiceDao = new BookingServiceDao();
        ServiceDao serviceDao = new ServiceDao();
        RoomDao roomDao = new RoomDao();
        // GET: AdminUser
        public ActionResult Index(string msg)
        {
            ViewBag.Msg = msg;
            ViewBag.List = bookingDao.getAll();
            return View();
        }

        public ActionResult Detail(int id)
        {
            ViewBag.Booking = bookingDao.GetBookingById(id);
            ViewBag.List = bookingDao.getBS(id);
            ViewBag.ListService = serviceDao.GetServicesByBookingId(id);
            return View();
        }

        public ActionResult Bill(int id)
        {
            ViewBag.Booking = bookingDao.GetBookingById(id);
            ViewBag.List = bookingDao.getBS(id);
            return View();
        }

        public ActionResult update(Booking booking)
        {
            bookingDao.UpdateStatus(booking);
            return RedirectToAction("Index", new { msg = "1" });
        }

        [HttpPost]
        public ActionResult UpdateBookingService(int bookingId, int[] idService)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            User user = (User)Session["ADMIN"];
            if (user == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Kiểm tra xem đặt phòng tồn tại không
            Booking existingBooking = bookingDao.GetBookingById(bookingId);
            if (existingBooking == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            // Lấy thông tin phòng
            var room = roomDao.GetDetail(existingBooking.idRoom);
            if (room == null)
            {
                return RedirectToAction("Detail", new { id = bookingId, mess = "ErrorRoomNotFound" });
            }

            // Tính toán số ngày booking
            DateTime dateCheckin = existingBooking.checkInDate;
            DateTime dateCheckout = existingBooking.checkOutDate;
            TimeSpan timeSpan = dateCheckout - dateCheckin;
            int numberBookingDays = timeSpan.Days;
            if (numberBookingDays <= 0)
            {
                return RedirectToAction("Detail", new { id = bookingId, mess = "ErrorInvalidDates" });
            }

            // Tính lại tổng tiền phòng
            int totalRoomCost = (room.cost * numberBookingDays - room.cost * numberBookingDays * room.discount / 100);

            // Xóa tất cả các dịch vụ đã liên kết với đặt phòng hiện tại
            bookingServiceDao.DeleteByBookingId(bookingId);

            // Tính tổng tiền dịch vụ
            int totalServiceCost = 0;
            if (idService != null)
            {
                foreach (var serviceId in idService)
                {
                    BookingService bookingService = new BookingService
                    {
                        idBooking = bookingId,
                        idService = serviceId
                    };
                    bookingServiceDao.Add(bookingService);

                    // Lấy chi phí của từng dịch vụ và cộng vào tổng tiền dịch vụ
                    var cost = serviceDao.GetCostById(serviceId);
                    if (cost != null)
                    {
                        totalServiceCost += cost;
                    }
                }
            }

            // Cập nhật tổng tiền booking
            existingBooking.totalMoney = totalRoomCost + totalServiceCost;
            bookingDao.update(existingBooking);

            return RedirectToAction("Detail", new { id = bookingId });

        }
        [HttpPost]
        public ActionResult DeleteServiceFromBooking(int idService, int idBooking)
        {
            // Lấy thông tin dịch vụ cần xóa
            var cost = serviceDao.GetCostById(idService);
            if (cost == null)
            {
                return RedirectToAction("Detail", new { id = idBooking, mess = "ErrorServiceNotFound" });
            }

            // Xóa dịch vụ từ booking
            bookingServiceDao.DeleteByServiceAndBookingId(idService, idBooking);

            // Lấy thông tin booking hiện tại
            var booking = bookingDao.GetBookingById(idBooking);
            if (booking == null)
            {
                return RedirectToAction("Detail", new { id = idBooking, mess = "ErrorBookingNotFound" });
            }

            // Trừ tiền dịch vụ khỏi tổng tiền
            booking.totalMoney -= cost;
            bookingDao.update(booking);

            return RedirectToAction("Detail", new { id = idBooking });
        }
    }
}
