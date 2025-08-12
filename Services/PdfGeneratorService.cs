using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using G3NexusBackend.Models;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly string G3NexusBlue = "#2b4b93";
    private readonly string LightGray = "#f8f9fa";
    private readonly string BorderGray = "#dee2e6";
    private readonly string TextGray = "#666";
    private readonly ICompanyService CompanyService;
    
    public PdfGeneratorService(ICompanyService companyService)
    {
        CompanyService = companyService;
    }

    public byte[] GenerateProjectQuotation(ProjectResponseDTO project)
    {
        throw new NotImplementedException();
    }

    public byte[] GenerateProjectQuotation(ProjectResponseDTO project, string clientName, string clientContact, string clientEmail, List<string> selectedTerms)
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

                page.Header().Height(80).Element(container => CreateHeader(container, project, clientContact, clientEmail));

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

                page.Header().Height(80).Element(container => CreateHeader(container, project, clientContact, clientEmail));

                page.Content().Column(col =>
                {
                    // Company Details
                    col.Item().PaddingBottom(15).Element(CreateCompanyDetails);

                    // TERMS & CONDITIONS Section
                    col.Item().Element(container => CreateSectionHeader(container, "TERMS & CONDITIONS"));
                    col.Item().Element(container => CreateTermsAndConditions(container, selectedTerms));
                    
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

    private void CreateHeader(IContainer container, ProjectResponseDTO project, string clientContact, string clientEmail)
    {
        var company = CompanyService.GetCompanyByIdAsync(project.CompanyId).Result;
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
                    txt.Span($"{project.ProjectId}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Date: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{DateTime.Now:dd/MM/yyyy}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Company Name: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{company!.CompanyName}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Client Contact: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{clientContact}").FontSize(9).FontColor(TextGray);
                });

                col.Item().AlignRight().Text(txt =>
                {
                    txt.Span("Client Email: ").Bold().FontSize(9).FontColor(TextGray);
                    txt.Span($"{clientEmail}").FontSize(9).FontColor(TextGray);
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

    private void CreateTermsAndConditions(IContainer container, List<string> selectedTerms)
    {
        container.PaddingTop(10).Column(col =>
        {
            if (selectedTerms != null && selectedTerms.Count > 0)
            {
                foreach (var term in selectedTerms)
                {
                    col.Item().PaddingBottom(6).Text(txt =>
                    {
                        txt.Span("• ").FontColor(G3NexusBlue).Bold();
                        txt.Span(term).FontSize(9).FontColor("#555");
                    });
                }
            }
            else
            {
                col.Item().Text("No terms & conditions selected.").FontSize(9).FontColor("#999");
            }
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

    public byte[] GenerateRequirementQuotation(Requirement requirement, RequirementQuotationRequestDTO quotationRequest, string clientName, string clientContact, string clientEmail, string projectName)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Segoe UI"));

                page.Header().Height(80).Element(container => CreateRequirementQuotationHeader(container, requirement, clientContact, clientEmail, projectName));

                page.Content().Column(col =>
                {
                    // Company Details
                    col.Item().PaddingBottom(15).Element(CreateCompanyDetails);

                    // REQUIREMENT DETAILS Section
                    col.Item().Element(container => CreateSectionHeader(container, "REQUIREMENT DETAILS"));
                    col.Item().Element(container => CreateRequirementDetailsTable(container, requirement, quotationRequest, projectName));

                    // COST BREAKDOWN Section  
                    col.Item().PaddingTop(10).Element(container => CreateSectionHeader(container, "COST BREAKDOWN"));
                    col.Item().Element(container => CreateRequirementCostTable(container, quotationRequest));

                    // Footer
                    col.Item().PaddingTop(40).Element(CreateFooter);
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    private void CreateRequirementQuotationHeader(IContainer container, Requirement requirement, string clientContact, string clientEmail, string projectName)
    {
        container.Row(row =>
        {
            // Left side - Logo and Title
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("G3NEXUS")
                   .FontSize(28)
                   .Bold()
                   .FontColor(G3NexusBlue);

                col.Item().Text("REQUIREMENT QUOTATION")
                   .FontSize(14)
                   .FontColor("#333");
            });

            // Right side - Quotation Info
            row.ConstantItem(200).Column(col =>
            {
                col.Item().Text($"Quotation ID: REQ-{requirement.RequirementId:D6}")
                   .FontSize(9).FontColor(TextGray);
                col.Item().Text($"Date: {DateTime.Now:dd/MM/yyyy}")
                   .FontSize(9).FontColor(TextGray);
                col.Item().Text($"Project: {projectName}")
                   .FontSize(9).FontColor(TextGray);
                col.Item().Text($"Client: {clientContact}")
                   .FontSize(9).FontColor(TextGray);
                col.Item().Text($"Email: {clientEmail}")
                   .FontSize(9).FontColor(TextGray);
            });
        });
    }

    private void CreateRequirementDetailsTable(IContainer container, Requirement requirement, RequirementQuotationRequestDTO quotationRequest, string projectName)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(120);
                columns.RelativeColumn();
            });

            // Title
            table.Cell().Element(CellStyle).Text("Requirement Title:").Bold();
            table.Cell().Element(CellStyle).Text(requirement.RequirementTitle ?? "N/A");

            // Priority
            table.Cell().Element(CellStyle).Text("Priority:").Bold();
            table.Cell().Element(CellStyle).Text(requirement.Priority ?? "N/A");

            // Project
            table.Cell().Element(CellStyle).Text("Project:").Bold();
            table.Cell().Element(CellStyle).Text(projectName);

            // Description
            table.Cell().Element(CellStyle).Text("Description:").Bold();
            table.Cell().Element(CellStyle).Text(requirement.RequirementDescription ?? "N/A");

            // Estimated Duration
            table.Cell().Element(CellStyle).Text("Estimated Duration:").Bold();
            table.Cell().Element(CellStyle).Text(quotationRequest.EstimatedDuration ?? "N/A");

            // Delivery Date
            table.Cell().Element(CellStyle).Text("Expected Delivery:").Bold();
            table.Cell().Element(CellStyle).Text(quotationRequest.DeliveryDate.ToString("dd/MM/yyyy"));

            // Additional Notes
            if (!string.IsNullOrEmpty(quotationRequest.Description))
            {
                table.Cell().Element(CellStyle).Text("Additional Notes:").Bold();
                table.Cell().Element(CellStyle).Text(quotationRequest.Description);
            }
        });

        IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor("#dee2e6").Padding(8).Background("#f8f9fa");
        }
    }

    private void CreateRequirementCostTable(IContainer container, RequirementQuotationRequestDTO quotationRequest)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(HeaderCellStyle).Text("Description");
                header.Cell().Element(HeaderCellStyle).Text("Duration");
                header.Cell().Element(HeaderCellStyle).Text("Cost (LKR)");
            });

            // Cost Row
            table.Cell().Element(DataCellStyle).Text("Requirement Implementation");
            table.Cell().Element(DataCellStyle).Text(quotationRequest.EstimatedDuration);
            table.Cell().Element(DataCellStyle).AlignRight().Text($"{quotationRequest.QuotationCost:N2}");

            // Total Row
            table.Cell().Element(TotalCellStyle).Text("TOTAL COST").Bold();
            table.Cell().Element(TotalCellStyle).Text("");
            table.Cell().Element(TotalCellStyle).AlignRight().Text($"LKR {quotationRequest.QuotationCost:N2}").Bold();
        });

        IContainer HeaderCellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(BorderGray).Padding(8).Background(LightGray);
        }

        IContainer DataCellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(BorderGray).Padding(8).Background(Colors.White);
        }

        IContainer TotalCellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(BorderGray).Padding(8).Background(LightGray);
        }
    }
}