using System;
using System.Collections.Generic;

namespace ProjectFLS.Models
{
    // Main Delivery Info Model
    public class ActiveDeliveryInfo
    {
        // Delivery table fields
        public int DeliveryID { get; set; }
        public int DeliveryTypeID { get; set; }
        public int FromWarehouseID { get; set; }
        public int ToWarehouseID { get; set; }
        public int ToDealerID { get; set; }
        public int TransportID { get; set; }
        public int StatusID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // DeliveryType table fields
        public string DeliveryTypeName { get; set; }

        // Warehouse table fields
        public string FromWarehouseName { get; set; }
        public string ToWarehouseName { get; set; }

        // Dealer table fields
        public string ToDealerName { get; set; }

        // Transport table fields
        public string TransportName { get; set; }

        // Status table fields
        public string StatusName { get; set; }

        // Optionally add City and Manager info if needed
        public int? CityID { get; set; }  // Assuming you want to track CityID in case of City data
        public int? ManagerID { get; set; }  // Assuming you want to track ManagerID in case of Manager data

        // New related fields for deliveryItems
        public List<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();  // List of items in the delivery

        // New related fields for tractorUnits
        public List<TractorUnit> TractorUnits { get; set; } = new List<TractorUnit>();  // List of tractors in the delivery
    }

    // Delivery Item Model (new table: deliveryItems)
    public class DeliveryItem
    {
        public int ItemID { get; set; }
        public int DeliveryID { get; set; }
        public int TractorID { get; set; }
        public int PartID { get; set; }
        public int Quantity { get; set; }
    }

    // Tractor Unit Model (new table: tractorUnits)
    public class TractorUnit
    {
        public int TractorID { get; set; }
        public string Model { get; set; }
        public double LengthM { get; set; }
        public double WidthM { get; set; }
        public double HeightM { get; set; }
        public double WeightKg { get; set; }
        public double EnginePowerHP { get; set; }
    }
}
