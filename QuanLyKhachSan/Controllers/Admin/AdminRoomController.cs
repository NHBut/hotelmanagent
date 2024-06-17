using Microsoft.Ajax.Utilities;
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
    public class AdminRoomController : Controller
    {
        QuanLyKhachSanDBContext db = new QuanLyKhachSanDBContext();
        RoomDao roomDao = new RoomDao();
        TypeDao typeDao = new TypeDao();
        HotelDao hotelDao = new HotelDao();
        // GET: Adminservice
        public ActionResult Index(string msg)
        {
            ViewBag.Msg = msg;
            ViewBag.List = roomDao.GetRooms();
            ViewBag.listType = typeDao.GetTypes();
            ViewBag.listHotel = hotelDao.GetHotels();
            return View();
        }


        [HttpGet]
        public ActionResult GetRoomsByHotel(int idHotel)
        {
            var rooms = roomDao.GetRoomsByHotel(idHotel); // Giả sử bạn đã có phương thức này trong roomDao
            var roomDtos = rooms.Select(room => new RoomDto
            {
                IdRoom = room.idRoom,
                Name = room.name,
                HotelName =  room.Hotel.Name,
                Image = room.image,
                TypeName = room.Type != null ? room.Type.name : "",
                NumberAdult = room.numberAdult,
                NumberChildren = room.numberChildren,
                Cost = room.cost,
                Discount = room.discount,
                Description = room.description
            }).ToList(); 
            return Json(roomDtos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Room room)
        {
            var file = Request.Files["file"];
            string reName = DateTime.Now.Ticks.ToString() + file.FileName;
            file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            room.image = reName;
            room.view = 0;
            roomDao.add(room);
            return RedirectToAction("Index", new { msg = "1" });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Room room)
        {
            string reName = "";
            var objCourse = roomDao.GetDetail(room.idRoom);
            var file = Request.Files["file"];
            if (file.FileName == "")
            {
                reName = objCourse.image;
            }
            else
            {
                reName = DateTime.Now.Ticks.ToString() + file.FileName;
                file.SaveAs(Server.MapPath("~/Content/images/" + reName));
            }
            room.image = reName;
            roomDao.update(room);
            return RedirectToAction("Index", new { msg = "1" });
        }

        [HttpPost]
        public ActionResult Delete(Room room)
        {
            var check = roomDao.getCheck(room.idRoom);
            if (check.Count == 0)
            {
                roomDao.delete(room.idRoom);
                return RedirectToAction("Index", new { msg = "1" });
            }
            else
            {
                return RedirectToAction("Index", new { msg = "2" });
            }
        }
    }
}