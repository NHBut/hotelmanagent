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
    [CustomAuthorize(1)]
    public class AdminCityController : Controller
    {
        // GET: AdminCity
        CityDao cityDAO = new CityDao();
        public ActionResult Index(string msg)
        {

            ViewBag.Msg = msg;
            ViewBag.List = cityDAO.GetCities();
            
            return View();
 
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(City city)
        {
            var file = Request.Files["file"];
            string reName = DateTime.Now.Ticks.ToString() + file.FileName;
            file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            city.Image = reName;

            cityDAO.add(city);
            return RedirectToAction("Index", new { msg = "1" });
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(City city)
        {
            string reName = "";
            var objCourse = cityDAO.GetDetail(city.CityId);
            var file = Request.Files["file"];
            if (file.FileName == "")
            {
                reName = objCourse.Image;
            }
            else
            {
                reName = DateTime.Now.Ticks.ToString() + file.FileName;
                file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            }
            city.Image = reName;
            cityDAO.update(city);
            return RedirectToAction("Index", new { msg = "1" });
        }
        [HttpPost]
        public ActionResult Delete(City city)
        {
            var check = cityDAO.getCheck(city.CityId);
            if (check.Count == 0)
            {
                cityDAO.delete(city.CityId);
                return RedirectToAction("Index", new { msg = "1" });
            }
            else
            {
                return RedirectToAction("Index", new { msg = "2" });
            }
        }
    }
}