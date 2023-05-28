### FossPDF is a modern open-source .NET library for PDF document generation - it's a fork of QuestPDF at the latest commit that still used a FOSS (Free, Open-Source Software license).

It's MIT-licensed regardless of who you are, or what you're using the library for.

<table>
<tr>
    <td>👨‍💻</td>
    <td>Design PDF documents using C# and employ a code-only approach. Utilize your version control system to its fullest potential.</td>
</tr>
<tr>
    <td>🧱</td>
    <td>Compose PDF document with a range of powerful and predictable structural elements, such as text, image, border, table, and many more.</td>
</tr>
<tr>
    <td>⚙️</td>
    <td>Utilize a comprehensive layout engine, specifically designed for PDF document generation and paging support.</td>
</tr>
<tr>
    <td>📖</td>
    <td>Write code using concise and easy-to-understand C# Fluent API. Utilize IntelliSense to quickly discover available options.</td>
</tr>
<tr>
    <td>🔗</td>
    <td>Don't be limited to any proprietary scripting language or format. Follow your experience and leverage all modern C# features.</td>
</tr>
<tr>
    <td>⌛</td>
    <td>Save time thanks to a hot-reload capability, allowing real-time PDF document preview without code recompilation.</td>
</tr>
</table>

<br />

## Simplicity is the key

How easy it is to start and prototype with FossPDF? Really easy thanks to its minimal API! Please analyse the code below:

```#
using FossPDF.Fluent;
using FossPDF.Helpers;
using FossPDF.Infrastructure;

// code in your main method
Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.Background(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(20));
        
        page.Header()
            .Text("Hello PDF!")
            .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);
        
        page.Content()
            .PaddingVertical(1, Unit.Centimetre)
            .Column(x =>
            {
                x.Spacing(20);
                
                x.Item().Text(Placeholders.LoremIpsum());
                x.Item().Image(Placeholders.Image(200, 100));
            });
        
        page.Footer()
            .AlignCenter()
            .Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
    });
})
.GeneratePdf("hello.pdf");
```
