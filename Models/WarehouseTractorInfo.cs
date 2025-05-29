using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    internal class WarehouseTractorInfo
    {
        public int WarehouseID { get; set; }

        //public int TractorID { get; set; }
        public int Quantity { get; set; }
        public string Model { get; set; }
        public double LengthM { get; set; }
        public double WidthM { get; set; }
        public double HeightM { get; set; }
        public double WeightKg { get; set; }
        public int EnginePowerHP { get; set; }
    }
}
