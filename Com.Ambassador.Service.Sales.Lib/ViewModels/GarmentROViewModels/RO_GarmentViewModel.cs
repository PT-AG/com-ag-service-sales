using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentROViewModels
{
    public class RO_GarmentViewModel : BaseViewModel, IValidatableObject
    {
        public string Code { get; set; }
        public CostCalculationGarmentViewModel CostCalculationGarment { get; set; }
        public List<RO_Garment_SizeBreakdownViewModel> RO_Garment_SizeBreakdowns { get; set; }
        public string Instruction { get; set; }
        public int Total { get; set; }
        public List<string> ImagesFile { get; set; }
        public List<string> ImagesPath { get; set; }
        public List<string> ImagesName { get; set; }
        public List<string> DocumentsFile { get; set; }
        public List<string> DocumentsFileName { get; set; }
        public List<string> DocumentsPath { get; set; }

        public bool IsPosted { get; set; }
        public bool IsRejected { get; set; }
        public string RejectReason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.CostCalculationGarment == null || string.IsNullOrWhiteSpace(CostCalculationGarment.RO_Number))
                yield return new ValidationResult("Nomor RO harus diisi", new List<string> { "CostCalculationGarment" });

            if (string.IsNullOrWhiteSpace(Instruction))
            {
                yield return new ValidationResult("Instruksi harus diisi", new List<string> { "Instruction" });
            }

            if (ImagesFile == null || ImagesFile.Count.Equals(0))
            {
                yield return new ValidationResult("Gambar harus Ada", new List<string> { "ImageFile" });
            }
            else if (ImagesName.Count.Equals(0) || ImagesName.Count != ImagesFile.Count)
            {
                yield return new ValidationResult("Nama Gambar harus diisi", new List<string> { "ImageFile" });
            }

            if (DocumentsFileName != null && DocumentsFileName.Count > 0)
            {
                int DocumentsFileErrorCount = 0;
                string DocumentsFileError = "[";

                foreach (var doc in DocumentsFileName)
                {
                    if (string.IsNullOrWhiteSpace(doc))
                    {
                        DocumentsFileError += "'Tidak ada file dipilih',";
                        DocumentsFileErrorCount++;
                    }
                    else
                    {
                        DocumentsFileError += ",";
                    }
                }

                DocumentsFileError += "]";

                if (DocumentsFileErrorCount > 0)
                {
                    yield return new ValidationResult(DocumentsFileError, new List<string> { "DocumentsFile" });
                }
            }

            if (this.RO_Garment_SizeBreakdowns == null || this.RO_Garment_SizeBreakdowns.Count == 0)
                yield return new ValidationResult("Size Breakdown harus diisi", new List<string> { "SizeBreakdowns" });
            else
            {
                int Count = 0;
                string error = "[";
                int QtyTotal = 0;

                foreach (var item in this.RO_Garment_SizeBreakdowns)
                {
                    QtyTotal += item.Total;
                }

                if (Total != QtyTotal)
                {
                    yield return new ValidationResult("Total Jumlah Size Breakdown harus sama dengan Quantity RO", new List<string> { "TotalQuantity" });
                }

                foreach (var item in this.RO_Garment_SizeBreakdowns)
                {

                    error += " { ";

                    if (item.Color == null || string.IsNullOrWhiteSpace(item.Color.Name))
                    {
                        Count++;
                        error += "Color: 'Color harus diisi', ";
                    }

                    if (item.RO_Garment_SizeBreakdown_Details == null || item.RO_Garment_SizeBreakdown_Details.Count == 0)
                    {
                        yield return new ValidationResult("Details harus diisi", new List<string> { "SizeBreakdownDetails" });

                    }
                    else
                    {
                        int DetailCount = 0;
                        string Detailerror = "[";
                        foreach (var detail in item.RO_Garment_SizeBreakdown_Details)
                        {
                            Detailerror += " { ";
                            if (string.IsNullOrWhiteSpace(detail.Size))
                            {
                                DetailCount++;
                                Detailerror += "Size: 'Size harus diisi', ";
                            }
                            if (detail.Quantity <= 0)
                            {
                                DetailCount++;
                                Detailerror += "Quantity: 'Quantity harus lebih dari 0', ";
                            }
                            if (string.IsNullOrWhiteSpace(detail.Information))
                            {
                                DetailCount++;
                                Detailerror += "Information: 'Keterangan harus diisi', ";
                            }
                            Detailerror += " }, ";
                        }
                        Detailerror += "]";
                        if (DetailCount > 0)
                        {
                            Count++;
                            error += string.Concat("SizeBreakdownDetails: ", Detailerror);
                            //yield return new ValidationResult(Detailerror, new List<string> { "SizeBreakdownDetails" });
                        }
                    }

                    error += " }, ";
                }

                error += "]";

                if (Count > 0)
                {
                    yield return new ValidationResult(error, new List<string> { "SizeBreakdownItems" });
                }
            }
        }
    }
}
