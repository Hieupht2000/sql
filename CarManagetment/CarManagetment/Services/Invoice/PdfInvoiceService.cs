using CarManagetment.DTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CarManagetment.Services.Invoices
{
    public class PdfInvoiceService
    {
        public byte[] GeneratePdfInvoice(InvoiceDTO invoiceDTO)
        {
            // Implement PDF generation logic here
            // This is a placeholder implementation
            using(var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4,50, 50, 50, 50);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("Car Maintenance Invoice", titleFont));
                doc.Add(new Paragraph($"Date Issued: {invoiceDTO.DateIssued:d}\n\n", textFont));
                
                
                doc.Add(new Paragraph($"Customer Name: {invoiceDTO.FullName}", textFont));
                doc.Add(new Paragraph($"Email: {invoiceDTO.Email}\n\n", textFont));


                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.AddCell("Service Name");
                table.AddCell("Quantity");
                table.AddCell("Unit Price");

                if(invoiceDTO.InvoiceDetails != null)
                {
                    foreach (var detail in invoiceDTO.InvoiceDetails)
                    {
                        table.AddCell(detail.ServiceName);
                        table.AddCell(detail.Quantity.ToString());
                        table.AddCell(detail.UnitPrice.ToString("C0"));
                    }
                }
                doc.Add(table);
                doc.Add(new Paragraph("\n"));

                doc.Add(new Paragraph($"VAT: {invoiceDTO.VAT:N2}", textFont));
                doc.Add(new Paragraph($"Total Amount: {invoiceDTO.TotalAmount:C0}\n\n", textFont));



                doc.Close();
                return ms.ToArray();
            }
            
        }
    }
}
