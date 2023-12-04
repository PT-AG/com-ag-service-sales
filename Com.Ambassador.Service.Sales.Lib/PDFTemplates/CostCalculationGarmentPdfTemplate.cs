using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.PDFTemplates
{
    public class CostCalculationGarmentPdfTemplate
    {
        private string GetCurrencyValue(double value, bool isDollar)
        {
            if (isDollar)
            {
                return Number.ToDollar(value);
            }
            else
            {
                return Number.ToRupiah(value);
            }
        }

        public MemoryStream GeneratePdfTemplate(CostCalculationGarmentViewModel viewModel, int timeoffset)
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
            BaseFont bf_bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
            Font bold_font_8 = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            DateTime now = DateTime.Now;

            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;
            document.Open();
            PdfContentByte cb = writer.DirectContent;

            float margin = 10;
            float printedOnHeight = 10;
            float startY = 840 - margin;

            #region Header
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PT. AMBASSADOR GARMINDO", 10, 820, 0);
            cb.SetFontAndSize(bf_bold, 12);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "COST CALCULATION GARMENT" + (viewModel.IsPosted ? "" : " (DRAFT)"), 10, 805, 0);
            cb.EndText();
            #endregion

            #region Detail 1 (Top)
            PdfPTable table_detail1 = new PdfPTable(9);
            table_detail1.TotalWidth = 500f;

            float[] detail1_widths = new float[] { 1f, 0.1f, 2f, 1f, 0.1f, 2f, 1f, 0.1f, 2f };
            table_detail1.SetWidths(detail1_widths);

            PdfPCell cell_detail1 = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 1, PaddingBottom = 2, PaddingTop = 2 };
            PdfPCell cell_colon = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE };
            cell_colon.Phrase = new Phrase(":", normal_font);

            cell_detail1.Phrase = new Phrase("NO. RO", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.RO_Number}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("NO. PRE SC", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.PreSCNo}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("LEAD TIME", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.LeadTime} hari", normal_font);
            table_detail1.AddCell(cell_detail1);

            cell_detail1.Phrase = new Phrase("ARTICLE", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.Article}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("SECTION", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.Section}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("FABRIC", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.FabricAllowance}%", normal_font);
            table_detail1.AddCell(cell_detail1);

            cell_detail1.Phrase = new Phrase("DATE", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.CreatedUtc.ToString("dd MMMM yyyy")}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("COMMODITY", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.Comodity.Name}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("ACC", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.AccessoriesAllowance}%", normal_font);
            table_detail1.AddCell(cell_detail1);

            cell_detail1.Phrase = new Phrase("KONVEKSI", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.Unit.Code}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("SIZE RANGE", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_colon);
            cell_detail1.Phrase = new Phrase($"{viewModel.SizeRange}", normal_font);
            table_detail1.AddCell(cell_detail1);
            cell_detail1.Phrase = new Phrase("", normal_font);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_detail1);
            table_detail1.AddCell(cell_detail1);
            #endregion

            #region Image
            float imageHeight;
            try
            {
                byte[] imageByte = Convert.FromBase64String(Base64.GetBase64File(viewModel.ImageFile));
                Image image = Image.GetInstance(imgb: imageByte);
                if (image.Width > 60)
                {
                    float percentage = 0.0f;
                    percentage = 60 / image.Width;
                    image.ScalePercent(percentage * 100);
                }
                imageHeight = image.ScaledHeight;
                float imageY = 800 - imageHeight;
                image.SetAbsolutePosition(520, imageY);
                cb.AddImage(image, inlineImage: true);
            }
            catch (Exception)
            {
                imageHeight = 0;
            }
            #endregion

            #region Draw Top
            float row1Y = 800;
            table_detail1.WriteSelectedRows(0, -1, 10, row1Y, cb);
            #endregion

            bool isDollar = viewModel.Rate.Value != 1;

            #region Detail 2.1 (Bottom, Column 1.1)
            string fabric = "";
            List<string> remark = new List<string>();
            for (int i = 0; i < viewModel.CostCalculationGarment_Materials.Count; i++)
            {

                if (viewModel.CostCalculationGarment_Materials[i].Category.name == "FABRIC")
                {
                    remark.Add(viewModel.CostCalculationGarment_Materials[i].Product.Composition + " " + viewModel.CostCalculationGarment_Materials[i].Product.Const + " " + viewModel.CostCalculationGarment_Materials[i].Product.Yarn + " " + viewModel.CostCalculationGarment_Materials[i].Product.Width);
                }
            }
            if (remark.Count == 0)
                fabric = viewModel.Description;
            else
            {
                foreach (var data in remark)
                {
                    fabric += "FABRIC \n" + data + "\n\n";
                }
                fabric += viewModel.Description + "\n";
            }
            PdfPTable table_bottom_column1_1 = new PdfPTable(2);
            table_bottom_column1_1.TotalWidth = 180f;

            float[] table_bottom_column1_1_widths = new float[] { 1f, 2f };
            table_bottom_column1_1.SetWidths(table_bottom_column1_1_widths);

            PdfPCell cell_bottom_column1_1 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 4, PaddingLeft = 2, PaddingTop = 4 };

            cell_bottom_column1_1.Phrase = new Phrase("QTY", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.Quantity} {viewModel.UOM.Unit}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);

            cell_bottom_column1_1.Phrase = new Phrase("DESCRIPTION", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.Comodity.Code}" + " - " + $"{viewModel.CommodityDescription}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);

            cell_bottom_column1_1.Phrase = new Phrase("CONT/STYLE", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.Article}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);

            cell_bottom_column1_1.Phrase = new Phrase("BUYER AGENT", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.Buyer.Code}" + " - " + $"{viewModel.Buyer.Name}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);

            cell_bottom_column1_1.Phrase = new Phrase("BUYER BRAND", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.BuyerBrand.Code}" + " - " + $"{viewModel.BuyerBrand.Name}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);

            cell_bottom_column1_1.Phrase = new Phrase("DELIVERY", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            cell_bottom_column1_1.Phrase = new Phrase($"{viewModel.DeliveryDate.AddHours(timeoffset).ToString("dd/MM/yyyy")}", normal_font);
            table_bottom_column1_1.AddCell(cell_bottom_column1_1);
            #endregion

            #region Detail 2_2 (Bottom, Column 1.2)
            PdfPTable table_bottom_column1_2 = new PdfPTable(2);
            table_bottom_column1_2.TotalWidth = 180f;

            float[] table_bottom_column1_2_widths = new float[] { 1f, 1f };
            table_bottom_column1_2.SetWidths(table_bottom_column1_2_widths);

            PdfPCell cell_bottom_column1_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 3 };

            cell_bottom_column1_2.Phrase = new Phrase("FOB PRICE", bold_font);
            table_bottom_column1_2.AddCell(cell_bottom_column1_2);
            cell_bottom_column1_2.Phrase = new Phrase("CMT PRICE", bold_font);
            table_bottom_column1_2.AddCell(cell_bottom_column1_2);

            double CM_Price = 0;
            var isFabricCM = 0;
            foreach (CostCalculationGarment_MaterialViewModel item in viewModel.CostCalculationGarment_Materials)
            {
                CM_Price += item.CM_Price ?? 0;
                if (item.isFabricCM)
                {
                    isFabricCM++;
                }
            }
            double ConfirmPrice = viewModel.ConfirmPrice ?? 0;
            double CMT = CM_Price > 0 ? ConfirmPrice : 0;
            double FOB = ConfirmPrice;
            double FOB_Remark = 0;
            if (CMT > 0)
            {
                FOB = 0;
                var b = Convert.ToDouble(viewModel.Rate.Value);
                double a = (1.05 * CM_Price / Convert.ToDouble(viewModel.Rate.Value)) - (viewModel.Insurance.GetValueOrDefault() + viewModel.Freight.GetValueOrDefault());
                FOB_Remark = ConfirmPrice + a;
            }
            string CMT_Price = CMT > 0 ? this.GetCurrencyValue(CMT, isDollar) : isFabricCM > 0 ? this.GetCurrencyValue(FOB, isDollar) : this.GetCurrencyValue(0, isDollar);
            string FOB_Price = isFabricCM > 0 ? this.GetCurrencyValue(0, isDollar) : this.GetCurrencyValue(FOB, isDollar);

            cell_bottom_column1_2.Phrase = new Phrase($"{FOB_Price}", normal_font);
            table_bottom_column1_2.AddCell(cell_bottom_column1_2);
            cell_bottom_column1_2.Phrase = new Phrase($"{CMT_Price}", normal_font);
            table_bottom_column1_2.AddCell(cell_bottom_column1_2);
            #endregion

            #region Detail 2.3 (Bottom, Column 1.3)
            PdfPTable table_bottom_column1_3 = new PdfPTable(2);
            table_bottom_column1_3.TotalWidth = 180f;

            float[] table_bottom_column1_3_widths = new float[] { 1f, 1f };
            table_bottom_column1_3.SetWidths(table_bottom_column1_3_widths);

            PdfPCell cell_bottom_column1_3 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 3 };

            cell_bottom_column1_3.Phrase = new Phrase("CNF PRICE", bold_font);
            table_bottom_column1_3.AddCell(cell_bottom_column1_3);
            cell_bottom_column1_3.Phrase = new Phrase("CIF PRICE", bold_font);
            table_bottom_column1_3.AddCell(cell_bottom_column1_3);

            string CNF_Price = this.GetCurrencyValue(Convert.ToDouble(viewModel.Freight + viewModel.ConfirmPrice), isDollar);
            if (viewModel.Freight == 0) CNF_Price = "$";
            cell_bottom_column1_3.Phrase = new Phrase($"{CNF_Price}", normal_font);
            table_bottom_column1_3.AddCell(cell_bottom_column1_3);
            string CIF_Price = this.GetCurrencyValue(Convert.ToDouble(viewModel.Insurance + viewModel.ConfirmPrice), isDollar);
            if (viewModel.Insurance == 0) CIF_Price = "$";
            cell_bottom_column1_3.Phrase = new Phrase($"{CIF_Price}", normal_font);
            table_bottom_column1_3.AddCell(cell_bottom_column1_3);
            #endregion

            #region Detail 3.1 (Bottom, Column 2.1)
            PdfPTable table_bottom_column2_1 = new PdfPTable(3);
            table_bottom_column2_1.TotalWidth = 190f;

            float[] table_bottom_column2_1_widths = new float[] { 1.5f, 1f, 1.5f };
            table_bottom_column2_1.SetWidths(table_bottom_column2_1_widths);

            PdfPCell cell_bottom_column2_1 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 3 };

            PdfPCell cell_bottom_column2_1_colspan2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 3, Colspan = 3 };

            cell_bottom_column2_1.Phrase = new Phrase("TOTAL", normal_font);

            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double total = 0;
            foreach (CostCalculationGarment_MaterialViewModel item in viewModel.CostCalculationGarment_Materials)
            {
                total += item.Total;
            }
            //total += viewModel.ProductionCost;
            cell_bottom_column2_1_colspan2.Phrase = new Phrase(Number.ToRupiahWithoutSymbol(total), normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1_colspan2);

            cell_bottom_column2_1.Phrase = new Phrase("OTL 1", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double OTL1CalculatedValue = viewModel.OTL1.CalculatedValue ?? 0;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(OTL1CalculatedValue)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double afterOTL1 = total + OTL1CalculatedValue;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(afterOTL1)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);

            cell_bottom_column2_1.Phrase = new Phrase("OTL 2", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double OTL2CalculatedValue = viewModel.OTL2.CalculatedValue ?? 0;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(OTL2CalculatedValue)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double afterOTL2 = afterOTL1 + OTL2CalculatedValue;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(afterOTL2)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);

            cell_bottom_column2_1.Phrase = new Phrase("RISK", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            cell_bottom_column2_1.Phrase = new Phrase(String.Format("{0:0.00}%", viewModel.Risk), normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double afterRisk = (100 + viewModel.Risk) * afterOTL2 / 100; ;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(afterRisk)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);

            cell_bottom_column2_1.Phrase = new Phrase("BEA ANGKUT", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(viewModel.FreightCost)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            double afterFreightCost = afterRisk + viewModel.FreightCost;
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(afterFreightCost)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);

            cell_bottom_column2_1.Phrase = new Phrase("SUB TOTAL", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            cell_bottom_column2_1_colspan2.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(afterFreightCost)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1_colspan2);

            cell_bottom_column2_1.Phrase = new Phrase("NET/FOB (%)", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            cell_bottom_column2_1.Phrase = new Phrase(String.Format("{0:0.00}%", viewModel.NETFOBP), normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);
            cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(viewModel.NETFOB)}", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1);

			cell_bottom_column2_1.Phrase = new Phrase("COMM (%)", normal_font);
			table_bottom_column2_1.AddCell(cell_bottom_column2_1);
			cell_bottom_column2_1.Phrase = new Phrase(String.Format("{0:0.00}%", viewModel.CommissionPortion), normal_font);
			table_bottom_column2_1.AddCell(cell_bottom_column2_1);
			cell_bottom_column2_1.Phrase = new Phrase($"{Number.ToRupiahWithoutSymbol(viewModel.CommissionRate)}", normal_font);
			table_bottom_column2_1.AddCell(cell_bottom_column2_1);

            cell_bottom_column2_1.Phrase = new Phrase("CONFIRM PRICE", normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1); ;
            double confirmPrice = viewModel.ConfirmPrice ?? 0 + viewModel.Rate.Value ?? 0;
            double confirmPriceWithRate = isDollar ? confirmPrice * viewModel.Rate.Value ?? 1 : confirmPrice;
            cell_bottom_column2_1_colspan2.Phrase = new Phrase(string.Format("{0:n2}", confirmPriceWithRate), normal_font);
            table_bottom_column2_1.AddCell(cell_bottom_column2_1_colspan2);
            #endregion

            #region Detail 3.2 (Bottom, Column 2.2)
            PdfPTable table_bottom_column2_2 = new PdfPTable(4);
            table_bottom_column2_2.TotalWidth = 180f;

            float[] table_bottom_column2_2_widths = new float[] { 1f, 1.25f, 1f, 1.25f };
            table_bottom_column2_2.SetWidths(table_bottom_column2_2_widths);
            PdfPCell cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 2, PaddingBottom = 4, PaddingLeft = 2, Colspan = 2 };

            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 2, PaddingLeft = 2, Colspan = 2 };
            cell_bottom_column2_2.Phrase = new Phrase("FREIGHT", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 2, PaddingLeft = 2, Colspan = 2 };
            string freight = this.GetCurrencyValue(viewModel.Freight ?? 0, isDollar);
            cell_bottom_column2_2.Phrase = new Phrase($"= {freight}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);

            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.LEFT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 2, PaddingBottom = 4, PaddingLeft = 2, Colspan = 2 };
            cell_bottom_column2_2.Phrase = new Phrase("INSURANCE", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 2, PaddingBottom = 4, PaddingLeft = 2, Colspan = 2 };
            string insurance = this.GetCurrencyValue(viewModel.Insurance ?? 0, isDollar);
            cell_bottom_column2_2.Phrase = new Phrase($"= {insurance}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);

            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3, Colspan = 2 };
            cell_bottom_column2_2.Phrase = new Phrase("CONFIRM PRICE", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3, Colspan = 2 };
            string confirmPriceFOB = this.GetCurrencyValue(viewModel.ConfirmPrice ?? 0, isDollar);
            cell_bottom_column2_2.Phrase = new Phrase($"= {confirmPriceFOB}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);

            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase("SMV CUT", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase($"= {viewModel.SMV_Cutting}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase("SMV SEW", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase($"= {viewModel.SMV_Sewing}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);

            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase("SMV FIN", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase($"= {viewModel.SMV_Finishing}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase("SMV TOT", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            cell_bottom_column2_2 = new PdfPCell() { Border = Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingTop = 4, PaddingRight = 3, PaddingBottom = 4, PaddingLeft = 3 };
            cell_bottom_column2_2.Phrase = new Phrase($"= {viewModel.SMV_Total}", normal_font);
            table_bottom_column2_2.AddCell(cell_bottom_column2_2);
            #endregion

            #region Detail 4 (Bottom, Column 3.1)
            PdfPTable table_bottom_column3_1 = new PdfPTable(2);
            table_bottom_column3_1.TotalWidth = 180f;

            float[] table_bottom_column3_1_widths = new float[] { 1f, 2f };
            table_bottom_column3_1.SetWidths(table_bottom_column3_1_widths);

            PdfPCell cell_bottom_column3_1 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7 };



            var DESC = (viewModel.CostCalculationGarment_Materials.Any(m => m.isFabricCM) ? "FOB PRICE : $ " + Number.ToRupiahWithoutSymbol(FOB_Remark) + "\n\n" : string.Empty) + fabric;
            //         cell_bottom_column3_1.Phrase = new Phrase("DESCRIPTION", normal_font);
            //table_bottom_column3_1.AddCell(cell_bottom_column3_1);
            //cell_bottom_column3_1.Phrase = new Phrase($"{viewModel.SizeRange + "\n" + viewModel.FabricAllowance + " - " + viewModel.AccessoriesAllowance + "\n" + fabric}", normal_font);
            //table_bottom_column3_1.AddCell(cell_bottom_column3_1);
            #endregion

            #region Signature
            PdfPTable table_signature = new PdfPTable(4);
            table_signature.TotalWidth = 570f;

            float[] signature_widths = new float[] { 1f, 1f, 1f, 1f };
            table_signature.SetWidths(signature_widths);

            PdfPCell cell_signature = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);
            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);

            string signatureArea = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                signatureArea += Environment.NewLine;
            }
            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            table_signature.AddCell(cell_signature);

            var AssignmentKabag = "";
            var AssignmentKadiv = "";


            if (viewModel.ApprovalMD.IsApproved)
            {
                AssignmentKabag = viewModel.ApprovalMD.ApprovedBy;
            }
            else
            {
                AssignmentKabag = " ____________________ ";
            }

            if (viewModel.ApprovalKadivMD.IsApproved)
            {
                AssignmentKadiv = viewModel.ApprovalKadivMD.ApprovedBy;
            }
            else
            {
                AssignmentKadiv = " ____________________ ";
            }

            string AssignMD = viewModel.IsPosted ? viewModel.CreatedBy : " ";

            cell_signature.Phrase = new Phrase("(  " + AssignMD + "  )", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(  ____________________  )", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(  " + AssignmentKabag + "  )", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(  " + AssignmentKadiv + "  )", normal_font);
            table_signature.AddCell(cell_signature);

            cell_signature.Phrase = new Phrase("Bag. Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Marketing", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Ka. Sie/Ka. Bag Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Direktur Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            #endregion

            #region Cost Calculation Material


            PdfPTable table_outer = new PdfPTable(2);
            table_outer.TotalWidth = 570f;

            float[] outer_widths = new float[] { 10f, 5f };
            table_outer.SetWidths(outer_widths);

            PdfPTable table_ccm = new PdfPTable(7);
            table_ccm.TotalWidth = 520f;

            float[] ccm_widths = new float[] { 1f, 2f, 1.5f, 3.5f, 2.5f, 3f, 2f };
            table_ccm.SetWidths(ccm_widths);

            PdfPCell cell_ccm_center = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            PdfPCell cell_ccm_left = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            PdfPCell cell_ccm_right = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

            cell_ccm_center.Phrase = new Phrase("NO", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("KATEGORI", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("KODE", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("DESKRIPSI", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("QUANTITY", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("RP.PTC/PC", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            cell_ccm_center.Phrase = new Phrase("RP.TOTAL", bold_font);
            table_ccm.AddCell(cell_ccm_center);

            //double Total = 0;
            float row1Height = imageHeight > table_detail1.TotalHeight ? imageHeight : table_detail1.TotalHeight;
            float row2Y = row1Y - row1Height - 10;
            float[] row3Heights = { table_bottom_column1_1.TotalHeight, table_bottom_column2_1.TotalHeight, table_bottom_column3_1.TotalHeight + 10 + table_bottom_column2_2.TotalHeight + table_bottom_column1_2.TotalHeight + 2 + table_bottom_column1_3.TotalHeight + 2 };
            float dollarDetailHeight = 10;
            if (isDollar)
                row3Heights[1] += dollarDetailHeight;
            float row3Height = row3Heights.Max();
            float secondHighestRow3Height = row3Heights[1] > row3Heights[2] ? row3Heights[1] : row3Heights[2];
            bool signatureInsideRow3 = row3Heights.Max() == row3Heights[0] && row3Heights[0] - 10 - secondHighestRow3Height > table_signature.TotalHeight;
            float row2RemainingHeight = row2Y - 10 - row3Height - printedOnHeight - margin;
            float row2AllowedHeight = row2Y - printedOnHeight - margin;

            for (int i = 0; i < viewModel.CostCalculationGarment_Materials.Count; i++)
            {
                //NO
                cell_ccm_center.Phrase = new Phrase((i + 1).ToString(), normal_font);
                table_ccm.AddCell(cell_ccm_center);

                //KATEGORI
                cell_ccm_left.Phrase = new Phrase(viewModel.CostCalculationGarment_Materials[i].Category.name, normal_font);
                table_ccm.AddCell(cell_ccm_left);

                //KODE PRODUK
                cell_ccm_left.Phrase = new Phrase(viewModel.CostCalculationGarment_Materials[i].Product.Code, normal_font);
                table_ccm.AddCell(cell_ccm_left);

                //DESKRIPSI
                cell_ccm_left.Phrase = new Phrase(viewModel.CostCalculationGarment_Materials[i].Description + " - " + viewModel.CostCalculationGarment_Materials[i].ProductRemark, normal_font);
                table_ccm.AddCell(cell_ccm_left);

                cell_ccm_right.Phrase = new Phrase(String.Format("{0} {1}", viewModel.CostCalculationGarment_Materials[i].Quantity, viewModel.CostCalculationGarment_Materials[i].UOMQuantity.Unit), normal_font);
                table_ccm.AddCell(cell_ccm_right);

                cell_ccm_right.Phrase = new Phrase(String.Format("{0} / {1}", string.Format("{0:n2}", viewModel.CostCalculationGarment_Materials[i].isFabricCM ? 0 : viewModel.CostCalculationGarment_Materials[i].Price), viewModel.CostCalculationGarment_Materials[i].UOMPrice.Unit), normal_font);
                table_ccm.AddCell(cell_ccm_right);

                cell_ccm_right.Phrase = new Phrase(string.Format("{0:n2}", viewModel.CostCalculationGarment_Materials[i].Total), normal_font);
                table_ccm.AddCell(cell_ccm_right);

                //Total += viewModel.CostCalculationGarment_Materials[i].Total;
                //float currentHeight = table_ccm.TotalHeight;
                //if (currentHeight / row2RemainingHeight > 1)
                //{
                //	if (currentHeight / row2AllowedHeight > 1)
                //	{
                //		PdfPRow headerRow = table_ccm.GetRow(0);
                //		PdfPRow lastRow = table_ccm.GetRow(table_ccm.Rows.Count - 1);
                //		table_ccm.DeleteLastRow();
                //		table_ccm.WriteSelectedRows(0, -1, 10, row2Y, cb);
                //		table_ccm.DeleteBodyRows();
                //		this.DrawPrintedOn(now, bf, cb);
                //		document.NewPage();
                //		table_ccm.Rows.Add(headerRow);
                //		table_ccm.Rows.Add(lastRow);
                //		table_ccm.CalculateHeightsFast();
                //		row2Y = startY;
                //		row2RemainingHeight = row2Y - 10 - row3Height - printedOnHeight - margin;
                //		row2AllowedHeight = row2Y - printedOnHeight - margin;
                //	}
                //}
            }

            cell_ccm_right = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2, Colspan = 6 };
            cell_ccm_right.Phrase = new Phrase("", bold_font_8);
            table_ccm.AddCell(cell_ccm_right);

            cell_ccm_right = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            cell_ccm_right.Phrase = new Phrase("", bold_font_8);
            table_ccm.AddCell(cell_ccm_right);
            table_outer.AddCell(table_ccm);

            PdfPCell cell_breakDown_center = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                Padding = 2
            };
            cell_breakDown_center.Phrase = new Phrase("REMARK : \n\n" + DESC, normal_font);

            table_outer.AddCell(cell_breakDown_center);
            #endregion

            #region Draw Middle and Bottom

            //        // table_ccm.WriteSelectedRows(0, -1, 10, row2Y, cb);
            //         table_outer.WriteSelectedRows(0, -1, 10, row2Y, cb);

            //         float row3Y = row2Y - table_outer.TotalHeight - 10;
            //float row3RemainingHeight = row3Y - printedOnHeight - margin;
            //if (row3RemainingHeight < row3Height)
            //{
            //	this.DrawPrintedOn(now, bf, cb);
            //	row3Y = startY;
            //	document.NewPage();
            //}

            //table_bottom_column1_1.WriteSelectedRows(0, -1, 10, row3Y, cb);

            //         table_bottom_column2_1.WriteSelectedRows(0, -1, 200, row3Y, cb);

            //float noteY = row3Y - table_bottom_column2_1.TotalHeight;
            //float table_bottom_column2_2Y;
            //if (isDollar)
            //{
            //	noteY = noteY - 15;
            //	table_bottom_column2_2Y = noteY - 5;
            //	cb.BeginText();
            //	cb.SetFontAndSize(bf, 6);
            //	cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, $"NOTE: 1 US$ = {Number.ToRupiah(viewModel.Rate.Value)}", 200, noteY, 0);
            //	cb.EndText();
            //}
            //else
            //{
            //	table_bottom_column2_2Y = noteY - 10;
            //}
            //         table_bottom_column2_2.WriteSelectedRows(0, -1, 400, row3Y, cb);

            //         float detail1_2Y = row3Y - table_bottom_column2_2.TotalHeight - 2;
            //         table_bottom_column1_2.WriteSelectedRows(0, -1, 400, detail1_2Y, cb);

            //         float detail1_3Y = detail1_2Y - table_bottom_column1_2.TotalHeight - 2;
            //         table_bottom_column1_3.WriteSelectedRows(0, -1, 400, detail1_3Y, cb);


            //         //table_bottom_column1_2.WriteSelectedRows(0, -1, 400, row3Y, cb);
            //         table_bottom_column3_1.WriteSelectedRows(0, -1, 400, row3Y, cb);

            //float table_signatureX;
            //float table_signatureY;
            //if (signatureInsideRow3)
            //{
            //	table_signatureX = margin + table_bottom_column2_2.TotalWidth + 10;
            //	table_signatureY = row3Y - row3Height + table_signature.TotalHeight;
            //	table_signature.TotalWidth = 390f;
            //}
            //else
            //{
            //	table_signatureX = margin;
            //	table_signatureY = row3Y - row3Height - 10;
            //	float signatureRemainingHeight = table_signatureY - printedOnHeight - margin;
            //	if (signatureRemainingHeight < table_signature.TotalHeight)
            //	{
            //		this.DrawPrintedOn(now, bf, cb);
            //		table_signatureY = startY;
            //		document.NewPage();
            //	}
            //}
            //table_signature.WriteSelectedRows(0, -1, table_signatureX, table_signatureY, cb);

            //this.DrawPrintedOn(now, bf, cb);
            #endregion

            #region Add Middle and Bottom

            document.Add(new Paragraph("\n") { SpacingAfter = row1Height + 20 });
            new PdfPCell(table_outer);
            table_outer.ExtendLastRow = false;
            table_outer.SplitLate = false;
            table_outer.SpacingAfter = 10f;
            document.Add(table_outer);

            PdfPTable table_bottom = new PdfPTable(5);
            table_bottom.SetWidths(new float[] { 1f, 0.05f, 1f, 0.05f, 1f });
            table_bottom.DefaultCell.Border = Rectangle.NO_BORDER;
            table_bottom.DefaultCell.Padding = 0;

            var cell_bottom_left = new PdfPCell() { Border = Rectangle.NO_BORDER };
            new PdfPCell(table_bottom_column1_1);
            table_bottom_column1_1.ExtendLastRow = false;
            cell_bottom_left.AddElement(table_bottom_column1_1);
            table_bottom.AddCell(cell_bottom_left);

            table_bottom.AddCell("\n");

            var cell_bottom_center = new PdfPCell() { Border = Rectangle.NO_BORDER };
            new PdfPCell(table_bottom_column2_1);
            table_bottom_column2_1.ExtendLastRow = false;
            cell_bottom_center.AddElement(table_bottom_column2_1);
            if (isDollar)
            {
                cell_bottom_center.AddElement(new Paragraph($"NOTE: 1 US$ = {Number.ToRupiah(viewModel.Rate.Value)}", normal_font));
            }
            table_bottom.AddCell(cell_bottom_center);

            table_bottom.AddCell("\n");

            var cell_bottom_right = new PdfPCell() { Border = Rectangle.NO_BORDER };

            new PdfPCell(table_bottom_column2_2);
            table_bottom_column2_2.ExtendLastRow = false;
            cell_bottom_right.AddElement(table_bottom_column2_2);
            new PdfPCell(table_bottom_column1_2);
            table_bottom_column1_2.ExtendLastRow = false;
            cell_bottom_right.AddElement(table_bottom_column1_2);
            new PdfPCell(table_bottom_column1_3);
            table_bottom_column1_3.ExtendLastRow = false;
            cell_bottom_right.AddElement(table_bottom_column1_3);

            table_bottom.AddCell(cell_bottom_right);

            new PdfPCell(table_bottom);
            table_bottom.ExtendLastRow = false;
            table_bottom.SpacingAfter = 20f;
            document.Add(table_bottom);

            new PdfPCell(table_signature);
            table_signature.ExtendLastRow = false;
            document.Add(table_signature);

            #endregion

            document.Close();

            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        private void DrawPrintedOn(DateTime now, BaseFont bf, PdfContentByte cb)
        {
            cb.BeginText();
            cb.SetFontAndSize(bf, 6);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Printed on: " + now.ToString("dd/MM/yyyy | HH:mm"), 10, 10, 0);
            cb.EndText();
        }
    }
}
