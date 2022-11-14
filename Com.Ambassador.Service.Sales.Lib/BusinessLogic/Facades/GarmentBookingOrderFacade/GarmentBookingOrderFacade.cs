using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentBookingOrderInterface;
using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic.GarmentBookingOrderLogics;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades.GarmentBookingOrderFacade
{
    public class GarmentBookingOrderFacade : IGarmentBookingOrder
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<GarmentBookingOrder> DbSet;
        private readonly IdentityService identityService;
        private readonly GarmentBookingOrderLogic garmentBookingOrderLogic;
        public IServiceProvider ServiceProvider;

        public GarmentBookingOrderFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<GarmentBookingOrder>();
            identityService = serviceProvider.GetService<IdentityService>();
            garmentBookingOrderLogic = serviceProvider.GetService<GarmentBookingOrderLogic>();
            ServiceProvider = serviceProvider;
        }

        public async Task<int> CreateAsync(GarmentBookingOrder model)
        {
            garmentBookingOrderLogic.Create(model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            await garmentBookingOrderLogic.DeleteAsync(id);
            return await DbContext.SaveChangesAsync();
        }

        public ReadResponse<GarmentBookingOrder> Read(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return garmentBookingOrderLogic.Read(page, size, order, select, keyword, filter);
        }

        public ReadResponse<GarmentBookingOrder> ReadByBookingOrderNo(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return garmentBookingOrderLogic.ReadByBookingOrderNo(page, size, order, select, keyword, filter);
        }

        public ReadResponse<GarmentBookingOrderForCCGViewModel> ReadByBookingOrderNoForCCG(int page, int size, string order, List<string> select, string keyword, string filter)
        {
            return garmentBookingOrderLogic.ReadByBookingOrderNoForCCG(page, size, order, select, keyword, filter);
        }

        public async Task<GarmentBookingOrder> ReadByIdAsync(int id)
        {
            return await garmentBookingOrderLogic.ReadByIdAsync(id);
        }

        public async Task<int> UpdateAsync(int id, GarmentBookingOrder model)
        {
            garmentBookingOrderLogic.UpdateAsync(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> BOCancel(int id, GarmentBookingOrder model)  
        {
            garmentBookingOrderLogic.BOCancel(id, model);
            return await DbContext.SaveChangesAsync();
        }

        public async Task<int> BODelete(int id, GarmentBookingOrder model)
        {
            garmentBookingOrderLogic.BODelete(id, model);
            return await DbContext.SaveChangesAsync();
        }
    }
}
