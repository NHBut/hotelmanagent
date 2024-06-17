using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Daos
{
    public class BookingServiceDao
    {
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        public void Add(BookingService bookingService)
        {
            myDb.BookingServices.Add(bookingService);
            myDb.SaveChanges();
        }
        public void DeleteByBookingId(int id)
        {
            var services = myDb.BookingServices.Where(bs => bs.idBooking == id).ToList();
            myDb.BookingServices.RemoveRange(services);
            myDb.SaveChanges();

        }
        public void DeleteByServiceAndBookingId(int idService, int idBooking)
        {
            var bookingService = myDb.BookingServices
                .FirstOrDefault(bs => bs.idService == idService && bs.idBooking == idBooking);
            if (bookingService != null)
            {
                myDb.BookingServices.Remove(bookingService);
                myDb.SaveChanges();
            }
        }
    }
}