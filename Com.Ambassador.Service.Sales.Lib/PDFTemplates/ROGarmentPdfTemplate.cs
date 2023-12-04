using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentROViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.PDFTemplates
{
    public class ROGarmentPdfTemplate
    {
        public MemoryStream GeneratePdfTemplate(RO_GarmentViewModel viewModel, int offset)
        {
            //set pdf stream
            MemoryStream stream = new MemoryStream();
            Document document = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter writer = PdfWriter.GetInstance(document, stream);

            writer.PageEvent = new ROGarmentPdfTemplatePageEvent();

            document.Open();

            //set content configuration
            Font company_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font title_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 12);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

            #region Header
            document.Add(new Paragraph("PT.AMBASSADOR GARMINDO", company_font) { Alignment = Element.ALIGN_LEFT, Leading = 0, MultipliedLeading = 1 });
            document.Add(new Paragraph("RO GARMENT", title_font) { Alignment = Element.ALIGN_LEFT, Leading = 0, MultipliedLeading = 1 });
            #endregion

            #region Top
            PdfPTable table_top_with_images = new PdfPTable(2);
            table_top_with_images.SetWidths(new float[] { 5f, 0.7f });

            PdfPTable table_top = new PdfPTable(9);
            float[] top_widths = new float[] { 1f, 0.1f, 2f, 1f, 0.1f, 2f, 1.2f, 0.1f, 2f };

            table_top.TotalWidth = 500f;
            table_top.SetWidths(top_widths);

            PdfPCell cell_top = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2
            };

            PdfPCell cell_colon = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP
            };

            PdfPCell cell_top_keterangan = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2,
                Colspan = 7
            };

            cell_colon.Phrase = new Phrase(":", normal_font);

            cell_top.Phrase = new Phrase("NO RO", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.RO_Number}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("DATE", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.ConfirmDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMMM yyyy")}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("DELIVERY DATE", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.DeliveryDate.AddHours(offset).ToString("dd MMMM yyyy")}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("ARTIKEL", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.Article}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("AGENT", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.Buyer.Name}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("BRAND", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.BuyerBrand.Name}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("KONVEKSI", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.Unit.Code + "      SEKSI   :   " + viewModel.CostCalculationGarment.Section}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("QUANTITY", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.Total.ToString()} {viewModel.CostCalculationGarment.UOM.Unit}" , normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("SIZE RANGE", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.SizeRange}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("DESC", normal_font);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_colon);
            cell_top.Phrase = new Phrase($"{viewModel.CostCalculationGarment.CommodityDescription}", normal_font);
            table_top.AddCell(cell_top);

            cell_top.Phrase = new Phrase("", normal_font);

            table_top.AddCell(cell_top);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_top);

            table_top.AddCell(cell_top);
            table_top.AddCell(cell_top);
            table_top.AddCell(cell_top);

            table_top.LockedWidth = true;
            table_top.HorizontalAlignment = Element.ALIGN_LEFT;
            table_top.ExtendLastRow = false;

            table_top_with_images.AddCell(new PdfPCell(table_top)
            {
                Border = Rectangle.NO_BORDER
            });

            byte[] imageByte;
            try
            {
                imageByte = Convert.FromBase64String(Base64.GetBase64File(viewModel.CostCalculationGarment.ImageFile));
            }
            catch (Exception)
            {
                //var webClient = new WebClient();
                //imageByte = webClient.DownloadData("https://bellamozzarella.files.wordpress.com/2013/07/blank-canvas1.jpg");
                imageByte = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAA0NDQ0ODQ4QEA4UFhMWFB4bGRkbHi0gIiAiIC1EKjIqKjIqRDxJOzc7STxsVUtLVWx9aWNpfZeHh5e+tb75+f8BDQ0NDQ4NDhAQDhQWExYUHhsZGRseLSAiICIgLUQqMioqMipEPEk7NztJPGxVS0tVbH1pY2l9l4eHl761vvn5///CABEIAAoACgMBIgACEQEDEQH/xAAVAAEBAAAAAAAAAAAAAAAAAAAAB//aAAgBAQAAAACnD//EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQIQAAAAf//EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQMQAAAAf//EABQQAQAAAAAAAAAAAAAAAAAAACD/2gAIAQEAAT8AH//EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQIBAT8Af//EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQMBAT8Af//Z");
            }

            Image image = Image.GetInstance(imgb: imageByte);

            if (image.Width > 60)
            {
                float percentage = 0.0f;
                percentage = 60 / image.Width;
                image.ScalePercent(percentage * 100);
            }

            float row1Y = 800;
            float imageY = 800 - image.ScaledHeight;

            table_top_with_images.AddCell(new PdfPCell(image)
            {
                Border = Rectangle.NO_BORDER
            });

            PdfPCell cell_table_top_with_images = new PdfPCell(table_top_with_images);
            table_top_with_images.SpacingBefore = 5f;
            table_top_with_images.ExtendLastRow = false;
            document.Add(table_top_with_images);
            #endregion

            #region Table Fabric
            //Fabric title
            PdfPTable table_fabric_top = new PdfPTable(1);
            table_fabric_top.TotalWidth = 570f;

            float[] fabric_widths_top = new float[] { 5f };
            table_fabric_top.SetWidths(fabric_widths_top);

            PdfPCell cell_top_fabric = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2
            };

            cell_top_fabric.Phrase = new Phrase("FABRIC", bold_font);
            table_fabric_top.AddCell(cell_top_fabric);

            table_fabric_top.LockedWidth = true;
            table_fabric_top.HorizontalAlignment = Element.ALIGN_LEFT;
            table_fabric_top.SpacingBefore = 5f;
            table_fabric_top.ExtendLastRow = false;
            document.Add(table_fabric_top);

            //Main fabric table
            PdfPTable table_fabric = new PdfPTable(9);
            table_fabric.TotalWidth = 570f;

            float[] fabric_widths = new float[] { 5f, 5f, 5f, 5f, 5f, 5f, 5f, 5f, 5f };
            table_fabric.SetWidths(fabric_widths);

            PdfPCell cell_fabric_center = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            PdfPCell cell_fabric_left = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            //cell_fabric_center.Phrase = new Phrase("FABRIC", bold_font);
            //table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("PRODUCT CODE", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("COMPOSITION", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("CONSTRUCTION", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("YARN", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("WIDTH", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("DESCRIPTION", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("DETAIL BARANG", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("QUANTITY", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("REMARK", bold_font);
            table_fabric.AddCell(cell_fabric_center);

            foreach (var materialModel in viewModel.CostCalculationGarment.CostCalculationGarment_Materials)
            {
                if (materialModel.Category.name == "FABRIC")
                {
                    //cell_fabric_left.Phrase = new Phrase(materialModel.Category.SubCategory != null ? materialModel.Category.SubCategory : "", normal_font);
                    //table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Product.Code, normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Product.Composition, normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Product.Const, normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Product.Yarn, normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Product.Width, normal_font);

                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Description != null ? materialModel.Description : "", normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.ProductRemark != null ? materialModel.ProductRemark : "", normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Quantity.ToString() != null ? String.Format("{0} " + materialModel.UOMQuantity.Unit, materialModel.Quantity.ToString()) : "0", normal_font);
                    table_fabric.AddCell(cell_fabric_left);

                    cell_fabric_left.Phrase = new Phrase(materialModel.Information != null ? materialModel.Information : "", normal_font);
                    table_fabric.AddCell(cell_fabric_left);
                }
            }

            table_fabric.LockedWidth = true;
            table_fabric.HorizontalAlignment = Element.ALIGN_LEFT;
            table_fabric.SpacingBefore = 5f;
            table_fabric.ExtendLastRow = false;
            document.Add(table_fabric);
            
            #endregion

            #region Table Accessories
            //Accessories Title
            PdfPTable table_acc_top = new PdfPTable(1);
            table_acc_top.TotalWidth = 570f;

            float[] acc_width_top = new float[] { 5f };
            table_acc_top.SetWidths(acc_width_top);

            PdfPCell cell_top_acc = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2
            };

            cell_top_acc.Phrase = new Phrase("ACCESSORIES", bold_font);
            table_acc_top.AddCell(cell_top_acc);

            table_acc_top.LockedWidth = true;
            table_acc_top.HorizontalAlignment = Element.ALIGN_LEFT;
            table_acc_top.SpacingBefore = 5f;
            table_acc_top.ExtendLastRow = false;
            document.Add(table_acc_top);

            //Main Accessories Table
            PdfPTable table_accessories = new PdfPTable(6);
            table_accessories.TotalWidth = 570f;

            float[] accessories_widths = new float[] { 2f, 3f, 5f, 5f, 2.5f, 7.5f };
            table_accessories.SetWidths(accessories_widths);

            PdfPCell cell_acc_center = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            PdfPCell cell_acc_left = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            //cell_acc_center.Phrase = new Phrase("ACCESSORIES", bold_font);
            //table_accessories.AddCell(cell_acc_center);

            cell_fabric_center.Phrase = new Phrase("PRODUCT CODE", bold_font);
            table_accessories.AddCell(cell_fabric_center);

            cell_fabric_center.Phrase = new Phrase("PRODUCT NAME", bold_font);
            table_accessories.AddCell(cell_fabric_center);

            cell_acc_center.Phrase = new Phrase("DESCRIPTION", bold_font);
            table_accessories.AddCell(cell_acc_center);

            cell_acc_center.Phrase = new Phrase("DETAIL PRODUCT", bold_font);
            table_accessories.AddCell(cell_acc_center);

            cell_acc_center.Phrase = new Phrase("QUANTITY", bold_font);
            table_accessories.AddCell(cell_acc_center);

            cell_acc_center.Phrase = new Phrase("REMARK", bold_font);
            table_accessories.AddCell(cell_acc_center);

            foreach (var materialModel in viewModel.CostCalculationGarment.CostCalculationGarment_Materials)
            {
                if (materialModel.Category.name != "FABRIC")
                {

                    cell_acc_left.Phrase = new Phrase(materialModel.Product.Code, normal_font);
                    table_accessories.AddCell(cell_acc_left);

                    cell_acc_left.Phrase = new Phrase(materialModel.Product.Name, normal_font);
                    table_accessories.AddCell(cell_acc_left);

                    cell_acc_left.Phrase = new Phrase(materialModel.Description != null ? materialModel.Description : "", normal_font);
                    table_accessories.AddCell(cell_acc_left);

                    cell_acc_left.Phrase = new Phrase(materialModel.ProductRemark != null ? materialModel.ProductRemark : "", normal_font);
                    table_accessories.AddCell(cell_acc_left);

                    cell_acc_left.Phrase = new Phrase(materialModel.Quantity != null ? String.Format("{0} " + materialModel.UOMQuantity.Unit, materialModel.Quantity.ToString()) : "0", normal_font);
                    table_accessories.AddCell(cell_acc_left);

                    cell_acc_left.Phrase = new Phrase(materialModel.Information != null ? materialModel.Information : "", normal_font);
                    table_accessories.AddCell(cell_acc_left);
                }
            }

            table_accessories.LockedWidth = true;
            table_accessories.HorizontalAlignment = Element.ALIGN_LEFT;
            table_accessories.SpacingBefore = 5f;
            table_accessories.ExtendLastRow = false;
            document.Add(table_accessories);

            #endregion

            #region Table Size Breakdown
            //Title
            PdfPTable table_breakdown_top = new PdfPTable(1);
            table_breakdown_top.TotalWidth = 570f;

            float[] breakdown_width_top = new float[] { 5f };
            table_breakdown_top.SetWidths(breakdown_width_top);

            PdfPCell cell_top_breakdown = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2
            };

            cell_top_breakdown.Phrase = new Phrase("SIZE BREAKDOWN", bold_font);
            table_breakdown_top.AddCell(cell_top_breakdown);

            table_breakdown_top.LockedWidth = true;
            table_breakdown_top.HorizontalAlignment = Element.ALIGN_LEFT;
            table_breakdown_top.SpacingBefore = 5f;
            table_breakdown_top.ExtendLastRow = false;
            document.Add(table_breakdown_top);

            //Main Table Size Breakdown
            PdfPTable table_breakDown = new PdfPTable(2);
            table_breakDown.TotalWidth = 570f;

            float[] breakDown_widths = new float[] { 3f, 12f };
            table_breakDown.SetWidths(breakDown_widths);

            PdfPCell cell_breakDown_center = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            PdfPCell cell_breakDown_left = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            PdfPCell cell_breakDown_total = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            PdfPCell cell_breakDown_total_2 = new PdfPCell()
            {
                Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2
            };

            cell_breakDown_center.Phrase = new Phrase("WARNA", bold_font);
            table_breakDown.AddCell(cell_breakDown_center);

            cell_breakDown_center.Phrase = new Phrase("SIZE RANGE", bold_font);
            table_breakDown.AddCell(cell_breakDown_center);

            foreach (var productRetail in viewModel.RO_Garment_SizeBreakdowns)
            {
                if (productRetail.Total != 0)
                {
                    cell_breakDown_left.Phrase = new Phrase(productRetail.Color.Name != null ? productRetail.Color.Name : "", normal_font);
                    table_breakDown.AddCell(cell_breakDown_left);

                    PdfPTable table_breakDown_child = new PdfPTable(3);
                    table_breakDown_child.TotalWidth = 300f;

                    float[] breakDown_child_widths = new float[] { 9f, 3f, 3f };
                    table_breakDown_child.SetWidths(breakDown_child_widths);

                    PdfPCell cell_breakDown_child_center = new PdfPCell()
                    {
                        Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 2
                    };

                    PdfPCell cell_breakDown_child_left = new PdfPCell()
                    {
                        Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 2
                    };

                    cell_breakDown_child_center.Phrase = new Phrase("KETERANGAN", bold_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_center);

                    cell_breakDown_child_center.Phrase = new Phrase("SIZE", bold_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_center);

                    cell_breakDown_child_center.Phrase = new Phrase("QUANTITY", bold_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_center);

                    foreach (var size in productRetail.RO_Garment_SizeBreakdown_Details)
                    {

                        cell_breakDown_child_left.Phrase = new Phrase(size.Information != null ? size.Information : "", normal_font);
                        table_breakDown_child.AddCell(cell_breakDown_child_left);

                        cell_breakDown_child_left.Phrase = new Phrase(size.Size != null ? size.Size : "", normal_font);
                        table_breakDown_child.AddCell(cell_breakDown_child_left);

                        cell_breakDown_child_left.Phrase = new Phrase(size.Quantity.ToString() != null ? size.Quantity.ToString() : "0", normal_font);
                        table_breakDown_child.AddCell(cell_breakDown_child_left);
                    }

                    cell_breakDown_child_left.Phrase = new Phrase(" ", bold_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_left);

                    cell_breakDown_child_left.Phrase = new Phrase("TOTAL", bold_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_left);

                    cell_breakDown_child_left.Phrase = new Phrase(productRetail.Total.ToString() != null ? productRetail.Total.ToString() : "0", normal_font);
                    table_breakDown_child.AddCell(cell_breakDown_child_left);

                    table_breakDown.AddCell(table_breakDown_child);
                }
            }

            cell_breakDown_total_2.Phrase = new Phrase("TOTAL", bold_font);
            table_breakDown.AddCell(cell_breakDown_total_2);

            cell_breakDown_total_2.Phrase = new Phrase(viewModel.Total.ToString(), bold_font);
            table_breakDown.AddCell(cell_breakDown_total_2);

            table_breakDown.LockedWidth = true;
            table_breakDown.HorizontalAlignment = Element.ALIGN_LEFT;
            table_breakDown.SpacingBefore = 5f;
            table_breakDown.ExtendLastRow = false;
            document.Add(table_breakDown);
            #endregion

            #region Table Instruksi
            //Title
            PdfPTable table_instruction = new PdfPTable(1);
            float[] instruction_widths = new float[] { 5f };

            table_instruction.TotalWidth = 500f;
            table_instruction.SetWidths(instruction_widths);

            PdfPCell cell_top_instruction = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2
            };

            PdfPCell cell_colon_instruction = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };

            PdfPCell cell_top_keterangan_instruction = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 1,
                PaddingBottom = 2,
                PaddingTop = 2,
                Colspan = 7
            };

            cell_top_instruction.Phrase = new Phrase("INSTRUCTION", normal_font);
            table_instruction.AddCell(cell_top_instruction);
            table_instruction.AddCell(cell_colon_instruction);
            cell_top_keterangan_instruction.Phrase = new Phrase($"{viewModel.Instruction}", normal_font);
            table_instruction.AddCell(cell_top_keterangan_instruction);

            table_instruction.LockedWidth = true;
            table_instruction.HorizontalAlignment = Element.ALIGN_LEFT;
            table_instruction.SpacingBefore = 5f;
            table_instruction.ExtendLastRow = false;
            document.Add(table_instruction);

            #endregion

            #region RO Image
            var countImageRo = 0;
            byte[] roImage;

            foreach (var index in viewModel.ImagesFile)
            {
                countImageRo++;
            }


            if (countImageRo != 0)
            {
                if (countImageRo > 5)
                {
                    countImageRo = 5;
                }

                PdfPTable table_ro_image = new PdfPTable(countImageRo);
                float[] ro_widths = new float[countImageRo];

                for (var i = 0; i < countImageRo; i++)
                {
                    ro_widths.SetValue(5f, i);
                }

                if (countImageRo != 0)
                {
                    table_ro_image.SetWidths(ro_widths);
                }

                table_ro_image.TotalWidth = 570f;

                foreach (var imageFromRo in viewModel.ImagesFile)
                {
                    try
                    {
                        roImage = Convert.FromBase64String(Base64.GetBase64File(imageFromRo));
                    }
                    catch (Exception)
                    {
                        //var webClient = new WebClient();
                        //roImage = webClient.DownloadData("https://bateeqstorage.blob.core.windows.net/other/no-image.jpg");
                        roImage = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAA0NDQ0ODQ4QEA4UFhMWFB4bGRkbHi0gIiAiIC1EKjIqKjIqRDxJOzc7STxsVUtLVWx9aWNpfZeHh5e+tb75+f8BDQ0NDQ4NDhAQDhQWExYUHhsZGRseLSAiICIgLUQqMioqMipEPEk7NztJPGxVS0tVbH1pY2l9l4eHl761vvn5///CABEIAAoACgMBIgACEQEDEQH/xAAVAAEBAAAAAAAAAAAAAAAAAAAAB//aAAgBAQAAAACnD//EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQIQAAAAf//EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQMQAAAAf//EABQQAQAAAAAAAAAAAAAAAAAAACD/2gAIAQEAAT8AH//EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQIBAT8Af//EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAIAQMBAT8Af//Z");
                    }

                    Image images = Image.GetInstance(imgb: roImage);

                    if (images.Width > 60)
                    {
                        float percentage = 0.0f;
                        percentage = 60 / images.Width;
                        images.ScalePercent(percentage * 100);
                    }

                    PdfPCell imageCell = new PdfPCell(images);
                    imageCell.Border = 0;
                    table_ro_image.AddCell(imageCell);
                }

                PdfPCell cell_image = new PdfPCell()
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 2,
                };

                foreach (var name in viewModel.ImagesName)
                {
                    cell_image.Phrase = new Phrase(name, normal_font);
                    table_ro_image.AddCell(cell_image);
                }

                table_ro_image.LockedWidth = true;
                table_ro_image.HorizontalAlignment = Element.ALIGN_LEFT;
                table_ro_image.SpacingBefore = 5f;
                table_ro_image.ExtendLastRow = false;
                document.Add(table_ro_image);
            }
            #endregion

            #region Signature
            PdfPTable table_signature = new PdfPTable(2);
            table_signature.TotalWidth = 570f;

            float[] signature_widths = new float[] { 1f, 1f };
            table_signature.SetWidths(signature_widths);

            PdfPCell cell_signature = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2,
            };

            PdfPCell cell_signature_noted = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 2,
                PaddingTop = 50
            };

            cell_signature.Phrase = new Phrase("Bagian Penjualan", normal_font);
            table_signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Kasie/Kabag Penjualan", normal_font);
            table_signature.AddCell(cell_signature);

            cell_signature_noted.Phrase = new Phrase($"( {viewModel.CreatedBy} )", normal_font);
            table_signature.AddCell(cell_signature_noted);
            cell_signature_noted.Phrase = new Phrase("(                                         )", normal_font);
            table_signature.AddCell(cell_signature_noted);

            table_signature.LockedWidth = true;
            table_signature.HorizontalAlignment = Element.ALIGN_LEFT;
            table_signature.SpacingBefore = 5f;
            table_signature.ExtendLastRow = false;
            document.Add(table_signature);
            
            #endregion

            document.Close();

            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;
            return stream;
        }

    }

    class ROGarmentPdfTemplatePageEvent : PDFPages
    {
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);

            PdfContentByte cb = writer.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(bf, 6);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Printed on: " + DateTime.Now.ToString("dd/MM/yyyy | HH:mm"), 10, 10, 0);
            cb.EndText();
        }
    }
}