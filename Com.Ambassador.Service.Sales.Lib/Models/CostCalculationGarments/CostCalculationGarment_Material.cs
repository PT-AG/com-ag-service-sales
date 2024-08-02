using Com.Ambassador.Service.Sales.Lib.Utilities.BaseClass;
using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.Models.CostCalculationGarments
{
    public class CostCalculationGarment_Material : BaseModel
    {
        public long CostCalculationGarmentId { get; set; }
        [ForeignKey("CostCalculationGarmentId")]
        public virtual CostCalculationGarment CostCalculationGarment { get; set; }
        public int MaterialIndex { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string PO_SerialNumber { get; set; }
        [MaxLength(50)]
        public string PO { get; set; }
        [MaxLength(50)]
        public string CategoryId { get; set; }
        [MaxLength(50)]
        public string CategoryCode { get; set; }
        [MaxLength(255)]
        public string CategoryName { get; set; }
        //public int MaterialId { get; set; }
        //public string MaterialName { get; set; }
        public int AutoIncrementNumber { get; set; }
        [MaxLength(50)]
        public string ProductId { get; set; }
        [MaxLength(50)]
        public string ProductCode { get; set; }
        [MaxLength(255)]
        public string Composition { set; get; }
        [MaxLength(255)]
        public string Construction { set; get; }
        [MaxLength(255)]
        public string Yarn { set; get; }
        [MaxLength(255)]
        public string Width { set; get; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(3000)]
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        [MaxLength(50)]
        public string UOMQuantityId { get; set; }
        [MaxLength(255)]
        public string UOMQuantityName { get; set; }
        public double Price { get; set; }
        [MaxLength(50)]
        public string UOMPriceId { get; set; }
        [MaxLength(255)]
        public string UOMPriceName { get; set; }
        public double Conversion { get; set; }
        public double Total { get; set; }
        public bool isFabricCM { get; set; }
        public double? CM_Price { get; set; }
        public double ShippingFeePortion { get; set; }
        public double TotalShippingFee { get; set; }
        public double BudgetQuantity { get; set; }
        [MaxLength(3000)]
        public string Information { get; set; }
        public bool IsPosted { get; set; }

        public bool? IsPRMaster { get; set; } // Terisi waktu validasi RO, cek apakah barang dibuat PR Master

        public long PRMasterId { get; set; }
        public long PRMasterItemId { get; set; }
        [MaxLength(50)]
        public string POMaster { get; set; }
    }
}
