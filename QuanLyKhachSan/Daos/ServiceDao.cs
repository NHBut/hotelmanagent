using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;

namespace QuanLyKhachSan.Daos
{
    public class ServiceDao
    {
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();

        public List<Service> GetServicesByRoomId()
        {
            return myDb.services.ToList();
        }
        public List<Service> GetServicesByRoomId(int roomId)
        {
            // Lấy HotelId từ phòng với roomId
            var hotelId = myDb.rooms.Where(r => r.idRoom == roomId).Select(r => r.HotelId).FirstOrDefault();

            // Lọc danh sách các dịch vụ dựa trên HotelId
            return myDb.services.Where(s => s.HotelId == hotelId).ToList();
        }
        public List<Service> GetServicesByBookingId(int bookingId)
        {
            var hotelId = myDb.bookings
                        .Where(b => b.idBooking == bookingId)
                        .Select(b => b.Room.HotelId)
                        .FirstOrDefault();

            // Lọc danh sách các dịch vụ dựa trên HotelId
            return myDb.services.Where(s => s.HotelId == hotelId).ToList();
        }

        public List<Service> GetServicesTop5()
        {
            return myDb.services.Take(5).ToList();
        }

        public List<Service> GetServicesTop5(int hotelId)
        {
            return myDb.services.Where(s => s.HotelId == hotelId).Take(5).ToList();
        }
        public int GetCostById(int id)
        {
            return myDb.services.FirstOrDefault(x => x.idService == id).cost;
        }

        public void add(Service service)
        {
            myDb.services.Add(service);
            myDb.SaveChanges();
        }

        public void delete(int id)
        {
            var obj = myDb.services.FirstOrDefault(x => x.idService == id);
            myDb.services.Remove(obj);
            myDb.SaveChanges();
        }

        public void update(Service service)
        {
            var obj = myDb.services.FirstOrDefault(x => x.idService == service.idService);
            obj.name = service.name;
            obj.cost = service.cost;
            myDb.SaveChanges();
        }

        public Service GetServiceID(int id)
        {
            return myDb.services.FirstOrDefault(x => x.idService == id);
        }

        public List<BookingService> getCheck(int id)
        {
            return myDb.BookingServices.Where(x => x.idService == id).ToList();
        }

    }
}