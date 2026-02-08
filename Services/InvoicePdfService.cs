using Cab_Management_System.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Cab_Management_System.Services
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public byte[] GenerateInvoice(Billing billing)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Cab Management System")
                                    .FontSize(22).Bold().FontColor(Colors.Blue.Darken2);
                                col.Item().Text("Professional Transportation Services")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                            });

                            row.ConstantItem(150).AlignRight().Column(col =>
                            {
                                col.Item().Text("INVOICE").FontSize(28).Bold().FontColor(Colors.Blue.Darken2);
                            });
                        });

                        column.Item().PaddingVertical(5).LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Invoice #: INV-{billing.Id:D5}").SemiBold();
                                col.Item().Text($"Date: {billing.PaymentDate:MMM dd, yyyy}");
                                col.Item().Text($"Status: {billing.Status}");
                            });

                            if (billing.Trip != null)
                            {
                                row.RelativeItem().AlignRight().Column(col =>
                                {
                                    col.Item().Text("Bill To:").SemiBold();
                                    col.Item().Text(billing.Trip.CustomerName);
                                    if (!string.IsNullOrWhiteSpace(billing.Trip.CustomerPhone))
                                        col.Item().Text($"Phone: {billing.Trip.CustomerPhone}");
                                    if (!string.IsNullOrWhiteSpace(billing.Trip.CustomerEmail))
                                        col.Item().Text($"Email: {billing.Trip.CustomerEmail}");
                                });
                            }
                        });
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Trip Details Section
                        if (billing.Trip != null)
                        {
                            column.Item().Text("Trip Details").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                            column.Item().PaddingTop(5);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                });

                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Background(Colors.Grey.Lighten4).Text("Trip ID").SemiBold();
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Text($"#{billing.Trip.Id}");

                                if (billing.Trip.Route != null)
                                {
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Background(Colors.Grey.Lighten4).Text("Route").SemiBold();
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Text($"{billing.Trip.Route.Origin} - {billing.Trip.Route.Destination}");
                                }

                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Background(Colors.Grey.Lighten4).Text("Trip Date").SemiBold();
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Text(billing.Trip.TripDate.ToString("MMM dd, yyyy"));

                                if (billing.Trip.Driver?.Employee != null)
                                {
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Background(Colors.Grey.Lighten4).Text("Driver").SemiBold();
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Text(billing.Trip.Driver.Employee.Name);
                                }

                                if (billing.Trip.Vehicle != null)
                                {
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Background(Colors.Grey.Lighten4).Text("Vehicle").SemiBold();
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                        .Text($"{billing.Trip.Vehicle.Make} {billing.Trip.Vehicle.Model} ({billing.Trip.Vehicle.RegistrationNumber})");
                                }

                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Background(Colors.Grey.Lighten4).Text("Trip Status").SemiBold();
                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                    .Text(billing.Trip.Status.ToString());
                            });
                        }

                        column.Item().PaddingTop(20);

                        // Billing Summary Section
                        column.Item().Text("Billing Summary").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                        column.Item().PaddingTop(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                            });

                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Background(Colors.Grey.Lighten4).Text("Amount").SemiBold();
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(billing.Amount.ToString("C")).Bold();

                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Background(Colors.Grey.Lighten4).Text("Payment Method").SemiBold();
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(billing.PaymentMethod.ToString());

                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Background(Colors.Grey.Lighten4).Text("Payment Date").SemiBold();
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(billing.PaymentDate.ToString("MMM dd, yyyy"));

                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Background(Colors.Grey.Lighten4).Text("Payment Status").SemiBold();
                            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text(billing.Status.ToString());
                        });
                    });

                    page.Footer().AlignCenter().Column(column =>
                    {
                        column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        column.Item().Text("Thank you for choosing Cab Management System!")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                        column.Item().Text($"Generated on {DateTime.Now:MMM dd, yyyy HH:mm}")
                            .FontSize(8).FontColor(Colors.Grey.Lighten1);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
