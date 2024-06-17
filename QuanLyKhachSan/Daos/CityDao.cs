using Microsoft.Ajax.Utilities;
using QuanLyKhachSan.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace QuanLyKhachSan.Daos
{
    public class CityDao
    {
        QuanLyKhachSanDBContext myDb = new QuanLyKhachSanDBContext();
        public List<City> GetCities()
        {
            return myDb.cities.ToList();
        }

        public List<CityViewModel> GetCitiesPage(int page, int pageSize)
        {
            var cities = myDb.cities
                             .OrderBy(x => x.CityId)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();
            
            var cityViewModels = cities.Select(city => new CityViewModel
            {
                CityId = city.CityId,
                CityName = city.Name,
                NumberOfHotels = myDb.hotels.Count(h => h.CityId == city.CityId),
                Image = city.Image
            }).ToList();

            return cityViewModels;
        }


        public int GetNumberCity()
        {
            return myDb.cities.Count();
        }
        public List<Hotel> getCheck(int id)
        {
            return myDb.hotels.Where(x => x.CityId == id).ToList();
        }

        public void add(City city)
        {
            myDb.cities.Add(city);
            myDb.SaveChanges();
        }
        public City GetDetail(int id)
        {
            return myDb.cities.FirstOrDefault(x => x.CityId == id);
        }
        public void delete(int id)
        {
            var obj = myDb.cities.FirstOrDefault(x => x.CityId == id);
            myDb.cities.Remove(obj);
            myDb.SaveChanges();
        }

        public void update(City city)
        {
            var obj = myDb.cities.FirstOrDefault(x => x.CityId == city.CityId);
            obj.Name = city.Name;
            obj.Image = city.Image;

            myDb.SaveChanges();
        }
    }
}
public class CityViewModel
{
    public int CityId { get; set; }
    public string CityName { get; set; }
    public int NumberOfHotels { get; set; }
    // Bạn có thể thêm thuộc tính đường dẫn hình ảnh nếu cần, ví dụ:
    public string Image { get; set; }
}