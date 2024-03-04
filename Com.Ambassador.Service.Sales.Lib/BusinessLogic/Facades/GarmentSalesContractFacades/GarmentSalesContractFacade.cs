using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentSalesContractInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentSalesContractLogics;
using Com.Ambassador.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentSalesContractModel;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel;
using Com.Ambassador.Service.Purchasing.Lib.Interfaces;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using System.Linq;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentSalesContractFacades
{
    public class GarmentSalesContractFacade : IGarmentSalesContract
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<GarmentSalesContract> DbSet;
        private readonly IdentityService identityService;
        private readonly GarmentSalesContractLogic garmentSalesContractLogic;
        private readonly ICostCalculationGarment costCalGarmentLogic;

        public GarmentSalesContractFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentSalesContract>();
            identityService = serviceProvider.GetService<IdentityService>();
            garmentSalesContractLogic = serviceProvider.GetService<GarmentSalesContractLogic>();
            costCalGarmentLogic = serviceProvider.GetService<ICostCalculationGarment>();
        }

        public async Task<int> CreateAsync(GarmentSalesContract model)
        {
            //int result = 0;

            //garmentSalesContractLogic.Create(model);
            //result =  await DbContext.SaveChangesAsync();

            //foreach (var ro in model.SalesContractROs)
            //{
            //    CostCalculationGarment costCal = await costCalGarmentLogic.ReadByIdAsync(ro.CostCalculationId);
            //    result += await UpdateCostCalAsync(costCal, (int)ro.Id);
            //}
            //return result;

            //do
            //{
            //    model.Code = CodeGenerator.Generate();
            //}
            //while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            int Created = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    garmentSalesContractLogic.Create(model);

                    ////Create Log History
                    //logHistoryLogic.Create("PENJUALAN", "Create Sales Contract - " + model.SalesContractNo);
                    await DbContext.SaveChangesAsync();

                    //Update CC
                    foreach (var ro in model.SalesContractROs)
                    {
                        CostCalculationGarment costCal = await costCalGarmentLogic.ReadByIdAsync(ro.CostCalculationId);
                        //Created += await UpdateCostCalAsync(costCal, (int)ro.Id);
                        costCal.SCGarmentId = (long)ro.Id;
                        await DbContext.SaveChangesAsync();
                    }

                    Created = await DbContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
            //return Created += await UpdateCostCalAsync(costCal, (int)model.Id);
        }

        public async Task<int> UpdateCostCalAsync(CostCalculationGarment costCalculationGarment, int Id)
        {
            costCalculationGarment.SCGarmentId = Id;
            int result = await costCalGarmentLogic.UpdateAsync((int)costCalculationGarment.Id, costCalculationGarment);

            return result += await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {

            int Deleted = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    GarmentSalesContract sc = await ReadByIdAsync(id);
                    foreach (var ro in sc.SalesContractROs)
                    {
                        CostCalculationGarment costCal = await DbContext.CostCalculationGarments.Include(cc => cc.CostCalculationGarment_Materials).FirstOrDefaultAsync(a => a.Id.Equals(ro.CostCalculationId));
                        costCal.SCGarmentId = null;
                        //await costCalGarmentLogic.UpdateAsync((int)ro.CostCalculationId, costCal);
                        await DbContext.SaveChangesAsync();
                    }

                    await garmentSalesContractLogic.DeleteAsync(id);
                    Deleted = await DbContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Deleted;
        }

        public ReadResponse<GarmentSalesContract> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return garmentSalesContractLogic.Read(page, size, order, select, keyword, filter);
        }

        public async Task<GarmentSalesContract> ReadByIdAsync(int id)
        {
            return await garmentSalesContractLogic.ReadByIdAsync(id);
        }

        public async Task<int> UpdateAsync(int id, GarmentSalesContract model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<int> costCalIds = new List<int>();
                    foreach (var newRO in model.SalesContractROs)
                    {
                        if (newRO.Id <= 0)
                        {
                            costCalIds.Add(newRO.CostCalculationId);
                        }
                    }

                    int result = 0;
                    garmentSalesContractLogic.UpdateAsync(id, model);
                    result = await DbContext.SaveChangesAsync();

                    if (costCalIds.Count > 0)
                    {
                        foreach (var cc in costCalIds)
                        {
                            var newRO = model.SalesContractROs.FirstOrDefault(a => a.CostCalculationId == cc);
                            CostCalculationGarment costCal = await costCalGarmentLogic.ReadByIdAsync(newRO.CostCalculationId);
                            //result += await UpdateCostCalAsync(costCal, (int)newRO.Id);
                            costCal.SCGarmentId = (long)newRO.Id;
                            await DbContext.SaveChangesAsync();
                        }
                    }

                    GarmentSalesContract scExist = await ReadByIdAsync(id);
                    foreach (var sc in scExist.SalesContractROs)
                    {
                        if (sc.Id > 0)
                        {
                            GarmentSalesContractRO existRO = model.SalesContractROs.FirstOrDefault(a => a.Id == sc.Id);
                            if (existRO == null || existRO.IsDeleted)
                            {
                                CostCalculationGarment costCal = await DbContext.CostCalculationGarments.Include(cc => cc.CostCalculationGarment_Materials).FirstOrDefaultAsync(a => a.Id.Equals(sc.CostCalculationId));
                                costCal.SCGarmentId = null;
                                await DbContext.SaveChangesAsync();
                            }
                        }
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Updated;
        }

        public async Task<int> UpdatePrinted(int id, GarmentSalesContract model)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    model.DocPrinted = true;
                    DbSet.Update(model);
                    Updated = await DbContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }

            }
            return Updated;
        }

        public GarmentSalesContract ReadByCostCal(int id)
        {
            return garmentSalesContractLogic.ReadByCostCal(id);
        }

        public GarmentSalesContract ReadByRO(string ro)
        {
            return garmentSalesContractLogic.ReadByRO(ro);
        }
    }
}
