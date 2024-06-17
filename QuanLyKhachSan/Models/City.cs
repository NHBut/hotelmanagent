using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        // Các trường dữ liệu khác cho thành phố
        public virtual ICollection<Hotel> Hotels { get; set; }
    }
}