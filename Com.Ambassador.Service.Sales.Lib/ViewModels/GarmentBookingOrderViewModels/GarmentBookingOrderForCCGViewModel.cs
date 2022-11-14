using Com.Ambassador.Service.Sales.Lib.Models.GarmentMasterPlan.WeeklyPlanModels;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Com.Danliris.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels
{
    public class GarmentBookingOrderForCCGViewModel
    {
        public long BookingOrderId { get; set; }
        public long BookingOrderItemId { get; set; }
        public string BookingOrderNo { get; set; }
        public DateTimeOffset ConfirmDate { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public long SectionId { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public long ComodityId { get; set; }
        public string ComodityCode { get; set; }
        public string ComodityName { get; set; }
        public double ConfirmQuantity { get; set; }
        public double CCQuantity { get; set; }
    }
}
