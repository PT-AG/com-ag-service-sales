using Com.Ambassador.Service.Sales.Lib.PDFTemplates;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;         

namespace Com.Ambassador.Sales.Test.PDFTemplates
{
    public class CostCalculationGarmentBudgetPdfTemplateTest
    {
        [Fact]
        public void GeneratePdfTemplate_Return_Success()
        {
            CostCalculationGarmentBudgetPdfTemplate pdf = new CostCalculationGarmentBudgetPdfTemplate();
            CostCalculationGarmentViewModel viewModel = new CostCalculationGarmentViewModel()
            {
                AccessoriesAllowance = 1,
                Active = true,
                ApprovalIE = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalKadivMD = new Approval()
                {
                    ApprovedBy = "fetih han",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalMD = new Approval()
                {
                    ApprovedBy = "fetih han",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalPPIC = new Approval
                {
                    ApprovedBy = "fetih han",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalPurchasing = new Approval
                {
                    ApprovedBy = "fetih han",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                Article = "Article",
                AutoIncrementNumber = 1,
                Buyer = new BuyerViewModel
                {
                    Active = true,
                    address1 = "jakarta",
                    address2 = "solo",
                    Code = "Code",
                    email = "fetihkhan@gmail.com",
                    Name = "Name",
                    Id = 1,
                    UId = "Uid"
                },
                SMV_Finishing = 1,
                BuyerBrand = new BuyerBrandViewModel()
                {
                    Active = true,
                    UId = "UId",
                    Code = "Code",
                    Id = 1,
                    Name = "Name",
                    CreatedBy = "Fetih han",

                },
                Code = "Code",
                CommissionPortion = 1,
                CommissionRate = 1,
                CommodityDescription = "CommodityDescription",
                Comodity = new MasterPlanComodityViewModel
                {
                    Name = "Name",
                    Code = "Code",
                    UId = "UId"
                },
                ConfirmDate = DateTimeOffset.Now,
                ConfirmPrice = 1,
                CostCalculationGarment_Materials = new List<CostCalculationGarment_MaterialViewModel>()
                {
                    new CostCalculationGarment_MaterialViewModel()
                    {
                        
                        AutoIncrementNumber=1,
                        AvailableQuantity =1,
                        BudgetQuantity =1,
                        Category =new CategoryViewModel()
                        {
                            name ="FABRIC",
                            code ="code",
                            UId ="UId"
                        },
                        CM_Price =1,
                        Code ="Code",
                        Conversion =1,
                        Description ="Description",
                        Information ="Information",
                        isFabricCM =true,
                        IsPosted =true,
                        IsPRMaster =true,
                        MaterialIndex =1,
                        PO ="PO",
                        POMaster ="POMaster",
                        PO_SerialNumber ="PO_SerialNumber",
                        Price =1,
                        PRMasterId =1,
                        PRMasterItemId =1,
                        Product =new GarmentProductViewModel()
                        {
                            Code ="Code",
                            Composition ="Composition",
                            Const ="Const",
                            Name ="Name",
                            Width ="1",
                            UId ="Uid",
                            Yarn="Yarn",
                        },
                        ProductRemark="ProductRemark",
                        Quantity=1,
                        ShippingFeePortion=1,
                        Total=1,
                        TotalShippingFee=1,
                        UId="Uid",
                        UOMPrice=new UOMViewModel()
                        {
                            UId ="Uid",
                            Unit ="meter",
                            code="code"
                        },
                        UOMQuantity =new UOMViewModel()
                        {
                            UId ="Uid",
                            Unit ="meter",
                            code="code"
                        },
                        
                    }
                },
                Description = "Description",
                DeliveryDate = DateTimeOffset.Now,
                SMV_Sewing = 1,
                SMV_Cutting = 1,
                Efficiency = new EfficiencyViewModel()
                {
                    Code = "Code",
                    FinalRange = 1,
                    InitialRange = 1,
                    Name = "Name",
                    Value = 1,
                    UId = "Uid"
                },
                FabricAllowance = 1,
                Freight = 1,
                FreightCost = 1,
                ImageFile = "",
                ImagePath = "",
                Index = 1,
                Insurance = 1,
                IsPosted = true,
                IsROAccepted = true,
                IsROAvailable = true,
                IsDeleted = false,
                IsRODistributed = true,
                IsValidatedROMD = true,
                IsValidatedROPPIC = true,
                IsValidatedROSample = true,
                Id = 1,
                NETFOB = 1,
                NETFOBP = 1,
                SMV_Total = 1,
                UOM = new UOMViewModel()
                {
                    UId = "Uid",
                    Unit = "meter",
                    code = "code"
                },
                UId = "Uid",
                Unit = new UnitViewModel()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                LeadTime = 1,
                OTL1 = new RateCalculatedViewModel()
                {
                    Code = "Code",
                    Name = "Name",
                    Unit = new UnitViewModel()
                    {
                        Id = 1,
                        Code = "Code",
                        Name = "Name"
                    },
                    Value = 1,
                    UId = "Uid"
                },
                UnitName = "UnitName",
                OTL2 = new RateCalculatedViewModel()
                {
                    Code = "Code",
                    Name = "Name",
                    Unit = new UnitViewModel()
                    {
                        Id = 1,
                        Code = "Code",
                        Name = "Name"
                    },
                    Value = 1,
                    UId = "Uid"
                },
                ROAcceptedBy = "fetih han",
                Rate = new RateViewModel()
                {
                    Id = 1,
                    Unit = new UnitViewModel()
                    {
                        Id = 1,
                        Code = "Code",
                        Name = "Name"
                    },
                    UId = "Uid",
                    Name = "Name",
                    Value = 1,

                },
                PreSCId = 1,
                PreSCNo = "PreSCNo",
                ProductionCost = 1,
                ValidationPPICBy = "fetih han",
                ValidationMDBy = "fetih han",
                ValidationMDDate = DateTimeOffset.Now,
                ValidationPPICDate = DateTimeOffset.Now,
                ValidationSampleBy = "fetih han",
                ValidationSampleDate = DateTimeOffset.Now,
                Quantity = 1,
                Risk = 1,
                ROAcceptedDate = DateTimeOffset.Now,
                ROAvailableBy = "fetih han",
                ROAvailableDate = DateTimeOffset.Now,
                RODistributionBy = "fetih han",
                RODistributionDate = DateTimeOffset.Now,
                RO_GarmentId = 1,
                RO_Number = "RO_Number",
                RO_RetailId = 1,
                SCGarmentId = 1,
                Section = "Section",
                SectionName = "SectionName",
                SizeRange = "SizeRange",
                THR = new RateViewModel()
                {
                    Id = 1,
                    Unit = new UnitViewModel()
                    {
                        Id = 1,
                        Code = "Code",
                        Name = "Name"
                    },
                    UId = "Uid",
                    Name = "Name",
                    Value = 1,
                },
                Wage = new RateViewModel()
                {
                    Id = 1,
                    Unit = new UnitViewModel()
                    {
                        Id = 1,
                        Code = "Code",
                        Name = "Name"
                    },
                    UId = "Uid",
                    Name = "Name",
                    Value = 1,
                },
                CreatedBy = "fetih han"
            };
            var result = pdf.GeneratePdfTemplate(viewModel, 2, new List<Service.Sales.Lib.ViewModels.IntegrationViewModel.GarmentCategoryViewModel>());
            Assert.NotNull(result);
            Assert.IsType<MemoryStream>(result);


        }
    }
}
