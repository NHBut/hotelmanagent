using QuanLyKhachSan.Daos;
using QuanLyKhachSan.DTO;
using QuanLyKhachSan.Models;
using QuanLyKhachSan.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhachSan.Controllers.Public
{
    public class PublicCityController : Controller
    {
        // GET: City
        CityDao cityDAO = new CityDao();
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        public ActionResult Index(int? page)
        {
            int pageSize = 3; // số lượng mục trên mỗi trang
            int currentPage = page ?? 1; // Default to page 1 if page is null

            // Get the total number of cities
            int totalCities = cityDAO.GetNumberCity();

            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling((double)totalCities / pageSize);

            // Get the cities for the current page
            var cities = cityDAO.GetCitiesPage(currentPage, pageSize);

            // Assign values to ViewBag for use in the view
            ViewBag.List = cities;
            ViewBag.tag = currentPage;
            ViewBag.pageSize = totalPages;

            return View();
        }


        public ActionResult CityDetails(int id, string mess)
        {
            ViewBag.mess = mess;
            // Lấy thông tin chi tiết của thành phố
            var city = myDb.cities.FirstOrDefault(c => c.CityId == id);
            if (city == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách khách sạn trong thành phố

            var hotels = myDb.hotels.Where(h => h.CityId == id).ToList();

            // Tạo ViewModel để gửi dữ liệu đến View
            var cityDetailsViewModel = new CityHotelsViewModel
            {
                CityId = city.CityId,
                CityName = city.Name,
                Image = city.Image,
                Hotels = hotels
            };
            

            return View(cityDetailsViewModel);
        }


    }
}
