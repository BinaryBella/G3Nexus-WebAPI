using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;

public class PdfGeneratorService : IPdfGeneratorService
{
    public byte[] GenerateProjectQuotation(ProjectDTO project)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeColumn().Column(col =>
                    {
                        col.Item().Text("G3NEXUS").FontSize(20).Bold().FontColor("#2b4b93");
                        col.Item().Text("Quotation").FontSize(14);
                        col.Item().Text("Prepared By: G3 Technologies (Pvt) Ltd");
                        col.Item().Text("Phone: +123-456-7890");
                        col.Item().Text("Email: info@g3tech.com");
                    });

                    row.ConstantColumn(220).Column(col =>
                    {
                        col.Item().Text($"Quotation ID: {project.ProjectId}").Bold();
                        col.Item().Text($"Date: {DateTime.Now:dd/MM/yyyy}");
                        col.Item().Text($"Client Name: {project.ClientName}");
                        col.Item().Text($"Client Contact: {project.ClientEmail}");
                    });
                });

                page.Content().Column(col =>
                {
                    // SECTION: PROJECT DETAILS
                    col.Item().Element(container => SectionTitle(container, "PROJECT DETAILS"));

                    col.Item().Element(container => ContentBox(container, content =>
                    {
                        content.Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            table.Cell().Element(CellStyle).Text("Project Title:");
                            table.Cell().Element(CellStyle).Text(project.ProjectName);

                            table.Cell().Element(CellStyle).Text("Project Description:");
                            table.Cell().Element(CellStyle).Text(project.ProjectDescription);

                            table.Cell().Element(CellStyle).Text("Estimated Duration:");
                            table.Cell().Element(CellStyle).Text($"{project.ActualStartDate?.ToString("MMMM dd, yyyy") ?? "N/A"} – {project.ActualEndDate?.ToString("MMMM dd, yyyy") ?? "N/A"}");
                        });
                    }));

                    // SECTION: COST BREAKDOWN
                    col.Item().PaddingTop(10).Element(container => SectionTitle(container, "COST BREAKDOWN"));

                    col.Item().Element(container => ContentBox(container, content =>
                    {
                        content.Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(140);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ITEM").Bold();
                                header.Cell().Element(CellStyle).Text("DESCRIPTION").Bold();
                                header.Cell().Element(CellStyle).Text("AMOUNT RS").Bold();
                            });

                            table.Cell().Element(CellStyle).Text("Project Cost");
                            table.Cell().Element(CellStyle).Text("Total Project Cost");
                            table.Cell().Element(CellStyle).Text($"{project.TotalBudget:N2}");
                        });
                    }));

                    // SECTION: TERMS & CONDITIONS
                    col.Item().PaddingTop(10).Element(container => SectionTitle(container, "TERMS & CONDITIONS"));

                    col.Item().Element(container => ContentBox(container, content =>
                    {
                        content.Column(column =>
                        {
                            column.Item().Text("• Hosting, Domain, and SSL are valid for 1 year from the date of deployment.");
                            column.Item().Text("• After full payment, the source code and all deliverables will be handed over.");
                            column.Item().Text("• Support & maintenance available for 1 month post-deployment.");
                        });
                    }));

                    // SIGNATURE AREA
                    col.Item().PaddingTop(20).Element(container => ContentBox(container, content =>
                    {
                        content.Row(row =>
                        {
                            row.RelativeColumn().Column(inner =>
                            {
                                inner.Item().Text("Authorized Signature: ............................");
                                inner.Item().Text("Date: ..................................................");
                            });

                            row.RelativeColumn().Column(inner =>
                            {
                                inner.Item().Text("Client Signature: .................................");
                                inner.Item().Text("Date: ..................................................");
                            });
                        });
                    }));

                    col.Item().AlignCenter().PaddingTop(10).Text("Thank you for choosing G3 Technologies (Pvt) Ltd.").Italic();
                });

                // Style helpers
                static IContainer CellStyle(IContainer container) =>
                    container.PaddingVertical(4).PaddingHorizontal(3);

                static void SectionTitle(IContainer container, string title) =>
                    container.Background("#2b4b93").Padding(7)
                        .Text(title).FontSize(11).Bold().FontColor(Colors.White);

                static void ContentBox(IContainer container, Action<IContainer> content) =>
                    container.Background("#f5f5f5").Padding(10).ExtendHorizontal().Border(1).BorderColor("#ddd")
                        .Column(col => col.Item().Element(content));
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
}