using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly string G3NexusBlue = "#4a90e2";
    private readonly string LightGray = "#f8f9fa";
    private readonly string BorderGray = "#dee2e6";
    private readonly string TextGray = "#666";

    public byte[] GenerateProjectQuotation(ProjectResponseDTO project)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            // PAGE 1: Project Details and Cost Breakdown
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Segoe UI"));

                page.Header().Height(80).Element(container => CreateHeader(container, project));

                page.Content().Column(col =>
                {
                    // Company Details
                    col.Item().PaddingBottom(15).Element(CreateCompanyDetails);

                    // PROJECT DETAILS Section
                    col.Item().Element(container => CreateSectionHeader(container, "PROJECT DETAILS"));
                    col.Item().Element(container => CreateProjectDetailsTable(container, project));

                    // COST BREAKDOWN Section  
                    col.Item().PaddingTop(10).Element(container => CreateSectionHeader(container, "COST BREAKDOWN"));
                    col.Item().Element(container => CreateCostBreakdownTable(container, project));

                    // PAYMENT SCHEDULE Section
                    col.Item().PaddingTop(10).Element(container => CreateSectionHeader(container, "PAYMENT SCHEDULE"));
                    col.Item().Element(container => CreatePaymentScheduleTable(container, project));
                });
            });

            // PAGE 2: Terms & Conditions
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Segoe UI"));

                page.Header().Height(80).Element(container => CreateHeader(container, project));

                page.Content().Column(col =>
                {
                    // Company Details
                    col.Item().PaddingBottom(15).Element(CreateCompanyDetails);

                    // TERMS & CONDITIONS Section
                    col.Item().Element(container => CreateSectionHeader(container, "TERMS & CONDITIONS"));
                    col.Item().Element(container => CreateTermsAndConditions(container));

                    // Signature Section
                    col.Item().PaddingTop(40).Element(CreateSignatureSection);

                    // Footer
                    col.Item().PaddingTop(20).Element(CreateFooter);
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private void CreateHeader(IContainer container, ProjectResponseDTO project)
    {
        container.Row(row =>
        {
            // Left side - Logo and Title
            row.RelativeColumn().Column(col =>
            {
                col.Item().Text("G3NEXUS")
                   .FontSize(28)
                   .Bold()
                   .FontColor(G3NexusBlue);

                col.Item().Text("QUOTATION")
                   .FontSize(16)
                   .FontColor("#333");
            });

            // Right side - Quotation Info
            row.ConstantColumn(200).Column(col =>
            {
                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Quotation Id: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{project.ProjectId.ToString() ?? "3110-01"}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Date: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{DateTime.Now:dd/MM/yyyy}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Client Name: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("ABC Pvt Ltd").FontSize(9).FontColor(TextGray); // Default client name
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Client Contact: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("Mr. De Silva").FontSize(9).FontColor(TextGray); // Default contact
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Client Email: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("silva@abcpvt.com").FontSize(9).FontColor(TextGray); // Default email
                });
            });
        });
    }

    private void CreateCompanyDetails(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Column(col =>
            {
                col.Item().Text(txt =>
                {
                    txt.Span("Prepared By: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("G3 Technologies (Pvt) Ltd").FontSize(9).FontColor(TextGray);
                });

                col.Item().Text(txt =>
                {
                    txt.Span("Phone: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("+123-456-7890").FontSize(9).FontColor(TextGray);
                });

                col.Item().Text(txt =>
                {
                    txt.Span("Email: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span("info@g3tech.com").FontSize(9).FontColor(TextGray);
                });
            });

            row.RelativeColumn(); // Empty right column
        });
    }

    private void CreateSectionHeader(IContainer container, string title)
    {
        container.Background(G3NexusBlue)
                 .Padding(8)
                 .Text(title)
                 .FontSize(12)
                 .Bold()
                 .FontColor(Colors.White);
    }

    private void CreateProjectDetailsTable(IContainer container, ProjectResponseDTO project)
    {
        container.PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(120);
                columns.RelativeColumn();
            });

            // Project Title
            table.Cell().Background(LightGray).Padding(8).Text("Project Title:").Bold().FontSize(9);
            table.Cell().Background("#e9ecef").Padding(8).Text(project.ProjectName ?? "Web Portal Development").FontSize(9);

            // Project Description
            table.Cell().Background(LightGray).Padding(8).Text("Project Description:").Bold().FontSize(9);
            table.Cell().Background("#e9ecef").Padding(8).Text(project.ProjectDescription ?? "A full-stack responsive web portal with an admin panel, user authentication, and reporting tools.").FontSize(9);

            // Estimated Duration
            table.Cell().Background(LightGray).Padding(8).Text("Estimated Duration:").Bold().FontSize(9);
            var duration = $"8 Weeks ({project.ActualStartDate?.ToString("MMMM dd, yyyy") ?? "July 22, 2025"} – {project.ActualEndDate?.ToString("MMMM dd, yyyy") ?? "September 13, 2025"})";
            table.Cell().Background("#e9ecef").Padding(8).Text(duration).FontSize(9);
        });
    }

    private void CreateCostBreakdownTable(IContainer container, ProjectResponseDTO project)
    {
        // Use QuotationCost data if available, otherwise use calculated values
        var quotationCost = project.QuotationCost;
        var totalBudget = project.TotalBudget;
        
        var advancePayment = quotationCost?.AdvancePayment ?? (totalBudget * 0.25m);
        var developmentCost = quotationCost?.DevelopmentCost ?? (totalBudget * 0.60m);
        var hostingDomain = quotationCost?.HostingAndDomain ?? 10000m;
        var sslCert = quotationCost?.SSLCertificate ?? 5000m;
        var deployment = quotationCost?.DeploymentCost ?? quotationCost?.ServerCost ?? 15000m;

        container.PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(120);
                columns.RelativeColumn();
                columns.ConstantColumn(80);
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("ITEM").Bold().FontSize(9);
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("DESCRIPTION").Bold().FontSize(9);
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("AMOUNT RS").Bold().FontSize(9);
            });

            // Advance Payment
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Advance Payment").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Required before project kickoff").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{advancePayment:F2}").Bold().FontSize(9);

            // Development Cost
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Development Cost").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("UI/UX Design, Frontend & Backend Dev").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{developmentCost:F2}").Bold().FontSize(9);

            // Hosting & Domain
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Hosting & Domain (1 Year)").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text(".com domain + 10GB SSD Hosting").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{hostingDomain:F2}").Bold().FontSize(9);

            // SSL Certificate
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("SSL Certificate").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Standard 256-bit SSL (1 Year)").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{sslCert:F2}").Bold().FontSize(9);

            // Deployment & Handover
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Deployment & Handover").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Server setup & final delivery").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{deployment:F2}").Bold().FontSize(9);

            // Total Row
            table.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("TOTAL PROJECT COST").Bold().FontSize(10);
            table.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("").FontSize(10);
            table.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{totalBudget:F2}").Bold().FontSize(10);
        });
    }

    private void CreatePaymentScheduleTable(IContainer container, ProjectResponseDTO project)
    {
        var totalBudget = project.TotalBudget;
        var advancePayment = project.QuotationCost?.AdvancePayment ?? (totalBudget * 0.25m);
        var finalPayment = totalBudget - advancePayment;

        container.PaddingTop(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(120);
                columns.RelativeColumn();
                columns.ConstantColumn(80);
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("PAYMENT TYPE").Bold().FontSize(9);
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("DESCRIPTION").Bold().FontSize(9);
                header.Cell().Background(LightGray).Border(1).BorderColor(BorderGray).Padding(8).Text("AMOUNT RS").Bold().FontSize(9);
            });

            // Advance Payment
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Advance Payment").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Required to start the project").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{advancePayment:F2}").Bold().FontSize(9);

            // Final Payment
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Final Payment").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).Text("Upon project completion").FontSize(9);
            table.Cell().Border(1).BorderColor(BorderGray).Padding(8).AlignRight().Text($"{finalPayment:F2}").Bold().FontSize(9);
        });
    }

    private void CreateTermsAndConditions(IContainer container)
    {
        container.PaddingTop(10).Column(col =>
        {
            col.Item().PaddingBottom(6).Text(txt =>
            {
                txt.Span("• ").FontColor(G3NexusBlue).Bold();
                txt.Span("Hosting, Domain, and SSL are valid for 1 year from the date of deployment.").FontSize(9).FontColor("#555");
            });

            col.Item().PaddingBottom(6).Text(txt =>
            {
                txt.Span("• ").FontColor(G3NexusBlue).Bold();
                txt.Span("After full payment, the source code and all deliverables will be handed over.").FontSize(9).FontColor("#555");
            });

            col.Item().PaddingBottom(6).Text(txt =>
            {
                txt.Span("• ").FontColor(G3NexusBlue).Bold();
                txt.Span("Support & maintenance available for 1 month post-deployment.").FontSize(9).FontColor("#555");
            });
        });
    }

    private void CreateSignatureSection(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeColumn().Column(col =>
            {
                col.Item().BorderBottom(1).BorderColor("#333").Height(30);
                col.Item().PaddingTop(3).Text("Authorized Signature: .........................").FontSize(9).FontColor(TextGray);
                col.Item().PaddingTop(8).Text("Date: ....................................................").FontSize(9).FontColor(TextGray);
            });

            row.ConstantColumn(20); // Spacing

            row.RelativeColumn().Column(col =>
            {
                col.Item().BorderBottom(1).BorderColor("#333").Height(30);
                col.Item().PaddingTop(3).Text("Client Signature: .............................").FontSize(9).FontColor(TextGray);
                col.Item().PaddingTop(8).Text("Date: ....................................................").FontSize(9).FontColor(TextGray);
            });
        });
    }

    private void CreateFooter(IContainer container)
    {
        container.BorderTop(1)
                 .BorderColor("#eee")
                 .PaddingTop(15)
                 .AlignCenter()
                 .Text("Thank you for choosing G3 Technologies (Pvt) Ltd.")
                 .FontSize(10)
                 .Bold()
                 .FontColor(G3NexusBlue);
    }
}