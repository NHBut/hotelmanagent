using QuanLyKhachSan.Daos;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace QuanLyKhachSan.Controllers.Public
{
    public class PublicRoomController : Controller
    {
        RoomDao roomDao = new RoomDao();
        ServiceDao serviceDao = new ServiceDao();
        BookingDao bookingDao = new BookingDao();
        BookingServiceDao bookingServiceDao = new BookingServiceDao();
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        RoomCommentDao roomComment = new RoomCommentDao();
        // GET: PublicRoom
        public ActionResult Index(int page)
        {
            if (page == 0)
            {
                page = 1;
            }
            ViewBag.List = roomDao.GetRoomsBlank(page, 6);
            ViewBag.tag = page;
            ViewBag.pageSize = roomDao.GetNumberRoom();
     
            return View();
        }

        public ActionResult DetailRoom(int id,string mess)
        {
            ViewBag.mess = mess;
            ViewBag.listComment = roomComment.GetByIdRoom(id);
            ViewBag.Ave = roomComment.getAve(id);
            roomDao.updateView(id);
            Room obj = roomDao.GetDetail(id);
            ViewBag.Room = obj;
            ViewBag.ListService = serviceDao.GetServicesByRoomId(id);
            ViewBag.ListRoomRelated = roomDao.GetRoomByType(obj.idType);
            return View();
        }

        /*[HttpPost]
        public ActionResult Booking(Booking booking,int[] idService)
        {
            User user = (User)Session["USER"];
            string action = "DetailRoom/" + booking.idRoom;
            if (user == null)
            {              
                return RedirectToAction(action, new { mess = "ErrorLogin" });
            }
            else
            {
                Booking checkExist = bookingDao.CheckBooking(booking.idRoom);
                int priceService = 0;
                if (idService != null)
                {                 
                    for (int i = 0; i < idService.Count(); i++)
                    {

                        priceService += serviceDao.GetCostById(idService[i]);
                    }
                }
                
                if (checkExist == null || (checkExist != null && DateTime.Now > DateTime.Parse(checkExist.checkOutDate)))
                {
                    DateTime dateCheckout = DateTime.Parse(booking.checkOutDate);
                    DateTime dateCheckin = DateTime.Parse(booking.checkInDate);
                    int numberBooking = dateCheckout.Day - dateCheckin.Day;
                    Room room = roomDao.GetDetail(booking.idRoom);
                    booking.idUser = user.idUser;
                    booking.createdDate = DateTime.Now;
                    booking.isPayment = false;
                    booking.status = 0;
                    booking.totalMoney = (room.cost * numberBooking - room.cost * numberBooking * room.discount / 100) + priceService;
                    bookingDao.Add(booking);
                    if(idService != null)
                    {
                       for(int i = 0; i < idService.Count(); i++)
                       {
                            BookingService obj = new BookingService
                            {
                                idService = idService[i],
                                idBooking = booking.idBooking
                            };
                            bookingServiceDao.Add(obj);
                       }
                    }
                    return RedirectToAction(action, new { mess = "Success" });
                }
                else
                {
                    return RedirectToAction(action, new { mess = "ErrorExist" });
                }
            }
        }*/

        [HttpPost]
        public ActionResult Booking(Booking booking, int[] idService)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            QuanLyKhachSan.Models.User userInfomatiom = null;

            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userId = int.Parse(ticket.UserData); // Assuming ticket.Name contains the user ID
                userInfomatiom = new QuanLyKhachSan.Daos.UserDao().getInfor(userId); // Get user information from database by user ID
            }
            User user = userInfomatiom;
            string action = "DetailRoom/" + booking.idRoom;
            if (user == null)
            {
                return RedirectToAction(action, new { mess = "ErrorLogin" });
            }
            else
            {
                List<Booking> checkExist = bookingDao.CheckBook(booking.idRoom);
                int priceService = 0;
                if (idService != null)
                {
                    for (int i = 0; i < idService.Count(); i++)
                    {

                        priceService += serviceDao.GetCostById(idService[i]);
                    }
                }
                    if(checkExist.Count == 0)
                    {
                    DateTime dateCheckout = booking.checkOutDate;
                    DateTime dateCheckin =booking.checkInDate;
                    TimeSpan time = dateCheckout - dateCheckin;
                    int numberBooking = time.Days;
                    if(numberBooking <= 0)
                    {
                        return RedirectToAction(action, new { mess = "Error" });
                    }
                    Room room = roomDao.GetDetail(booking.idRoom);
                    booking.idUser = user.idUser;
                    booking.createdDate = DateTime.Now;
                    booking.isPayment = false;
                    booking.status = 0;
                    booking.totalMoney = (room.cost * numberBooking - room.cost * numberBooking * room.discount / 100) + priceService;
                    bookingDao.Add(booking);
                    if (idService != null)
                    {
                        for (int i = 0; i < idService.Count(); i++)
                        {
                            BookingService obj = new BookingService
                            {
                                idService = idService[i],
                                idBooking = booking.idBooking
                            };
                            bookingServiceDao.Add(obj);
                        }
                    }
                    return RedirectToAction(action, new { mess = "Success" });
                }
                else
                {
                    DateTime dateCheckout = booking.checkOutDate;
                    DateTime dateCheckin = booking.checkInDate;
                    foreach (Booking checkbooking in checkExist)
                    {
                        if((dateCheckin <= checkbooking.checkOutDate && dateCheckin >= checkbooking.checkInDate) || (dateCheckout <= checkbooking.checkOutDate && dateCheckout >= checkbooking.checkInDate))
                        {
                            return RedirectToAction(action, new { mess = "ErrorExist" });
                        }
                    }
                    TimeSpan time = dateCheckout - dateCheckin;
                    int numberBooking = time.Days;
                    if (numberBooking <= 0)
                    {
                        return RedirectToAction(action, new { mess = "Error" });
                    }
                    Room room = roomDao.GetDetail(booking.idRoom);
                    booking.idUser = user.idUser;
                    booking.createdDate = DateTime.Now;
                    booking.isPayment = false;
                    booking.status = 0;
                    booking.totalMoney = (room.cost * numberBooking - room.cost * numberBooking * room.discount / 100) + priceService;
                    bookingDao.Add(booking);
                    if (idService != null)
                    {
                        for (int i = 0; i < idService.Count(); i++)
                        {
                            BookingService obj = new BookingService
                            {
                                idService = idService[i],
                                idBooking = booking.idBooking
                            };
                            bookingServiceDao.Add(obj);
                        }
                    }
                    return RedirectToAction(action, new { mess = "Success" });
                }
            }
        }

        /*[HttpPost]
        public ActionResult Search(FormCollection form)
        {
            string cityId = form["CityId"];
            int idType = Int32.Parse(form["idType"]);
            int numberChildren = Int32.Parse(form["numberChildren"]);
            int numberAdult = Int32.Parse(form["numberAdult"]);
            return RedirectToAction("Search", new { page = 0, cityId = cityId, idType = idType, numberChildren = numberChildren,numberAdult = numberAdult });
        }*/

        [HttpPost]
        public ActionResult Search(FormCollection form)
        {
            int cityId = Int32.Parse(form["CityId"]);
            int idType = Int32.Parse(form["idType"]);
            int numberChildren = Int32.Parse(form["numberChildren"]);
            int numberAdult = Int32.Parse(form["numberAdult"]);
            return RedirectToAction("Search", new { page = 0, cityId = cityId, idType = idType, numberChildren = numberChildren, numberAdult = numberAdult });
        }
        public ActionResult Search(int page, int cityId, int idType, int numberChildren, int numberAdult)
        {
            if (idType == 0)
            {
                page = 1;
            }
            if (cityId == 0)
            {
                ViewBag.mess = "ErrorSearch";
                return View();
            }
            if (idType == null && cityId != 0)
            {
                ViewBag.List = roomDao.SearchByCity(page,3, cityId, numberChildren, numberAdult);

                ViewBag.tag = page;
                ViewBag.key = 1;
                ViewBag.idType = idType;
                ViewBag.numberChildren = numberChildren;
                ViewBag.numberAdult = numberAdult;
                ViewBag.pageSize = roomDao.GetNumberRoomByType(idType, numberChildren, numberAdult);
            }
           /* else if (name != null && idType == 0)
            {
                ViewBag.List = roomDao.SearchByName(page, 3, cityId, numberChildren, numberAdult);
                ViewBag.tag = page;
                ViewBag.key = 2;
                ViewBag.name = name;
                ViewBag.numberChildren = numberChildren;
                ViewBag.numberAdult = numberAdult;
                ViewBag.pageSize = roomDao.GetNumberRoomByName(name, numberChildren, numberAdult);
            }*/

            return View();
        }

        /*[HttpGet]
        public ActionResult Search(int page,string cityId , int idType,int numberChildren, int numberAdult)
        {
            if (page == 0)
            {
                page = 1;
            }
            if (name == null && idType != 0)
            {
                ViewBag.List = roomDao.SearchByType(page, 3, idType, numberChildren, numberAdult);
                ViewBag.tag = page;
                ViewBag.key = 1;
                ViewBag.idType = idType;
                ViewBag.numberChildren = numberChildren;
                ViewBag.numberAdult = numberAdult;
                ViewBag.pageSize = roomDao.GetNumberRoomByType(idType, numberChildren, numberAdult);
            }
            else if (name != null && idType == 0)
            {
                ViewBag.List = roomDao.SearchByName(page, 3, name, numberChildren, numberAdult);
                ViewBag.tag = page;
                ViewBag.key = 2;
                ViewBag.name = name;
                ViewBag.numberChildren = numberChildren;
                ViewBag.numberAdult = numberAdult;
                ViewBag.pageSize = roomDao.GetNumberRoomByName(name, numberChildren, numberAdult);
            }
            else if (name != null && idType != 0)
            {
                ViewBag.List = roomDao.SearchByTypeAndName(page, 3, idType, name, numberChildren, numberAdult);
                ViewBag.tag = page;
                ViewBag.key = 3;
                ViewBag.name = name;
                ViewBag.idType = idType;
                ViewBag.numberChildren = numberChildren;
                ViewBag.numberAdult = numberAdult;
                ViewBag.pageSize = roomDao.GetNumberRoomByNameAndType(name, idType, numberChildren, numberAdult);
            }
            else if (name == null && idType == 0)
            {
                List<Room> list = new List<Room>();
                ViewBag.List = list;
                RedirectToAction("Search", "PublicRoom");
            }
            return View();
        }*/

        [HttpPost]
        public JsonResult PostComment(string comment, int idRoom, int star)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            QuanLyKhachSan.Models.User userInfomatiom = null;

            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userId = int.Parse(ticket.UserData); // Assuming ticket.Name contains the user ID
                userInfomatiom = new QuanLyKhachSan.Daos.UserDao().getInfor(userId); // Get user information from database by user ID
            }
            User user = userInfomatiom;
            roomComment.Add(new RoomComment
            {
                createdDate = DateTime.Now,
                idRoom = idRoom,
                text = comment,
                idUser = user.idUser,
                star = star
            });
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}