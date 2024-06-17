using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Models
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int CityId { get; set; } 

        public virtual City City { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
