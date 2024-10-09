using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment.Cancel_Approval;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.CostCalculationGarments
{
    public class CostCalculationGarmentLogic : BaseLogic<CostCalculationGarment>
	{ 
		private CostCalculationGarmentMaterialLogic costCalculationGarmentMaterialLogic;

		private readonly SalesDbContext DbContext;
        private readonly PurchasingDbContext PurchasingDbContext;

        private readonly LogHistoryLogic logHistoryLogic;
        public CostCalculationGarmentLogic(CostCalculationGarmentMaterialLogic costCalculationGarmentMaterialLogic, IServiceProvider serviceProvider, IIdentityService identityService, SalesDbContext dbContext) : base(identityService, serviceProvider, dbContext)
		{
			this.costCalculationGarmentMaterialLogic = costCalculationGarmentMaterialLogic;
			this.DbContext = dbContext;
            this.PurchasingDbContext = serviceProvider.GetService<PurchasingDbContext>();
          
            logHistoryLogic = serviceProvider.GetService<LogHistoryLogic>();
        }

		public override ReadResponse<CostCalculationGarment> Read(int page, int size, string order, List<string> select, string keyword, string filter)
		{
			IQueryable<CostCalculationGarment> Query = DbSet;
            var a = Query.Where(x => x.RO_Number == "AG2420007");
			List<string> SearchAttributes = new List<string>()
			{
                "PreSCNo", "RO_Number","Article","UnitName"
			};

			Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);

            //var checkAllUser = false;

            //if (FilterDictionary.ContainsKey("AllUser"))
            //{
            //    try
            //    {
            //        checkAllUser = (bool)FilterDictionary.GetValueOrDefault("AllUser");
            //    }
            //    catch (Exception) { }
            //    FilterDictionary.Remove("AllUser");
            //}

            //if (!checkAllUser)
            //{
            //    Query = Query.Where(w => w.CreatedBy == IdentityService.Username);
            //}

            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                  "Id", "Code", "PreSCNo", "RO_Number", "Quantity", "ConfirmPrice", "Article", "Unit", "LastModifiedUtc","UnitName",
                  "Comodity", "UOM", "Buyer", "DeliveryDate", "BuyerBrand", "ApprovalMD", "ApprovalPurchasing", "ApprovalIE", "ApprovalKadivMD", "ApprovalPPIC",
                  "IsPosted","SectionName","CreatedBy","Section","CommodityDescription"
            };

            Query = Query
                 .Select(ccg => new CostCalculationGarment
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     UnitId = ccg.UnitId,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     ConfirmPrice = ccg.ConfirmPrice,
                     BuyerCode = ccg.BuyerCode,
                     BuyerId = ccg.BuyerId,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandId = ccg.BuyerBrandId,
                     BuyerBrandName = ccg.BuyerBrandName,
                     Commodity = ccg.Commodity,
                     ComodityCode = ccg.ComodityCode,
                     CommodityDescription = ccg.CommodityDescription,
                     ComodityID = ccg.ComodityID,
                     DeliveryDate = ccg.DeliveryDate,
                     UOMCode = ccg.UOMCode,
                     UOMID = ccg.UOMID,
                     UOMUnit = ccg.UOMUnit,

                     PreSCNo = ccg.PreSCNo,

                     IsApprovedMD = ccg.IsApprovedMD,
                     IsApprovedPurchasing = ccg.IsApprovedPurchasing,
                     IsApprovedIE = ccg.IsApprovedIE,
                     IsApprovedKadivMD = ccg.IsApprovedKadivMD,
                     IsApprovedPPIC = ccg.IsApprovedPPIC,

                     IsPosted = ccg.IsPosted,

                     LastModifiedUtc = ccg.LastModifiedUtc,
                     SectionName = ccg.SectionName,
                     Section = ccg.Section,
                     CreatedBy = ccg.CreatedBy
                 });

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
			Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);

			Pageable<CostCalculationGarment> pageable = new Pageable<CostCalculationGarment>(Query, page - 1, size);
			List<CostCalculationGarment> data = pageable.Data.ToList<CostCalculationGarment>();
			int totalData = pageable.TotalCount;

			return new ReadResponse<CostCalculationGarment>(data, totalData, OrderDictionary, SelectedFields);
		}

		 
		public override void Create(CostCalculationGarment model)
		{
			GeneratePONumbers(model);
			foreach (var detail in model.CostCalculationGarment_Materials)
			{
				costCalculationGarmentMaterialLogic.Create(detail);
				//EntityExtension.FlagForCreate(detail, IdentityService.Username, "sales-service");
			}

            var preSC = DbContext.GarmentPreSalesContracts.Single(w => w.Id == model.PreSCId);
            preSC.IsCC = true;

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
			DbSet.Add(model);
		}
	 
		private void GeneratePONumbers(CostCalculationGarment model)
		{
			int lastFabricNumber = GetLastMaterialFabricNumberByCategoryName(model.UnitCode);
			int lastNonFabricNumber = GetLastMaterialNonFabricNumberByCategoryName(model.UnitCode);
            List<string> convectionOption = new List<string> { "AG1", "AG2" };
            int convectionCode = convectionOption.IndexOf(model.UnitCode) + 1;

            DateTime Now = DateTime.Now;
			string Year = Now.ToString("yy");

			foreach (CostCalculationGarment_Material item in model.CostCalculationGarment_Materials)
			{
				if (string.IsNullOrWhiteSpace(item.PO_SerialNumber))
				{
					string Number = "";
					if (item.CategoryName.ToUpper().Equals("FABRIC"))
					{
						lastFabricNumber += 1;
						Number = lastFabricNumber.ToString().PadLeft(5, '0');
						item.PO_SerialNumber = $"PM{Year}{convectionCode}{Number}";
						item.AutoIncrementNumber = lastFabricNumber;
					}
					else
					{
						lastNonFabricNumber += 1;
						Number = lastNonFabricNumber.ToString().PadLeft(5, '0');
						item.PO_SerialNumber = $"PA{Year}{convectionCode}{Number}";
						item.AutoIncrementNumber = lastNonFabricNumber;
					}
				}
			}
		}
		
		private int GetLastMaterialNonFabricNumberByCategoryName(string convection)
		{
			var result = (from a in DbContext.CostCalculationGarments
						  join b in DbContext.CostCalculationGarment_Materials on a.Id equals b.CostCalculationGarmentId
													  where !b.CategoryName.ToUpper().Equals("FABRIC") && a.UnitCode.Equals(convection)
													  select b).AsNoTracking()
													 .OrderByDescending(o => o.CreatedUtc.Year).ThenByDescending(t => t.AutoIncrementNumber).FirstOrDefault();
			return result == null ? 0 : result.AutoIncrementNumber;
		}

		private int GetLastMaterialFabricNumberByCategoryName(string convection)
		{
			var result = (from a in DbContext.CostCalculationGarments
						  join b in DbContext.CostCalculationGarment_Materials on a.Id equals b.CostCalculationGarmentId
													  where b.CategoryName.ToUpper()=="FABRIC" && a.UnitCode==(convection)
													  select b).AsNoTracking().OrderByDescending(o => o.CreatedUtc.Year).ThenByDescending(t => t.AutoIncrementNumber).FirstOrDefault();
			return result == null ? 0 : result.AutoIncrementNumber;
		}

		public override void UpdateAsync(long id, CostCalculationGarment model)
		{
            GeneratePONumbers(model);
            if (model.CostCalculationGarment_Materials != null)
			{
				HashSet<long> detailIds = costCalculationGarmentMaterialLogic.GetCostCalculationIds(id);
				foreach (var itemId in detailIds)
				{
					CostCalculationGarment_Material data = model.CostCalculationGarment_Materials.FirstOrDefault(prop => prop.Id.Equals(itemId));
                    if (data == null)
                    {
                        CostCalculationGarment_Material dataItem = DbContext.CostCalculationGarment_Materials.FirstOrDefault(prop => prop.Id.Equals(itemId));
                        EntityExtension.FlagForDelete(dataItem, IdentityService.Username, "sales-service");
                        //await costCalculationGarmentMaterialLogic.DeleteAsync(itemId);
                    }
                    else
                    {
                        costCalculationGarmentMaterialLogic.UpdateAsync(itemId, data);
                    }

					foreach (CostCalculationGarment_Material item in model.CostCalculationGarment_Materials)
					{
						if (item.Id <= 0)
							costCalculationGarmentMaterialLogic.Create(item);
					}
				}
			}

			EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
			DbSet.Update(model);
		}

        public override async Task DeleteAsync(long id)
        {
            var model = await DbSet.Include(d => d.CostCalculationGarment_Materials).FirstOrDefaultAsync(d => d.Id == id);
            EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service", true);
            DbSet.Update(model);

            var countPreSCinOtherCC = DbSet.Count(c => c.Id != id && c.PreSCId == model.PreSCId);
            if (countPreSCinOtherCC == 0)
            {
                var preSC = DbContext.GarmentPreSalesContracts.Single(w => w.Id == model.PreSCId);
                preSC.IsCC = false;
            }

            foreach (var material in model.CostCalculationGarment_Materials)
            {
                await costCalculationGarmentMaterialLogic.DeleteAsync(material.Id);
            }
        }

        public async Task<Dictionary<long, string>> GetProductNames(List<long> productIds)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, IdentityService.Token);

            var param = "";
            foreach (var id in productIds)
            {
                param = string.Concat(param, $"garmentProductList[]={id}&");
            }
            param = param.Trim('&');

            var httpResponseMessage = await httpClient.GetAsync($@"{APIEndpoint.Core}master/garmentProducts/byId?{param}");

            if (httpResponseMessage.StatusCode.Equals(HttpStatusCode.OK))
            {
                var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> resultDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);

                var data = resultDict.SingleOrDefault(p => p.Key.Equals("data")).Value;

                List<Dictionary<string, object>> dataDicts = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data.ToString());

                var productDicts = new Dictionary<long, string>();

                foreach (var dataDict in dataDicts)
                {
                    var Id = dataDict["Id"] != null ? Convert.ToInt64(dataDict["Id"].ToString()) : 0;
                    var Name = dataDict["Name"] != null ? dataDict["Name"].ToString() : "";

                    productDicts.Add(Id, Name);
                }

                return productDicts;
            }
            else
            {
                return new Dictionary<long, string>();
            }
        }

        internal void Patch(long id, JsonPatchDocument<CostCalculationGarment> jsonPatch)
        {
            var data = DbSet.Where(d => d.Id == id)
                .Single();

            EntityExtension.FlagForUpdate(data, IdentityService.Username, "sales-service");

            jsonPatch.ApplyTo(data);

            //Delete isRejected if validated by RO Sample
            if (data.IsValidatedROPPIC)
            {
                RO_Garment ROGarment = DbContext.RO_Garments.FirstOrDefault(x => x.CostCalculationGarmentId == id);

                ROGarment.IsRejected = false;
                ROGarment.RejectReason = null;

                EntityExtension.FlagForUpdate(ROGarment, IdentityService.Username, "sales-service");
            }
        }

        public ReadResponse<CostCalculationGarment> ReadForROAcceptance(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<CostCalculationGarment> Query = DbSet;
            List<string> SearchAttributes = new List<string>()
            {
                "Section", "RO_Number","Article","UnitName",
            };

            Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);
            List<string> SelectedFields = new List<string>()
            {
                  "Id", "Code", "PreSCNo", "RO_Number", "Quantity", "ConfirmPrice", "Article", "Unit", "LastModifiedUtc","UnitName",
                    "Comodity", "UOM", "Buyer", "DeliveryDate", "BuyerBrand", "Section", "IsROAccepted", "ROAcceptedBy", "ROAcceptedDate"
            };

            Query = Query
                 .Select(ccg => new CostCalculationGarment
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     UnitId = ccg.UnitId,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     ConfirmPrice = ccg.ConfirmPrice,
                     BuyerCode = ccg.BuyerCode,
                     BuyerId = ccg.BuyerId,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandId = ccg.BuyerBrandId,
                     BuyerBrandName = ccg.BuyerBrandName,
                     Commodity = ccg.Commodity,
                     ComodityCode = ccg.ComodityCode,
                     CommodityDescription = ccg.CommodityDescription,
                     ComodityID = ccg.ComodityID,
                     DeliveryDate = ccg.DeliveryDate,
                     UOMCode = ccg.UOMCode,
                     UOMID = ccg.UOMID,
                     UOMUnit = ccg.UOMUnit,
                     LastModifiedUtc = ccg.LastModifiedUtc,
                     CostCalculationGarment_Materials = ccg.CostCalculationGarment_Materials,

                     PreSCNo = ccg.PreSCNo,
                     Section = ccg.Section,
                     IsROAccepted = ccg.IsROAccepted,
                     ROAcceptedBy = ccg.ROAcceptedBy,
                     ROAcceptedDate = ccg.ROAcceptedDate,
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);
            Pageable<CostCalculationGarment> pageable = new Pageable<CostCalculationGarment>(Query, page - 1, size);
            List<CostCalculationGarment> data = pageable.Data.ToList<CostCalculationGarment>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<CostCalculationGarment>(data, totalData, OrderDictionary, SelectedFields);
        }

        internal ReadResponse<dynamic> ReadDynamic(int page, int size, string order, string select, string keyword, string filter, string search)
        {
            IQueryable<CostCalculationGarment> Query = DbSet;

            List<string> SearchAttributes = JsonConvert.DeserializeObject<List<string>>(search);
            if (SearchAttributes.Count < 1)
            {
                SearchAttributes = new List<string>() { "Code", "RO_Number", "Article" };
            }
            Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);

            IQueryable SelectedQuery = Query;
            if (!string.IsNullOrWhiteSpace(select))
            {
                SelectedQuery = QueryHelper<CostCalculationGarment>.Select(Query, select);
            }

            int totalData = SelectedQuery.Count();

            List<dynamic> Data = SelectedQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToDynamicList();

            return new ReadResponse<dynamic>(Data, totalData, OrderDictionary, new List<string>());
        }

        public ReadResponse<CostCalculationGarment> ReadForROAvailable(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<CostCalculationGarment> Query = DbSet;
            List<string> SearchAttributes = new List<string>()
            {
                "Section", "RO_Number","Article","UnitName",
            };

            Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);
            List<string> SelectedFields = new List<string>()
            {
                  "Id", "Code", "PreSCNo", "RO_Number", "Quantity", "ConfirmPrice", "Article", "Unit", "LastModifiedUtc","UnitName",
                    "Comodity", "UOM", "Buyer", "DeliveryDate", "BuyerBrand", "Section", "IsROAvailable", "ROAvailableBy", "ROAvailableDate"
            };

            Query = Query
                 .Select(ccg => new CostCalculationGarment
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     UnitId = ccg.UnitId,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     ConfirmPrice = ccg.ConfirmPrice,
                     BuyerCode = ccg.BuyerCode,
                     BuyerId = ccg.BuyerId,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandId = ccg.BuyerBrandId,
                     BuyerBrandName = ccg.BuyerBrandName,
                     Commodity = ccg.Commodity,
                     ComodityCode = ccg.ComodityCode,
                     CommodityDescription = ccg.CommodityDescription,
                     ComodityID = ccg.ComodityID,
                     DeliveryDate = ccg.DeliveryDate,
                     UOMCode = ccg.UOMCode,
                     UOMID = ccg.UOMID,
                     UOMUnit = ccg.UOMUnit,
                     LastModifiedUtc = ccg.LastModifiedUtc,
                     CostCalculationGarment_Materials = ccg.CostCalculationGarment_Materials,

                     PreSCNo = ccg.PreSCNo,
                     Section = ccg.Section,
                     IsROAvailable = ccg.IsROAvailable,
                     ROAvailableBy = ccg.ROAvailableBy,
                     ROAvailableDate = ccg.ROAvailableDate,
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);
            Pageable<CostCalculationGarment> pageable = new Pageable<CostCalculationGarment>(Query, page - 1, size);
            List<CostCalculationGarment> data = pageable.Data.ToList<CostCalculationGarment>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<CostCalculationGarment>(data, totalData, OrderDictionary, SelectedFields);
        }

        public ReadResponse<CostCalculationGarment> ReadForRODistribution(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<CostCalculationGarment> Query = DbSet;
            List<string> SearchAttributes = new List<string>()
            {
                "Section", "RO_Number","Article","UnitName",
            };

            Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);
            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);
            List<string> SelectedFields = new List<string>()
            {
                  "Id", "Code", "PreSCNo", "RO_Number", "Quantity", "ConfirmPrice", "Article", "Unit", "LastModifiedUtc","UnitName",
                    "Comodity", "UOM", "Buyer", "DeliveryDate", "BuyerBrand", "Section", "IsRODistributed", "RODistributionBy", "RODistributionDate"
            };

            Query = Query
                 .Select(ccg => new CostCalculationGarment
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     UnitId = ccg.UnitId,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     ConfirmPrice = ccg.ConfirmPrice,
                     BuyerCode = ccg.BuyerCode,
                     BuyerId = ccg.BuyerId,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandId = ccg.BuyerBrandId,
                     BuyerBrandName = ccg.BuyerBrandName,
                     Commodity = ccg.Commodity,
                     ComodityCode = ccg.ComodityCode,
                     CommodityDescription = ccg.CommodityDescription,
                     ComodityID = ccg.ComodityID,
                     DeliveryDate = ccg.DeliveryDate,
                     UOMCode = ccg.UOMCode,
                     UOMID = ccg.UOMID,
                     UOMUnit = ccg.UOMUnit,
                     LastModifiedUtc = ccg.LastModifiedUtc,
                     CostCalculationGarment_Materials = ccg.CostCalculationGarment_Materials,

                     PreSCNo = ccg.PreSCNo,
                     Section = ccg.Section,
                     IsRODistributed = ccg.IsRODistributed,
                     RODistributionBy = ccg.RODistributionBy,
                     RODistributionDate = ccg.RODistributionDate,
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);
            Pageable<CostCalculationGarment> pageable = new Pageable<CostCalculationGarment>(Query, page - 1, size);
            List<CostCalculationGarment> data = pageable.Data.ToList<CostCalculationGarment>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<CostCalculationGarment>(data, totalData, OrderDictionary, SelectedFields);
        }

        internal void PostCC(List<long> listId)
        {
            var models = DbSet.Where(w => listId.Contains(w.Id));
            foreach (var model in models)
            {
                model.IsPosted = true;
                EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
            }
        }

        internal void UnpostCC(long id)
        {
            var model = DbSet.Single(m => m.Id == id);
            model.IsPosted = false;
            model.IsApprovedMD = false;
            model.IsApprovedPurchasing = false;
            model.IsApprovedIE = false;
            model.IsApprovedKadivMD = false;
            model.IsApprovedPPIC = false;
            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
        }

        internal void InsertUnpostReason(long id, string reason)
        {
            var costCalculation = DbSet.AsNoTracking().Single(m => m.Id == id);
            var reasonDbSet = DbContext.Set<CostCalculationGarmentUnpostReason>();
            CostCalculationGarmentUnpostReason costCalculationGarmentUnpostReason = new CostCalculationGarmentUnpostReason
            {
                CostCalculationId = costCalculation.Id,
                RONo = costCalculation.RO_Number,
                UnpostReason = reason
            };
            EntityExtension.FlagForCreate(costCalculationGarmentUnpostReason, IdentityService.Username, "sales-service");
            reasonDbSet.Add(costCalculationGarmentUnpostReason);
        }

		internal List<CostCalculationGarmentDataProductionReport> GetComodityQtyOrderHoursBuyerByRo(string ro)
		{
            var listRO = ro.Split(",");

            var data = DbSet.Where(w => listRO.Contains(w.RO_Number)).Select(s => new CostCalculationGarmentDataProductionReport
            {
			    ro = s.RO_Number,
			    buyerCode = s.BuyerBrandCode,
			    hours = s.SMV_Cutting,
			    comodityName = s.Commodity,
			    qtyOrder = s.Quantity,
            }).ToList();

			return data;
		}

        internal List<CostCalculationGarmentForJournal> GetCCGByRo(string RO_Number)
        {
            IQueryable<CostCalculationGarment> Query = DbSet;

            List<CostCalculationGarmentForJournal> data = new List<CostCalculationGarmentForJournal>();

            var QueryX = (from a in Query
                          join b in DbContext.CostCalculationGarment_Materials on a.Id equals b.CostCalculationGarmentId
                          where a.IsApprovedKadivMD == true && a.RO_Number == RO_Number

                          group new { Amt = b.Total } by new { a.RO_Number, a.Quantity, a.OTL1CalculatedRate, a.OTL2CalculatedRate, a.Risk } into G

                          select new CostCalculationGarmentForJournal
                          {
                             RONo = G.Key.RO_Number,                          
                             OTL1 = Math.Round(G.Key.OTL1CalculatedRate,0),
                             OTL2 = Math.Round(G.Key.OTL2CalculatedRate,0),
                             Risk = G.Key.Risk,
                             Amount = Math.Round(G.Sum(c => c.Amt), 0),
                             AmountCC = 0,
                          });


            foreach (CostCalculationGarmentForJournal i in QueryX)
            {
               

                data.Add(new CostCalculationGarmentForJournal
                {
                    RONo = i.RONo,                   
                    OTL1 = i.OTL1,
                    OTL2 = i.OTL2,
                    Risk = i.Risk,
                    Amount = i.Amount,
                    AmountCC = Math.Round(((i.Amount + i.OTL1 + i.OTL2) * ((100 + i.Risk)/100)),0),
                });
            };

            return data;
        }

        internal List<string> ReadUnpostReasonCreators(string keyword, int page, int size)
        {
            IQueryable<CostCalculationGarmentUnpostReason> Query = DbContext.Set<CostCalculationGarmentUnpostReason>();

            if (keyword != null)
            {
                Query = Query.Where(w => w.CreatedBy.StartsWith(keyword));
            }

            return Query
                .OrderBy(o => o.CreatedBy)
                .Select(s => s.CreatedBy)
                .Distinct()
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public virtual Task<CostCalculationGarment> ReadByRO(string ro)
        {
            return DbSet.FirstOrDefaultAsync(d => d.RO_Number.Equals(ro) && d.IsDeleted.Equals(false));
        }

        #region Cancel Approval
        public ReadResponse<CostCalculationGarment> ReadForCancelApproval(int page, int size, string order, List<string> select, string keyword, string filter)
        {

            //add query left join with GarmentPurchaseRequest from purchasing dbcontext

            var prs = PurchasingDbContext.GarmentPurchaseRequests.Where(w => w.IsDeleted == false && w.IsUsed == false && w.PRType == "JOB ORDER").Select(s => s.RONo).ToHashSet();
            var Query = from a in DbSet
                        join b in prs on a.RO_Number equals b into gpr
                        from gprs in gpr.DefaultIfEmpty()
                        where a.IsApprovedIE == true && a.IsApprovedPurchasing == true/* && (gprs == null || gprs.IsUsed == false && gprs.IsDeleted == false)*/
                        select a;

            List<string> SearchAttributes = new List<string>()
            {
                "PreSCNo", "RO_Number","Article","UnitName"
            };

            Query = QueryHelper<CostCalculationGarment>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);

            Query = QueryHelper<CostCalculationGarment>.Filter(Query, FilterDictionary);

            Query = Query
                 .Select(ccg => new CostCalculationGarment
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     UnitId = ccg.UnitId,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     ConfirmPrice = ccg.ConfirmPrice,
                     BuyerCode = ccg.BuyerCode,
                     BuyerId = ccg.BuyerId,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandId = ccg.BuyerBrandId,
                     BuyerBrandName = ccg.BuyerBrandName,
                     Commodity = ccg.Commodity,
                     ComodityCode = ccg.ComodityCode,
                     CommodityDescription = ccg.CommodityDescription,
                     ComodityID = ccg.ComodityID,
                     DeliveryDate = ccg.DeliveryDate,
                     UOMCode = ccg.UOMCode,
                     UOMID = ccg.UOMID,
                     UOMUnit = ccg.UOMUnit,

                     PreSCNo = ccg.PreSCNo,

                     IsApprovedMD = ccg.IsApprovedMD,
                     IsApprovedPurchasing = ccg.IsApprovedPurchasing,
                     IsApprovedIE = ccg.IsApprovedIE,
                     IsApprovedKadivMD = ccg.IsApprovedKadivMD,
                     IsApprovedPPIC = ccg.IsApprovedPPIC,

                     IsPosted = ccg.IsPosted,

                     LastModifiedUtc = ccg.LastModifiedUtc,
                     SectionName = ccg.SectionName,
                     Section = ccg.Section,
                     CreatedBy = ccg.CreatedBy
                 });

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<CostCalculationGarment>.Order(Query, OrderDictionary);

            Pageable<CostCalculationGarment> pageable = new Pageable<CostCalculationGarment>(Query, page - 1, size);
            List<CostCalculationGarment> data = pageable.Data.ToList<CostCalculationGarment>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<CostCalculationGarment>(data, totalData, OrderDictionary, new List<string>());
        }

        public async Task<int> CancelApproval(long id, string deletedRemark)
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
                                var data = await DbSet.Include(s => s.CostCalculationGarment_Materials).FirstOrDefaultAsync(d => d.Id == id);

                                //update CostCalculationGarment approved
                                if (data != null)
                                {
                                    //update approved to false
                                    data.IsApprovedIE = false;
                                    data.IsApprovedPurchasing = false;
                                    data.IsApprovedMD = false;
                                    data.IsApprovedKadivMD = false;
                                    data.IsApprovedPPIC = false;

                                    //update appovedDate to null
                                    data.ApprovedIEDate = DateTimeOffset.MinValue;
                                    data.ApprovedPurchasingDate = DateTimeOffset.MinValue;
                                    data.ApprovedMDDate = DateTimeOffset.MinValue;
                                    data.ApprovedKadivMDDate = DateTimeOffset.MinValue;
                                    data.ApprovedPPICDate = DateTimeOffset.MinValue;

                                    //update approvedBy to null
                                    data.ApprovedIEBy = null;
                                    data.ApprovedPurchasingBy = null;
                                    data.ApprovedMDBy = null;
                                    data.ApprovedKadivMDBy = null;
                                    data.ApprovedPPICBy = null;

                                    data.IsPosted = false;

                                    EntityExtension.FlagForUpdate(data, IdentityService.Username, "sales-service");

                                    foreach (var item in data.CostCalculationGarment_Materials)
                                    {
                                        item.IsPRMaster = false;
                                        item.IsPosted = false;
                                        EntityExtension.FlagForUpdate(item, IdentityService.Username, "sales-service");
                                    }

                                    DbSet.Update(data);

                                    //Create Log History
                                    logHistoryLogic.Create("PENJUALAN", "Cancel Approve Cost Calculation - " + data.RO_Number, deletedRemark);

                                    Updated = await DbContext.SaveChangesAsync();
                                }

                                //update GarmentPurchaseRequest
                                var garmentPurchaseRequest = PurchasingDbContext.GarmentPurchaseRequests.Include(s => s.Items).FirstOrDefault(f => f.RONo == data.RO_Number && f.IsDeleted == false);

                                if (garmentPurchaseRequest != null)
                                {
                                    EntityExtension.FlagForDelete(garmentPurchaseRequest, IdentityService.Username, "sales-service");

                                    foreach (var item in garmentPurchaseRequest.Items)
                                    {
                                        EntityExtension.FlagForDelete(item, IdentityService.Username, "sales-service");
                                    }

                                    PurchasingDbContext.GarmentPurchaseRequests.Update(garmentPurchaseRequest);
                                    Updated += await PurchasingDbContext.SaveChangesAsync();
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
                }
            }
            return Updated;
        }

        public Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadCancelApproval(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset)
        {
            var Query = GetReportQueryCancelApproval(dateFrom, dateTo, offset);

            var Data = Query.OrderByDescending(b => b.CancelDate).ToList();

            Pageable<CancelApprovalCostCalculationReportViewModel> pageable = new Pageable<CancelApprovalCostCalculationReportViewModel>(Data, page - 1, size);
            List<CancelApprovalCostCalculationReportViewModel> Data_ = pageable.Data.ToList<CancelApprovalCostCalculationReportViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data_, TotalData);
        }

        private IQueryable<CancelApprovalCostCalculationReportViewModel> GetReportQueryCancelApproval(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //get data log history where remark is not null and activity start with Cancel
            var dataLogHistory = DbContext.LogHistories.Where(x =>
                    x.Remark != null && x.Activity.StartsWith("Cancel") &&
                    x.CreatedDate.AddHours(offset).Date >= dateFrom.Value.Date && x.CreatedDate.AddHours(offset).Date <= dateTo.Value.Date);

            //mapping data to cancel approval cost calculation report view model
            var result = dataLogHistory.Select(x => new CancelApprovalCostCalculationReportViewModel
            {
                Activity = x.Activity,
                CancelDate = x.CreatedDate.AddHours(offset),
                CancelBy = x.CreatedBy,
                CancelReason = x.Remark,
                RequestedBy = DbSet.First(s => s.RO_Number == x.Activity.Substring(x.Activity.Length - 9)).CreatedBy
            });

            return result;
        }

        public MemoryStream GenerateExcelCancelApproval(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQueryCancelApproval(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.CancelDate);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Aktifitas", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Cancel", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dibatalkan Oleh", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Yang Meminta", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Alasan Pembatalan", DataType = typeof(string) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "");
            else
            {
                int index = 1;
                foreach (var d in Query)
                {
                    result.Rows.Add(index, d.Activity, d.CancelDate.ToString("dd MMM yyyy"), d.CancelBy, d.RequestedBy, d.CancelReason);
                    index++;
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Cancel Approval Cost Calculation") }, true);
        }

        #region Report Reject RO
        public Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset)
        {
            var Query = GetReportQueryReadRejectRO(dateFrom, dateTo, offset);

            var Data = Query.OrderByDescending(b => b.CancelDate).ToList();

            Pageable<CancelApprovalCostCalculationReportViewModel> pageable = new Pageable<CancelApprovalCostCalculationReportViewModel>(Data, page - 1, size);
            List<CancelApprovalCostCalculationReportViewModel> Data_ = pageable.Data.ToList<CancelApprovalCostCalculationReportViewModel>();

            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data_, TotalData);
        }

        private IQueryable<CancelApprovalCostCalculationReportViewModel> GetReportQueryReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            //get data log history where remark is not null and activity start with Cancel
            var dataLogHistory = DbContext.LogHistories.Where(x =>
                    x.Remark != null && x.Activity.StartsWith("Reject") &&
                    x.CreatedDate.AddHours(offset).Date >= dateFrom.Value.Date && x.CreatedDate.AddHours(offset).Date <= dateTo.Value.Date);

            //mapping data to cancel approval cost calculation report view model
            var result = dataLogHistory.Select(x => new CancelApprovalCostCalculationReportViewModel
            {
                Activity = x.Activity,
                CancelDate = x.CreatedDate.AddHours(offset),
                CancelBy = x.CreatedBy,
                CancelReason = x.Remark,
                RequestedBy = DbSet.First(s => s.RO_Number == x.Activity.Substring(x.Activity.Length - 9)).CreatedBy
            });

            return result;
        }

        public MemoryStream GenerateExcelReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int offset)
        {
            var Query = GetReportQueryReadRejectRO(dateFrom, dateTo, offset);
            Query = Query.OrderByDescending(b => b.CancelDate);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Aktifitas", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal Cancel", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dibatalkan Oleh", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Yang Meminta", DataType = typeof(string) });
            result.Columns.Add(new DataColumn() { ColumnName = "Alasan Pembatalan", DataType = typeof(string) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "");
            else
            {
                int index = 1;
                foreach (var d in Query)
                {
                    result.Rows.Add(index, d.Activity, d.CancelDate.ToString("dd MMM yyyy"), d.CancelBy, d.RequestedBy, d.CancelReason);
                    index++;
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Cancel Approval Cost Calculation") }, true);
        }
        #endregion
        #endregion
    }
}
