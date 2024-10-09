using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseInterface;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment.Cancel_Approval;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.CostCalculationGarmentLogic
{
    public interface ICostCalculationGarment : IBaseFacade<CostCalculationGarment>
    {
        Task<Dictionary<long, string>> GetProductNames(List<long> productIds);
        ReadResponse<CostCalculationGarment> ReadForROAcceptance(int page, int size, string order, List<string> select, string keyword, string filter);
        Task<int> AcceptanceCC(List<long> listId, string user);
        ReadResponse<CostCalculationGarment> ReadForROAvailable(int page, int size, string order, List<string> select, string keyword, string filter);
        Task<int> AvailableCC(List<long> listId, string user);
        ReadResponse<CostCalculationGarment> ReadForRODistribution(int page, int size, string order, List<string> select, string keyword, string filter);
        Task<int> DistributeCC(List<long> listId, string user);
        Task<int> PostCC(List<long> listId);
        Task<int> UnpostCC(long id, string reason);
        Task<int> Patch(long id, JsonPatchDocument<CostCalculationGarment> jsonPatch);
        List<string> ReadUnpostReasonCreators(string keyword, int page, int size);
		List<CostCalculationGarmentDataProductionReport> GetComodityQtyOrderHoursBuyerByRo(string ro_Number);
        ReadResponse<dynamic> ReadDynamic(int page, int size, string order, string select, string keyword, string filter, string search);
        ReadResponse<dynamic> ReadMaterials(int page, int size, string order, string select, string keyword, string filter, string search);
        ReadResponse<dynamic> ReadMaterialsByPRMasterItemIds(int page, int size, string order, string select, string keyword, string filter, string search, string prmasteritemids);
        Task<CostCalculationGarment> ReadByRO(string ro);
        List<CostCalculationGarmentForJournal> GetCCGByRo(string RO_Number);
        #region CancelApproval
        ReadResponse<CostCalculationGarment> ReadForCancelApproval(int page, int size, string order, List<string> select, string keyword, string filter);
        Task<int> CancelApproval(long id, string deletedRemark);
        Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadCancelApproval(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset);
        MemoryStream GenerateExcelCancelApproval(DateTime? dateFrom, DateTime? dateTo, int offset);

        Tuple<List<CancelApprovalCostCalculationReportViewModel>, int> ReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int page, int size, int offset);
        MemoryStream GenerateExcelReadRejectRO(DateTime? dateFrom, DateTime? dateTo, int offset);
        #endregion
    }
}
