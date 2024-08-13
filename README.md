# ReportViewer Core
This project is a port of Microsoft Reporting Services (Report Viewer) to .NET 6+. It is feature-complete and ready for production use, but keep in mind it is not officially supported by Microsoft.

For version history and recent fixes, see [changelog](CHANGELOG.md).

# Why
With WinForms inclusion in .NET Core 3.1 and .NET 5 as a replacement for .NET Framework, it became feasible to port existing business desktop applications to .NET Core SDK to benefit from new C# and JIT features. Microsoft team stated on multiple occasions (https://github.com/dotnet/aspnetcore/issues/1528, https://github.com/dotnet/aspnetcore/issues/12666, https://github.com/dotnet/aspnetcore/issues/22304, https://github.com/dotnet/docs/issues/9607) that there are no plans to have official Reporting Services / ReportViewer package for .NET Core, which is a showstopper for applications using this technology for printing and reporting. The goal of this project is to provide transitional solution for such applications, until existing reports are reimplemented using more modern technology.

# How to use
You should be able to replace references to Report Viewer in your WinForms project with ones provided in this repo and use `Microsoft.Reporting.WinForms.ReportViewer` as usual. See project `ReportViewerCore.Sample.WinForms` for a simplified example using local report processing and `ReportViewerCore.Sample.WinFormsServer` for remote processing using Reporting Services server.

For ASP.NET Core applications, add reference to `Microsoft.Reporting.NETCore`, which is based on WinForms version with stripped UI, and load/render report programmatically. See project `ReportViewerCore.Console` for an example or use following code as a starting point:

    Stream reportDefinition; // your RDLC from file or resource
    IEnumerable dataSource; // your datasource for the report
    
    LocalReport report = new LocalReport();
    report.LoadReportDefinition(reportDefinition);
    report.DataSources.Add(new ReportDataSource("source", dataSource));
    report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
    byte[] pdf = report.Render("PDF");

For consuming Reporting Services (server-side) reports, use:

    ServerReport report = new ServerReport();
    report.ReportServerCredentials.NetworkCredentials = new NetworkCredential("login", "password", "DOMAIN");
    report.ReportServerUrl = new Uri("http://localhost/ReportServer");
    report.ReportPath = "/Invoice";
    report.SetParameters(new[] { new ReportParameter("Date", DateTime.Now.Date.ToString()) });
    byte[] pdf = report.Render("PDF");

or see project `ReportViewerCore.WinFormsServer` for more complete example.

There is no interactive, web-based report viewer provided in this project, but there are `HTML4.0` and `HTML5` rendering formats available. `HTML5` format has been modified to also work without JavaScript. See `ReportViewerCore.Sample.AspNetCore` project for a simple demo.

# Designing new reports

Visual Studio 2019 (version 16.9 and similar) does not include Report Designer by default. There is an extension provided by Microsoft called "[Microsoft RDLC Report Designer](https://marketplace.visualstudio.com/items?itemName=ProBITools.MicrosoftRdlcReportDesignerforVisualStudio-18001)" you need to be able to open and modify your reports. For Visual Studio 2022, you need "[Microsoft RDLC Report Designer 2022](https://marketplace.visualstudio.com/items?itemName=ProBITools.MicrosoftRdlcReportDesignerforVisualStudio2022)" extension.

Even after installing the extension, new dataset wizard fails to show classes from your project and .NET Core projects don't have `.datasource` file support either. The workaround is to create and add `.xsd` file to your project with definitions of types you want to use in your reports. You can either create this file by hand or use the following snippet to produce one for all the classes you need:

    var types = new[] { typeof(ReportItemClass1), typeof(ReportItemClass2), typeof(ReportItemClass3) };
    var xri = new System.Xml.Serialization.XmlReflectionImporter();
    var xss = new System.Xml.Serialization.XmlSchemas();
    var xse = new System.Xml.Serialization.XmlSchemaExporter(xss);
    foreach (var type in types)
    {
        var xtm = xri.ImportTypeMapping(type);
        xse.ExportTypeMapping(xtm);
    }
    using var sw = new System.IO.StreamWriter("ReportItemSchemas.xsd", false, Encoding.UTF8);
    for (int i = 0; i < xss.Count; i++)
    {
        var xs = xss[i];
        xs.Id = "ReportItemSchemas";
        xs.Write(sw);
    }

After including `ReportItemSchemas.xsd` file in your project, Report Designer should see a new datasource called `ReportItemSchemas` which you can use to add a dataset to your report.

# What works
 * RDLC file loading and compiling
 * Local data sources
 * Parameter passing
 * All rendering formats, including PDF and XLS
 * WinForms report preview
 * Remote processing using Reporting Services
 * Linux and MacOS support
 * MSChart control

# Supported rendering formats

All formats are supported on Windows, Linux and Mac OS. For formats marked with asterisk (*), see "Linux rendering workaround" section below.

 * HTML4.0 / HTML5 / MHTML
 * PDF (*)
 * IMAGE (TIFF/EMF) (*)
 * EXCEL (Microsoft Excel 97/2003) (*)
 * EXCELOPENXML (Microsoft Excel Open XML)
 * WORD (Microsoft Word 97/2003) (*)
 * WORDOPENXML (Microsoft Word Open XML)
 * CSV
 * XML

# Linux rendering workaround

Some rendering formats (most notably PDF) uses Windows-provided native libraries for font measurements (Uniscribe), which are unavailable on other platforms. They are, however, provided in barely working condition, by [Wine](https://www.winehq.org/) version 5.0 or higher. To export your reports to PDF, TIFF or XLS:

 * Install Wine 5.0 or newer. For Debian Bullseye, `apt install wine` will do.
 * Download **Windows version** of .NET runtime **binaries** from `https://dotnet.microsoft.com/en-us/download/dotnet` - e.g. `https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-3.1.30-windows-x86-binaries`.
 * Extract/install those binaries to some local folder, e.g. `~/dotnet-3.1-windows`.
 * Start your application using 64-bit version of Wine and Windows version of .NET, e.g. `wine64 ~/dotnet-3.1-windows/dotnet.exe YourApplication.dll`.

If your application crashes with `unsupported flags 00000020` somewhere inside `bcrypt`, make sure you have proper version of Wine installed. Version 4.1 provided in Debian Buster and earlier won't work.

# What doesn't work
 * Spatial SQL types. Those require `Microsoft.SqlServer.Types` package, which is available only in .NET Framework. Reports using SqlGeography won't load.
 * Expression sandboxing and code security. Do not load and run reports from untrusted sources.
 * Interactive web report preview. It is closely tied to WebForms and ASP.NET architecture and porting it to ASP.NET Core would involve rewriting significant portions of the codebase.
 * WinForms control designer. To use ReportViewer in your WinForms project, add the control programmatically, as in `ReportViewerCore.Sample.WinForms\ReportViewerForm.cs`.
 * Single .exe deployment. Roslyn needs to be able to reference .NET and ReportViewer assemblies at runtime. When compiled to a single file, those are unavailable and any non-trivial report won't compile.
 * Map control. Not really tested, but included in project anyway.
 * As of .NET 6, Microsoft [deprecated](https://aka.ms/systemdrawingnonwindows) `System.Drawing` on non-windows platforms and removed it completely in .NET 7. This breaks reports using images on those platforms. Using a workaround mentioned above in `Linux rendering workaround` might help in those cases.

If you get `Version conflict detected for "Microsoft.CodeAnalysis.Common"` error when adding this NuGet package, try first adding `Microsoft.CodeAnalysis.CSharp.Workspaces 3.6.0` or `Microsoft.CodeAnalysis.Common 3.6.0` (manually selecting version 3.6.0) package to your project. For .NET 5 use 3.8.0 version. For .NET 6 use 4.0.1.

# Reporting bugs

Before reporting issue, please make sure that your problem occurs only when using this package, i.e. it doesn't happen on original Report Viewer. When filing an issue, include full exception stack trace and - where possible - provide relevant RDLC or a sample project.

# Sources
Source code for this project comes from decompiling Report Viewer for WinForms, version 15.0.1404.0 using ILSpy. Original Reporting Services use external Visual Basic and `System.CodeDom` compilation, both of which are not available in .NET Core. Those have been replaced with Roslyn Visual Basic compiler. References to .NET Framework assemblies have been updated to NuGet packages where possible. References to unavailable assemblies, such as `Microsoft.SqlServer.Types`, have been removed along with functionalities that depend on them. Sources are intentionally left formatted as decompiled by ILSpy.

Project `Microsoft.ReportViewer.WinForms` is mostly one-to-one recompilation of original ReportViewer for WinForms. Project `Microsoft.ReportViewer.NETCore` is heavilly stripped down version suitable for web applications, web services and batch processing.

# Binary package
A precompiled package is available as `ReportViewerCore.NETCore` and `ReportViewerCore.WinForms` at nuget.org: [ReportViewerCore.NETCore](https://www.nuget.org/packages/ReportViewerCore.NETCore/), [ReportViewerCore.WinForms](https://www.nuget.org/packages/ReportViewerCore.WinForms/). Legal aspects of redistributing binary package are uncertain. Feel free to compile this project on your own. You'll need Visual Studio 2022 (Community version will do) and .NET 6 SDK. Reference either `Microsoft.ReportViewer.NETCore.dll` or `Microsoft.ReportViewer.WinForms.dll` in your solution.

# License
Reporting Services is a free Microsoft product. While decompiling and modifying it for compatibility reasons is legal in my local jurisdiction, redistributing modified version most likely is not. Use at your own risk.
