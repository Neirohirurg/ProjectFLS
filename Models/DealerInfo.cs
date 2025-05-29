using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    internal class DealerInfo
    {
        public flsdbDataSet.dealersRow Dealer { get; set; }
        public string CityName { get; set; }
        public string ManagerName { get; set; }
    }
}
