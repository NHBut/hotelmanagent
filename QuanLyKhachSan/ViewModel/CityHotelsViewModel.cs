using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.ViewModel
{
    public class CityHotelsViewModel
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Image { get; set; }
        public List<Hotel> Hotels { get; set; }
    }
}   