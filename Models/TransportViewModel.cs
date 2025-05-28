using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    internal class TransportViewModel
    {
        public int transportID { get; set; }
        public string transportName { get; set; }
        public string TransportTypeName { get; set; }
        public string FuelTypeName { get; set; }
        public decimal maxLengthM { get; set; }
        public decimal maxWidthM { get; set; }
        public decimal maxHeightM { get; set; }
        public decimal consumptionLPer100Km { get; set; }
    }
}
