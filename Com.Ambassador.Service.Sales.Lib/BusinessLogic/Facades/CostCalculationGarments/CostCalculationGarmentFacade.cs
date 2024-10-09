using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface;
using Com.Moonlay.Models;
using Microsoft.AspNetCore.JsonPatch;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment.Cancel_Approval;
using System.IO;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.CostCalculationGarments
{
    public class CostCalculationGarmentFacade : ICostCalculationGarment
	{
        private string USER_AGENT = "sales-service";

        private readonly SalesDbContext DbContext;
		private readonly DbSet<CostCalculationGarment> DbSet;
		private readonly IdentityService identityService;
		private readonly CostCalculationGarmentLogic costCalculationGarmentLogic ;
		public IServiceProvider ServiceProvider;

		public CostCalculationGarmentFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
		{
			DbContext = dbContext;
			DbSet = DbContext.Set<CostCalculationGarment>();
			identityService = serviceProvider.GetService<IdentityService>();
			costCalculationGarmentLogic = serviceProvider.GetService<CostCalculationGarmentLogic>();
			ServiceProvider = serviceProvider;
		}

		public async Task<CostCalculationGarment> CustomCodeGenerator(CostCalculationGarment Model)
		{
			List<string> convectionOption = new List<string> { "AG1", "AG2" };
			int convectionCode = convectionOption.IndexOf(Model.UnitCode) + 1;

			var lastData = await this.DbSet.Where(w => w.IsDeleted == false && w.UnitCode == Model.UnitCode).OrderByDescending(o => o.CreatedUtc).FirstOrDefaultAsync();

			DateTime Now = DateTime.Now;
			string Year = Now.ToString("yy");

			if (lastData == null)
			{
				Model.AutoIncrementNumber = 1;
				string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
				Model.RO_Number = $"AG{Year}{convectionCode.ToString()}{Number}";
			}
			else
			{
				if (lastData.CreatedUtc.Year < Now.Year)
				{
					Model.AutoIncrementNumber = 1;
					string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
					Model.RO_Number = $"AG{Year}{convectionCode.ToString()}{Number}";
				}
				else
				{
					Model.AutoIncrementNumber = lastData.AutoIncrementNumber + 1;
					string Number = Model.AutoIncrementNumber.ToString().PadLeft(4, '0');
					Model.RO_Number = $"AG{Year}{convectionCode.ToString()}{Number}";
				}
			}

			return Model;
		}
		public async Task<int> CreateAsync(CostCalculationGarment model)
		{
            int Created = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    do
                    {
                        model.Code = CodeGenerator.Generate();
                        await CustomCodeGenerator(model);
                    }
                    while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

                    costCalculationGarmentLogic.Create(model);
                    Created = await DbContext.SaveChangesAsync();

                    if (!string.IsNullOrWhiteSpace(model.ImageFile))
                    {
                        model.ImagePath = await this.AzureImageFacade.UploadImage(model.GetType().Name, model.Id, model.CreatedUtc, model.ImageFile);
                    }
                    await DbContext.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
            return Created;
		}

        public async Task<int> DeleteAsync(int id)
		{
            int Deleted = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    await costCalculationGarmentLogic.DeleteAsync(id);
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

		public ReadResponse<CostCalculationGarment> Read(int page, int size, string order, List<string> select, string keyword, string filter)
		{
			return costCalculationGarmentLogic.Read(page, size, order, select, keyword, filter);
		}
		private IAzureImageFacade AzureImageFacade
		{
			get { return this.ServiceProvider.GetService<IAzureImageFacade>(); }
		}
		public async Task<CostCalculationGarment> ReadByIdAsync(int id)
		{
			CostCalculationGarment read = await this.DbSet
			   .Where(d => d.Id.Equals(id) && d.IsDeleted.Equals(false))
			   .Include(d => d.CostCalculationGarment_Materials)
			   .FirstOrDefaultAsync();

            read.CostCalculationGarment_Materials = read.CostCalculationGarment_Materials.OrderBy(o => o.MaterialIndex).ToList();

            if (read.ImagePath != null)
            {
                read.ImageFile = await this.AzureImageFacade.DownloadImage(read.GetType().Name, read.ImagePath);
            }

            return read;
		}

		public async Task<int> UpdateAsync(int id, CostCalculationGarment model)
		{
            int Updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                     costCalculationGarmentLogic.UpdateAsync(id, model);

                    if (!string.IsNullOrWhiteSpace(model.ImageFile))
                    {
                        model.ImagePath = await this.AzureImageFacade.UploadImage(model.GetType().Name, model.Id, model.CreatedUtc, model.ImageFile);
                    }

                    ////Create Log History
                    //logHistoryLogic.Create("PENJUALAN", "Update Cost Calculation - " + model.RO_Number);

                    Updated =  await DbContext.SaveChangesAsync();
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

        public async Task<Dictionary<long, string>> GetProductNames(List<long> productIds)
        {
            return await costCalculationGarmentLogic.GetProductNames(productIds);
        }

        public ReadResponse<CostCalculationGarment> ReadForROAcceptance(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return costCalculationGarmentLogic.ReadForROAcceptance(page, size, order, select, keyword, filter);
        }

        public async Task<int> AcceptanceCC(List<long> listId, string user)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var listData = DbSet.
                        Where(w => listId.Contains(w.Id))
                        .ToList();

                    foreach (var data in listData)
                    {
                        EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                        data.IsROAccepted = true;
                        data.ROAcceptedDate = DateTimeOffset.Now;
                        data.ROAcceptedBy = user;
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    if (Updated < 1)
                    {
                        throw new Exception("No data updated");
                    }

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

        public ReadResponse<CostCalculationGarment> ReadForROAvailable(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return costCalculationGarmentLogic.ReadForROAvailable(page, size, order, select, keyword, filter);
        }

        public async Task<int> AvailableCC(List<long> listId, string user)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var listData = DbSet.
                        Where(w => listId.Contains(w.Id))
                        .ToList();

                    foreach (var data in listData)
                    {
                        EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                        data.IsROAvailable = true;
                        data.ROAvailableDate = DateTimeOffset.Now;
                        data.ROAvailableBy = user;
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    if (Updated < 1)
                    {
                        throw new Exception("No data updated");
                    }

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

        public ReadResponse<CostCalculationGarment> ReadForRODistribution(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return costCalculationGarmentLogic.ReadForRODistribution(page, size, order, select, keyword, filter);
        }

        public async Task<int> DistributeCC(List<long> listId, string user)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var listData = DbSet.
                        Where(w => listId.Contains(w.Id))
                        .ToList();

                    foreach (var data in listData)
                    {
                        EntityExtension.FlagForUpdate(data, user, USER_AGENT);
                        data.IsRODistributed = true;
                        data.RODistributionDate = DateTimeOffset.Now;
                        data.RODistributionBy = user;
                    }

                    Updated = await DbContext.SaveChangesAsync();

                    if (Updated < 1)
                    {
                        throw new Exception("No data updated");
                    }

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

        public async Task<int> Patch(long id, JsonPatchDocument<CostCalculationGarment> jsonPatch)
        {
            int Updated = 0;

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    costCalculationGarmentLogic.Patch(id, jsonPatch);
                    Updated = await DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return Updated;
        }

        public async Task<int> PostCC(List<long> listId)
        {
            int Updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    costCalculationGarmentLogic.PostCC(listId);
                    Updated = await DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return Updated;
        }

        public async Task<int> UnpostCC(long id, string reason)
        {
            int Updated = 0;
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    costCalculationGarmentLogic.UnpostCC(id);
                    costCalculationGarmentLogic.InsertUnpostReason(id, reason);
                    Updated = await DbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
            return Updated;
        }
		public List<CostCalculationGarmentDataProductionReport> GetComodityQtyOrderHoursBuyerByRo(string ro)
		{
			return costCalculationGarmentLogic.GetComodityQtyOrderHoursBuyerByRo(ro);

		}

        public List<CostCalculationGarmentForJournal> GetCCGByRo(string RO_Number)
        {
            return costCalculationGarmentLogic.GetCCGByRo(RO_Number);

        }
        public List<string> ReadUnpostReasonCreators(string keyword, int page, int size)
        {
            return costCalculationGarmentLogic.ReadUnpostReasonCreators(keyword, page, size);
        }

        public ReadResponse<dynamic> ReadDynamic(int page, int size, string order, string select, string keyword, string filter, string search)
        {
            return costCalculationGarmentLogic.ReadDynamic(page, size, order, select, keyword, filter, search);
        }

        public ReadResponse<dynamic> ReadMaterials(int page, int size, string order, string select, string keyword, string filter, string search)
        {
            CostCalculationGarmentMaterialLogic costCalculationGarmentMaterialLogic = ServiceProvider.GetService<CostCalculationGarmentMaterialLogic>();
            return costCalculationGarmentMaterialLogic.ReadMaterials(page, size, order, select, keyword, filter, search);
        }

        public ReadResponse<dynamic> ReadMaterialsByPRMasterItemIds(int page, int size, string order, string select, string keyword, string filter, string search, string prmasteritemids)
        {
            CostCalculationGarmentMaterialLogic costCalculationGarmentMaterialLogic = ServiceProvider.GetService<CostCalculationGarmentMaterialLogic>();
            return costCalculationGarmentMaterialLogic.ReadMaterialsByPRMasterItemIds(page, size, order, select, keyword, filter, search, prmasteritemids);
        }

        public async Task<CostCalculationGarment> ReadByRO(string ro)
        {
            CostCalculationGarment read = await this.DbSet
               .Where(d => d.RO_Number.Equals(ro) && d.IsDeleted.Equals(false))
               .Include(d => d.CostCalculationGarment_Materials)
               .FirstOrDefaultAsync();

            read.CostCalculationGarment_Materials = read.CostCalculationGarment_Materials.OrderBy(o => o.MaterialIndex).ToList();

            if (read.ImagePath != null)
            {
                read.ImageFile = await this.AzureImageFacade.DownloadImage(read.GetType().Name, read.ImagePath);
            }

            return read;
        }

        #region CancelApproval
        public ReadResponse<CostCalculationGarment> ReadForCancelApproval(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return costCalculationGarmentLogic.ReadForCancelApproval(page, size, order, select, keyword, filter);
        }

        public async Task<int> CancelApproval(long id, string deletedRemark)
        {
            return await costCalculationGarmentLogic.CancelApproval(id, deletedRemark);
        }

        public Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadCancelApproval(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset)
        {
            return costCalculationGarmentLogic.ReadCancelApproval(dateFrom, dateTo, page, size, offset);
        }

        public MemoryStream GenerateExcelCancelApproval(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            return costCalculationGarmentLogic.GenerateExcelCancelApproval(dateFrom, dateTo, offset);
        }

        #region Report Reject RO
        public Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset)
        {
            return costCalculationGarmentLogic.ReadRejectRO(dateFrom, dateTo, page, size, offset);
        }

        public MemoryStream GenerateExcelReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            return costCalculationGarmentLogic.GenerateExcelReadRejectRO(dateFrom, dateTo, offset);
        }
        #endregion
        #endregion
    }
}
