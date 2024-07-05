using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment.Cancel_Approval
{
    public class CancelApprovalCostCalculationReportViewModel
    {
        public string Activity { get; set; }
        public DateTime CancelDate { get; set; }
        public string CancelBy { get; set; }
        public string RequestedBy { get; set; }
        public string CancelReason { get; set; }
    }
}
