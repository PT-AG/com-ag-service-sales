using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.ViewModels.CostCalculationGarment;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.PDFTemplates
{
	public class CostCalculationGarmentBudgetPdfTemplate
	{
		public MemoryStream GeneratePdfTemplate(CostCalculationGarmentViewModel viewModel, int timeoffset, List<GarmentCategoryViewModel> garmentCategoryViewModels)
		{
			BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
			BaseFont bf_bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);
			Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
			Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 6);
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
			PdfPCell cell_colon = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Phrase = new Phrase(":", normal_font) };

			#region Header
			cb.BeginText();
			cb.SetFontAndSize(bf, 10);
			cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PT. AMBASSADOR GARMINDO", 10, 820, 0);
			cb.SetFontAndSize(bf_bold, 12);
			cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "BUDGET GARMENT" + (viewModel.IsPosted ? "" : " (DRAFT)"), 10, 805, 0);
			cb.EndText();
			#endregion

			#region Detail 1 (Top)
			PdfPTable table_detail1 = new PdfPTable(9);
			table_detail1.TotalWidth = 570f;

			float[] detail1_widths = new float[] { 1f, 0.1f, 2f, 1f, 0.1f, 2f, 1.5f, 0.1f, 2f };
			table_detail1.SetWidths(detail1_widths);

			PdfPCell cell_detail1 = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 1, PaddingBottom = 2, PaddingTop = 2 };

			cell_detail1.Phrase = new Phrase("RO", normal_font);
			table_detail1.AddCell(cell_detail1);
			table_detail1.AddCell(cell_colon);
			cell_detail1.Phrase = new Phrase($"{viewModel.RO_Number}", normal_font);
			table_detail1.AddCell(cell_detail1);
			cell_detail1.Phrase = new Phrase("SECTION", normal_font);
			table_detail1.AddCell(cell_detail1);
			table_detail1.AddCell(cell_colon);
			cell_detail1.Phrase = new Phrase($"{viewModel.Section}", normal_font);
			table_detail1.AddCell(cell_detail1);
			cell_detail1.Phrase = new Phrase("CONFIRM ORDER", normal_font);
			table_detail1.AddCell(cell_detail1);
			table_detail1.AddCell(cell_colon);
			cell_detail1.Phrase = new Phrase($"{viewModel.ConfirmDate.AddHours(timeoffset).ToString("dd/MM/yyyy")}", normal_font);
			table_detail1.AddCell(cell_detail1);
			#endregion

			#region Draw Detail 1
			float row1Y = 800;
			table_detail1.WriteSelectedRows(0, -1, 10, row1Y, cb);
			#endregion

			bool isDollar = viewModel.Rate.Value != 1;

			#region Detail 2 (Bottom, Column 1)
			PdfPTable table_detail2 = new PdfPTable(2);
			table_detail2.TotalWidth = 230f;

			float[] detail2_widths = new float[] { 2f, 5f };
			table_detail2.SetWidths(detail2_widths);

			PdfPCell cell_detail2 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7 };

			cell_detail2.Phrase = new Phrase("BUYER AGENT", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.Buyer.Code}" + " - "+ $"{viewModel.Buyer.Name}", normal_font);
			table_detail2.AddCell(cell_detail2);

			cell_detail2.Phrase = new Phrase("BUYER BRAND", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.BuyerBrand.Code}" + " - " +$"{viewModel.BuyerBrand.Name}", normal_font);
			table_detail2.AddCell(cell_detail2);

			cell_detail2.Phrase = new Phrase("ARTICLE", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.Article}", normal_font);
			table_detail2.AddCell(cell_detail2);

			cell_detail2.Phrase = new Phrase("DESCRIPTION", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.Comodity.Code}" + " - " + $"{viewModel.CommodityDescription}", normal_font);
			table_detail2.AddCell(cell_detail2);

			cell_detail2.Phrase = new Phrase("QTY", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.Quantity} PCS", normal_font);
			table_detail2.AddCell(cell_detail2);

			cell_detail2.Phrase = new Phrase("DELIVERY", normal_font);
			table_detail2.AddCell(cell_detail2);
			cell_detail2.Phrase = new Phrase($"{viewModel.DeliveryDate.AddHours(timeoffset).ToString("dd/MM/yyyy")}", normal_font);
			table_detail2.AddCell(cell_detail2);
			#endregion

			#region Detail 3 (Bottom, Column 2)
			PdfPTable table_detail3 = new PdfPTable(8);
			table_detail3.TotalWidth = 330f;

			float[] detail3_widths = new float[] { 3.25f, 4.75f, 1.9f, 0.2f, 1.9f, 1.9f, 0.2f, 1.9f };
			table_detail3.SetWidths(detail3_widths);

			double budgetCost = isDollar ? viewModel.ConfirmPrice * viewModel.Rate.Value ?? 0 : viewModel.ConfirmPrice ?? 0;
			double totalBudget = budgetCost * viewModel.Quantity ?? 0;
			PdfPCell cell_detail3 = new PdfPCell() { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7 };
			PdfPCell cell_detail3_right = new PdfPCell() { HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7 };
			PdfPCell cell_detail3_colspan6 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7, Colspan = 6 };
			PdfPCell cell_detail3_colspan8 = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingRight = 2, PaddingBottom = 7, PaddingLeft = 2, PaddingTop = 7, Colspan = 8 };

            double newtotalBudget = viewModel.CostCalculationGarment_Materials.Sum(a => (a.isFabricCM == true ? 0 : a.BudgetQuantity) * (a.Price ?? 0));
            double newbudgetCost = newtotalBudget > 0 && viewModel.Quantity >0 ? newtotalBudget / viewModel.Quantity.GetValueOrDefault() : 0;
            cell_detail3.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
			cell_detail3.Phrase = new Phrase("TOTAL BUDGET", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
			cell_detail3.Phrase = new Phrase($"{Number.ToRupiah(newtotalBudget)}", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3_colspan6.Phrase = new Phrase("STANDARD MINUTE VALUE", normal_font);
			table_detail3.AddCell(cell_detail3_colspan6);

			double freightCost = 0;
			foreach (CostCalculationGarment_MaterialViewModel item in viewModel.CostCalculationGarment_Materials)
			{
				freightCost += item.TotalShippingFee * viewModel.Quantity.GetValueOrDefault();
			}

			cell_detail3.Border = Rectangle.LEFT_BORDER;
			cell_detail3.Phrase = new Phrase("BEA ANGKUT", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.RIGHT_BORDER;
			cell_detail3.Phrase = new Phrase($"{Number.ToRupiah(freightCost)}", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.LEFT_BORDER;
			cell_detail3.Phrase = new Phrase("SMV. CUT", normal_font);
			table_detail3.AddCell(cell_detail3);
			table_detail3.AddCell(cell_colon);
			cell_detail3.Border = Rectangle.NO_BORDER;
			cell_detail3.Phrase = new Phrase($"{viewModel.SMV_Cutting}", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.NO_BORDER;
			cell_detail3.Phrase = new Phrase("SMV. SEW", normal_font);
			table_detail3.AddCell(cell_detail3);
			table_detail3.AddCell(cell_colon);
			cell_detail3.Border = Rectangle.RIGHT_BORDER;
			cell_detail3.Phrase = new Phrase($"{viewModel.SMV_Sewing}", normal_font);
			table_detail3.AddCell(cell_detail3);

			cell_detail3.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER;
			cell_detail3.Phrase = new Phrase("", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
			cell_detail3.Phrase = new Phrase("", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER;
			cell_detail3.Phrase = new Phrase("SMV. FIN", normal_font);
			table_detail3.AddCell(cell_detail3);
			table_detail3.AddCell(cell_colon);
			cell_detail3.Border = Rectangle.BOTTOM_BORDER;
			cell_detail3.Phrase = new Phrase($"{viewModel.SMV_Finishing}", normal_font);
			table_detail3.AddCell(cell_detail3);
			cell_detail3.Border = Rectangle.BOTTOM_BORDER;
			cell_detail3.Phrase = new Phrase("SMV. TOT", normal_font);
			table_detail3.AddCell(cell_detail3);
			table_detail3.AddCell(cell_colon);
			cell_detail3.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
			cell_detail3.Phrase = new Phrase($"{viewModel.SMV_Total}", normal_font);
			table_detail3.AddCell(cell_detail3);

			cell_detail3_colspan8.Phrase = new Phrase("BUDGET COST / PCS" + "".PadRight(5) + $"{Number.ToRupiah(newbudgetCost)}", normal_font);
			table_detail3.AddCell(cell_detail3_colspan8);
			cell_detail3_colspan8.Phrase = isDollar ? new Phrase($"US$ 1 = {Number.ToRupiah(viewModel.Rate.Value)}" + "".PadRight(10) + $"CONFIRM PRICE : {Number.ToDollar(viewModel.ConfirmPrice)} / PCS", normal_font) : new Phrase($"CONFIRM PRICE : {Number.ToRupiah(viewModel.ConfirmPrice)} / PCS", normal_font);
			table_detail3.AddCell(cell_detail3_colspan8);
			cell_detail3_colspan8.Border = Rectangle.NO_BORDER;
			cell_detail3_colspan8.HorizontalAlignment = Element.ALIGN_CENTER;
			cell_detail3_colspan8.Phrase = new Phrase($"ALLOWANCE >> FABRIC = {viewModel.FabricAllowance}%, ACC = {viewModel.AccessoriesAllowance}%", normal_font);
			table_detail3.AddCell(cell_detail3_colspan8);
            #endregion

            #region Signature
            //PdfPTable table_signature = new PdfPTable(5);
            //table_signature.TotalWidth = 570f;

            //float[] signature_widths = new float[] { 1f, 1f, 1f, 1f, 1f };
            //table_signature.SetWidths(signature_widths);

            //PdfPCell cell_signature = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);
            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);
            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);
            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);
            //cell_signature.Phrase = new Phrase("", normal_font);
            //table_signature.AddCell(cell_signature);

            //string signatureArea = string.Empty;
            //for (int i = 0; i < 5; i++)
            //{
            //	signatureArea += Environment.NewLine;
            //}

            //cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            //table_signature.AddCell(cell_signature);
            //table_signature.AddCell(cell_signature);
            //table_signature.AddCell(cell_signature);
            //table_signature.AddCell(cell_signature);
            //table_signature.AddCell(cell_signature);

            //         var AssignmentKabag = "";
            //         var AssignmentPurch = "";
            //         var AssignmentKadiv = "";

            //         if (viewModel.ApprovalMD.IsApproved)
            //         {
            //             AssignmentKabag = viewModel.ApprovalMD.ApprovedBy;
            //         }
            //         else
            //         {
            //             AssignmentKabag = " ____________________ ";
            //         }

            //         if (viewModel.ApprovalPurchasing.IsApproved)
            //         {
            //             AssignmentPurch = viewModel.ApprovalPurchasing.ApprovedBy;
            //         }
            //         else
            //         {
            //             AssignmentPurch = " ____________________ ";
            //         }

            //         if (viewModel.ApprovalKadivMD.IsApproved)
            //         {
            //             AssignmentKadiv = viewModel.ApprovalKadivMD.ApprovedBy;
            //         }
            //         else
            //         {
            //             AssignmentKadiv = " ____________________ ";
            //         }

            //         string AssignMD = viewModel.IsPosted ? viewModel.CreatedBy : " ";

            //         cell_signature.Phrase = new Phrase("(  " + AssignMD + "  )", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("(  " + AssignmentKabag + "  )", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("(  " + AssignmentPurch + "  )", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("( ____________________ )", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("(  " + AssignmentKadiv + "  )", normal_font);
            //         table_signature.AddCell(cell_signature);

            //         cell_signature.Phrase = new Phrase("Bag. Penjualan", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("Ka. Sie/Ka. Bag Penjualan", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("Ka. Bag Pembelian", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("Ka. Div Produksi Garment", normal_font);
            //         table_signature.AddCell(cell_signature);
            //         cell_signature.Phrase = new Phrase("Ka. Div Penjualan", normal_font);
            //         table_signature.AddCell(cell_signature);
            #endregion

            #region New Signature
            PdfPTable table_signature = new PdfPTable(3);
            table_signature.TotalWidth = 570f;

            float[] signature_widths = new float[] { 1f, 1f, 1f };
            table_signature.SetWidths(signature_widths);

            PdfPCell cell_signature = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

            cell_signature.Phrase = new Phrase("", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("", normal_font);
            table_signature.AddCell(cell_signature);

            string signatureArea = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                signatureArea += Environment.NewLine;
            }

            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            table_signature.AddCell(cell_signature);
            table_signature.AddCell(cell_signature);
            table_signature.AddCell(cell_signature);

            var AssignmentKabag = "";
            var AssignmentPurch = "";

            if (viewModel.ApprovalMD.IsApproved)
            {
                AssignmentKabag = viewModel.ApprovalMD.ApprovedBy;
            }
            else
            {
                AssignmentKabag = " ____________________ ";
            }

            if (viewModel.ApprovalPurchasing.IsApproved)
            {
                AssignmentPurch = viewModel.ApprovalPurchasing.ApprovedBy;
            }
            else
            {
                AssignmentPurch = " ____________________ ";
            }

            string AssignMD = viewModel.IsPosted ? viewModel.CreatedBy : " ";

            cell_signature.Phrase = new Phrase("(  " + AssignMD + "  )", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(  " + AssignmentKabag + "  )", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(  " + AssignmentPurch + "  )", normal_font);
            table_signature.AddCell(cell_signature);

            cell_signature.Phrase = new Phrase("Bag. Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Ka. Sie/Ka. Bag Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Ka. Sie/Ka .Bag Pembelian", normal_font);
            table_signature.AddCell(cell_signature);
            #endregion

            #region Cost Calculation Material
            PdfPTable table_ccm = new PdfPTable(12);
			table_ccm.TotalWidth = 570f;

			float[] ccm_widths = new float[] { 1f, 3f, 2f, 2f, 6f, 3f, 3f, 2f, 2f, 3f, 3f, 2f };
			table_ccm.SetWidths(ccm_widths);

			PdfPCell cell_ccm = new PdfPCell() { Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };

			cell_ccm.Phrase = new Phrase("NO", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("CATEGORIES", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("KODE", bold_font);
			table_ccm.AddCell(cell_ccm);
            cell_ccm.Phrase = new Phrase("JENIS", bold_font);
            table_ccm.AddCell(cell_ccm);
            cell_ccm.Phrase = new Phrase("DESCRIPTION", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("USAGE", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("PRICE", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("QTY", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("UNIT", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("AMOUNT", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("PO NUMBER", bold_font);
			table_ccm.AddCell(cell_ccm);
			cell_ccm.Phrase = new Phrase("BEA KIRIM", bold_font);
			table_ccm.AddCell(cell_ccm);

			float row2Y = row1Y - table_detail1.TotalHeight - 5;
			float row3Height = table_detail2.TotalHeight > table_detail3.TotalHeight ? table_detail2.TotalHeight : table_detail3.TotalHeight;
			float row2RemainingHeight = row2Y - 10 - row3Height - printedOnHeight - margin;
			float row2AllowedHeight = row2Y - printedOnHeight - margin;

			for (int i = 0; i < viewModel.CostCalculationGarment_Materials.Count; i++)
            {
                var matchCategory = garmentCategoryViewModels.FirstOrDefault(x => x.Name == viewModel.CostCalculationGarment_Materials[i].Category.name);

                viewModel.CostCalculationGarment_Materials[i].Category.CodeRequirement = matchCategory.CodeRequirement;
            }

            var MaterialBB = viewModel.CostCalculationGarment_Materials.Where(x => x.Category.CodeRequirement == "BB");
            var MaterialBP = viewModel.CostCalculationGarment_Materials.Where(x => x.Category.CodeRequirement == "BP");
            var MaterialBE = viewModel.CostCalculationGarment_Materials.Where(x => x.Category.CodeRequirement == "BE");
            var MaterialOther = viewModel.CostCalculationGarment_Materials.Where(x => x.Category.CodeRequirement != "BB" && x.Category.CodeRequirement != "BP" && x.Category.CodeRequirement != "BE");

            var orderedMaterial = MaterialBB.Concat(MaterialBP).Concat(MaterialBE).Concat(MaterialOther).ToList();

            for (int i = 0; i < orderedMaterial.Count; i++)
			{ 

                //NO
                cell_ccm.Phrase = new Phrase((i + 1).ToString(), normal_font);
				table_ccm.AddCell(cell_ccm);

				cell_ccm.HorizontalAlignment = Element.ALIGN_LEFT;

				//CATEGORY
				cell_ccm.Phrase = new Phrase(orderedMaterial[i].Category.name, normal_font);
				table_ccm.AddCell(cell_ccm);

				//KODE PRODUK
				cell_ccm.Phrase = new Phrase(orderedMaterial[i].Product.Code, normal_font);
				table_ccm.AddCell(cell_ccm);

                //BAGIAN GARMENT
                cell_ccm.Phrase = new Phrase(orderedMaterial[i].Category.CodeRequirement, normal_font);
                table_ccm.AddCell(cell_ccm);

                //DESCRIPTION
                cell_ccm.Phrase = new Phrase(orderedMaterial[i].Description + " - " + orderedMaterial[i].ProductRemark, normal_font);
				table_ccm.AddCell(cell_ccm);

				cell_ccm.HorizontalAlignment = Element.ALIGN_RIGHT;

				double usage = orderedMaterial[i].Quantity ?? 0;
				cell_ccm.Phrase = new Phrase(Number.ToRupiahWithoutSymbol(usage) + "  " + (orderedMaterial[i].UOMQuantity.Unit), normal_font);
				table_ccm.AddCell(cell_ccm);

				double price = orderedMaterial[i].Price ?? 0;
				cell_ccm.Phrase = new Phrase(String.Format("{0}/{1}", Number.ToRupiahWithoutSymbol(price), orderedMaterial[i].UOMPrice.Unit), normal_font);
				table_ccm.AddCell(cell_ccm);

				double factor;
				if (orderedMaterial[i].Category.name == "FABRIC")
				{
					factor = viewModel.FabricAllowance ?? 0;
				}
				else
				{
					factor = viewModel.AccessoriesAllowance ?? 0;
				}
				double totalQuantity = viewModel.Quantity ?? 0;
				double quantity = (100 + factor) / 100 * usage * totalQuantity;
                var budgetQuantity = Number.ToRupiahWithoutSymbol(Math.Ceiling(orderedMaterial[i].BudgetQuantity));
                cell_ccm.Phrase = new Phrase(budgetQuantity.Substring(0, budgetQuantity.Length - 3), normal_font);
				table_ccm.AddCell(cell_ccm);

				cell_ccm.HorizontalAlignment = Element.ALIGN_CENTER;
				cell_ccm.Phrase = new Phrase(orderedMaterial[i].UOMPrice.Unit, normal_font);
				table_ccm.AddCell(cell_ccm);

				cell_ccm.HorizontalAlignment = Element.ALIGN_RIGHT;

				if(orderedMaterial[i].isFabricCM==true)
				{
					cell_ccm.Phrase = new Phrase(Number.ToRupiahWithoutSymbol(0), normal_font);
					table_ccm.AddCell(cell_ccm);
				}
				else
				{
					double amount = orderedMaterial[i].BudgetQuantity * price;
					cell_ccm.Phrase = new Phrase(Number.ToRupiahWithoutSymbol(amount), normal_font);
					table_ccm.AddCell(cell_ccm);
				}
				

				cell_ccm.HorizontalAlignment = Element.ALIGN_CENTER;
				cell_ccm.Phrase = new Phrase(orderedMaterial[i].PO_SerialNumber, normal_font);
				table_ccm.AddCell(cell_ccm);

				cell_ccm.HorizontalAlignment = Element.ALIGN_RIGHT;
                var beaKirim = Number.ToRupiahWithoutSymbol(Math.Ceiling(orderedMaterial[i].TotalShippingFee * viewModel.Quantity.GetValueOrDefault()));
                cell_ccm.Phrase = new Phrase(beaKirim.Substring(0, beaKirim.Length - 3), normal_font);
				table_ccm.AddCell(cell_ccm);

				float currentHeight = table_ccm.TotalHeight;
				if (currentHeight / row2RemainingHeight > 1)
				{
					if (currentHeight / row2AllowedHeight > 1)
					{
						PdfPRow headerRow = table_ccm.GetRow(0);
						PdfPRow lastRow = table_ccm.GetRow(table_ccm.Rows.Count - 1);
						table_ccm.DeleteLastRow();
						table_ccm.WriteSelectedRows(0, -1, 10, row2Y, cb);
						table_ccm.DeleteBodyRows();
						this.DrawPrintedOn(now, bf, cb);
						document.NewPage();
						table_ccm.Rows.Add(headerRow);
						table_ccm.Rows.Add(lastRow);
						table_ccm.CalculateHeightsFast();
						row2Y = startY;
						row2RemainingHeight = row2Y - 10 - row3Height - printedOnHeight - margin;
						row2AllowedHeight = row2Y - printedOnHeight - margin;
					}
				}
			}
			#endregion

			#region Draw Others
			table_ccm.WriteSelectedRows(0, -1, 10, row2Y, cb);

			float row3Y = row2Y - table_ccm.TotalHeight - 5;
			float row3RemainigHeight = row3Y - printedOnHeight - margin;
			if (row3RemainigHeight < row3Height)
			{
				this.DrawPrintedOn(now, bf, cb);
				row3Y = startY;
				document.NewPage();
			}

			table_detail2.WriteSelectedRows(0, -1, margin, row3Y, cb);

			table_detail3.WriteSelectedRows(0, -1, margin + table_detail2.TotalWidth + 10, row3Y, cb);

			float signatureY = row3Y - row3Height - 10;
			float signatureRemainingHeight = signatureY - printedOnHeight - margin;
			if (signatureRemainingHeight < table_signature.TotalHeight)
			{
				this.DrawPrintedOn(now, bf, cb);
				signatureY = startY;
				document.NewPage();
			}
			table_signature.WriteSelectedRows(0, -1, margin, signatureY, cb);

			this.DrawPrintedOn(now, bf, cb);
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
