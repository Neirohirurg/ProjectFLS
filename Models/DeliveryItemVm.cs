using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    public class DeliveryItemVm
    {
        public bool IsTractor { get; set; }   // true = трактор, false = запчасть
        public int? TractorID { get; set; }
        public int? PartID { get; set; }

        public string DeliveryTypeName { get; set; }
        public string ItemName { get; set; }  // model или partName

        public decimal LengthM { get; set; }
        public decimal WidthM { get; set; }
        public decimal HeightM { get; set; }
        public decimal? WeightKg { get; set; }
        public int? EnginePowerHP { get; set; }

        public int Quantity { get; set; }
    }
}
