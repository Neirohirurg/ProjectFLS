using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    internal class Route
    {
        public int RouteID { get; set; }  // ID маршрута
        public int FromCityID { get; set; }  // ID города отправления
        public int ToCityID { get; set; }  // ID города назначения
        public string FromCityName { get; set; }  // Название города отправления
        public string ToCityName { get; set; }  // Название города назначения
        public double DistanceKm { get; set; }  // Расстояние в километрах
    }
}
