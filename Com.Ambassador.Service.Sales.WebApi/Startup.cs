using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Com.Ambassador.Service.Sales.Lib;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Spinning;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.FinishingPrinting;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Weaving;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.Spinning;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.FinishingPrinting;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.Weaving;
using Com.Ambassador.Service.Sales.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Weaving;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Spinning;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrinting;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.ProductionOrder;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.ProductionOrder;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.ProductionOrder;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentSalesContractLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentSalesContractInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentSalesContractFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.ROGarmentInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.ROGarment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.ROGarmentLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Garment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.Garment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.Garment;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentBookingOrderLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentBookingOrderInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentBookingOrderFacade;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.Garment.WeeklyPlanInterfaces;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentMasterPlan.WeeklyPlanFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentMasterPlan.WeeklyPlanLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentMasterPlan.MonitoringInterfaces;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentMasterPlan.MonitoringFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentMasterPlan.MonitoringLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentMasterPlan.GarmentSewingBlockingPlanFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentMasterPlan.GarmentSewingBlockingPlanInterfaces;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentMasterPlan.GarmentSewingBlockingPlanLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentMasterPlan.MaxWHConfirmInterfaces;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentMasterPlan.MaxWHConfirmFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentMasterPlan.MaxWHConfirmLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentPreSalesContractFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentPreSalesContractInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentPreSalesContractLogics;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentOmzetTargetInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentOmzetTargetFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentOmzetTargetLogics;
using Swashbuckle.AspNetCore.Swagger;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.SalesInvoice;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.SalesInvoice;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.SalesInvoice;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.FinishingPrintingCostCalculation;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.FinishingPrintingCostCalculation;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.FinishingPrintingCostCalculation;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.LocalMerchandiserInterfaces;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.LocalMerchandiserFacades;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.DOSales;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.DOSales;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.DOSales;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.DOReturn;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.DOReturn;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.DOReturn;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.DeliveryNoteProduction;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.SalesInvoiceExport;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.SalesInvoiceExport;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.SalesInvoiceExport;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.DOAval;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.DOAval;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.DOAval;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.DOStock;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.DOStock;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.DOStock;

namespace Com.Ambassador.Service.Sales.WebApi
{
    public class Startup
    {
        /* Hard Code */
        private string[] EXPOSED_HEADERS = new string[] { "Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time" };
        private string SALES_POLICY = "SalesPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Register

        private void RegisterFacades(IServiceCollection services)
        {
            services
                .AddTransient<IWeavingSalesContract, WeavingSalesContractFacade>()
                .AddTransient<ISpinningSalesContract, SpinningSalesContractFacade>()
                .AddTransient<SpinningSalesContractReportFacade>()
                .AddTransient<FinishingPrintingSalesContractReportFacade>()
                .AddTransient<IGarmentBookingOrderMonitoringInterface, GarmentBookingOrderMonitoringFacade>()
                .AddTransient<IFinishingPrintingSalesContract, FinishingPrintingSalesContractFacade>()
                .AddTransient<IGarmentSalesContract, GarmentSalesContractFacade>()
                .AddTransient<IProductionOrder, ProductionOrderFacade>()
                .AddTransient<WeavingSalesContractReportFacade>()
				.AddTransient<ICostCalculationGarment,CostCalculationGarmentFacade>()
                .AddTransient<ICostCalculationGarmentByUnitReport, CostCalculationGarmentByUnitReportFacade>()
                .AddTransient<ICostCalculationGarmentBySectionReport, CostCalculationGarmentBySectionReportFacade>()
                .AddTransient<ICostCalculationGarmentByBuyer1Report, CostCalculationGarmentByBuyer1ReportFacade>()
                .AddTransient<ICostCalculationGarmentByBuyer2Report, CostCalculationGarmentByBuyer2ReportFacade>()
                .AddTransient<ISMVGarmentByUnitReport, SMVGarmentByUnitReportFacade>()
                .AddTransient<IDetailCMGarmentByUnitReport, DetailCMGarmentByUnitReportFacade>()
                .AddTransient<IDistributionROGarmentReport, DistributionROGarmentReportFacade>()
                .AddTransient<ICostCalculationGarmentValidationReport, CostCalculationGarmentValidationReportFacade>()
                .AddTransient<IProfitGarmentBySectionReport, ProfitGarmentBySectionReportFacade>()
                .AddTransient<IAvailableROGarmentReportFacade, AvailableROGarmentReportFacade>()
                .AddTransient<ICostCalculationGarmentApprovalReport, CostCalculationGarmentApprovalReportFacade>()
                .AddTransient<IProfitGarmentByComodityReport, ProfitGarmentByComodityReportFacade>()
                .AddTransient<IBudgetExportGarmentReport, BudgetExportGarmentReportFacade>()
                .AddTransient<ICCGEmbroideryApprovalReport, CCGEmbroideryApprovalReportFacade>()
                .AddTransient<ICCROGarmentHistoryBySectionReport, CCROGarmentHistoryBySectionReportFacade>()
                .AddTransient<IROGarment, ROGarmentFacade>()
                .AddTransient<IArticleColor, ArticleColorFacade>()
                .AddTransient<IRate, RateFacade>()
                .AddTransient<IEfficiency, EfficiencyFacade>()
                .AddTransient<IAzureImageFacade, AzureImageFacade>()
                .AddTransient<IAzureDocumentFacade, AzureDocumentFacade>()
                .AddTransient<IGarment_BudgetValidationPPIC, Garment_BudgetValidationPPICFacade>()
                .AddTransient<IGarmentBookingOrder, GarmentBookingOrderFacade>()
                .AddTransient<IWeeklyPlanFacade, WeeklyPlanFacade>()
                .AddTransient<ICanceledGarmentBookingOrderReportFacade, CanceledGarmentBookingOrderReportFacade>()
                .AddTransient<IMonitoringRemainingEHFacade, MonitoringRemainingEHFacade>()
                .AddTransient<IGarmentSewingBlockingPlan, GarmentSewingBlockingPlanFacade>()
                .AddTransient<IAcceptedOrderMonitoringFacade, AcceptedOrderMonitoringFacade>()
                .AddTransient<IExpiredGarmentBookingOrder, ExpiredGarmentBookingOrderFacade>()
                .AddTransient<ISewingBlockingPlanReportFacade, SewingBlockingPlanReportFacade>()
                .AddTransient<IOverScheduleMonitoringFacade, OverScheduleMonitoringFacade>()
                .AddTransient<IWeeklyWorkingScheduleMonitoringFacade, WeeklyWorkingScheduleMonitoringFacade>()
                .AddTransient<IMaxWHConfirmFacade, MaxWHConfirmFacade>()
                .AddTransient<IBudgetJobOrderDisplayFacade, BudgetJobOrderDisplayFacade>()
                .AddTransient<IMonitoringUnpostCostCalculationFacade, MonitoringUnpostCostCalculationFacade>()
                .AddTransient<IGarmentProductionOrderReportFacade, GarmentProductionOrderReportFacade>()
                .AddTransient<IAcceptedROReportFacade, AcceptedROReportFacade>()
                .AddTransient<IAvailableROReportFacade, AvailableROReportFacade>()
                .AddTransient<IAvailableBudgetReportFacade, AvailableBudgetReportFacade>()
                .AddTransient<ILatestAvailableROGarmentReportFacade, LatestAvailableROGarmentReportFacade>()
                .AddTransient<ILatestAvailableBudgetReportFacade, LatestAvailableBudgetReportFacade>()
                .AddTransient<IMonitoringPreSalesContractFacade, MonitoringPreSalesContractFacade>()
                .AddTransient<IGarmentPreSalesContract, GarmentPreSalesContractFacade>()
                .AddTransient<IGarmentPurchasingQualityObjectiveReportFacade, GarmentPurchasingQualityObjectiveReportFacade>()
                .AddTransient<IGarmentOmzetTarget, GarmentOmzetTargetFacade>()
                .AddTransient<IDOSalesContract, DOSalesFacade>()
                .AddTransient<IDOReturnContract, DOReturnFacade>()
                .AddTransient<ISalesInvoiceContract, SalesInvoiceFacade>()
                .AddTransient<ISalesInvoiceExportContract, SalesInvoiceExportFacade>()
                .AddTransient<IFinishingPrintingPreSalesContractFacade, FinishingPrintingPreSalesContractFacade>()
                .AddTransient<IFinishingPrintingCostCalculationService, FinishingPrintingCostCalculationFacade>()
                .AddTransient<IShinFinishingPrintingSalesContractFacade, ShinFinishingPrintingSalesContractFacade>()
                .AddTransient<IShinProductionOrder, ShinProductionOrderFacade>()
                .AddTransient<IHOrderFacade, HOrderFacade>()
                .AddTransient<IDOAvalFacade, DOAvalFacade>()
                .AddTransient<IDOStockFacade,DOStockFacade>()
                .AddTransient<IDeliveryNoteProduction, DeliveryNoteProductionFacade>();
        }

        private void RegisterLogic(IServiceCollection services)
        {

			services
				.AddTransient<WeavingSalesContractLogic>()
				.AddTransient<SpinningSalesContractLogic>()
				.AddTransient<FinishingPrintingSalesContractLogic>()
				.AddTransient<FinishingPrintingSalesContractDetailLogic>()
				.AddTransient<ProductionOrder_DetailLogic>()
				.AddTransient<ProductionOrder_LampStandardLogic>()
				.AddTransient<ProductionOrder_RunWidthLogic>()
				.AddTransient<ProductionOrderLogic>()
				.AddTransient<CostCalculationGarmentLogic>()
				.AddTransient<CostCalculationGarmentMaterialLogic>()
                .AddTransient<CostCalculationByUnitReportLogic>()
                .AddTransient<CostCalculationBySectionReportLogic>()
                .AddTransient<CostCalculationByBuyer1ReportLogic>()
                .AddTransient<CostCalculationByBuyer2ReportLogic>()
                .AddTransient<SMVGarmentByUnitReportLogic>()
                .AddTransient<DetailCMGarmentByUnitReportLogic>()
                .AddTransient<DistributionROGarmentReportLogic>()
                .AddTransient<CostCalculationGarmentValidationReportLogic>()
                .AddTransient<ProfitGarmentBySectionReportLogic>()
                .AddTransient<AvailableROGarmentReportLogic>()
                .AddTransient<CostCalculationGarmentApprovalReportLogic>()
                .AddTransient<ProfitGarmentByComodityReportLogic>()
                .AddTransient<BudgetExportGarmentReportLogic>()
                .AddTransient<CCGEmbroideryApprovalReportLogic>()
                .AddTransient<CCROGarmentHistoryBySectionReportLogic>()
                .AddTransient<GarmentSalesContractLogic>()
                .AddTransient<GarmentSalesContractROLogic>()
                .AddTransient<GarmentSalesContractItemLogic>()
                .AddTransient<ArticleColorLogic>()
                .AddTransient<ROGarmentLogic>()
                .AddTransient<ROGarmentSizeBreakdownLogic>()
                .AddTransient<ROGarmentSizeBreakdownDetailLogic>()
                .AddTransient<RateLogic>()
                .AddTransient<EfficiencyLogic>()
                .AddTransient<Garment_BudgetValidationPPICLogic>()
                .AddTransient<GarmentBookingOrderLogic>()
                .AddTransient<GarmentBookingOrderItemLogic>()
                .AddTransient<MonitoringRemainingEHLogic>()
                .AddTransient<WeeklyPlanLogic>()
                .AddTransient<GarmentSewingBlockingPlanLogic>()
                .AddTransient<AcceptedOrderMonitoringLogic>()
                .AddTransient<SewingBlockingPlanReportLogic>()
                .AddTransient<OverScheduleMonitoringLogic>()
                .AddTransient<WeeklyWorkingScheduleMonitoringLogic>()
                .AddTransient<MaxWHConfirmLogic>()
                .AddTransient<BudgetJobOrderDisplayLogic>()
                .AddTransient<MonitoringUnpostCostCalculationLogic>()
                .AddTransient<GarmentProductionOrderReportLogic>()
                .AddTransient<AcceptedROReportLogic>()
                .AddTransient<AvailableROReportLogic>()
                .AddTransient<AvailableBudgetReportLogic>()
                .AddTransient<LatestAvailableROGarmentReportLogic>()
                .AddTransient<LatestAvailableBudgetReportLogic>()
                .AddTransient<MonitoringPreSalesContractLogic>()
                .AddTransient<GarmentPreSalesContractLogic>()
                .AddTransient<GarmentPurchasingQualityObjectiveReportLogic>()
                .AddTransient<GarmentOmzetTargetLogic>()
                .AddTransient<DOSalesLogic>()
                .AddTransient<DOReturnLogic>()
                .AddTransient<DOReturnDetailLogic>()
                .AddTransient<DOReturnDetailItemLogic>()
                .AddTransient<DOReturnItemLogic>()
                .AddTransient<DOSalesDetailLogic>()
                .AddTransient<SalesInvoiceLogic>()
                .AddTransient<SalesInvoiceDetailLogic>()
                .AddTransient<SalesInvoiceItemLogic>()
                .AddTransient<SalesInvoiceExportLogic>()
                .AddTransient<SalesInvoiceExportDetailLogic>()
                .AddTransient<SalesInvoiceExportItemLogic>()
                .AddTransient<FinishingPrintingPreSalesContractLogic>()
                .AddTransient<FinishingPrintingCostCalculationLogic>()
                .AddTransient<ShinFinishingPrintingSalesContractLogic>()
                .AddTransient<ShinProductionOrderLogic>()
                .AddTransient<DOAvalLogic>()
                .AddTransient<DOStockLogic>()
                .AddTransient<DeliveryNoteProductionLogic>()
                .AddTransient<LogHistoryLogic>() ;
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddScoped<IIdentityService,IdentityService>()
                .AddScoped<IHttpClientService, HttpClientService>()
                .AddScoped<IValidateService,ValidateService>();
        }

        private void RegisterEndpoint()
        {
            Com.Ambassador.Service.Sales.WebApi.Utilities. APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
			Com.Ambassador.Service.Sales.WebApi.Utilities.APIEndpoint.AzureCore = Configuration.GetValue<string>("AzureCoreEndpoint") ?? Configuration["AzureCoreEndpoint"];
			Com.Ambassador.Service.Sales.Lib.Helpers.APIEndpoint.StorageAccountName = Configuration.GetValue<string>("StorageAccountName") ?? Configuration["StorageAccountName"];
			Com.Ambassador.Service.Sales.Lib.Helpers.APIEndpoint.StorageAccountKey = Configuration.GetValue<string>("StorageAccountKey") ?? Configuration["StorageAccountKey"];
            Com.Ambassador.Service.Sales.Lib.Helpers.APIEndpoint.AzurePurchasing = Configuration.GetValue<string>("AzurePurchasingEndpoint") ?? Configuration["PurchasingEndpoint"];
            Com.Ambassador.Service.Sales.Lib.Helpers.APIEndpoint.Core = Configuration.GetValue<string>("CoreEndpoint") ?? Configuration["CoreEndpoint"];
            Lib.Helpers.APIEndpoint.Production = Configuration.GetValue<string>("ProductionEndpoint") ?? Configuration["ProductionEndpoint"];
            Lib.Helpers.APIEndpoint.Finance = Configuration.GetValue<string>("FinanceEndpoint") ?? Configuration["FinanceEndpoint"];
            Lib.Helpers.APIEndpoint.PackingInventory = Configuration.GetValue<string>("PackingInventoryEndpoint") ?? Configuration["PackingInventoryEndpoint"];

        }

        #endregion Register

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];
            string connectionStringLocalMerchandiser = Configuration.GetConnectionString("LocalMerchandiserConnection") ?? Configuration["LocalMerchandiserConnection"];
            string connectionStringPurchasing = Configuration.GetConnectionString("PurchasingConnection") ?? Configuration["PurchasingConnection"];

            Com.Ambassador.Service.Sales.Lib.Helpers.APIEndpoint.ConnectionString = connectionString;
            /* Register */
            services.AddDbContext<SalesDbContext>(options => options.UseSqlServer(connectionString));
            services.AddTransient<ILocalMerchandiserDbContext>(s => new LocalMerchandiserDbContext(connectionStringLocalMerchandiser));
            services.AddDbContext<PurchasingDbContext>(options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionStringPurchasing));

            RegisterFacades(services);
            RegisterLogic(services);
            RegisterServices(services);
            RegisterEndpoint();
            services.AddAutoMapper();

            /* Versioning */
            services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1, 0); });

            /* Authentication */
            string Secret = Configuration.GetValue<string>("Secret") ?? Configuration["Secret"];
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });
		 
			/* CORS */
			services.AddCors(options => options.AddPolicy(SALES_POLICY, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders(EXPOSED_HEADERS);
            }));

            /* API */
            services
               .AddMvcCore()
               .AddApiExplorer()
               .AddAuthorization()
               .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
               .AddJsonFormatters();

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey",
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>()
                {
                    {
                        "Bearer",
                        Enumerable.Empty<string>()
                    }
                });
                c.CustomSchemaIds(i => i.FullName);
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* Update Database */
            //using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    SalesDbContext context = serviceScope.ServiceProvider.GetService<SalesDbContext>();
            //    context.Database.Migrate();
            //}

            app.UseAuthentication();
            app.UseCors(SALES_POLICY);
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
        }
    }
}
