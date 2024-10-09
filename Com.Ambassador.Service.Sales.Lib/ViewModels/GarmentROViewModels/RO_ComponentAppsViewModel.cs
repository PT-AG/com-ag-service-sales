using Com.Ambassador.Service.Sales.Lib.Models.ROGarments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentROViewModels
{
    public class RO_ComponentAppsViewModel
    {
        public string Ro_Number { get; set; }
        public string Article { get; set; }
        public List<Colors> Colors { get; set; }

        public RO_ComponentAppsViewModel(RO_Garment data)
        {
            Ro_Number = data.CostCalculationGarment.RO_Number;
            Article = data.CostCalculationGarment.Article;
            Colors = data.RO_Garment_SizeBreakdowns.Select(x => new Colors(x)).ToList();

        }

    }


    public class Colors
    {
        public string Color { get; set; }
        public List<Sizes> Sizes { get; set; }

        public Colors(RO_Garment_SizeBreakdown data)
        {
            Color = data.ColorName;
            Sizes = data.RO_Garment_SizeBreakdown_Details.Select(x => new Sizes(x)).ToList();

        }
    }

    public class Sizes
    {
        public string Size { get; set; }

        public Sizes(RO_Garment_SizeBreakdown_Detail data)
        {
            Size = data.Size;
        }
    }


}
