using QuanLyKhachSan.Daos;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QuanLyKhachSan.Controllers.Public
{
    public class PublicHotelController : Controller
    {
        private QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        // GET: PublicHotel
        RoomDao roomDao = new RoomDao();
        ServiceDao serviceDao = new ServiceDao();
        TypeDao typeDao = new TypeDao();
        HotelDao hotelDao = new HotelDao();
        public ActionResult Index(int? page)
        {
            int pageSize = 6; // số lượng mục trên mỗi trang
            int currentPage = page ?? 1;

            // Get the total number of cities
            int totalHotel = hotelDao.GetNumberHotel();

            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling((double)totalHotel / pageSize);

            // Get the cities for the current page
            var hotels = hotelDao.GetHotelsPage(currentPage, pageSize);

            // Assign values to ViewBag for use in the view
            ViewBag.List = hotels;
            ViewBag.tag = currentPage;
            ViewBag.pageSize = totalPages;

            return View();
        }

        [HttpPost]
        public ActionResult Search(FormCollection form)
        {
            int cityId = Int32.Parse(form["CityId"]);
            int numberChildren = Int32.Parse(form["numberChildren"]);
            int numberAdult = Int32.Parse(form["numberAdult"]);
            DateTime checkInDate = DateTime.Parse(form["checkInDate"]);
            DateTime checkOutDate = DateTime.Parse(form["checkOutDate"]);   
            return RedirectToAction("Search", new
            {
                page = 0,
                cityId = cityId,
                numberChildren = numberChildren,
                numberAdult = numberAdult,
                checkInDate = checkInDate,
                checkOutDate = checkOutDate
            });
        }

        [ChildActionOnly]
        public ActionResult _SearchResults()
        {
            return PartialView();
        }
        public ActionResult Search(int page, int cityId, int numberChildren, int numberAdult, DateTime checkInDate, DateTime checkOutDate)
        {
            if (page == 0)
            {
                page = 1;
            }

            if (cityId == 0)
            {
                ViewBag.mess = "ErrorSearch";
                ViewBag.List = new List<Room>(); // Gán danh sách trống để tránh lỗi null
            }
            else
            {
                // Ví dụ gán ViewBag.List từ dữ liệu thực tế
                ViewBag.List = hotelDao.SearchByCriteria(page, cityId, numberChildren, numberAdult, checkInDate, checkOutDate);
            }

            return View();
        }

        [HttpPost]
        public ActionResult SearchRooms(FormCollection form)
        {
            int hotelId = Int32.Parse(form["hotelId"]);
            int numberChildren = Int32.Parse(form["numberChildren"]);
            int numberAdult = Int32.Parse(form["numberAdult"]);
            DateTime checkInDate = DateTime.Parse(form["checkInDate"]);
            DateTime checkOutDate = DateTime.Parse(form["checkOutDate"]);
            return RedirectToAction("SearchRoom", new
            {
                hotelId= hotelId,
                numberChildren = numberChildren,
                numberAdult = numberAdult,
                checkInDate = checkInDate,
                checkOutDate = checkOutDate
            });
        }
        public ActionResult SearchRoom( int hotelId, int numberChildren, int numberAdult, DateTime checkInDate, DateTime checkOutDate)
        {
      

            if (hotelId == 0)
            {
                ViewBag.mess = "ErrorSearch";
                ViewBag.List = new List<Room>(); // Gán danh sách trống để tránh lỗi null
            }
            else
            {
                // Ví dụ gán ViewBag.List từ dữ liệu thực tế
                

                ViewBag.checkInDate = checkInDate.ToString("yyyy-MM-dd");
                ViewBag.checkOutDate = checkOutDate.ToString("yyyy-MM-dd");

                ViewBag.Hotel = myDb.hotels.FirstOrDefault(h => h.HotelId == hotelId);
                ViewBag.List = hotelDao.SearchRoom( hotelId, numberChildren, numberAdult, checkInDate, checkOutDate);

            }

            return View();
        }

        

        public ActionResult DetailHotel(int id, string mess, int page = 1 )
        {
            ViewBag.mess = mess;

            var hotel = myDb.hotels.FirstOrDefault(c => c.HotelId == id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            ViewBag.HotelId = id; // Thêm dòng này
            // Lấy danh sách khách sạn trong thành phố

            var rooms = myDb.rooms.Where(h => h.HotelId == id).ToList();

            // Tạo ViewModel để gửi dữ liệu đến View
            var hotelsViewModel = new HotelDetailsViewModel
            {
                HotelId = hotel.CityId,
                Name = hotel.Name,
                ImageUrl = hotel.ImageUrl,
                Address = hotel.Address,
                PhoneNumber = hotel.PhoneNumber,
                Description= hotel.Description,
                Rooms = rooms
            };
   

            ViewBag.ListRoomTop5 = roomDao.GetRoomTop5(id);
            ViewBag.ListRoomDiscount = roomDao.GetRoomDiscount(id);
            ViewBag.ListService = serviceDao.GetServicesTop5(id);
            ViewBag.ListType = typeDao.GetTypes();

            ViewBag.List = roomDao.GetRoomsBlank(id,page, 3);
            ViewBag.tag = page;
            ViewBag.pageSize = hotelDao.GetNumberRoom(id);

            return View(hotelsViewModel);

        }
       

    }
}