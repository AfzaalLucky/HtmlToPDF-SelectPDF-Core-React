using HtmlToPdfApi;
using SelectPdf;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Logo for the StringBuilder-generated HTML, inlined as a data URI.
var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
var logoDataUri = "data:image/png;base64," + Convert.ToBase64String(File.ReadAllBytes(logoPath));

const string PdfFileName = "AI-Foundry-Policy-AIF-POL-GOV-001.pdf";

app.MapGet("/", () => Results.Redirect("/policy-builder.pdf"));

// The policy HTML is built in code with a StringBuilder (see PolicyHtmlBuilder).
app.MapGet("/policy-builder.html", () =>
    Results.Content(PolicyHtmlBuilder.Build(logoDataUri), "text/html; charset=utf-8"));

app.MapGet("/policy-builder.pdf", () =>
    Results.File(RenderPdf(PolicyHtmlBuilder.Build(logoDataUri)), "application/pdf", PdfFileName));

app.Run();

static byte[] RenderPdf(string html)
{
    var converter = new HtmlToPdf();
    var options = converter.Options;

    // The HTML lays itself out as fixed A4 sheets under @media print, so render
    // with Blink (CSS variables, flex, grid) at A4 width with no PDF margins.
    options.RenderingEngine = RenderingEngine.Blink;
    options.CssMediaType = HtmlToPdfCssMediaType.Print;
    options.PdfPageSize = PdfPageSize.A4;
    options.PdfPageOrientation = PdfPageOrientation.Portrait;
    options.WebPageWidth = 794; // 210mm at 96 dpi
    options.WebPageHeight = 0;
    options.MarginTop = options.MarginBottom = options.MarginLeft = options.MarginRight = 0;
    options.PdfDocumentInformation.Title = "AI Foundry Responsible AI Policy (AIF-POL-GOV-001)";

    var doc = converter.ConvertHtmlString(html);
    try
    {
        return doc.Save();
    }
    finally
    {
        doc.Close();
    }
}
