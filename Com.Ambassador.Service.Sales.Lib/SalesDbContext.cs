using Com.Ambassador.Service.Sales.Lib.Models.Spinning;
using Com.Ambassador.Service.Sales.Lib.Models.Weaving;
using Com.Ambassador.Service.Sales.Lib.Models.FinishingPrinting;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Com.Ambassador.Service.Sales.Lib.Models.ProductionOrder;
using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentSalesContractModel;
using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using Com.Ambassador.Service.Sales.Lib.Models;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentMasterPlan.WeeklyPlanModels;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentSewingBlockingPlanModel;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentMasterPlan.MaxWHConfirmModel;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentPreSalesContractModel;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentOmzetTargetModel;
using Com.Ambassador.Service.Sales.Lib.Models.SalesInvoice;
using Com.Ambassador.Service.Sales.Lib.Models.FinishingPrintingCostCalculation;
using Com.Ambassador.Service.Sales.Lib.Models.DOSales;
using Com.Ambassador.Service.Sales.Lib.Models.DOReturn;
using Com.Ambassador.Service.Sales.Lib.Models.SalesInvoiceExport;

namespace Com.Ambassador.Service.Sales.Lib
{
    public class SalesDbContext : StandardDbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
        {
        }

        public DbSet<WeavingSalesContractModel> WeavingSalesContract { get; set; }
        public DbSet<SpinningSalesContractModel> SpinningSalesContract { get; set; }
        public DbSet<FinishingPrintingSalesContractModel> FinishingPrintingSalesContracts { get; set; }
        public DbSet<FinishingPrintingSalesContractDetailModel> FinishingPrintingSalesContractDetails { get; set; }
		public DbSet<CostCalculationGarment> CostCalculationGarments { get; set; }
		public DbSet<CostCalculationGarment_Material> CostCalculationGarment_Materials { get; set; }
        public DbSet<GarmentSalesContract> GarmentSalesContracts { get; set; }
        public DbSet<GarmentSalesContractRO> GarmentSalesContractROs { get; set; }
        public DbSet<GarmentSalesContractItem> GarmentSalesContractItems { get; set; }

        #region PRODUCTION ORDER DBSET
        public DbSet<ProductionOrderModel> ProductionOrder { get; set; }
        public DbSet<ProductionOrder_DetailModel> ProductionOrder_Details { get; set; }
        public DbSet<ProductionOrder_LampStandardModel> ProductionOrder_LampStandard { get; set; }
        public DbSet<ProductionOrder_RunWidthModel> ProductionOrder_RunWidth { get; set; }

        #endregion

        public DbSet<RO_Garment> RO_Garments { get; set; }
        public DbSet<RO_Garment_SizeBreakdown> RO_Garment_SizeBreakdowns { get; set; }
        public DbSet<RO_Garment_SizeBreakdown_Detail> RO_Garment_SizeBreakdown_Details { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<ArticleColor> ArticleColors { get; set; }
        public DbSet<Efficiency> Efficiencies { get; set; }
        public DbSet<GarmentBookingOrder> GarmentBookingOrders { get; set; }
        public DbSet<GarmentBookingOrderItem> GarmentBookingOrderItems { get; set; }
        public DbSet<GarmentWeeklyPlan> GarmentWeeklyPlans { get; set; }
        public DbSet<GarmentWeeklyPlanItem> GarmentWeeklyPlanItems { get; set; }

        public DbSet<GarmentSewingBlockingPlan> GarmentSewingBlockingPlans { get; set; }
        public DbSet<GarmentSewingBlockingPlanItem> GarmentSewingBlockingPlanItems { get; set; }

        public DbSet<MaxWHConfirm> MaxWHConfirms { get; set; }
        public DbSet<GarmentPreSalesContract> GarmentPreSalesContracts { get; set; }
        public DbSet<CostCalculationGarmentUnpostReason> CostCalculationGarmentUnpostReasons { get; set; }
        public DbSet<GarmentOmzetTarget> GarmentOmzetTargets { get; set; }
        public DbSet<FinishingPrintingPreSalesContractModel> FinishingPrintingPreSalesContracts { get; set; }

        public DbSet<FinishingPrintingCostCalculationModel> FinishingPrintingCostCalculations { get; set; }
        public DbSet<FinishingPrintingCostCalculationMachineModel> FinishingPrintingCostCalculationMachines { get; set; }
        public DbSet<FinishingPrintingCostCalculationChemicalModel> FinishingPrintingCostCalculationChemicals { get; set; }

        public DbSet<DOSalesModel> DOSales { get; set; }
        public DbSet<DOSalesDetailModel> DOSalesLocalItems { get; set; }

        public DbSet<SalesInvoiceModel> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceDetailModel> SalesInvoiceDetails { get; set; }
        public DbSet<SalesInvoiceItemModel> SalesInvoiceItems { get; set; }

        public DbSet<SalesInvoiceExportModel> SalesInvoiceExports { get; set; }
        public DbSet<SalesInvoiceExportDetailModel> SalesInvoiceExportDetails { get; set; }
        public DbSet<SalesInvoiceExportItemModel> SalesInvoiceExportItems { get; set; }

        public DbSet<DOReturnModel> DOReturns { get; set; }
        public DbSet<DOReturnDetailModel> DOReturnDetails { get; set; }
        public DbSet<DOReturnDetailItemModel> DOReturnDetailItems { get; set; }
        public DbSet<DOReturnItemModel> DOReturnItems { get; set; }

        public DbSet<DeliveryNoteProductionModel> DeliveryNoteProduction { get; set; }

        public DbSet<LogHistory> LogHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<FinishingPrintingSalesContractModel>()
                .HasIndex(h => h.SalesContractNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0)");

            modelBuilder.Entity<FinishingPrintingCostCalculationModel>()
               .Ignore(c => c.ImageFile);

            modelBuilder.Entity<CostCalculationGarment>()
                .Ignore(c => c.ImageFile);

            modelBuilder.Entity<RO_Garment>()
                .Ignore(c => c.ImagesFile);

            modelBuilder.Entity<RO_Garment>()
                .Ignore(c => c.DocumentsFile);

            modelBuilder.Entity<GarmentPreSalesContract>()
                .HasIndex(i => i.SCNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0)");

            modelBuilder.Entity<CostCalculationGarment>()
                .HasIndex(i => i.RO_Number)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0)");

            modelBuilder.Entity<GarmentSalesContract>()
                .HasIndex(i => i.SalesContractNo)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0)");
        }
    }
}
