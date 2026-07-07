using MwSolucoes.Domain.DTOs;

namespace MwSolucoes.Domain.PdfGenerator
{
    public interface IReceiptReportGenerator
    {
        byte[] GenerateReceiptPdf(ReceiptReportDto receiptData);
    }
}
