using MwSolucoes.Domain.DTOs;
using MwSolucoes.Domain.PdfGenerator;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MwSolucoes.Infrastructure.PdfGenerator
{
    public class ReceiptReportGenerator : IReceiptReportGenerator
    {
        public byte[] GenerateReceiptPdf(ReceiptReportDto receipt)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Content().Element(c => ComposeForm(c, receipt));
                    page.Footer().Element(ComposeFooter);
                });
            });
            return document.GeneratePdf();
        }

        private void ComposeForm(IContainer container, ReceiptReportDto receipt)
        {
            container.Column(column =>
            {
                column.Item().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).AlignCenter()
                    .Text("MW SOLUÇÕES - RECIBO DE QUITAÇÃO E TERMO DE GARANTIA")
                    .FontSize(12).Bold();

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text($"Protocolo OS: #{receipt.Protocol ?? "N/A"}").SemiBold();
                    row.RelativeItem().AlignRight().Text($"Data de Finalização: {receipt.FinishedAt:dd/MM/yyyy HH:mm}");
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Cliente: ").SemiBold();
                        t.Span(receipt.CustomerName);
                    });
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("CPF: ").SemiBold();
                        t.Span(receipt.CustomerCpf);
                    });
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("2. EQUIPAMENTO E SERVIÇOS EXECUTADOS").Bold();
                    c.Item().PaddingTop(2).Text($"Equipamento: {receipt.Equipment} - {receipt.BrandModel}");

                    c.Item().PaddingTop(4).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(120);
                        });

                        foreach (var servico in receipt.Services)
                        {
                            table.Cell().Text($"- {servico.Name}");
                            table.Cell().AlignRight().Text(servico.Price.ToString("C"));
                        }

                        if (receipt.PartsCost > 0)
                        {
                            table.Cell().Text("- Peças de Reposição");
                            table.Cell().AlignRight().Text(receipt.PartsCost.Value.ToString("C"));
                        }
                    });
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Background(Colors.Green.Lighten4).Padding(5).Column(c =>
                {
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"VALOR PAGO: {receipt.TotalAmount:C}").Bold().FontSize(11);
                        row.RelativeItem().AlignRight().Text($"Forma de Pagamento: {receipt.PaymentMethod}").SemiBold();
                    });
                });

                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Black).Padding(5).Column(c =>
                {
                    c.Item().Text("4. TERMO DE GARANTIA E DECLARAÇÃO DE RECEBIMENTO").Bold();

                    c.Item().PaddingTop(4).Text(t =>
                    {
                        t.Span("Fica assegurada a garantia legal de 90 (noventa) dias sobre os serviços de mão de obra e peças substituídas descritos neste documento, a contar da data de retirada. ").FontSize(9);
                        t.Span($"A garantia é válida até: {receipt.WarrantyEndDate:dd/MM/yyyy}.").Bold().FontSize(9);
                    });

                    c.Item().PaddingTop(4).Text("Declaro que retirei o equipamento acima citado devidamente consertado, testado e em perfeito funcionamento, estando de acordo com os serviços executados e dando plena quitação aos valores cobrados, previamente autorizados na plataforma eletrônica.")
                        .FontSize(9).Italic();
                });
                column.Item().PaddingTop(40).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().AlignCenter().Text("_________________________________________");
                        c.Item().AlignCenter().PaddingTop(2).Text("Assinatura do Cliente").SemiBold();
                        c.Item().AlignCenter().Text(receipt.CustomerName).FontSize(9);
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
                x.Span("Documento gerado eletronicamente pela MW Soluções. Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }
    }
}
