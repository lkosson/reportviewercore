# ReportViewer Core
This project is aimed at porting Microsoft Reporting Services (Report Viewer) to .NET Core 3.1+. This is still work-in-progress and not ready for production use.

For version history and recent fixes, see [changelog](CHANGELOG.md).

# Why
With WinForms inclusion in .NET Core 3.1 and .NET 5 as a replacement for .NET Framework, it became feasible to port existing business desktop applications to .NET Core SDK to benefit from new C# and JIT features. Microsoft team stated on multiple occasions (https://github.com/dotnet/aspnetcore/issues/1528, https://github.com/dotnet/aspnetcore/issues/12666, https://github.com/dotnet/aspnetcore/issues/22304, https://github.com/dotnet/docs/issues/9607) that there are no plans to have official Reporting Services / ReportViewer package for .NET Core, which is a showstopper for applications using this technology for printing and reporting. The goal of this project is to provide transitional solution for such applications, until existing reports are reimplemented using more modern technology.

# How to use
You should be able to replace references to Report Viewer in your WinForms project with ones provided in this repo and use `Microsoft.Reporting.WinForms.ReportViewer` as usual. See project `ReportViewerCore.Sample.WinForms` for a simplified example using local report processing and `ReportViewerCore.Sample.WinFormsServer` for remote processing using Reporting Services server.

For ASP.NET Core applications, add reference to `Microsoft.Reporting.NETCore`, which is based on WinForms version with stripped UI and remote processing, and load/render report programmatically. See project `ReportViewerCore.Console` for an example or use following code as a starting point:

    Stream reportDefinition; // your RDLC from file or resource
    IEnumerable dataSource; // your datasource for the report
    
    LocalReport report = new LocalReport();
    report.LoadReportDefinition(reportDefinition);
    report.DataSources.Add(new ReportDataSource("source", dataSource));
    report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
    byte[] pdf = report.Render("PDF");

For consuming Reporting Services (server-side) reports, use:

    ServerReport report = new ServerReport();
    reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential("login", "password", "DOMAIN");
    reportViewer.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer");
    reportViewer.ServerReport.ReportPath = "/Invoice";
    reportViewer.ServerReport.SetParameters(new[] { new ReportParameter("Date", DateTime.Now.Date.ToString()) });
    byte[] pdf = report.Render("PDF");

or see project `ReportViewerCore.WinFormsServer` for more complete example.

There is no interactive, web-based report viewer provided in this project, but there are `HTML4.0` and `HTML5` rendering formats available. `HTML5` format has been modified to also work without JavaScript.

# What works
 * RDLC file loading and compiling
 * Local data sources
 * Parameter passing
 * PDF report rendering
 * Microsoft Word rendering
 * Microsoft Excel report rendering
 * WinForms report preview
 * Remote processing using Reporting Services
 * Partial Linux / MacOS support

# Supported rendering formats
 * HTML4.0 / HTML5 / MHTML (works on Windows, Linux and Mac OS)
 * PDF (Windows only)
 * IMAGE (TIFF/EMF, Windows only)
 * EXCEL (Microsoft Excel 97/2003, Windows only)
 * EXCELOPENXML (Microsoft Excel Open XML - works on Windows, Linux and Mac OS)
 * WORD (Microsoft Word 97/2003, Windows only)
 * WORDOPENXML (Microsoft Word Open XML - works on Windows, Linux and Mac OS)

# What doesn't work
 * Spatial SQL types. Those require `Microsoft.SqlServer.Types` package, which is available only in .NET Framework. Reports using SqlGeography won't load.
 * Expression sandboxing and code security. Do not load and run reports from untrusted sources.
 * Interactive web report preview. It is closely tied to WebForms and ASP.NET architecture and porting it to ASP.NET Core would involve rewriting significant portions of the codebase.
 * IMAGE, PDF, DOC and XLS output format rendering in non-Windows systems due to GDI32 and OLE32 dependencies.
 * WinForms control designer. To use ReportViewer in your WinForms project, add the control programmatically, as in `ReportViewerCore.Sample.WinForms\ReportViewerForm.cs`.

# What's untested
 * SQL Server data sources
 * ODBC Data sources. Most likely won't work.
 * MSChart control
 * Map control

# Sources
Source code for this project comes from decompiling Report Viewer for WinForms, version 15.0.1404.0 using ILSpy. Original Reporting Services use external Visual Basic and `System.CodeDom` compilation, both of which are not available in .NET Core. Those have been replaced with Roslyn Visual Basic compiler. References to .NET Framework assemblies have been updated to NuGet packages where possible. References to unavailable assemblies, such as `Microsoft.SqlServer.Types`, have been removed along with functionalities that depend on them. Sources are intentionally left formatted as decompiled by ILSpy.

Project `Microsoft.ReportViewer.WinForms` is mostly one-to-one recompilation of original ReportViewer for WinForms. Project `Microsoft.ReportViewer.NETCore` is heavilly stripped down version suitable for web applications, web services and batch processing.

# Binary package
A precompiled package is available as `ReportViewerCore.NETCore` and `ReportViewerCore.WinForms` at nuget.org: [ReportViewerCore.NETCore](https://www.nuget.org/packages/ReportViewerCore.NETCore/), [ReportViewerCore.WinForms](https://www.nuget.org/packages/ReportViewerCore.WinForms/). Legal aspects of redistributing binary package are uncertain. Feel free to compile this project on your own. You'll need Visual Studio 2019 (Community version will do) and .NET Core SDK 3.1. Reference either `Microsoft.ReportViewer.NETCore.dll` or `Microsoft.ReportViewer.WinForms.dll` in your solution.

# License
Reporting Services is a free Microsoft product. While decompiling and modifying it for compatibility reasons is legal in my local jurisdiction, redistributing modified version most likely is not. Use at your own risk.
