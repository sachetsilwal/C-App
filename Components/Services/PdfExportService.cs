using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Services;

public class PdfExportService
{
    private readonly IJournalEntryRepository _entries;

    public PdfExportService(IJournalEntryRepository entries)
    {
        _entries = entries;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> ExportAsync(DateOnly from, DateOnly to)
    {
        var items = await _entries.SearchAsync(null, from, to, null, null);

        var file = Path.Combine(
            FileSystem.AppDataDirectory,
            $"Journal_{from}_{to}.pdf"
        );

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text($"Journal Export ({from} → {to})")
                    .FontSize(18).Bold();

                page.Content().Column(col =>
                {
                    foreach (var e in items)
                    {
                        col.Item().PaddingBottom(10).BorderBottom(1).Column(c =>
                        {
                            c.Item().Text($"{e.EntryDate} — {e.Title}").Bold();
                            c.Item().Text(e.ContentMarkdown);
                            c.Item().Text($"{e.WordCount} words")
                                .FontSize(10).Italic();
                        });
                    }
                });
            });
        }).GeneratePdf(file);

        return file;
    }
}
