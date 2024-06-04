using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using System.Linq;
using Newtonsoft.Json;
using Com.Moonlay.NetCore.Lib;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Moonlay.Models;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.ROGarmentLogics
{
    public class ROGarmentLogic : BaseLogic<RO_Garment>
    {
        private ROGarmentSizeBreakdownLogic roGarmentSizeBreakdownLogic;
        private ROGarmentSizeBreakdownDetailLogic roGarmentSizeBreakdownDetailLogic;

        private readonly SalesDbContext DbContext;
        public ROGarmentLogic(IServiceProvider serviceProvider, IIdentityService identityService, SalesDbContext dbContext) : base(identityService, serviceProvider, dbContext)
        {
            this.roGarmentSizeBreakdownLogic = serviceProvider.GetService<ROGarmentSizeBreakdownLogic>();
            this.roGarmentSizeBreakdownDetailLogic = serviceProvider.GetService<ROGarmentSizeBreakdownDetailLogic>();
            this.DbContext = dbContext;
        }

        public override ReadResponse<RO_Garment> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<RO_Garment> Query = DbSet;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<RO_Garment>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                  "Id", "Code", "CostCalculationGarment", "Total", "IsPosted","IsRejected"
            };

            Query = Query.Join(DbContext.CostCalculationGarments, ro => ro.CostCalculationGarmentId, ccg => ccg.Id, (ro, ccg) =>
             new RO_Garment
             {
                 Id = ro.Id,
                 Code = ro.Code,
                 CostCalculationGarment = new CostCalculationGarment()
                 {
                     Id = ccg.Id,
                     Code = ccg.Code,
                     RO_Number = ccg.RO_Number,
                     Article = ccg.Article,
                     BuyerCode = ccg.BuyerCode,
                     BuyerName = ccg.BuyerName,
                     BuyerBrandCode = ccg.BuyerBrandCode,
                     BuyerBrandName = ccg.BuyerBrandName,
                     ValidationMDDate = ccg.ValidationMDDate,
                     ValidationSampleDate = ccg.ValidationSampleDate,
                     UnitCode = ccg.UnitCode,
                     UnitName = ccg.UnitName,
                     Quantity = ccg.Quantity,
                     UOMUnit = ccg.UOMUnit,
                     IsValidatedROPPIC = ccg.IsValidatedROPPIC,
                     IsValidatedROSample = ccg.IsValidatedROSample,
                     IsValidatedROMD = ccg.IsValidatedROMD,
                 },
                 Total = ro.Total,
                 IsPosted = ro.IsPosted,
                 LastModifiedUtc = ro.LastModifiedUtc,
                 IsRejected = ro.IsRejected
             });

            //List<string> SearchAttributes = new List<string>()
            //{
            //    "Code","CostCalculationGarments.Article"
            //};

            //Query = QueryHelper<RO_Garment>.Search(Query, SearchAttributes, keyword);
            if(!string.IsNullOrWhiteSpace(keyword))
                Query = Query.Where(sc => sc.CostCalculationGarment.Article.Contains(keyword) || sc.CostCalculationGarment.RO_Number.Contains(keyword) || sc.CostCalculationGarment.UnitName.Contains(keyword));

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<RO_Garment>.Order(Query, OrderDictionary);

            Pageable<RO_Garment> pageable = new Pageable<RO_Garment>(Query, page - 1, size);
            List<RO_Garment> data = pageable.Data.ToList<RO_Garment>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<RO_Garment>(data, totalData, OrderDictionary, SelectedFields);
        }


        public override void Create(RO_Garment model)
        {
            do
            {
                model.Code = Code.Generate();
            }
            while (this.DbSet.Any(d => d.Code.Equals(model.Code)));

            foreach (var size in model.RO_Garment_SizeBreakdowns)
            {
                roGarmentSizeBreakdownLogic.Create(size);
            }

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
            DbSet.Add(model);
        }

        public override void UpdateAsync(long id, RO_Garment model)
        {
            if (model.RO_Garment_SizeBreakdowns != null)
            {
                HashSet<long> detailIds = roGarmentSizeBreakdownLogic.GetIds(id);

                foreach (var itemId in detailIds)
                {
                    RO_Garment_SizeBreakdown data = model.RO_Garment_SizeBreakdowns.FirstOrDefault(prop => prop.Id.Equals(itemId));
                    if (data == null)
                    {
                        RO_Garment_SizeBreakdown dataItem = DbContext.RO_Garment_SizeBreakdowns.FirstOrDefault(prop => prop.Id.Equals(itemId));
                        List<RO_Garment_SizeBreakdown_Detail> details = DbContext.RO_Garment_SizeBreakdown_Details.Where(a => a.RO_Garment_SizeBreakdownId.Equals(itemId)).ToList();
                        foreach (RO_Garment_SizeBreakdown_Detail detail in details)
                        {
                            EntityExtension.FlagForDelete(detail, IdentityService.Username, "sales-service");
                        }

                        EntityExtension.FlagForDelete(dataItem, IdentityService.Username, "sales-service");
                        
                    }
                    else
                    {
                        roGarmentSizeBreakdownLogic.UpdateAsync(itemId, data);
                    }

                    foreach (RO_Garment_SizeBreakdown item in model.RO_Garment_SizeBreakdowns)
                    {
                        if (item.Id == 0)
                            roGarmentSizeBreakdownLogic.Create(item);
                    }
                }
            }

            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
            DbSet.Update(model);
        }

        //public override async void DeleteAsync(int id)
        //{
        //    TModel model = await ReadByIdAsync(id);
        //    EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service", true);
        //    DbSet.Update(model);
        //}

        public override async Task DeleteAsync(long id)
        {
            RO_Garment model = await ReadByIdAsync(id);
            if (model.RO_Garment_SizeBreakdowns != null)
            {
                HashSet<long> detailIds = roGarmentSizeBreakdownLogic.GetIds(id);
                foreach (var itemId in detailIds)
                {
                    await roGarmentSizeBreakdownLogic.DeleteAsync(itemId);
                }
            }

            EntityExtension.FlagForDelete(model, IdentityService.Username, "sales-service");
            DbSet.Update(model);
        }

        internal void PostRO(List<long> listId)
        {
            var models = DbSet.Where(w => listId.Contains(w.Id));
            foreach (var model in models)
            {
                model.IsPosted = true;
                EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");
            }
        }

        internal void UnpostRO(long id)
        {
            var model = DbSet.Single(m => m.Id == id);
            model.IsPosted = false;
            EntityExtension.FlagForUpdate(model, IdentityService.Username, "sales-service");

            var cc = DbContext.CostCalculationGarments.Single(m => m.Id == model.CostCalculationGarmentId);

            cc.IsValidatedROPPIC = false;
            cc.ValidationPPICBy = null;
            cc.ValidationPPICDate = DateTimeOffset.MinValue;

            cc.IsValidatedROSample = false;
            cc.ValidationSampleBy = null;
            cc.ValidationSampleDate = DateTimeOffset.MinValue;

            cc.IsValidatedROMD = false;
            cc.ValidationMDBy = null;
            cc.ValidationMDDate = DateTimeOffset.MinValue;

            EntityExtension.FlagForUpdate(cc, IdentityService.Username, "sales-service");
        }
    }
}
