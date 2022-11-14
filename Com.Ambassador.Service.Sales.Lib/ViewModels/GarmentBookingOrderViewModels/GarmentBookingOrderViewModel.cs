using Com.Ambassador.Service.Sales.Lib.Models.GarmentMasterPlan.WeeklyPlanModels;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels
{
    public class GarmentBookingOrderViewModel : BaseViewModel, IValidatableObject
    {
        public string BookingOrderNo { get; set; }
        public DateTimeOffset BookingOrderDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public long BuyerId { get; set; }
        public string BuyerCode { get; set; }
        public string BuyerName { get; set; }
        public long SectionId { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public double OrderQuantity { get; set; }
        public string Remark { get; set; }
        public bool IsBlockingPlan { get; set; }
        public bool IsCanceled { get; set; }
        public DateTimeOffset? CanceledDate { get; set; }
        public double CanceledQuantity { get; set; }
        public DateTimeOffset? ExpiredBookingDate { get; set; }
        public double ExpiredBookingQuantity { get; set; }
        public double ConfirmedQuantity { get; set; }
        public bool HadConfirmed { get; set; }
        public bool cancelConfirm { get; set; }
        public double maxWH { get; set; }
        public bool isUpdate { get; set; }
        public List<GarmentBookingOrderItemViewModel> Items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SalesDbContext dbContext = (SalesDbContext)validationContext.GetService(typeof(SalesDbContext));

            int clientTimeZoneOffset = 0;
            DateTimeOffset dt = DateTimeOffset.Now.AddDays(45);
            dt.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID"));
            if (SectionName == null)
                yield return new ValidationResult("Seksi harus diisi", new List<string> { "Section" });
            if (BuyerName == null)
                yield return new ValidationResult("Buyer harus diisi", new List<string> { "Buyer" });
            if (this.OrderQuantity <= 0)
                yield return new ValidationResult("Jumlah Order harus lebih besar dari 0", new List<string> { "OrderQuantity" });

            var expiredDate = dbContext.GarmentBookingOrders.FirstOrDefault(d => d.DeliveryDate != null && d.Id == 0);
            if (expiredDate == null && !cancelConfirm)
            {
                if (this.DeliveryDate == null || this.DeliveryDate == DateTimeOffset.MinValue)
                    yield return new ValidationResult("Tanggal Pengiriman harus diisi", new List<string> { "DeliveryDate" });
                else if (this.DeliveryDate < this.BookingOrderDate)
                    yield return new ValidationResult("Tanggal Pengiriman Harus lebih dari Tanggal Booking", new List<string> { "DeliveryDate" });
                else if (this.DeliveryDate < DateTimeOffset.Now.AddDays(45))
                    yield return new ValidationResult("Tanggal Pengiriman harus lebih dari 45 Hari (" + dt.ToOffset(new TimeSpan(clientTimeZoneOffset, 0, 0)).ToString("dd MMMM yyyy", new CultureInfo("id-ID")) + ")", new List<string> { "DeliveryDate" });
            }
            if (Id<=0 || isUpdate)
            {
                var dateDeliveryBook = DeliveryDate.Day;
                int yearBook = DeliveryDate.Year;
                int monthBook = DeliveryDate.Month;
                double averageWHBook = 0;
                double averageSKWHBook = 0;
                if (dateDeliveryBook < 6)
                {
                    var SKweeks = (from i in dbContext.GarmentWeeklyPlanItems
                                   join w in dbContext.GarmentWeeklyPlans on i.WeeklyPlanId equals w.Id
                                   where i.StartDate.Year == (monthBook == 1 ? yearBook - 1 : yearBook) && i.StartDate.Month == (monthBook == 1 ? 12 : monthBook - 1) && w.UnitCode == "SK"
                                   select i).ToList();
                    var weekly = dbContext.GarmentWeeklyPlans.Where(a => a.UnitCode != "SK");
                    var weeks = (from i in dbContext.GarmentWeeklyPlanItems
                                 join w in weekly on i.WeeklyPlanId equals w.Id
                                 where i.StartDate.Year == (monthBook == 1 ? yearBook-1 : yearBook) && i.StartDate.Month == (monthBook ==1 ? 12 : monthBook - 1)
                                 select i).ToList();
                    double wh = 0;
                    double SKwh = 0;
                    foreach (var sk in SKweeks)
                    {
                        SKwh += Math.Round(sk.WHConfirm, 2);
                    }
                    averageSKWHBook =SKwh / SKweeks.Count();
                    foreach (var w in weeks)
                    {
                        wh += Math.Round(w.WHConfirm, 2);
                    }
                    averageWHBook = Math.Round((wh / weeks.Count()), 2) + averageSKWHBook;
                }
                else
                {
                    var SKweeks = (from i in dbContext.GarmentWeeklyPlanItems
                                   join w in dbContext.GarmentWeeklyPlans on i.WeeklyPlanId equals w.Id
                                   where i.StartDate.Year == yearBook && i.StartDate.Month == monthBook && w.UnitCode == "SK"
                                   select i).ToList();
                    var weekly = dbContext.GarmentWeeklyPlans.Where(a => a.UnitCode != "SK");
                    var weeks = (from i in dbContext.GarmentWeeklyPlanItems
                                 join w in weekly on i.WeeklyPlanId equals w.Id
                                 where i.StartDate.Year == yearBook && i.StartDate.Month == monthBook
                                 select i).ToList();
                    double wh = 0;
                    double SKWh = 0;
                    foreach (var sk in SKweeks)
                    {
                        SKWh += Math.Round(sk.WHConfirm, 2);
                    }
                    averageSKWHBook = (SKWh / SKweeks.Count());
                    foreach (var w in weeks)
                    {
                        wh += Math.Round(w.WHConfirm, 2);
                    }
                    averageWHBook = (wh / weeks.Count()) + averageSKWHBook;
                }

                if (averageWHBook >= maxWH)
                {
                    yield return new ValidationResult("Tidak bisa simpan Boooking Order. WH Confirm sudah " + maxWH, new List<string> { "DeliveryDate" });
                }
            }
            

            if (Items != null)
            {
                int Count = 0;
                string ItemError = "[";

                //var maxWh = dbContext.MaxWHConfirms.OrderByDescending(a => a.CreatedUtc).First();
                foreach (GarmentBookingOrderItemViewModel item in Items)
                {
                    ItemError += "{";
                    if (string.IsNullOrWhiteSpace(item.ComodityName))
                    {
                        Count++;
                        ItemError += " Comodity: 'Komoditas harus diisi' , ";
                    }

                    if (item.ConfirmQuantity <= 0)
                    {
                        Count++;
                        ItemError += " ConfirmQuantity: 'Jumlah tidak boleh kurang dari 0' , ";
                    }

                    var totalQuantity = Items.Sum(s => s.ConfirmQuantity);
                    if (totalQuantity > this.OrderQuantity * 1.05)
                    {
                        Count++;
                        ItemError += $"ConfirmQuantity: 'Alllowance Total Confirm Quantity Max 5% dari Order Quantity ({this.OrderQuantity * 1.05})', ";
                    }

                    if (item.DeliveryDate == DateTimeOffset.MinValue || item.DeliveryDate == null)
                    {
                        Count++;
                        ItemError += " DeliveryDate: 'Tanggal Pengiriman Harus Diisi' , ";
                    }
                    else if (item.DeliveryDate > this.DeliveryDate && !item.IsCanceled)
                    {
                        Count++;
                        ItemError += " DeliveryDate: ' Tanggal Pengiriman tidak boleh lebih dari Tanggal Pengiriman Booking' , ";
                    }
                    else if (item.DeliveryDate <= this.BookingOrderDate)
                    {
                        Count++;
                        ItemError += " DeliveryDate: 'Tanggal Pengiriman Harus Lebih dari Tanggal Booking' , ";
                    }
                    bool cekTanggal = true;
                    if (item.Id > 0)
                    {
                        var book = dbContext.GarmentBookingOrderItems.AsNoTracking().FirstOrDefault(a => a.Id == item.Id);
                        if (item.DeliveryDate == book.DeliveryDate)
                        {
                            cekTanggal = false;
                        }
                    }
                    if (cekTanggal)
                    {
                        var dateDelivery = item.DeliveryDate.Day;
                        int year = item.DeliveryDate.Year;
                        int month = item.DeliveryDate.Month;
                        double averageWH = 0;
                        double averageSK = 0;
                        if (dateDelivery < 6)
                        {
                            var SKweeks = (from i in dbContext.GarmentWeeklyPlanItems
                                           join w in dbContext.GarmentWeeklyPlans on i.WeeklyPlanId equals w.Id
                                           where i.StartDate.Year == (month == 1 ? year-1 : year) && i.StartDate.Month == (month == 1 ? 12 : month - 1) && w.UnitCode == "SK"
                                           select i).ToList();
                            var weekly = dbContext.GarmentWeeklyPlans.Where(a => a.UnitCode != "SK");
                            var weeks = (from i in dbContext.GarmentWeeklyPlanItems
                                         join w in weekly on i.WeeklyPlanId equals w.Id
                                         where i.StartDate.Year == (month == 1 ? year - 1 : year) && i.StartDate.Month == (month == 1 ? 12 : month - 1)
                                         select i).ToList();
                            double wh = 0;
                            double SKwh = 0;
                            foreach (var sk in SKweeks)
                            {
                                SKwh += Math.Round(sk.WHConfirm, 2);
                            }
                            averageSK = (SKwh / SKweeks.Count());
                            foreach (var w in weeks)
                            {
                                wh += Math.Round(w.WHConfirm, 2);
                            }
                            averageWH = (wh / (weeks.Count())) + averageSK;
                        }
                        else
                        {
                            var SKweeks = (from i in dbContext.GarmentWeeklyPlanItems
                                           join w in dbContext.GarmentWeeklyPlans on i.WeeklyPlanId equals w.Id
                                           where i.StartDate.Year == year && i.StartDate.Month == month && w.UnitCode == "SK"
                                           select i).ToList();
                            var weekly = dbContext.GarmentWeeklyPlans.Where(a => a.UnitCode != "SK");
                            var weeks = (from i in dbContext.GarmentWeeklyPlanItems
                                         join w in weekly on i.WeeklyPlanId equals w.Id
                                         where i.StartDate.Year == year && i.StartDate.Month == month
                                         select i).ToList();
                            double wh = 0;
                            double SKwh = 0;
                            foreach (var sk in SKweeks)
                            {
                                SKwh += Math.Round(sk.WHConfirm, 2);
                            }
                            averageSK = (SKwh / SKweeks.Count());
                            foreach (var w in weeks)
                            {
                                wh += Math.Round(w.WHConfirm, 2);
                            }
                            averageWH = (wh / (weeks.Count())) + averageSK;
                        }
                        if (averageWH >= maxWH)
                        {
                            Count++;
                            ItemError += $" DeliveryDate: 'Tidak bisa simpan Booking Order. WH Confirm sudah {maxWH}' , ";
                        }
                    }
                    

                    
                    ItemError += "}, ";
                }

                ItemError += "]";

                if (Count > 0)
                {
                    yield return new ValidationResult(ItemError, new List<string> { "Items" });
                }
            }
        }
    }
}
