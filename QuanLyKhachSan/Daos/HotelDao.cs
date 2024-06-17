using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI.WebControls;

namespace QuanLyKhachSan.Daos
{
    public class HotelDao
    {
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        BookingDao bookingDao = new BookingDao();
        public List<Hotel> GetHotels()
        {
            return myDb.hotels.ToList();
        }
        public List<Room> getCheck(int id)
        {
            return myDb.rooms.Where(x => x.HotelId == id).ToList();
        }
        public int GetNumberHotel()
        {
            return myDb.hotels.Count();
        }
        public List<HotelViewModel> GetHotelsPage(int page, int pageSize)
        {
            var hotel = myDb.hotels
                             .OrderBy(x => x.HotelId)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var hotelViewModel = hotel.Select(x => new HotelViewModel
            {
                HotelId = x.HotelId,
                HotelName = x.Name,
                Image = x.ImageUrl,
                Address = x.Address,
                CityName = x.City.Name
            }).ToList();

            return hotelViewModel;
        }
        public List<Hotel> SearchByCriteria(int page, int cityId, int numberChildren, int numberAdult, DateTime checkInDate, DateTime checkOutDate)
        {
            int pageSize = 3; // Kích thước trang, bạn có thể thay đổi theo yêu cầu

            var availableRooms = myDb.rooms
                                    .Where(r => r.numberAdult >= numberAdult &&
                                                r.numberChildren >= numberChildren &&
                                                !myDb.bookings.Any(b => b.idRoom == r.idRoom &&
                                                                        ((b.checkInDate < checkOutDate &&  b.checkInDate >= checkInDate) ||
                                                                         (b.checkOutDate > checkInDate && b.checkOutDate <= checkOutDate) ||
                                                                         (b.checkInDate <= checkInDate && b.checkOutDate >= checkOutDate))))
                                    .Select(r => r.idRoom)
                                    .Distinct()
                                    .ToList();

            var hotels = myDb.hotels
                     .Join(myDb.rooms,
                           hotel => hotel.HotelId,
                           room => room.HotelId,
                           (hotel, room) => new { Hotel = hotel, Room = room })
                     .Where(hr => hr.Hotel.CityId == cityId &&
                                  availableRooms.Contains(hr.Room.idRoom))
                     .Select(hr => hr.Hotel)
                     .Distinct()
                     .OrderBy(h => h.HotelId) // Thêm OrderBy để sắp xếp dữ liệu
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();

            return hotels;
        }
        public List<Room> SearchRoom(int hotelId, int numberChildren, int numberAdult, DateTime checkInDate, DateTime checkOutDate)
        {

            /*var availableRooms = myDb.rooms
     .Where(r => r.HotelId == hotelId &&
                 r.numberAdult >= numberAdult &&
                 r.numberChildren >= numberChildren)
     .GroupJoin(
         myDb.bookings,
         room => room.idRoom,
         booking => booking.idRoom,
         (room, bookings) => new { room, bookings })
     .SelectMany(
         x => x.bookings.DefaultIfEmpty(),
         (x, booking) => new { x.room, booking })
     .Where(x => x.booking == null ||
                 ((x.booking.checkInDate < checkOutDate && x.booking.checkInDate >= checkInDate) ||
                  (x.booking.checkOutDate > checkInDate && x.booking.checkOutDate <= checkOutDate) ||
                  (x.booking.checkInDate <= checkInDate && x.booking.checkOutDate >= checkOutDate)))

     .Select(x => x.room)
     .OrderBy(r => r.idRoom)
     .Distinct().ToList();
     
            *//*var result = (from a in availableRooms join b in myDb.bookings on a.idRoom equals b.idRoom where b.status == 3
                          select a).ToList();
*//*
            return availableRooms;*/
            List<Room> availableRooms = new List<Room>();
            var listRoom = myDb.rooms.Where(x=>x.HotelId == hotelId && x.numberAdult>= numberAdult && x.numberChildren> numberChildren).ToList();

            foreach (var room in listRoom)
            {
                List<Booking> checkExist = bookingDao.CheckBook(room.idRoom);
                bool isAvailable = true;

                foreach (Booking booking in checkExist)
                {
                    if ((checkInDate < booking.checkOutDate && checkOutDate > booking.checkInDate))
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    availableRooms.Add(room);
                }
            }
            

            return availableRooms;

        }
            public int GetNumberRoom(int hotelId)
        {
            // Lấy danh sách các idRoom đã được đặt và có trạng thái 0 hoặc 1
            var arrIdRoom = myDb.bookings
                                .Where(x => (x.status == 0 || x.status == 1) && x.Room.HotelId == hotelId)
                                .Select(x => x.idRoom)
                                .Distinct()
                                .ToList();

            // Lấy danh sách tất cả các idRoom thuộc hotelId
            var allId = myDb.rooms
                            .Where(x => x.HotelId == hotelId)
                            .Select(x => x.idRoom)
                            .ToList();

            // Lấy danh sách các idRoom khả dụng
            var ids = allId.Except(arrIdRoom).ToList();

            // Tính tổng số phòng khả dụng
            int total = ids.Count;

            // Tính số trang dựa trên tổng số phòng khả dụng
            int count = total / 3;
            if (total % 3 != 0)
            {
                count++;
            }

            return count;
        }
        public void add(Hotel hotel)
        {
            myDb.hotels.Add(hotel);
            myDb.SaveChanges();
        }
        public Hotel GetDetail(int id)
        {
            return myDb.hotels.FirstOrDefault(x => x.HotelId == id);
        }
        public void delete(int id)
        {
            var obj = myDb.hotels.FirstOrDefault(x => x.HotelId == id);
            myDb.hotels.Remove(obj);
            myDb.SaveChanges();
        }

        public void update(Hotel hotel)
        {
            var obj = myDb.hotels.FirstOrDefault(x => x.HotelId == hotel.HotelId);
            obj.Name = hotel.Name;
            obj.ImageUrl = hotel.ImageUrl;
            obj.Description = hotel.Description;
            obj.PhoneNumber = hotel.PhoneNumber;
            obj.Address = hotel.Address;
            obj.CityId = hotel.CityId;

            myDb.SaveChanges();
        }
    }
}
public class HotelViewModel
{
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string Image { get; set; }
    public string Address { get; set; }
    public string CityName { get; set; }
}