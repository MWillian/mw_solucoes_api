using MwSolucoes.Domain.DTOs;
using MwSolucoes.Domain.PdfGenerator;
using MwSolucoes.Domain.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MwSolucoes.Infrastructure.PdfGenerator
{
    public class OrderServiceReportGenerator : IOrderServiceReportGenerator
    {
        private readonly IMaintenanceServiceRepository _maintenanceServiceRepository;
        public OrderServiceReportGenerator(IMaintenanceServiceRepository maintenanceServiceRepository)
        {
            _maintenanceServiceRepository = maintenanceServiceRepository;
        }
        public byte[] GenerateOrderServicePdf(ServiceRequestReportDto serviceRequest)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(x => ComposeContent(x, serviceRequest));

                    page.Footer().Element(ComposeFooter);
                });
            });
            return document.GeneratePdf();
        }
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("MW SOLUÇÕES").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Ordem de Serviço / Proposta de Orçamento").FontSize(14);
                    column.Item().Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });
        }
        private void ComposeContent(IContainer container, ServiceRequestReportDto os)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(c =>
                {
                    c.Item().Text("DADOS DO EQUIPAMENTO").SemiBold();
                    c.Item().Text($"Modelo: {os.BrandModel}");
                    c.Item().Text($"Problema Relatado: {os.ReportedProblem}");
                });

                if (!string.IsNullOrEmpty(os.TechnicalDiagnosis))
                {
                    column.Item().Background(Colors.Blue.Lighten5).Padding(10).Column(c =>
                    {
                        c.Item().Text("PARECER TÉCNICO E DIAGNÓSTICO").SemiBold().FontColor(Colors.Blue.Darken2);
                        c.Item().Text(os.TechnicalDiagnosis);
                    });
                }

                column.Item().Table(async table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Descrição dos Custos").SemiBold();
                        header.Cell().AlignRight().Text("Valor (R$)").SemiBold();
                    });
                    var services = await _maintenanceServiceRepository.GetByIds(os.ServiceIds);
                    foreach (var servico in services)
                    {
                        table.Cell().Text(servico.Name);
                        table.Cell().AlignRight().Text(servico.Price.ToString("C"));
                    }

                    table.Cell().Text("Mão de Obra Técnica");
                    table.Cell().AlignRight().Text(os.LaborCost.ToString());

                    table.Cell().Text("Peças Adicionais");
                    table.Cell().AlignRight().Text(os.PartsCost.ToString());

                    decimal? total = os.LaborCost + os.PartsCost;

                    table.Cell().PaddingTop(10).Text("TOTAL DO ORÇAMENTO:").SemiBold();
                    table.Cell().PaddingTop(10).AlignRight().Text(total.ToString()).SemiBold();
                });
            });
        }
        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }
    }
}
