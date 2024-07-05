using Com.Ambassador.Service.Sales.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface
{
    public interface ILogHistoryFacade
    {
        Task<List<LogHistoryViewModel>> GetReportQuery(DateTime dateFrom, DateTime dateTo);
    }
}
