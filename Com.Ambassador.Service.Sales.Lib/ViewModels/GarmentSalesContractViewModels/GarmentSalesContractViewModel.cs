using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentSalesContractFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentSalesContractInterface;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentSalesContractModel;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentSalesContractViewModels
{
    public class GarmentSalesContractViewModel : BaseViewModel, IValidatableObject
    {
        public string SalesContractNo { get; set; }
        public string SCType { get; set; }
        public string BuyerBrandId { get; set; }
        public string BuyerBrandName { get; set; }
        public string BuyerBrandCode { get; set; }

        public string Packing { get; set; }
        public string DocPresented { get; set; }
        public string FOB { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public string Delivery { get; set; }
        public string Country { get; set; }
        public string NoHS { get; set; }
        public bool IsMaterial { get; set; }
        public bool IsTrimming { get; set; }
        public bool IsEmbrodiary { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsWash { get; set; }
        public bool IsTTPayment { get; set; }
        public string PaymentDetail { get; set; }
        public AccountBankViewModel AccountBank { get; set; }
        public bool DocPrinted { get; set; }
        public double FreightCost { get; set; }
        public string PaymentMethod { get; set; }
        public string DownPayment { get; set; }
        public double LatePayment { get; set; }
        public int LateReturn { get; set; }
        public double? Claim { get; set; }

        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientJob { get; set; }

        public string BuyerType { get; set; }

        public List<GarmentSalesContractROViewModel> SalesContractROs { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SalesDbContext dbContext = (SalesDbContext)validationContext.GetService(typeof(SalesDbContext));
            //var duplicateRONo = dbContext.GarmentPurchaseRequests.Where(m => m.RONo.Equals(RONo) && m.Id != Id).Count();
            //IGarmentSalesContract Service = (IGarmentSalesContract)validationContext.GetService(typeof(IGarmentSalesContract));
            
            
            if (string.IsNullOrWhiteSpace(Delivery))
            {
                yield return new ValidationResult("Delivery harus diisi", new List<string> { "Delivery" });
            }

            if (AccountBank == null || AccountBank.Id.Equals(0))
            {
                yield return new ValidationResult("Bank Detail harus di isi", new List<string> { "AccountBankId" });
            }
            if (string.IsNullOrWhiteSpace(Packing))
            {
                yield return new ValidationResult("Packing harus diisi", new List<string> { "Packing" });
            }
            if (string.IsNullOrWhiteSpace(Country))
            {
                yield return new ValidationResult("Destination harus diisi", new List<string> { "Country" });
            }
            if (string.IsNullOrWhiteSpace(FOB))
            {
                yield return new ValidationResult("FOB harus diisi", new List<string> { "FOB" });
            }
            if (string.IsNullOrWhiteSpace(DocPresented))
            {
                yield return new ValidationResult("DocPresented harus diisi", new List<string> { "DocPresented" });
            }
            if (this.SCType == "Lokal")
            {
                if (string.IsNullOrWhiteSpace(RecipientName))
                {
                    yield return new ValidationResult("Nama Penerima harus diisi", new List<string> { "RecipientName" });
                }
                //if (string.IsNullOrWhiteSpace(RecipientJob))
                //{
                //    yield return new ValidationResult("Jabatan Penerima harus diisi", new List<string> { "RecipientJob" });
                //}
                if (string.IsNullOrWhiteSpace(RecipientAddress))
                {
                    yield return new ValidationResult("Alamat Penerima harus diisi", new List<string> { "RecipientAddress" });
                }
            }
            if (this.BuyerType == "Badan Hukum")
            {
                if (!string.IsNullOrWhiteSpace(RecipientJob))
                {
                    yield return new ValidationResult("Jabatan Penerima harus diisi", new List<string> { "RecipientJob" });
                }
            }
            
            
            if (this.LatePayment.Equals(null))
            {
                yield return new ValidationResult("Besar Denda harus diisi", new List<string> { "LatePayment" });
            }

            if (this.LateReturn.Equals(null))
            {
                yield return new ValidationResult("Hari Pengembalian harus diisi", new List<string> { "LateReturn" });
            }

            if (SalesContractROs.Count > 0)
            {
                int ROCount = 0;
                string ROError = "[";

                foreach (GarmentSalesContractROViewModel ro in SalesContractROs)
                {
                    ROError += "{";
                   
                    if (ro.CostCalculationId != 0)
                    {
                        var duplicateRONo = dbContext.GarmentSalesContractROs.Where(m => m.CostCalculationId.Equals(ro.CostCalculationId) && m.Id != ro.Id && m.IsDeleted == false).Count();
                        if (duplicateRONo > 0)
                        {
                            ROCount++;
                            ROError += " RONumber: 'Nomor RO sudah dibuat Sales Contract' , ";
                        }
                    }

                    if (string.IsNullOrWhiteSpace(ro.RONumber))
                    {
                        ROCount++;
                        ROError += " RONumber: 'Nomor RO harus diisi' , ";
                    }
                    if (string.IsNullOrWhiteSpace(ro.Material))
                    {
                        ROCount++;
                        ROError += " Material: 'Material harus diisi' , ";
                    }
                    if (ro.Price <= 0)
                    {
                        if (ro.Items.Count == 0)
                        {
                            ROCount++;
                            ROError += " Price: 'Harga harus diisi' , ";
                        }
                    }
                    if (ro.Items.Count > 0)
                    {
                        int Count = 0;
                        string ItemError = "[";
                        foreach (GarmentSalesContractItemViewModel item in ro.Items)
                        {
                            if (string.IsNullOrWhiteSpace(item.Description))
                            {
                                Count++;
                                ItemError += " Description: 'Keterangan harus diisi' , ";
                            }

                            if (item.Quantity <= 0)
                            {
                                Count++;
                                ItemError += " Quantity: 'Quantity harus lebih besar dari 0' , ";
                            }

                            if (item.Price <= 0)
                            {
                                Count++;
                                ItemError += " Price: 'Harga harus lebih besar dari 0' , ";
                            }
                            ItemError += "}, ";
                        }
                        ItemError += "]";
                    }

                    ROError += "}, ";
                }

                ROError += "]";

                if (ROCount > 0)
                {
                    yield return new ValidationResult(ROError, new List<string> { "SalesContractROs" });
                }
            }
        }
    }
}
