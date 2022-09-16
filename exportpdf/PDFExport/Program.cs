using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PDFExport.Core;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

IHost app = Host.CreateDefaultBuilder(args).ConfigureHostConfiguration(configHost =>
{
    configHost.SetBasePath(Directory.GetCurrentDirectory());
}).ConfigureServices((hostContext, services) =>
{
    services.AddMvc();
    services.AddControllersWithViews();
    services.AddControllers();

    services.AddSingleton(typeof(IConverter), new STASynchronizedConverter(new PdfTools()));

}).ConfigureWebHostDefaults(webBuilder =>
{

}).Build();


IConverter converter = app.Services.GetRequiredService<IConverter>();

var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Landscape,
        PaperSize = PaperKind.A4Plus,
    },
    Objects = {
        new ObjectSettings() {
            PagesCount = true,
            HtmlContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. In consectetur mauris eget ultrices  iaculis. Ut odio viverra, molestie lectus nec, venenatis turpis.",
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
        }
    }
};

byte[] pdf = converter.Convert(doc);

File.WriteAllBytes($"files/base{DateTime.Now:dd_MM_yyyy_HH_mm}.pdf", pdf);
