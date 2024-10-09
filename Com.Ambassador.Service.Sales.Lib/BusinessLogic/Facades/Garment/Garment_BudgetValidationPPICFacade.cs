using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.Garment;
using Com.Ambassador.Service.Sales.Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Garment;
using System.Linq;
using Com.Moonlay.Models;
using System.Threading.Tasks;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Models.PurchasingModel.GarmentPurchaseRequest;
using IValidateService = Com.Ambassador.Service.Sales.Lib.Services.IValidateService;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Garment
{
    public class Garment_BudgetValidationPPICFacade : IGarment_BudgetValidationPPIC
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<CostCalculationGarment> DbSet;
        private IIdentityService IdentityService;
        private Garment_BudgetValidationPPICLogic RO_Garment_ValidationLogic;

        private readonly PurchasingDbContext PurchasingDbContext;
        private IValidateService validateService;

        public Garment_BudgetValidationPPICFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<CostCalculationGarment>();
            this.IdentityService = serviceProvider.GetService<IIdentityService>();
            this.RO_Garment_ValidationLogic = serviceProvider.GetService<Garment_BudgetValidationPPICLogic>();

            PurchasingDbContext = serviceProvider.GetService<PurchasingDbContext>(); ;
            validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));
        }

        public async Task<int> ValidateROGarment(CostCalculationGarment CostCalculationGarment, Dictionary<long, string> productDicts)
        {
            int Updated = 0;
            using (var dbContext1 = DbContext)
            {
                using (var transaction = dbContext1.Database.BeginTransaction())
                {
                    using (var dbContext2 = PurchasingDbContext)
                    {
                        using (var transaction2 = dbContext2.Database.BeginTransaction())
                        {
                            try
                            {
                                var model = this.DbSet
                                    .Include(m => m.CostCalculationGarment_Materials)
                                    .FirstOrDefault(m => m.Id == CostCalculationGarment.Id);

                                EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
                                model.IsApprovedKadivMD = true;
                                model.ApprovedKadivMDDate = model.LastModifiedUtc;
                                model.ApprovedKadivMDBy = model.LastModifiedBy;
                                model.IsApprovedPPIC = true;
                                model.ApprovedPPICDate = model.LastModifiedUtc;
                                model.ApprovedPPICBy = model.LastModifiedBy;

                                foreach (var material in model.CostCalculationGarment_Materials)
                                {
                                    var sentMaterial = CostCalculationGarment.CostCalculationGarment_Materials.FirstOrDefault(m => m.Id == material.Id);
                                    if (sentMaterial != null)
                                    {
                                        material.IsPosted = true;
                                        material.IsPRMaster = sentMaterial.IsPRMaster;
                                        EntityExtension.FlagForUpdate(material, IdentityService.Username, "sales-service");
                                        //DbContext.CostCalculationGarment_Materials.Update(material);
                                    }
                                }
                                DbSet.Update(model);
                                Updated = await DbContext.SaveChangesAsync();


                                var listMaterial = model.CostCalculationGarment_Materials
                                    .Where(material => CostCalculationGarment.CostCalculationGarment_Materials.Any(oldMaterial => oldMaterial.Id == material.Id) && material.IsPRMaster == false).ToList();

                                //create alias from model to newModel
                                var newModel = new CostCalculationGarment()
                                {
                                    RO_Number = model.RO_Number,
                                    CreatedBy = model.CreatedBy,
                                    PreSCId = model.PreSCId,
                                    PreSCNo = model.PreSCNo,
                                    BuyerBrandId = model.BuyerBrandId,
                                    BuyerBrandCode = model.BuyerBrandCode,
                                    BuyerBrandName = model.BuyerBrandName,
                                    Article = model.Article,
                                    DeliveryDate = model.DeliveryDate,
                                    UnitId = model.UnitId,
                                    UnitCode = model.UnitCode,
                                    UnitName = model.UnitName,
                                    LastModifiedBy = model.LastModifiedBy,
                                    LastModifiedUtc = model.LastModifiedUtc,
                                    CostCalculationGarment_Materials = listMaterial
                                };

                                string[] productProcess = { "PROCESS", "PROCESS SUBCON" };
                                if (listMaterial.Count > 0)
                                {
                                    if (CostCalculationGarment.CostCalculationGarment_Materials.All(m => !productProcess.Contains(m.CategoryName.ToUpper())))
                                    {
                                        //Update Code 03-09-2024
                                        //Old Code
                                        //await RO_Garment_ValidationLogic.CreateGarmentPurchaseRequest(model, productDicts);

                                        //New Code
                                        // Create Garment Purchase Request
                                        var PRViewModel = RO_Garment_ValidationLogic.FillGarmentPurchaseRequest(newModel, productDicts);
                                        validateService.Validate(PRViewModel);

                                        var PRModel = AutoMapper.Mapper.Map<GarmentPurchaseRequests>(PRViewModel);
                                        PRModel.PRNo = $"PR{PRModel.RONo}";
                                        EntityExtension.FlagForCreate(PRModel, IdentityService.Username, "sales-service");

                                        if (PRModel.Items.Count(i => string.IsNullOrWhiteSpace(i.PO_SerialNumber)) > 0)
                                        {
                                            GeneratePOSerialNumber(PRModel, IdentityService.TimezoneOffset);
                                        }

                                        foreach (var item in PRModel.Items)
                                        {
                                            EntityExtension.FlagForCreate(item, IdentityService.Username, "sales-service");
                                            item.Status = "Belum diterima Pembelian";
                                        }

                                        PurchasingDbContext.GarmentPurchaseRequests.Add(PRModel);

                                        ////Add LogHistory Purchasing
                                        //LogHistory logHistory = new LogHistory
                                        //{
                                        //    Division = "PEMBELIAN",
                                        //    Activity = "Create PR Master - " + PRModel.PRNo,
                                        //    CreatedDate = DateTime.Now,
                                        //    CreatedBy = IdentityService.Username
                                        //};

                                        //PurchasingDbContext.LogHistories.Add(logHistory);

                                        Updated += await PurchasingDbContext.SaveChangesAsync();

                                        //Update IsPR in GarmentPreSalesContract
                                        var garmentSalesContract = DbContext.GarmentPreSalesContracts.FirstOrDefault(s => s.Id == PRModel.SCId);
                                        garmentSalesContract.IsPR = true;
                                        EntityExtension.FlagForUpdate(garmentSalesContract, IdentityService.Username, "sales-service");

                                        DbContext.GarmentPreSalesContracts.Update(garmentSalesContract);

                                        Updated += await DbContext.SaveChangesAsync();

                                    }
                                    else if (CostCalculationGarment.CostCalculationGarment_Materials.All(m => productProcess.Contains(m.CategoryName.ToUpper())))
                                    {

                                        //Update Code 03-09-2024
                                        //Old Code
                                        //await RO_Garment_ValidationLogic.AddItemsGarmentPurchaseRequest(model, productDicts);

                                        //New Code
                                        var PRViewModel = RO_Garment_ValidationLogic.FillGarmentPurchaseRequest(newModel, productDicts);
                                        validateService.Validate(PRViewModel);

                                        var newPR = AutoMapper.Mapper.Map<GarmentPurchaseRequests>(PRViewModel);

                                        var OldPR = PurchasingDbContext.GarmentPurchaseRequests.Include(x => x.Items).FirstOrDefault(s => s.RONo == newPR.RONo && s.IsDeleted == false);
                                        OldPR.IsUsed = false;
                                        if (newPR.Items.Count(i => i.Id == 0 && string.IsNullOrWhiteSpace(i.PO_SerialNumber)) > 0)
                                        {
                                            GeneratePOSerialNumber(newPR, IdentityService.TimezoneOffset);
                                        }

                                        //add new item to old PR
                                        foreach (var item in newPR.Items.Where(i => i.Id == 0))
                                        {
                                            EntityExtension.FlagForCreate(item, IdentityService.Username, "sales-service");
                                            item.Status = "Belum diterima Pembelian";
                                            OldPR.Items.Add(item);
                                        }

                                        EntityExtension.FlagForUpdate(OldPR, IdentityService.Username, "sales-service");
                                        PurchasingDbContext.GarmentPurchaseRequests.Update(OldPR);

                                        ////Add LogHistory Purchasing
                                        //LogHistory logHistory = new LogHistory
                                        //{
                                        //    Division = "PEMBELIAN",
                                        //    Activity = "Update PR Master - " + OldPR.PRNo,
                                        //    CreatedDate = DateTime.Now,
                                        //    CreatedBy = IdentityService.Username
                                        //};

                                        //PurchasingDbContext.LogHistories.Add(logHistory);

                                        Updated += await PurchasingDbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        throw new Exception("Kategori Ada Proses dan Lainnnya");
                                    }
                                }


                                transaction.Commit();
                                transaction2.Commit();
                            }
                            catch (Exception e)
                            {
                                // Rollback both transactions if any operation fails
                                transaction.Rollback();
                                transaction2.Rollback();
                                throw new Exception(e.Message);
                            }


                        }
                    }


                    //catch (ServiceValidationException e)
                    //{
                    //    transaction.Rollback();
                    //    throw new ServiceValidationException(e.Message, null, null);
                    //}

                }
            }
            return Updated;
        }

        private string GenerateRONo(GarmentPurchaseRequests m, int timeZone)
        {
            DateTimeOffset now = m.Date.ToOffset(new TimeSpan(timeZone, 0, 0));
            string y = now.ToString("yy");

            var unitCode = new List<string> { null, "AG" }.IndexOf(m.UnitCode);
            if (unitCode < 1)
            {
                throw new Exception("UnitCode format is invalid when Generate RONo");
            }

            var prefix = string.Concat(y, unitCode);
            var padding = 5;
            var suffix = string.Empty;

            if (m.PRType == "MASTER")
            {
                suffix = "M";
            }
            else if (m.PRType == "SAMPLE")
            {
                suffix = "S";
            }
            else if (m.PRType == "SUBCON")
            {
                suffix = "SC";
            }
            else
            {
                throw new Exception("PRType only accepting \"MASTER\" , \"SAMPLE\" and \"SUBCON\" in order to generate RONo.");
            }

            var lastRONo = PurchasingDbContext.GarmentPurchaseRequests.Where(w => !string.IsNullOrWhiteSpace(w.RONo) && w.RONo.Length == prefix.Length + padding + suffix.Length && w.RONo.StartsWith(prefix) && w.RONo.EndsWith(suffix))
                .OrderByDescending(o => o.RONo)
                .Select(s => int.Parse(s.RONo.Substring(prefix.Length, padding)))
                .FirstOrDefault();

            var RONo = $"{prefix}{(lastRONo + 1).ToString($"D{padding}")}{suffix}";

            return RONo;
        }



        private void GeneratePOSerialNumber(GarmentPurchaseRequests m, int timeZone)
        {
            DateTimeOffset now = m.Date.ToOffset(new TimeSpan(timeZone, 0, 0));
            string y = now.ToString("yy");

            var unitCode = new List<string> { null, "AG" }.IndexOf(m.UnitCode);
            if (unitCode < 1)
            {
                throw new Exception("UnitCode format is invalid when Generate POSerialnumber");
            }

            var prefix = string.Concat(y, unitCode);
            var padding = 6;
            var suffix = string.Empty;

            if (m.PRType == "MASTER")
            {
                suffix = "M";
            }
            else if (m.PRType == "SAMPLE")
            {
                suffix = "S";
            }
            else if (m.PRType == "SUBCON")
            {
                suffix = "SC";
            }
            else
            {
                throw new Exception("PRType only accepting \"MASTER\" , \"SAMPLE\" and \"SUBCON\" in order to generate POSerialNumber.");
            }

            var prefixPM = string.Concat("PM", prefix);
            var prefixPA = string.Concat("PA", prefix);
            int lasPM = 0, lasPA = 0;

            if (m.Items.Count(i => i.Id == 0 && i.CategoryName == "FABRIC") > 0)
            {
                lasPM = PurchasingDbContext.GarmentPurchaseRequestItems
                    .Where(w => !string.IsNullOrWhiteSpace(w.PO_SerialNumber) && w.PO_SerialNumber.Length == prefixPM.Length + padding + suffix.Length && w.PO_SerialNumber.StartsWith(prefixPM) && w.PO_SerialNumber.EndsWith(suffix))
                    .OrderByDescending(o => o.PO_SerialNumber)
                    .Select(s => int.Parse(s.PO_SerialNumber.Substring(prefixPM.Length, padding)))
                    .FirstOrDefault();
            }

            if (m.Items.Count(i => i.Id == 0 && i.CategoryName != "FABRIC") > 0)
            {
                lasPA = PurchasingDbContext.GarmentPurchaseRequestItems
                    .Where(w => !string.IsNullOrWhiteSpace(w.PO_SerialNumber) && w.PO_SerialNumber.Length == prefixPA.Length + padding + suffix.Length && w.PO_SerialNumber.StartsWith(prefixPA) && w.PO_SerialNumber.EndsWith(suffix))
                    .OrderByDescending(o => o.PO_SerialNumber)
                    .Select(s => int.Parse(s.PO_SerialNumber.Substring(prefixPA.Length, padding)))
                    .FirstOrDefault();
            }

            foreach (var item in m.Items.Where(i => i.Id == 0 && string.IsNullOrWhiteSpace(i.PO_SerialNumber)))
            {
                if (item.CategoryName == "FABRIC")
                {
                    item.PO_SerialNumber = $"{prefixPM}{(++lasPM).ToString($"D{padding}")}{suffix}";
                }
                else
                {
                    item.PO_SerialNumber = $"{prefixPA}{(++lasPA).ToString($"D{padding}")}{suffix}";
                }
            }
        }
    }
}
