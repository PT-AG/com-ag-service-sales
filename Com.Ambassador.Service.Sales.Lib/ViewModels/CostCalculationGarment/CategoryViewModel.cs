using Com.Ambassador.Service.Sales.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment
{
	public class CategoryViewModel : BaseViewModel, IValidatableObject
	{
		public string code { get; set; }
		public string name { get; set; }
		public string SubCategory { get; set; }
        public string CodeRequirement { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrWhiteSpace(this.name))
				yield return new ValidationResult("Nama Kategori harus diisi", new List<string> { "Name" });
		}
	}
}
