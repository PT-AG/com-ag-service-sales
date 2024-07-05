using Com.Ambassador.Service.Sales.Lib.Models;
using Com.Ambassador.Service.Sales.Lib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Logic
{
    public class LogHistoryLogic
    {
        private readonly SalesDbContext DbContext;
        protected DbSet<LogHistory> DbSet;
        protected IIdentityService IdentityService;
        public readonly IServiceProvider serviceProvider;

        public LogHistoryLogic(IIdentityService IdentityService, IServiceProvider serviceProvider, SalesDbContext dbContext)
        {
            this.DbContext = dbContext;
            this.DbSet = dbContext.Set<LogHistory>();
            this.IdentityService = IdentityService;
            this.serviceProvider = serviceProvider;
        }

        public void Create(string division, string activity)
        {
            LogHistory model = new LogHistory
            {
                Division = division,
                Activity = activity,
                CreatedDate = DateTime.Now,
                CreatedBy = IdentityService.Username
            };

            DbSet.Add(model);
        }

        public void Create(string division, string activity,string remark)
        {
            LogHistory model = new LogHistory
            {
                Division = division,
                Activity = activity,
                CreatedDate = DateTime.Now,
                CreatedBy = IdentityService.Username,
                Remark = remark
            };

            DbSet.Add(model);
        }
    }
}
