using ProjectFLS.flsdbDataSetTableAdapters;
using ProjectFLS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFLS.Models
{
    public class WarehouseTractorData
    {
        private warehouseTractorsTableAdapter _warehouseTractorsAdapter;
        private tractorUnitsTableAdapter _tractorUnitsAdapter;

        public WarehouseTractorData()
        {
            _warehouseTractorsAdapter = new warehouseTractorsTableAdapter();
            _tractorUnitsAdapter = new tractorUnitsTableAdapter();
        }

        public async Task<List<WarehouseTractorInfo>> GetWarehouseTractorsInfoAsync(int warehouseID)
        {
            var warehouseTractors = await Task.Run(() => _warehouseTractorsAdapter.GetData().Where(w => w.warehouseID == warehouseID).ToList());
            var tractorUnits = await Task.Run(() => _tractorUnitsAdapter.GetData().ToList());

            var result = (from wt in warehouseTractors
                          join tu in tractorUnits on wt.tractorID equals tu.tractorID
                          select new WarehouseTractorInfo
                          {
                              WarehouseID = wt.warehouseID,
                              //TractorID = wt.tractorID,
                              Quantity = wt.quantity,
                              Model = tu.model,
                              LengthM = (double)tu.lengthM,
                              WidthM = (double)tu.widthM,
                              HeightM = (double)tu.heightM,
                              WeightKg = (double)tu.weightKg,
                              EnginePowerHP = tu.enginePowerHP
                          }).ToList();

            return result;
        }
    }
}
