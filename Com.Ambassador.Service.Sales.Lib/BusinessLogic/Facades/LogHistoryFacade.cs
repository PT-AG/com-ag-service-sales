using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface;
using Com.Ambassador.Service.Sales.Lib.Models;
using Com.Ambassador.Service.Sales.Lib.Services;
using Com.Ambassador.Service.Sales.Lib.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Facades
{
    public class LogHistoryFacade : ILogHistoryFacade
    {
        private readonly SalesDbContext DbContext;
        private readonly DbSet<LogHistory> DbSet;
        private readonly IdentityService identityService;

        public IServiceProvider ServiceProvider;
        public LogHistoryFacade(IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<LogHistory>();
            identityService = serviceProvider.GetService<IdentityService>();
            ServiceProvider = serviceProvider;
        }

        public async Task<List<LogHistoryViewModel>> GetReportQuery(DateTime dateFrom, DateTime dateTo)
        {
            var query = await DbSet.Where(x => x.CreatedDate.AddHours(7).Date >= dateFrom.Date && x.CreatedDate.AddHours(7).Date <= dateTo.Date)
                .Select(x => new LogHistoryViewModel
                {
                    Activity = x.Activity,
                    Division = x.Division,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate.AddHours(7)
                }).ToListAsync();

            return query;
        }
    }
}
