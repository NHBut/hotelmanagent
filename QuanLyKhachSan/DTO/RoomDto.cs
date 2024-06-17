using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.DTO
{
    public class RoomDto
    {
        public int IdRoom { get; set; }
        public string Name { get; set; }
        public string HotelName { get; set; }
        public string Image { get; set; }
        public string TypeName { get; set; }
        public int NumberAdult { get; set; }
        public int NumberChildren { get; set; }
        public int Cost { get; set; }
        public int Discount { get; set; }
        public string Description { get; set; } 
    }
}