using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Garment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.Garment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.ROGarmentLogics;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.ViewModels.Garment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Garment
{
    public class LatestAvailableROGarmentReportFacade : ILatestAvailableROGarmentReportFacade
    {
        private readonly SalesDbContext DbContext;
        private LatestAvailableROGarmentReportLogic logic;
        private IIdentityService identityService;
        private IServiceProvider service;
        private readonly DbSet<RO_Garment_SizeBreakdown> DbSet;
        private readonly ROGarmentSizeBreakdownLogic rOGarmentSizeBreakdownLogic;

        public LatestAvailableROGarmentReportFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            logic = serviceProvider.GetService<LatestAvailableROGarmentReportLogic>();
            identityService = serviceProvider.GetService<IIdentityService>();
            service = serviceProvider;
            DbContext = dbContext;
            DbSet = DbContext.Set<RO_Garment_SizeBreakdown>();
            rOGarmentSizeBreakdownLogic = serviceProvider.GetService<ROGarmentSizeBreakdownLogic>();
        }

        public Tuple<MemoryStream, string> GenerateExcel(string filter = "{}")
        {
            var Query = logic.GetQuery(filter);
            var data = GetData(Query.ToList());

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(int) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal Penerimaan RO", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Tanggal Shipment", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "+/-\nTerima - Shipment", DataType = typeof(int) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Lead Time", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Tipe Buyer", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Artikel", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Style", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Fabric", DataType = typeof(string) });
            dataTable.Columns.Add(new DataColumn() { ColumnName = "Size", DataType = typeof(string) });

            List<(string, Enum, Enum)> mergeCells = new List<(string, Enum, Enum)>() { };

            if (data != null && data.Count > 0)
            {
                int i = 0;
                foreach (var d in data)
                {
                    dataTable.Rows.Add(++i, d.RONo, d.ApprovedSampleDate.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), d.DeliveryDate.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), d.DateDiff, d.LeadTime, d.BuyerCode, d.Buyer, d.Type, d.Article, d.CommodityDescription, d.Quantity, d.Uom, d.Fabric, d.SizeRange);
                    //dataTable.Rows.Add(++i, d.RONo, d.ApprovedSampleDate.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), d.DeliveryDate.ToString("dd MMMM yyyy", new CultureInfo("id-ID")), 26, 30, d.BuyerCode, d.Buyer, d.Type, d.Article, d.CommodityDescription, d.Quantity, d.Uom, d.Fabric, d.SizeRange);

                }
                dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);
                dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);
                var leadTime = "25";

                //LEAD 35
                var Count35 = data.Count(d => d.LeadTime == 35);
                var Count35Ok = data.Count(d => d.DateDiff >= 30 && d.LeadTime == 35);
                //var Percent35Ok = ((decimal)Count35Ok / Count35).ToString("P", new CultureInfo("id-ID"));
                var Count35NotOk = data.Count(d => d.DateDiff < 30 && d.LeadTime == 35);
                //var Percent35NotOk = ((decimal)Count35NotOk / Count35).ToString("P", new CultureInfo("id-ID"));

                //LEAD 30
                var Count30 = data.Count(d => d.LeadTime == 30);
                var Count30Ok = data.Count(d => d.DateDiff >= 25 && d.LeadTime == 30);
                //var Percent30Ok = ((decimal)Count30Ok / Count30).ToString("P", new CultureInfo("id-ID"));
                var Count30NotOk = data.Count(d => d.DateDiff < 25 && d.LeadTime == 30);
                //var Percent30NotOk = ((decimal)Count30NotOk / Count30).ToString("P", new CultureInfo("id-ID"));

                //LEAD 25
                var Count25 = data.Count(d => d.LeadTime == 25);
                var Count25Ok = data.Count(d => d.DateDiff >= 20 && d.LeadTime == 25);
                //var Percent25Ok = ((decimal)Count25Ok / Count25).ToString("P", new CultureInfo("id-ID"));
                var Count25NotOk = data.Count(d => d.DateDiff < 20 && d.LeadTime == 25);
                //var Percent25NotOk = ((decimal)Count25NotOk / Count25).ToString("P", new CultureInfo("id-ID"));

                var Count = Count25 + Count30 + Count35;
                var CountOk = Count35Ok + Count30Ok + Count25Ok;
                var PercentOk = ((decimal)CountOk / Count).ToString("P", new CultureInfo("id-ID"));
                var CountNotOk = Count35NotOk + Count30NotOk + Count25NotOk;
                var PercentNotOk = ((decimal)CountNotOk / Count).ToString("P", new CultureInfo("id-ID"));

                dataTable.Rows.Add(null, "KESIAPAN RO GARMENT DENGAN LEAD TIME  " + leadTime + "  HARI", null, null, null, null, null, null, null, null, null);
                dataTable.Rows.Add(null, "Status OK", null, "Selisih Tgl Penerimaan RO dengan Tgl Shipment >=  " + leadTime + "  hari", null, null, null, null, null, null, null);
                dataTable.Rows.Add(null, "Persentase Status OK", null, $"{CountOk}/{Count} X 100% = {PercentOk}", null, null, null, null, null, null, null);
                dataTable.Rows.Add(null, "Status NOT OK", null, "Selisih Tgl Penerimaan RO dengan Tgl Shipment <  " + leadTime + "  hari", null, null, null, null, null, null, null);
                dataTable.Rows.Add(null, "Persentase Status NOT OK", null, $"{CountNotOk}/{Count} X 100% = {PercentNotOk}", null, null, null, null, null, null, null);
                //sampai sini ------------------------------------------


                //dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);

                //dataTable.Rows.Add(null, "KESIAPAN RO GARMENT DENGAN LEAD TIME 25 HARI", null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Status OK", null, "Selisih Tgl Penerimaan RO dengan Tgl Shipment >= 20 hari", null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Persentase Status OK", null, $"{Count25Ok}/{Count25} X 100% = {Percent25Ok}", null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Status NOT OK", null, "Selisih Tgl Penerimaan RO dengan Tgl Shipment < 20 hari", null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Persentase Status NOT OK", null, $"{Count25NotOk}/{Count25} X 100% = {Percent25NotOk}", null, null, null, null, null, null, null);

                //dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);

                //dataTable.Rows.Add(null, "AKUMULASI KESIAPAN RO GARMENT", null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Status OK", null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Persentase Status OK", null, $"{CountOk}/{Count} X 100% = {PercentOk}", null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Status NOT OK", null, null, null, null, null, null, null, null, null);
                //dataTable.Rows.Add(null, "Persentase Status NOT OK", null, $"{CountNotOk}/{Count} X 100% = {PercentNotOk}", null, null, null, null, null, null, null);

                i += 3;
                mergeCells.Add(($"B{++i}:K{i}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                foreach (var n in Enumerable.Range(0, 4))
                {
                    mergeCells.Add(($"B{++i}:C{i}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                    mergeCells.Add(($"D{i}:K{i}", ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Bottom));
                }
            }
            else
            {
                dataTable.Rows.Add(null, null, null, null, null, null, null, null, null, null, null);
            }

            var excel = Excel.CreateExcel(new List<(DataTable, string, List<(string, Enum, Enum)>)>() { (dataTable, "AvailableROGarment", mergeCells) }, false);

            return Tuple.Create(excel, string.Concat("Laporan Kesiapan RO Garment", GetSuffixNameFromFilter(filter), ".xlsx"));
        }

        private string GetSuffixNameFromFilter(string filterString)
        {
            Dictionary<string, object> filter = JsonConvert.DeserializeObject<Dictionary<string, object>>(filterString);

            return string.Join(null, filter.Where(w => w.Value != null).Select(s => string.Concat(" - ", s.Value is string ? s.Value : ((DateTime)s.Value).AddHours(identityService.TimezoneOffset).ToString("dd MMMM yyyy") )).ToArray());
        }

        public Tuple<List<LatestAvailableROGarmentReportViewModel>, int> Read(int page = 1, int size = 25, string filter = "{}")
        {
            var Query = logic.GetQuery(filter);
            var data = GetData(Query.ToList());

            return Tuple.Create(data, data.Count);
        }

        private List<LatestAvailableROGarmentReportViewModel> GetData(IEnumerable<CostCalculationGarment> CostCalculationGarments)
        {
            IQueryable<ViewModels.IntegrationViewModel.BuyerViewModel> buyerQ = GetGarmentBuyer().AsQueryable();

            var data = CostCalculationGarments.Select(cc => new LatestAvailableROGarmentReportViewModel
            {
                ApprovedSampleDate = cc.ValidationMDDate.ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).Date,
                DeliveryDate = cc.DeliveryDate.ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).Date,
                RONo = cc.RO_Number,
                Article = cc.Article,
                DateDiff = (cc.DeliveryDate.ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).Date - cc.ValidationMDDate.ToOffset(TimeSpan.FromHours(identityService.TimezoneOffset)).Date).Days,
                BuyerCode = cc.BuyerBrandCode,
                Buyer = cc.BuyerBrandName,
                Type = buyerQ.Where(x => x.Code == cc.BuyerCode).Select(x => x.Type).FirstOrDefault(),
                Quantity = cc.Quantity,
                LeadTime = cc.LeadTime,
                Uom = cc.UOMUnit,
                SizeRange = cc.SizeRange,
                RO_GarmentId = Convert.ToInt32(cc.RO_GarmentId),
                Fabric = DbContext.RO_Garment_SizeBreakdowns.Where(d => d.RO_GarmentId == cc.RO_GarmentId).Select(d => d.ColorName).FirstOrDefault(),
                CommodityDescription = cc.CommodityDescription,
            }).ToList();

            return data;
        }

        public List<ViewModels.IntegrationViewModel.BuyerViewModel> GetGarmentBuyer()
        {
            string buyerUri = "master/garment-buyers/all";
            var httpClientService = (IHttpClientService)service.GetService(typeof(IHttpClientService));

            var response = httpClientService.GetAsync($@"{APIEndpoint.Core}{buyerUri}").Result.Content.ReadAsStringAsync();

            if (response != null)
            {
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);
                var json = result.Single(p => p.Key.Equals("data")).Value;
                List<ViewModels.IntegrationViewModel.BuyerViewModel> buyerList = JsonConvert.DeserializeObject<List<ViewModels.IntegrationViewModel.BuyerViewModel>>(json.ToString());

                return buyerList;
            }
            else
            {
                return null;
            }
        }
    }
}
