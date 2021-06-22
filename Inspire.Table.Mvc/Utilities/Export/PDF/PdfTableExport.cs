namespace Inspire.Table.Mvc.Utilities.Export.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    using Inspire.Core.Infrastructure.Attribute;
    using Inspire.Core.Infrastructure.ResourceManager;
    using Inspire.Table.Mvc.Helpers;
    using Inspire.Utilities.Extensions;

    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public class PdfTableExport : TableExport
    {
        private static readonly string FontPath = ExportConfiguration.FontPath;
        private static readonly int FontSize = ExportConfiguration.FontSize;
        private static readonly BaseFont Cp1251 = BaseFont.CreateFont(FontPath, "CP1251", BaseFont.EMBEDDED);
        private readonly Font fontNormal = new Font(Cp1251, FontSize, Font.NORMAL);
        private readonly Rectangle pageSize = PageSize.A4.Rotate();

        protected override byte[] Export<T>(
            IEnumerable<T> table,
            IEnumerable<KeyValuePair<PropertyInfo, TableOptionsAttribute>> tableColumns)
        {
            using (var ms = new MemoryStream())
            {
                var headerFontColor = new BaseColor(ExportConfiguration.HeaderFontColor);
                var headerBackgroundColor = new BaseColor(ExportConfiguration.HeaderBackgroundColor);
                var pdfPTable = new PdfPTable(tableColumns.Count()) { WidthPercentage = 100, HeaderRows = 1 };
                foreach (var tableData in tableColumns.Select(item => item.Value))
                {
                    var cell = new PdfPCell(
                                   new Phrase(
                                       tableData.Title,
                                       new Font(Cp1251, FontSize, Font.NORMAL, headerFontColor)))
                    { BackgroundColor = headerBackgroundColor };
                    pdfPTable.AddCell(cell);
                }

                foreach (var obj in table)
                {
                    foreach (var tableColumn in tableColumns)
                    {
                        var val = tableColumn.Key.GetPropertyValue(obj, tableColumn.Value);
                        pdfPTable.AddCell(
                            val != null
                                ? new PdfPCell(
                                    new Phrase(
                                        string.Format(
                                            tableColumn.Value.Format.IsNotNullOrEmpty()
                                                ? "{0:" + tableColumn.Value.Format + "}"
                                                : "{0}",
                                            val),
                                        fontNormal))
                                : new PdfPCell(new Phrase(string.Empty, fontNormal)));
                    }
                }

                var pDoc = new Document(pageSize);
                var writer = PdfWriter.GetInstance(pDoc, ms);

                writer.PageEvent = new PageNumber();

                /*Magic - http://stackoverflow.com/questions/2202254/itextsharp-nested-table-on-multiple-pages-causes-nullreferenceexception*/
                pdfPTable.SplitLate = false;
                pdfPTable.SplitRows = true;

                pDoc.Open();
                pDoc.Add(pdfPTable);
                pDoc.Close();

                return ms.ToArray();
            }
        }

        private class PageNumber : PdfPageEventHelper
        {
            private readonly IResourceManager resource = DependencyResolver.Current.GetService<IResourceManager>();

            // this is the BaseFont we are going to use for the header / footer
            private readonly BaseFont bf = Cp1251;

            // This keeps track of the creation time
            private readonly DateTime printTime = DateTime.Now;

            // This is the content byte object of the writer
            private PdfContentByte cb;

            // we will put the final number of pages in a template
            private PdfTemplate template;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    cb = writer.DirectContent;
                    template = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException)
                {
                }
                catch (IOException)
                {
                }
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                var pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, 8);
                cb.SetTextMatrix(pageSize.GetLeft(40), pageSize.GetBottom(30));
                cb.ShowText($"{resource?.Get("CreateDate")} {printTime.ToString(CultureInfo.CurrentUICulture)}".Trim());
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, 8);

                var text = $"{writer.PageNumber}/";
                cb.ShowTextAligned(
                    PdfContentByte.ALIGN_LEFT,
                    text,
                    pageSize.GetRight(55),
                    pageSize.GetBottom(30),
                    0);
                cb.EndText();

                cb.AddTemplate(template, pageSize.GetRight(55) + bf.GetWidthPoint(text, 8), pageSize.GetBottom(30));
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);

                template.BeginText();
                template.SetFontAndSize(bf, 8);
                template.SetTextMatrix(0, 0);
                template.ShowText((writer.PageNumber - 1).ToString());
                template.EndText();
            }
        }
    }
}