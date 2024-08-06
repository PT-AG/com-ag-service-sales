using Com.Ambassador.Service.Sales.Lib.Models.GarmentPreSalesContractModel;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentPreSalesContractLogics
{
    public class GarmentPreSalesContractLogic : BaseLogic<GarmentPreSalesContract>
    {
        private readonly SalesDbContext DbContext;
        public GarmentPreSalesContractLogic(IIdentityService identityService, SalesDbContext dbContext) : base(identityService, dbContext)
        {
            DbContext = dbContext;
        }

        public override ReadResponse<GarmentPreSalesContract> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            IQueryable<GarmentPreSalesContract> Query = DbSet;

            List<string> SearchAttributes = new List<string>()
            {
                "SCNo", "SCType", "BuyerAgentName", "BuyerBrandName"
            };

            Query = QueryHelper<GarmentPreSalesContract>.Search(Query, SearchAttributes, keyword);

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            Query = QueryHelper<GarmentPreSalesContract>.Filter(Query, FilterDictionary);

            List<string> SelectedFields = new List<string>()
            {
                "Id", "SectionId", "SectionCode", "BuyerBrandId", "BuyerBrandName", "BuyerBrandCode", "SCNo", "SCDate", "SCType", "LastModifiedUtc", "CreatedUtc", "BuyerAgentId", "BuyerAgentName", "BuyerAgentCode", "OrderQuantity", "IsPosted","Remark"
            };

            Query = Query
                .Select(sc => new GarmentPreSalesContract
                {
                    Id = sc.Id,
                    SCNo = sc.SCNo,
                    SCDate = sc.SCDate,
                    SCType = sc.SCType,
                    SectionId = sc.SectionId,
                    SectionCode = sc.SectionCode,
                    BuyerBrandCode = sc.BuyerBrandCode,
                    BuyerBrandId = sc.BuyerBrandId,
                    BuyerBrandName = sc.BuyerBrandName,
                    CreatedUtc = sc.CreatedUtc,
                    LastModifiedUtc = sc.LastModifiedUtc,
                    BuyerAgentId = sc.BuyerAgentId,
                    BuyerAgentName = sc.BuyerAgentName,
                    BuyerAgentCode = sc.BuyerAgentCode,
                    OrderQuantity = sc.OrderQuantity,
                    IsDeleted = sc.IsDeleted,
                    IsPosted = sc.IsPosted,
                    Remark=sc.Remark
                }).OrderByDescending(s => s.LastModifiedUtc);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            Query = QueryHelper<GarmentPreSalesContract>.Order(Query, OrderDictionary);

            Pageable<GarmentPreSalesContract> pageable = new Pageable<GarmentPreSalesContract>(Query, page - 1, size);
            List<GarmentPreSalesContract> data = pageable.Data.ToList<GarmentPreSalesContract>();
            int totalData = pageable.TotalCount;

            return new ReadResponse<GarmentPreSalesContract>(data, totalData, OrderDictionary, SelectedFields);
        }

        internal void Patch(long id, JsonPatchDocument<GarmentPreSalesContract> jsonPatch)
        {
            var data = DbSet.Where(d => d.Id == id)
                .Single();

            EntityExtension.FlagForUpdate(data, IdentityService.Username, "sales-service");

            jsonPatch.ApplyTo(data);
        }

        public override void Create(GarmentPreSalesContract model)
        {
            GenerateNo(model);

            EntityExtension.FlagForCreate(model, IdentityService.Username, "sales-service");
            DbSet.Add(model);
        }

        private void GenerateNo(GarmentPreSalesContract model)
        {
            string Year = model.SCDate.ToString("yy");
            //string Month = model.CreatedUtc.ToString("MM");

            string no = $"SC-{model.SectionCode}-{model.BuyerBrandCode}-{Year}-";
            int Padding = 5;

            var lastData = DbSet.IgnoreQueryFilters().Where(w => w.SCNo.StartsWith(no) && w.SCType == model.SCType && !w.IsDeleted).OrderByDescending(o => o.CreatedUtc).FirstOrDefault();

            //string DocumentType = model.BuyerType.ToLower().Equals("ekspor") || model.BuyerType.ToLower().Equals("export") ? "FPE" : "FPL";

            int YearNow = DateTime.Now.Year;

            if (lastData == null)
            {
                model.SCNo = no + "1".PadLeft(Padding, '0');
            }
            else
            {
                string tempSCNo = lastData.SCNo;
                if(model.SCType == "SAMPLE")
                {
                    tempSCNo = lastData.SCNo.Substring(0, lastData.SCNo.Length - 2);
                }

                if (model.SCType == "SUBCON")
                {
                    tempSCNo = lastData.SCNo.Substring(0, lastData.SCNo.Length - 4);
                }

                if (model.SCType == "SUBCON KELUAR")
                {
                    tempSCNo = lastData.SCNo.Substring(0, lastData.SCNo.Length - 3);
                }

                int lastNoNumber = Int32.Parse(tempSCNo.Replace(no, "")) + 1;
                model.SCNo = no + lastNoNumber.ToString().PadLeft(Padding, '0');
            }

            if(model.SCType == "SAMPLE")
            {
                model.SCNo = model.SCNo + "-S";
            }


            if (model.SCType == "SUBCON")
            {
                model.SCNo = model.SCNo + "-SUB";
            }

            if (model.SCType == "SUBCON KELUAR")
            {
                model.SCNo += "-SK";
            }
        }

    }
}