using Com.Ambassador.Service.Sales.Lib.Models.GarmentBookingOrderModel;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseInterface;
using Com.Danliris.Service.Sales.Lib.ViewModels.GarmentBookingOrderViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentBookingOrderInterface
{
    public interface IGarmentBookingOrder : IBaseFacade<GarmentBookingOrder>
    {
        Task<int> BODelete(int id, GarmentBookingOrder model);
        Task<int> BOCancel(int id, GarmentBookingOrder model);
        ReadResponse<GarmentBookingOrder> ReadByBookingOrderNo(int page, int size, string order, List<string> select, string keyword, string filter);
        ReadResponse<GarmentBookingOrderForCCGViewModel> ReadByBookingOrderNoForCCG(int page, int size, string order, List<string> select, string keyword, string filter);

    }
}
