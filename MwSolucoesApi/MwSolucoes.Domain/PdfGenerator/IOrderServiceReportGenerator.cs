using MwSolucoes.Domain.DTOs;

namespace MwSolucoes.Domain.PdfGenerator
{
    public interface IOrderServiceReportGenerator
    {
        byte[] GenerateOrderServicePdf(ServiceRequestReportDto serviceRequest);
    }
}
