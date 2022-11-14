using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel
{
    public class GarmentBookingOrderItem : BaseModel
    {
        public virtual long BookingOrderId { get; set; }
        [ForeignKey("BookingOrderId")]
        public virtual GarmentBookingOrder GarmentBookingOrder { get; set; }

        public long ComodityId { get; set; }
        [MaxLength(25)]
        public string ComodityCode { get; set; }
        [MaxLength(100)]
        public string ComodityName { get; set; }
        public double ConfirmQuantity { get; set; }
        public double CCQuantity { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public DateTimeOffset ConfirmDate { get; set; }
        public string Remark { get; set; }
        public bool IsCanceled { get; set; }
        public DateTimeOffset CanceledDate { get; set; }

    }
}
