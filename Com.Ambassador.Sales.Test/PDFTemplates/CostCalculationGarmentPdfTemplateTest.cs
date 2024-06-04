using Com.Ambassador.Service.Sales.Lib.PDFTemplates;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Com.Ambassador.Sales.Test.PDFTemplates
{
    public class CostCalculationGarmentPdfTemplateTest
    {
        [Fact]
        public void GeneratePdfTemplate_Return_Success()
        {
            string imagelogo = "data:image/png;base64,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";
            CostCalculationGarmentPdfTemplate pdf = new CostCalculationGarmentPdfTemplate();
            CostCalculationGarmentViewModel viewModel = new CostCalculationGarmentViewModel()
            {
                AccessoriesAllowance = 1,
                ApprovalIE = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalKadivMD = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalMD = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalPPIC = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                ApprovalPurchasing = new Approval()
                {
                    ApprovedBy = "fetih",
                    ApprovedDate = DateTimeOffset.Now,
                    IsApproved = true
                },
                Article = "Article",
                Active = true,
                AutoIncrementNumber = 1,
                Buyer = new BuyerViewModel()
                {
                    address1 = "address1",
                    address2 = "address2",
                    Code = "Code",
                    email = "fetihkhan@gmail.com",
                    IsDeleted = false,
                    Name = "Name",
                    UId = "Uid"
                },
                BuyerBrand=new BuyerBrandViewModel()
                {
                    Active=true,
                    Code="Code",
                    Name="name",
                    UId="Uid",
                    
                },
                Code="Code",
                CommissionPortion=1,
                CommissionRate=1,
                CommodityDescription= "CommodityDescription",
                Comodity=new MasterPlanComodityViewModel()
                {
                    Code= "Code",
                    Name= "Name",
                    UId= "UId",
                    
                },
                ConfirmDate=DateTimeOffset.Now,
                ConfirmPrice=1,
                CostCalculationGarment_Materials=new List<CostCalculationGarment_MaterialViewModel>()
                {
                    new CostCalculationGarment_MaterialViewModel()
                    {
                        Category=new CategoryViewModel()
                        {
                            name="FABRIC",
                            code="code",
                            UId="Uid"
                        },
                        CM_Price=1,
                        AutoIncrementNumber=1,
                        AvailableQuantity=1,
                        Active=true,
                        BudgetQuantity=1,
                        Code="Code",
                        Conversion=1,
                        Description="Description",
                        IsDeleted=true,
                        isFabricCM=true,
                        IsPosted=true,
                        IsPRMaster=true,
                        Information="Information",
                        MaterialIndex=1,
                        PO="PO",
                        POMaster="POMaster",
                        PO_SerialNumber="PO_SerialNumber",
                        Price=1,
                        PRMasterId=1,
                        PRMasterItemId=1,
                        Product=new GarmentProductViewModel()
                        {
                            Code="Code",
                            Composition="Composition",
                            Const="Const",
                            Name="Name",
                            UId="Uid"
                        },
                        ProductRemark="ProductRemark",
                        ShippingFeePortion=1,
                        Quantity=1,
                        Total=1,
                        UId="Uid",
                        TotalShippingFee=1,
                        UOMPrice=new UOMViewModel()
                        {
                            code="code",
                            Unit="Unit",
                            UId="Uid"
                        },
                        UOMQuantity=new UOMViewModel()
                        {
                             code="code",
                            Unit="Unit",
                            UId="Uid"
                        },
                        CreatedBy="Fetih han"
                    },
                    
                },
                DeliveryDate=DateTimeOffset.Now,
                Description= "Description",
                Efficiency=new EfficiencyViewModel()
                {
                    Code="code",
                    FinalRange=1,
                    InitialRange=1,
                    IsDeleted=true,
                    UId="Uid",
                    Name="Name",
                    Value=1
                },
                FabricAllowance=1,
                Freight=1,
                FreightCost=1,
                ImageFile= imagelogo,
                ImagePath="",
                Index=1,
                Insurance=1,
                IsPosted=true,
                IsROAccepted=true,
                IsROAvailable=true,
                IsRODistributed=true,
                IsValidatedROMD=true,
                IsValidatedROPPIC=true,
                IsValidatedROSample=true,
                IsDeleted=false,
                CreatedAgent= "CreatedAgent",
                LeadTime=1,
                SMV_Cutting=1,
                SCGarmentId=1,
                Section= "Section",
                SectionName= "SectionName",
                SizeRange= "SizeRange",
                ROAcceptedBy= "ROAcceptedBy",
                Rate=new RateViewModel()
                {
                    CreatedAgent= "CreatedAgent",
                    Code="code"
                },
                NETFOB=1,
                NETFOBP=1,
                PreSCNo= "PreSCNo",
                OTL1=new RateCalculatedViewModel()
                {
                    Code="Code",
                    CalculatedValue=1,
                    Name="Name",
                    Unit=new UnitViewModel()
                    {
                        Name="Name",
                        Code="Code",
                        UId="Uid"
                    }
                },
                OTL2=new RateCalculatedViewModel()
                {
                    Code = "Code",
                    CalculatedValue = 1,
                    Name = "Name",
                    Unit = new UnitViewModel()
                    {
                        Name = "Name",
                        Code = "Code",
                        UId = "Uid"
                    }
                },
                PreSCId = 1,
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
                CreatedBy = "fetih han",
                SMV_Finishing=1,
                SMV_Sewing=1,
                SMV_Total=1,
                Unit=new UnitViewModel()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                UId="UId",
                UnitName= "UnitName",
                UOM=new UOMViewModel()
                {
                    Unit="Unit"
                },
                
            };
            var result = pdf.GeneratePdfTemplate(viewModel, 2,new List<Service.Sales.Lib.ViewModels.IntegrationViewModel.GarmentCategoryViewModel>());
            Assert.NotNull(result);
            Assert.IsType<MemoryStream>(result);
        }
    }
}
