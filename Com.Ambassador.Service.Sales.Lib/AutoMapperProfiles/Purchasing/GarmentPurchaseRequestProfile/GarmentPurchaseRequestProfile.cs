using AutoMapper;
using Com.Ambassador.Service.Sales.Lib.Models.PurchasingModel.GarmentPurchaseRequest;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel.GarmentPurchaseRequestViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.AutoMapperProfiles.Purchasing.GarmentPurchaseRequestProfile
{
    public class GarmentPurchaseRequestProfile : Profile
    {
        public GarmentPurchaseRequestProfile()
        {
            CreateMap<GarmentPurchaseRequestItems, GarmentPurchaseRequestItemViewModel>()
                .ForPath(d => d.Product.Id, opt => opt.MapFrom(s => s.ProductId))
                .ForPath(d => d.Product.Code, opt => opt.MapFrom(s => s.ProductCode))
                .ForPath(d => d.Product.Name, opt => opt.MapFrom(s => s.ProductName))

                .ForPath(d => d.Uom.Id, opt => opt.MapFrom(s => s.UomId))
                .ForPath(d => d.Uom.Unit, opt => opt.MapFrom(s => s.UomUnit))

                //.ForPath(d => d.PriceUom.Id, opt => opt.MapFrom(s => s.PriceUomId))
                //.ForPath(d => d.PriceUom.Unit, opt => opt.MapFrom(s => s.PriceUomUnit))

                .ForPath(d => d.Category.Id, opt => opt.MapFrom(s => s.CategoryId))
                .ForPath(d => d.Category.Name, opt => opt.MapFrom(s => s.CategoryName))
                .ReverseMap();

            CreateMap<GarmentPurchaseRequests, GarmentPurchaseRequestViewModel>()
                .ForPath(d => d.Buyer.Id, opt => opt.MapFrom(s => s.BuyerId))
                .ForPath(d => d.Buyer.Code, opt => opt.MapFrom(s => s.BuyerCode))
                .ForPath(d => d.Buyer.Name, opt => opt.MapFrom(s => s.BuyerName))

                .ForPath(d => d.Unit.Id, opt => opt.MapFrom(s => s.UnitId))
                .ForPath(d => d.Unit.Code, opt => opt.MapFrom(s => s.UnitCode))
                .ForPath(d => d.Unit.Name, opt => opt.MapFrom(s => s.UnitName))
                .ReverseMap();
        }
    }
}
