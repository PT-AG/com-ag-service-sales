using Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments;
using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.Models.ROGarments
{
    public class RO_Garment : BaseModel
    {
        public long CostCalculationGarmentId { get; set; }
        [ForeignKey("CostCalculationGarmentId")]
        public virtual CostCalculationGarment CostCalculationGarment { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        public ICollection<RO_Garment_SizeBreakdown> RO_Garment_SizeBreakdowns { get; set; }
        public string Instruction { get; set; }
        public int Total { get; set; }
        public List<string> ImagesFile { get; set; }
        public List<string> DocumentsFile { get; set; }
        [MaxLength(1000)]
        public string ImagesPath { get; set; }
        [MaxLength(1000)]
        public string DocumentsPath { get; set; }
        [MaxLength(1000)]
        public string ImagesName { get; set; }
        [MaxLength(4000)]
        public string DocumentsFileName { get; set; }

        public bool IsPosted { get; set; }
        public bool IsRejected { get; set; }
        public string RejectReason { get; set; }
    }
}
