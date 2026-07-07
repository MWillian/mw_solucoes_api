using MwSolucoes.Domain.DTOs;
using MwSolucoes.Domain.Enums;
using MwSolucoes.Domain.PdfGenerator;
using MwSolucoes.Domain.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MwSolucoes.Infrastructure.PdfGenerator
{
    public class OrderServiceReportGenerator : IOrderServiceReportGenerator
    {
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

                    page.Content().Element(c => ComposeForm(c, serviceRequest));

                    page.Footer().Element(ComposeFooter);
                });
            });
            return document.GeneratePdf();
        }

        private void ComposeForm(IContainer container, ServiceRequestReportDto os)
        {
            container.Column(column =>
            {
                column.Item().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).AlignCenter()
                    .Text("MW SOLUÇÕES - ORDEM DE SERVIÇO (PROPOSTA DE ORÇAMENTO)")
                    .FontSize(12).Bold();

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text($"Protocolo: #{os.Protocol ?? "N/A"}").SemiBold();
                    row.RelativeItem().AlignRight().Text($"Data de Emissão: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("1. IDENTIFICAÇÃO DO CLIENTE").Bold();
                    c.Item().PaddingTop(2).Row(r =>
                    {
                        r.RelativeItem().Text($"Nome: {os.CustomerName}");
                        r.RelativeItem().Text($"CPF: {os.CustomerCpf}");
                    });
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text($"Contato: {os.CustomerPhone}");
                        r.RelativeItem().Text($"Email: {os.CustomerEmail}");
                    });
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("2. DADOS DO EQUIPAMENTO & TRIAGEM").Bold();
                    c.Item().PaddingTop(2).Row(r =>
                    {
                        r.RelativeItem().Text($"Tipo: {os.Equipment}");
                        r.RelativeItem().Text($"Marca/Modelo: {os.BrandModel}");
                    });
                    c.Item().PaddingTop(4).Text("Problema Relatado pelo Cliente:").SemiBold();
                    c.Item().Text($"\"{os.ReportedProblem}\"").Italic();
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("3. PARECER TÉCNICO & DIAGNÓSTICO").Bold();

                    var diag = string.IsNullOrWhiteSpace(os.TechnicalDiagnosis) ? "Aguardando avaliação técnica." : os.TechnicalDiagnosis;
                    c.Item().PaddingTop(2).Text("Diagnóstico Técnico:").SemiBold();
                    c.Item().Text($"\"{diag}\"").Italic();
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("4. DISCRIMINAÇÃO DE VALORES").Bold();

                    c.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(120);
                        });

                        table.Cell().ColumnSpan(2).PaddingBottom(2).Text("Serviços do Catálogo:").SemiBold();
                        foreach (var servico in os.Services)
                        {
                            table.Cell().Text($"- {servico.Name}");
                            table.Cell().AlignRight().Text(servico.Price.ToString("C"));
                        }
                        table.Cell().ColumnSpan(2).PaddingTop(6).PaddingBottom(2).Text("Custos Adicionais da Bancada:").SemiBold();

                        table.Cell().Text("- Mão de Obra");
                        var labor = os.LaborCost.HasValue ? os.LaborCost.Value.ToString("C") : "0,00";
                        table.Cell().AlignRight().Text($"{labor}");
                        table.Cell().Text("- Peças Extras");
                        var parts = os.PartsCost.HasValue ? os.PartsCost.Value.ToString("C") : "0,00";
                        table.Cell().AlignRight().Text($"{parts}");
                    });
                });

                decimal? total = os.LaborCost + os.PartsCost + os.Services.Sum(s => s.Price);

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten4).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text("VALOR TOTAL DO ORÇAMENTO:").Bold().FontSize(11);
                    row.RelativeItem().AlignRight().Text(total.ToString()).Bold().FontSize(11);
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("5. STATUS E VALIDADE JURÍDICA").Bold();

                    string statusText;
                    string notaText;
                    string statusColor;

                    switch (os.Status)
                    {
                        case (ServiceRequestStatus)0:
                            statusText = "[ AGUARDANDO ACEITE DO CLIENTE ]";
                            statusColor = Colors.Orange.Darken2;
                            notaText = "Nota: O início dos serviços está condicionado à aprovação eletrônica desta proposta através da plataforma.";
                            break;

                        case (ServiceRequestStatus)1:
                            statusText = "[ ORÇAMENTO APROVADO - EM MANUTENÇÃO ]";
                            statusColor = Colors.Blue.Darken2;
                            notaText = $"Proposta aprovada eletronicamente pelo cliente. Equipamento em bancada para execução dos serviços.";
                            break;

                        case (ServiceRequestStatus)2:
                            statusText = "[ SERVIÇO CONCLUÍDO ]";
                            statusColor = Colors.Green.Darken2;
                            notaText = "Manutenção finalizada com sucesso. Equipamento testado e pronto para retirada/entrega.";
                            break;

                        case (ServiceRequestStatus)3:
                            statusText = "[ SOLICITAÇÃO CANCELADA ]";
                            statusColor = Colors.Red.Darken2;
                            notaText = "Esta ordem de serviço foi cancelada e o escopo técnico foi desconsiderado.";
                            break;

                        case (ServiceRequestStatus)4:
                            statusText = "[ SOLICITAÇÃO REJEITADA ]";
                            statusColor = Colors.Red.Darken2;
                            notaText = "Esta ordem de serviço foi rejeitada pelo técnico e o seu escopo foi desconsiderado.";
                            break;

                        default:
                            statusText = "[ EM ANÁLISE ]";
                            statusColor = Colors.Grey.Darken2;
                            notaText = "Aguardando atualizações do sistema.";
                            break;
                    }

                    c.Item().PaddingTop(2).Text(t =>
                    {
                        t.Span("Status da OS: ").SemiBold();
                        t.Span(statusText).Bold().FontColor(statusColor);
                    });

                    c.Item().PaddingTop(2).Text(notaText).FontSize(9).Italic().FontColor(Colors.Grey.Darken2);
                });

                column.Item().PaddingTop(40).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().AlignCenter().Text("_________________________________________");
                        c.Item().AlignCenter().PaddingTop(2).Text("Assinatura do Cliente").SemiBold();
                        c.Item().AlignCenter().Text(os.CustomerName).FontSize(9);
                    });

                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().AlignCenter().Text("_________________________________________");
                        c.Item().AlignCenter().PaddingTop(2).Text("MW Soluções (Técnico Responsável)").SemiBold();
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().PaddingTop(10).Text(x =>
            {
                x.Span("Documento gerado eletronicamente. Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }
    }
}
