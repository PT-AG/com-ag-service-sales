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

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Garment
{
    public class Garment_BudgetValidationPPICFacade : IGarment_BudgetValidationPPIC
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<CostCalculationGarment> DbSet;
        private IIdentityService IdentityService;
        private Garment_BudgetValidationPPICLogic RO_Garment_ValidationLogic;

        public Garment_BudgetValidationPPICFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<CostCalculationGarment>();
            this.IdentityService = serviceProvider.GetService<IIdentityService>();
            this.RO_Garment_ValidationLogic = serviceProvider.GetService<Garment_BudgetValidationPPICLogic>();
        }

        public async Task<int> ValidateROGarment(CostCalculationGarment CostCalculationGarment, Dictionary<long, string> productDicts)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
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
                   

                    foreach (var material in model.CostCalculationGarment_Materials)
                    {
                        var sentMaterial = CostCalculationGarment.CostCalculationGarment_Materials.FirstOrDefault(m => m.Id == material.Id);
                        if (sentMaterial != null)
                        {
                            material.IsPosted = true;
                            material.IsPRMaster = sentMaterial.IsPRMaster;
                            EntityExtension.FlagForUpdate(material, IdentityService.Username, "sales-service");
                        }
                    }
                    DbSet.Update(model);

                    Updated = await DbContext.SaveChangesAsync();

                    model.CostCalculationGarment_Materials = model.CostCalculationGarment_Materials
                        .Where(material => CostCalculationGarment.CostCalculationGarment_Materials.Any(oldMaterial => oldMaterial.Id == material.Id) && material.IsPRMaster == false).ToList();

                    string[] productProcess = { "PROCESS", "PROCESS SUBCON" };
                    if (model.CostCalculationGarment_Materials.Count > 0)
                    {
                        if (CostCalculationGarment.CostCalculationGarment_Materials.All(m => !productProcess.Contains(m.CategoryName.ToUpper())))
                        {
                            await RO_Garment_ValidationLogic.CreateGarmentPurchaseRequest(model, productDicts);
                        }
                        else if (CostCalculationGarment.CostCalculationGarment_Materials.All(m => productProcess.Contains(m.CategoryName.ToUpper())))
                        {
                            await RO_Garment_ValidationLogic.AddItemsGarmentPurchaseRequest(model, productDicts);
                        }
                        else
                        {
                            throw new Exception("Kategori Ada Proses dan Lainnnya");
                        }
                    }

                    transaction.Commit();
                }
                //catch (ServiceValidationException e)
                //{
                //    transaction.Rollback();
                //    throw new ServiceValidationException(e.Message, null, null);
                //}
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Updated;
        }
    }
}
