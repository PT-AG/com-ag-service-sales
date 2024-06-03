using Com.Ambassador.Service.Sales.Lib.BusinessLogic.Interface.GarmentSalesContractInterface;
using Com.Ambassador.Service.Sales.Lib.Helpers;
using Com.Ambassador.Service.Sales.Lib.Utilities;
using Com.Ambassador.Service.Sales.Lib.ViewModels.GarmentSalesContractViewModels;
using Com.Ambassador.Service.Sales.Lib.ViewModels.IntegrationViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Ambassador.Service.Sales.Lib.PDFTemplates
{
    public class GarmentSalesContractLocalPDFTemplate
    {
        public MemoryStream GeneratePdfTemplate(GarmentSalesContractViewModel viewModel, IGarmentSalesContract facade, int timeoffset, Dictionary<string, object> buyer, Dictionary<string, object> bank)
        {
            Font header_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
            Font normal_font = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font bold_font = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10);
            Font normal_font_small = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
            Font bold_font_small = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);

            Document document = new Document(PageSize.A4, 40, 40, 120, 40);
            MemoryStream stream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PageEvent = new GarmentSalesContractPDFTemplatePageEvent();
            document.Open();

            #region Header
            //string codeNoString = "FM-PJ-00-03-003/R2";
            //Paragraph codeNo = new Paragraph(codeNoString, bold_font) { Alignment = Element.ALIGN_RIGHT };
            //document.Add(codeNo);

            string titleString = "KONTRAK PENJUALAN";
            bold_font.SetStyle(Font.UNDERLINE);
            Paragraph title = new Paragraph(titleString, bold_font) { Alignment = Element.ALIGN_CENTER };
            // title.SpacingAfter = 20f;
            document.Add(title);
            bold_font.SetStyle(Font.NORMAL);

            //string codeNoString = "FM-00-PJ-02-001/R1";
            Paragraph scNo = new Paragraph(viewModel.SalesContractNo, bold_font) { Alignment = Element.ALIGN_CENTER };
            scNo.SpacingAfter = 10f;
            document.Add(scNo);
            #endregion

            string HeaderParagraphString = "Yang bertanda tangan dibawah ini : ";
            Paragraph HeaderParagraph = new Paragraph(HeaderParagraphString, normal_font) { Alignment = Element.ALIGN_LEFT };
            HeaderParagraph.SpacingAfter = 10f;
            document.Add(HeaderParagraph);

            #region Body
            PdfPTable tableBody = new PdfPTable(3);
            tableBody.SetWidths(new float[] { 0.004f, 0.009f, 0.060f });
            PdfPCell bodyContentLeft = new PdfPCell() { Border = Rectangle.NO_BORDER, Padding = 1, HorizontalAlignment = Element.ALIGN_LEFT };
            //PdfPCell bodyJustify = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_JUSTIFIED };
            bodyContentLeft.Phrase = new Phrase("1.", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase("Nama", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase(": Haenis Gunarto ", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase("", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase("Jabatan", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase(": Direktur", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase("", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase("Alamat", normal_font);
            tableBody.AddCell(bodyContentLeft);
            bodyContentLeft.Phrase = new Phrase(": Kelurahan Banaran, Kecamatan Grogol, Kabupaten Sukoharjo", normal_font);
            tableBody.AddCell(bodyContentLeft);
            PdfPCell cellBody = new PdfPCell(tableBody); // dont remove
            tableBody.ExtendLastRow = false;
            tableBody.SpacingAfter = 0.5f;
            document.Add(tableBody);

            string ParagraphString1 = "          Bertindak untuk dan atas nama PT Ambassador Garmindo, selanjutnya disebut “Penjual”";
            Paragraph Paragraph1 = new Paragraph(ParagraphString1, normal_font) { Alignment = Element.ALIGN_LEFT };
            Paragraph1.SpacingAfter = 10f;
            document.Add(Paragraph1);

            string buyerName = buyer["Name"] != null ? buyer["Name"].ToString() : "";
            PdfPTable tableBodyBuyer = new PdfPTable(3);
            tableBodyBuyer.SetWidths(new float[] { 0.004f, 0.010f, 0.060f });
            PdfPCell bodyContentLefts = new PdfPCell() { Border = Rectangle.NO_BORDER, Padding = 1, HorizontalAlignment = Element.ALIGN_LEFT };
            bodyContentLefts.Phrase = new Phrase("2.", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase("Nama", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase(": " + "" + viewModel.RecipientName, normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase("", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase("Jabatan ", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase(":" + " " + viewModel.RecipientJob, normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase("", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase("Alamat", normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            bodyContentLefts.Phrase = new Phrase(":" + " " + viewModel.RecipientAddress, normal_font);
            tableBodyBuyer.AddCell(bodyContentLefts);
            PdfPCell cellBodys = new PdfPCell(tableBodyBuyer); // dont remove
            tableBodyBuyer.ExtendLastRow = false;
            tableBodyBuyer.SpacingAfter = 0.5f;
            document.Add(tableBodyBuyer);
            string ParagraphStringbuyer = "          Bertindak untuk dan atas nama " + "" + buyerName + "" + ", selanjutnya disebut “Pembeli”";
            Paragraph Paragraphbuyer = new Paragraph(ParagraphStringbuyer, normal_font) { Alignment = Element.ALIGN_LEFT };
            Paragraphbuyer.SpacingAfter = 10f;
            document.Add(Paragraphbuyer);

            string ParagraphString2 = "Secara bersama-sama Penjual dan Pembeli disebut Para Pihak ";
            Paragraph Paragraph2 = new Paragraph(ParagraphString2, normal_font) { Alignment = Element.ALIGN_LEFT };
            Paragraph2.SpacingAfter = 10f;
            document.Add(Paragraph2);

            string FirstParagraphString = "Para Pihak tersebut di atas sepakat mengadakan perjanjian jual beli produk yang diproduksi oleh Penjual, dengan spesifikasi dan syarat-syarat sebagai berikut: ";
            Paragraph FirstParagraph = new Paragraph(FirstParagraphString, normal_font) { Alignment = Element.ALIGN_LEFT };
            FirstParagraph.SpacingAfter = 10f;
            document.Add(FirstParagraph);

            string ParagraphString3 = "A. PRODUK YANG DIORDER";
            Paragraph Paragraph3 = new Paragraph(ParagraphString3, bold_font) { Alignment = Element.ALIGN_LEFT };
            Paragraph3.SpacingAfter = 4f;
            document.Add(Paragraph3);

            //#region Produk diorder
            PdfPTable tableOrder = new PdfPTable(8);
            tableOrder.TotalWidth = 500f;
            tableOrder.LockedWidth = true;
            float[] widths = new float[] { 1f, 5f, 5f, 5f, 3f, 4f, 5f, 3f };
            tableOrder.SetWidths(widths);
            tableOrder.HorizontalAlignment = 0;
            tableOrder.SpacingAfter = 20f;
            PdfPCell cellOrder = new PdfPCell() { MinimumHeight = 10, Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            
            cellOrder.Phrase = new Phrase("No", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Artikel", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Material/Konstruksi", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Keterangan", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Quantity Order", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Harga (Exclude PPn)", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Total Harga (Exclude PPn)", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("Delivery Date", bold_font_small);
            tableOrder.AddCell(cellOrder);

            var index = 0;
            double totalQty = 0;
            double totalPrice = 0;
            double totalAmount = 0;
            Dictionary<string, double> totalQtyPerUnit = new Dictionary<string, double>();

            foreach (var ro in viewModel.SalesContractROs)
            {
                var getQty = ro.Items.FirstOrDefault(x => x.Quantity == 0); // mengecek apakah qty item yang dimasukkan berjumlah 0
                if (ro.Items != null && ro.Items.Count > 0 && getQty == null)
                {
                    foreach (var item in ro.Items)
                    {
                        index++;
                        cellOrder.Phrase = new Phrase(index.ToString(), normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase(ro.Article, normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase(ro.Material, normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase(item.Description, normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase(item.Quantity.ToString() + " " + ro.Uom.Unit, normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase($"{Number.ToRupiah(item.Price)} / {ro.Uom.Unit}", normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase($"{Number.ToRupiah(item.Price * item.Quantity)}", normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        cellOrder.Phrase = new Phrase(ro.DeliveryDate.ToOffset(new TimeSpan(timeoffset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID")), normal_font_small);
                        tableOrder.AddCell(cellOrder);
                        totalQty += item.Quantity;
                        totalPrice += item.Price;
                        totalAmount += item.Quantity * item.Price;

                        //string key = ro.RONumber + "~" + ro.Uom.Unit;

                        if (!totalQtyPerUnit.ContainsKey(ro.Uom.Unit))
                        {
                            totalQtyPerUnit.Add(ro.Uom.Unit, item.Quantity);
                        }
                        else
                        {
                            totalQtyPerUnit[ro.Uom.Unit] += item.Quantity;
                        }
                    }
                }
                else
                {
                    index++;
                    cellOrder.Phrase = new Phrase(index.ToString(), normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase(ro.Article, normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase(ro.Material, normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase(ro.Description, normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase(ro.Quantity.ToString() + " " + ro.Uom.Unit, normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase($"{Number.ToRupiah(ro.Price)} / {ro.Uom.Unit}", normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase($"{Number.ToRupiah(ro.Amount)}", normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    cellOrder.Phrase = new Phrase(ro.DeliveryDate.ToOffset(new TimeSpan(timeoffset, 0, 0)).ToString("dd/MM/yyyy", new CultureInfo("id-ID")), normal_font_small);
                    tableOrder.AddCell(cellOrder);
                    totalQty += ro.Quantity;
                    totalPrice += ro.Price;
                    totalAmount += ro.Amount;


                    //string key = ro.RONumber + "~" + ro.Uom.Unit;

                    if (!totalQtyPerUnit.ContainsKey(ro.Uom.Unit))
                    {
                        totalQtyPerUnit.Add(ro.Uom.Unit, ro.Quantity);
                    }
                    else
                    {
                        totalQtyPerUnit[ro.Uom.Unit] += ro.Quantity;
                    }
                }
            }

            cellOrder.Phrase = new Phrase("", bold_font_small);
            cellOrder.Colspan = 3;
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("GRAND TOTAL", bold_font_small);
            cellOrder.Colspan = 1;
            tableOrder.AddCell(cellOrder);

            //string _totalQtyPerUnit = string.Join(";", totalQtyPerUnit.Select(x => x.Value + " " + x.Key.Split("~").LastOrDefault() + "\n").ToArray());
            string _totalQtyPerUnit = string.Join(";", totalQtyPerUnit.Select(x => x.Value + " " + x.Key + "\n").ToArray());

            //cellOrder.Phrase = new Phrase(totalQty.ToString() + " " + viewModel.SalesContractROs.First().Uom.Unit, bold_font_small);
            //tableOrder.AddCell(cellOrder);

            cellOrder.Phrase = new Phrase(_totalQtyPerUnit, bold_font_small);
            tableOrder.AddCell(cellOrder);

            //cellOrder.Phrase = new Phrase($"{Number.ToRupiah(totalPrice)}", bold_font_small);
            //tableOrder.AddCell(cellOrder);

            cellOrder.Phrase = new Phrase("", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase($"{Number.ToRupiah(totalAmount)}", bold_font_small);
            tableOrder.AddCell(cellOrder);
            cellOrder.Phrase = new Phrase("", bold_font_small);
            tableOrder.AddCell(cellOrder);

            //cellOrder.Phrase = new Phrase(viewModel.Description, normal_font);
            //tableOrder.AddCell(cellOrder);
            //tableOrder.AddCell(cellOrder);



            PdfPCell cellProduct = new PdfPCell(tableOrder); // dont remove
            tableOrder.ExtendLastRow = false;
            tableOrder.SpacingAfter = 10f;
            Paragraph p = new Paragraph();
            p.IndentationLeft = 15f;
            p.Add(tableOrder);
            document.Add(p);

            //cellOrder.VerticalAlignment = Element.ALIGN_TOP;
            //tableOrder.AddCell(cellOrder);

            //tableOrder.SpacingAfter = 10;
            //document.Add(tableOrder);
            //#endregion

            string ParagraphString4 = "B. KESEPAKATAN ORDER";
            Paragraph Paragraph4 = new Paragraph(ParagraphString4, bold_font) { Alignment = Element.ALIGN_LEFT };
            Paragraph4.SpacingAfter = 4f;
            document.Add(Paragraph4);


            //#region Pemenuhan Order
            PdfPTable tableDetailOrder = new PdfPTable(2);
            //tableDetailOrder.WidthPercentage = 20;
            //tableDetailOrder.SetWidths(new float[] { 20f, 20f });
            tableDetailOrder.TotalWidth = 400f;
            tableDetailOrder.LockedWidth = true;
            float[] widthsDetail = new float[] { 1f, 1f };
            tableDetailOrder.SetWidths(widthsDetail);
            tableDetailOrder.HorizontalAlignment = 0;
            PdfPCell cellDetailOrder = new PdfPCell() { MinimumHeight = 10, Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_MIDDLE };
            PdfPCell CellDetailCenter = new PdfPCell() { MinimumHeight = 10, Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
           
            cellDetailOrder.Phrase = new Phrase("FOB/CMT/CM", bold_font);
            tableDetailOrder.AddCell(cellDetailOrder);
            CellDetailCenter.Phrase = new Phrase(viewModel.FOB, normal_font);
            tableDetailOrder.AddCell(CellDetailCenter);
            cellDetailOrder.Phrase = new Phrase("Jenis Packing", bold_font);
            tableDetailOrder.AddCell(cellDetailOrder);
            CellDetailCenter.Phrase = new Phrase(viewModel.Packing, normal_font);
            tableDetailOrder.AddCell(CellDetailCenter);
            cellDetailOrder.Phrase = new Phrase("Ongkos Angkut", bold_font);
            tableDetailOrder.AddCell(cellDetailOrder);
            CellDetailCenter.Phrase = new Phrase(viewModel.FreightCost.ToString(), normal_font);
            tableDetailOrder.AddCell(CellDetailCenter);
            cellDetailOrder.Phrase = new Phrase("Alamat Pengiriman", bold_font);
            tableDetailOrder.AddCell(cellDetailOrder);
            CellDetailCenter.Phrase = new Phrase(viewModel.Country, normal_font);
            tableDetailOrder.AddCell(CellDetailCenter);


            PdfPCell cellDetail = new PdfPCell(tableDetailOrder); // dont remove
            tableDetailOrder.ExtendLastRow = false;
            tableDetailOrder.SpacingAfter = 10f;
            Paragraph p1 = new Paragraph();
            p1.IndentationLeft = 15f;
            p1.Add(tableDetailOrder);
            document.Add(p1);

            cellDetailOrder.VerticalAlignment = Element.ALIGN_TOP;
            tableDetailOrder.AddCell(cellDetailOrder);


            string ParagraphString5 = "C. Metode Pembayaran";
            Paragraph Paragraph5 = new Paragraph(ParagraphString5, bold_font) { Alignment = Element.ALIGN_LEFT };
            document.Add(Paragraph5);

            Paragraph p2 = new Paragraph();
            PdfPTable tablePembayaran = new PdfPTable(4);
            tablePembayaran.SetWidths(new float[] { 0.3f, 3f, 0.2f, 5f });
            PdfPCell bodyContentPembayaran = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };

            bodyContentPembayaran.Phrase = new Phrase("1.", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase("Cara Pembayaran", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(":", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(viewModel.PaymentMethod, normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);

            bodyContentPembayaran.Phrase = new Phrase("2.", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase("Down Payment (DP)", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(":", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(viewModel.DownPayment, normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);

            bodyContentPembayaran.Phrase = new Phrase("3.", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase("Rekening Tujuan Pembayaran", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(":", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase(viewModel.AccountBank.BankName + " - " + bank["AccountName"] + " - " + bank["AccountNumber"], normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);

            bodyContentPembayaran.Phrase = new Phrase("4.", normal_font);
            tablePembayaran.AddCell(bodyContentPembayaran);
            bodyContentPembayaran.Phrase = new Phrase("Pembayaran dianggap sah / lunas jika diterima penjual sesuai dengan nilai tagihan.", normal_font);
            bodyContentPembayaran.Colspan = 3;
            tablePembayaran.AddCell(bodyContentPembayaran);
            //bodyContentPembayaran.Phrase = new Phrase("", normal_font);
            //tablePembayaran.AddCell(bodyContentPembayaran);
            //bodyContentPembayaran.Phrase = new Phrase("", normal_font);
            //tablePembayaran.AddCell(bodyContentPembayaran);
            PdfPCell cellPembayaran = new PdfPCell(tablePembayaran); // dont remove
            tablePembayaran.ExtendLastRow = false;
            tablePembayaran.SpacingAfter = 0.5f;
            p2.IndentationLeft = 15f;
            p2.Add(tablePembayaran);
            document.Add(p2);
            //document.Add(tablePembayaran);
            //string ParagraphStringPembayaran = "4. Pembayaran dianggap sah / lunas jika diterima penjual sesusai dengan nilai tagihan.";
            //Paragraph ParagraphPembayaran = new Paragraph(ParagraphStringPembayaran, normal_font) { Alignment = Element.ALIGN_LEFT };
            //tablePembayaran.SpacingAfter = 30f;
            //document.Add(ParagraphPembayaran);


            #endregion

            #region Signatures
            PdfPTable tableSignature = new PdfPTable(2);
            //tableSignature.WidthPercentage = 20;
            //tableSignature.SetWidths(new int[] { 10, 10 });
            tableSignature.TotalWidth = 216f;
            tableSignature.LockedWidth = true;
            float[] widthsSignature = new float[] { 1f, 1f };
            tableSignature.SetWidths(widthsSignature);
            tableSignature.HorizontalAlignment = 0;
            tableSignature.SpacingBefore = 20f;
            tableSignature.HorizontalAlignment = Element.ALIGN_RIGHT;
            PdfPCell cellSignature = new PdfPCell() { MinimumHeight = 10, Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            PdfPCell cellBottomSignature = new PdfPCell() { MinimumHeight = 40, Border = Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
            cellSignature.Phrase = new Phrase("Paraf Penjual", normal_font);
            tableSignature.AddCell(cellSignature);
            cellSignature.Phrase = new Phrase("Paraf Pembeli", normal_font);
            tableSignature.AddCell(cellSignature);
            cellBottomSignature.Phrase = new Phrase(" ");
            tableSignature.AddCell(cellBottomSignature);
            cellBottomSignature.Phrase = new Phrase(" ");
            tableSignature.AddCell(cellBottomSignature);
            PdfPCell cellSignatures = new PdfPCell(tableSignature); // dont remove
            tableSignature.ExtendLastRow = false;
            document.Add(tableSignature);
            #endregion

            #region ConditionPage
            document.NewPage();

            string ConditionString = "D. SYARAT DAN KETENTUAN";
            Paragraph ConditionName = new Paragraph(ConditionString, bold_font) { Alignment = Element.ALIGN_LEFT };
            ConditionName.SpacingAfter = 1f;
            document.Add(ConditionName);

            List list = new List(List.ORDERED);
            list.IndentationLeft = 15f;
            list.Add(new ListItem("Kesepakatan", bold_font_small));

            List sublist1 = new List(List.ORDERED);
            sublist1.IndentationLeft = 10f;
            sublist1.PreSymbol = string.Format("{0}.", 1);
            ListItem one = new ListItem("Order yang telah diterima penjual tidak dapat dibatalkan secara sepihak oleh pembeli", normal_font_small);
            one.Alignment = Element.ALIGN_JUSTIFIED;
            one.Leading = 13;
            sublist1.Add(one);
            one = new ListItem("Setiap perubahan ketentuan dalam kontrak penjualan (apabila diperlukan) dapat dilakukan berdasarkan kesepakatan bersama", normal_font_small);
            one.Alignment = Element.ALIGN_JUSTIFIED;
            one.Leading = 13;
            sublist1.Add(one);
            list.Add(sublist1);

            list.Add(new ListItem("Keterlambatan Pembayaran Denda", bold_font_small));

            List sublist2 = new List(List.ORDERED);
            sublist2.IndentationLeft = 10f;
            sublist2.PreSymbol = string.Format("{0}.", 2);
            ListItem two = new ListItem("Bilamana terjadi keterlambatan pembayaran berdasarkan ketentuan pada huruf C angka 1, maka pembeli dikenakan denda sebesar " + viewModel.LatePayment + "% per bulan yang dihutang secara proporsi untuk keterlambatan per hari dari nominal yang belum dibayarkan, denda sekaligus pembayaran terutang tersebut harus dibayar secara tunai dan sekaligus lunas oleh pembeli", normal_font_small);
            two.Alignment = Element.ALIGN_JUSTIFIED;
            two.Leading = 13;
            sublist2.Add(two);
            two = new ListItem("Dalam hal Pembeli tidak dapat melakukan pembayaran beserta dendanya sampai dengan batas waktu yang ditentukan oleh penjual, maka tanpa mengesampingkan denda, Penjual dapat mengambil langkah sebagai berikut : ", normal_font_small);
            two.Alignment = Element.ALIGN_JUSTIFIED;
            two.Leading = 13;
            sublist2.Add(two);

            List bulletedlist = new List(List.UNORDERED, 10f);
            bulletedlist.IndentationLeft = 20f;
            two = new ListItem("Meminta Pembeli mengembalikan Produk yang belum dibayar dalam kondisi utuh dan lengkap, dalam hal ini Pembeli berkewajiban mengembalikan Produk sesuai permintaan Penjual dengan biaya ditanggung oleh Pembeli.", normal_font_small);
            two.Alignment = Element.ALIGN_JUSTIFIED;
            two.Leading = 13;
            bulletedlist.Add(two);
            two = new ListItem("Jika Pembeli tidak mengembalikan Produk dalam waktu " + viewModel.LateReturn + " hari setelah diminta oleh Penjual, maka Pembeli memberikan kuasa mutlak dan tidak dapat dicabut kepada Penjual untuk mengambil kembali Produk yang belum dibayar oleh Pembeli dalam kondisi utuh dan lengkap seperti waktu pengiriman dari Penjual, segala biaya yang timbul dalam proses tersebut ditanggung Pembeli.", normal_font_small);
            two.Alignment = Element.ALIGN_JUSTIFIED;
            two.Leading = 13;
            bulletedlist.Add(two);
            two = new ListItem("Jika Produk sudah tidak ada karena sebab apapun maka Pembeli wajib mengganti dengan sejumlah uang senilai harga Produk.", normal_font_small);
            two.Alignment = Element.ALIGN_JUSTIFIED;
            two.Leading = 13;
            bulletedlist.Add(two);
            sublist2.Add(bulletedlist);
            list.Add(sublist2);

            list.Add(new ListItem("Klaim", bold_font_small));
            List sublist3 = new List(List.ORDERED);
            sublist3.IndentationLeft = 10f;
            sublist3.PreSymbol = string.Format("{0}.", 3);
            string tanggalClaimTerbilang = NumberToTextIDN.terbilang(viewModel.Claim.GetValueOrDefault());
            ListItem three = new ListItem("Jika Produk yang diterima Pembeli tidak sesuai dengan kesepakatan, maka Pembeli wajib memberitahukan kepada Penjual, berikut dengan bukti yang cukup selambat-lambatnya " + viewModel.Claim + "(" + tanggalClaimTerbilang + ") hari setelah Produk diterima, selanjutnya klaim akan diselesaikan secara terpisah dan tidak dapat dihubungkan dan / atau diperhitungkan dengan pembayaran Produk dalam kontak Penjualan ini.", normal_font_small);
            three.Alignment = Element.ALIGN_JUSTIFIED;
            three.Leading = 13;
            sublist3.Add(three);
            three = new ListItem("Bilamana dalam jangka waktu tersebut diatas Pembeli tidak mengajukan klaim maka Produk dinyatakan sudah sesuai dengan Kontrak Penjualan.", normal_font_small);
            three.Alignment = Element.ALIGN_JUSTIFIED;
            three.Leading = 13;
            sublist3.Add(three);
            list.Add(sublist3);

            list.Add(new ListItem("Force Majeure", bold_font_small));
            List sublist4 = new List(List.UNORDERED);
            sublist4.IndentationLeft = 10f;
            sublist4.ListSymbol = new Chunk("");
            ListItem four = new ListItem("Dalam hal terjadinya Force Majeure termasuk hal-hal berikut tetapi tidak terbatas pada bencana alam, kebakaran, pemogokan pekerjaan, hambatan lalu lintas, tindakan pemerintah dalam bidang ekonomi dan moneter yang secara nyata berpengaruh terhadap pelaksanaan Kontrak Penjualan ini maupun hal-hal lain di luar kemampuan Penjual, maka Penjual tidak akan bertanggungjawab atas kegagalan penyerahan atau penyerahan yang tertunda, selanjutnya Penjual dan Pembeli sepakat untuk melakukan peninjauan kembali isi Kontrak Penjualan ini.", normal_font_small);
            four.Alignment = Element.ALIGN_JUSTIFIED;
            four.Leading = 13;
            sublist4.Add(four);
            list.Add(sublist4);

            list.Add(new ListItem("Perselisihan", bold_font_small));
            List sublist5 = new List(List.UNORDERED);
            sublist5.IndentationLeft = 10f;
            sublist5.ListSymbol = new Chunk("");
            ListItem five = new ListItem("Semua hal yang menyangkut adanya sengketa atau perselisihan semaksimal mungkin diselesaikan secara musyawarah. Jika tidak dapat tercapai mufakat maka Penjual dan Pembeli sepakat memilih domisili hukum yang umum dan tetap di Kantor Panitera Pengadilan Negeri Sukoharjo.", normal_font_small);
            five.Alignment = Element.ALIGN_JUSTIFIED;
            five.Leading = 13;
            sublist5.Add(five);
            list.Add(sublist5);

            document.Add(list);

            #endregion

            #region Signature
            PdfPTable signature = new PdfPTable(2);
            signature.SetWidths(new float[] { 1f, 1f });
            signature.SpacingBefore = 10f;
            PdfPCell cellIContentRights = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
            PdfPCell cellIContentLefts = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
            PdfPCell cell_signature = new PdfPCell() { Border = Rectangle.NO_BORDER, HorizontalAlignment = /*Element.ALIGN_CENTER*/ Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 2 };
            signature.SetWidths(new float[] { 1f, 1f });
            cell_signature.Phrase = new Phrase("Sukoharjo," + " " + viewModel.CreatedUtc.AddHours(timeoffset).ToString("dd MMMM yyyy"/*, new CultureInfo("id - ID")*/), normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Penjual,", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("Pembeli, ", normal_font);
            signature.AddCell(cell_signature);

            cell_signature.Phrase = new Phrase("", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("", normal_font);
            signature.AddCell(cell_signature);

            string signatureArea = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                signatureArea += Environment.NewLine;
            }

            cell_signature.Phrase = new Phrase(signatureArea, normal_font);
            signature.AddCell(cell_signature);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(" ", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(" ", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(" ", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(" ", normal_font);
            signature.AddCell(cell_signature);

            cell_signature.Phrase = new Phrase("( Haenis Gunarto )", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("(" + viewModel.RecipientName + ")", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase("PT Ambassador Garmindo", normal_font);
            signature.AddCell(cell_signature);
            cell_signature.Phrase = new Phrase(buyerName, normal_font);
            signature.AddCell(cell_signature);
            cellIContentRights.Phrase = new Phrase("");
            signature.AddCell(cellIContentRights);

            PdfPCell signatureCell = new PdfPCell(signature); // dont remove
            signature.ExtendLastRow = false;
            signature.SpacingAfter = 10f;
            document.Add(signature);
            #endregion


            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }
    }
}
