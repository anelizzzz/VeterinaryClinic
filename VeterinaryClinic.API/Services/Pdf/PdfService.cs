using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VeterinaryClinic.API.DTOs.LabResult;
using VeterinaryClinic.API.DTOs.MedicalRecord;

namespace VeterinaryClinic.API.Services.Pdf
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateMedicalRecordPdf(MedicalRecordResponseDto record)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().Text("FIȘĂ MEDICALĂ")
                            .FontSize(20).Bold().FontColor("#7c3aed").AlignCenter();

                        col.Item().PaddingTop(4).Text($"Data: {record.Date:dd.MM.yyyy}")
                            .FontSize(10).FontColor("#6b7280").AlignCenter();

                        col.Item().PaddingTop(24).Element(x => SectionBox(x, "Informații pacient", inner =>
                        {
                            inner.Item().Row(row =>
                            {
                                row.RelativeItem().Element(c => LabelValue(c, "Animal", record.PetName));
                                row.RelativeItem().Element(c => LabelValue(c, "Doctor", record.DoctorName));
                            });
                        }));

                        col.Item().PaddingTop(16).Element(x => SectionBox(x, "Diagnostic", inner =>
                        {
                            inner.Item().Text(string.IsNullOrWhiteSpace(record.Diagnosis) ? "Neprecizat" : record.Diagnosis);
                        }));

                        col.Item().PaddingTop(16).Element(x => SectionBox(x, "Tratament", inner =>
                        {
                            inner.Item().Text(string.IsNullOrWhiteSpace(record.Treatment) ? "Neprecizat" : record.Treatment);
                        }));

                        col.Item().PaddingTop(16).Element(x => SectionBox(x, "Observații", inner =>
                        {
                            inner.Item().Text(string.IsNullOrWhiteSpace(record.Observations) ? "Nicio observație" : record.Observations);
                        }));

                        col.Item().PaddingTop(40).Row(row =>
                        {
                            row.RelativeItem();
                            row.ConstantItem(200).Column(sig =>
                            {
                                sig.Item().BorderBottom(1).BorderColor("#d1d5db").PaddingBottom(4)
                                    .Text("").FontSize(10);
                                sig.Item().PaddingTop(4).Text("Semnătura medicului")
                                    .FontSize(9).FontColor("#6b7280").AlignCenter();
                            });
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        public byte[] GenerateLabResultPdf(LabResultResponseDto result)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var testTypeName = (int)result.TestType switch
            {
                0 => "Analiză sânge",
                1 => "Analiză urină",
                2 => "Radiografie",
                3 => "Ecografie",
                _ => "Necunoscut"
            };

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Item().Text("REZULTAT LABORATOR")
                            .FontSize(20).Bold().FontColor("#7c3aed").AlignCenter();

                        col.Item().PaddingTop(4).Text($"Data: {result.Date:dd.MM.yyyy}")
                            .FontSize(10).FontColor("#6b7280").AlignCenter();

                        col.Item().PaddingTop(24).Element(x => SectionBox(x, "Informații pacient", inner =>
                        {
                            inner.Item().Row(row =>
                            {
                                row.RelativeItem().Element(c => LabelValue(c, "Animal", result.PetName));
                                row.RelativeItem().Element(c => LabelValue(c, "Tip test", testTypeName));
                            });
                        }));

                        col.Item().PaddingTop(16).Element(x => SectionBox(x, "Valori cheie", inner =>
                        {
                            inner.Item().Text(string.IsNullOrWhiteSpace(result.KeyValues) ? "-" : result.KeyValues)
                                .FontFamily("Courier New").FontSize(10);
                        }));

                        col.Item().PaddingTop(16).Element(x => SectionBox(x, "Interpretare", inner =>
                        {
                            inner.Item().Text(string.IsNullOrWhiteSpace(result.Interpretation) ? "Fără interpretare" : result.Interpretation);
                        }));

                        col.Item().PaddingTop(16).Background("#fef3c7").Padding(12).Border(1)
                            .BorderColor("#fcd34d").Column(warn =>
                            {
                                warn.Item().Text("⚠ Notă")
                                    .FontSize(10).Bold().FontColor("#92400e");
                                warn.Item().PaddingTop(4).Text(
                                    "Acest document are caracter informativ. Interpretarea finală aparține medicului veterinar.")
                                    .FontSize(9).FontColor("#92400e");
                            });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private static void ComposeHeader(IContainer container)
        {
            container.BorderBottom(2).BorderColor("#7c3aed").PaddingBottom(12).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("VetCare").FontSize(22).Bold().FontColor("#7c3aed");
                    col.Item().Text("VETERINARY CLINIC").FontSize(9).FontColor("#9ca3af").LetterSpacing(0.1f);
                });
                row.ConstantItem(120).AlignRight().Column(col =>
                {
                    col.Item().Text("Document oficial").FontSize(9).FontColor("#6b7280").AlignRight();
                    col.Item().Text(DateTime.Now.ToString("dd.MM.yyyy")).FontSize(9).FontColor("#6b7280").AlignRight();
                });
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor("#e5e7eb").PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("VetCare Veterinary Clinic")
                    .FontSize(8).FontColor("#9ca3af");
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Pagina ").FontSize(8).FontColor("#9ca3af");
                    x.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    x.Span(" din ").FontSize(8).FontColor("#9ca3af");
                    x.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
            });
        }

        private static void SectionBox(IContainer container, string title, Action<ColumnDescriptor> content)
        {
            container.Border(1).BorderColor("#e5e7eb").Column(col =>
            {
                col.Item().Background("#f3f4f6").Padding(8).BorderBottom(1).BorderColor("#e5e7eb")
                    .Text(title).FontSize(10).Bold().FontColor("#374151");
                col.Item().Padding(12).Column(content);
            });
        }

        private static void LabelValue(IContainer container, string label, string value)
        {
            container.Column(col =>
            {
                col.Item().Text(label).FontSize(9).FontColor("#6b7280");
                col.Item().Text(value).FontSize(11).Bold();
            });
        }
    }
}