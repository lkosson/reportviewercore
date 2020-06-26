# ReportViewer Core
This project is aimed at porting Microsoft Reporting Services (Report Viewer) to .NET Core 3.1+. This is still work-in-progress and not ready for production use.

# How to use
You should be able to replace references to Report Viewer in your WinForms project with ones provided in this repo and use `Microsoft.Reporting.WinForms.ReportViewer` as usual. See project `ReportViewerCore.WinForms` for an example.

For web applications and batch processing, add reference to `Microsoft.Reporting.NETCore`, which is based on WinForms version with stripped UI and remote processing, and load/render report programmatically. See project `ReportViewerCore.Console` for an example or use following code:

    Stream reportDefinition; // your RDLC from file or resource
    IEnumerable dataSource; // your datasource for the report
    
    LocalReport report = new LocalReport();
    report.LoadReportDefinition(reportDefinition);
    report.DataSources.Add(new ReportDataSource("source", dataSource));
    report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
    byte[] pdf = report.Render("PDF");

# What works
 * RDLC file loading and compiling
 * Local data sources
 * Parameter passing
 * PDF report rendering
 * Microsoft Word rendering
 * Microsoft Excel report rendering
 * WinForms report preview
 * MSChart control
 * Map control

# What doesn't work
 * Assembly unloading - each invocation loads multiple dynamic assemblies that stay in memory
 * SQL Server data sources
 * ODBC Data sources
 * WebForms report preview
 * Spatial SQL types
 * Expression sandboxing and code security

# What won't ever work
 * Interactive web report preview

# Sources
Source code for this project comes from decompiling Report Viewer for WinForms, version 15.0.1404.0 using ILSpy. References to .NET Framework assemblies have been updated to NuGet packages and/or removed. System.CodeDom compilation and expression parsing is replaced with Roslyn Visual Basic compiler.

# Binary package
I'm not confident enough with technical and legal aspects of this project to provide ready-to-use, compiled and packaged version. Feel free to compile this project on your own. You'll need Visual Studio 2019 (Community version will do) and .NET Core SDK 3.1. Reference either `Microsoft.ReportViewer.NETCore.dll` or Microsoft.ReportViewer.WinForms.dll` in your solution.

# License
Reporting Services is a free Microsoft product. While decompiling and modifying it for compatibility reasons is legal in my local jurisdiction, redistributing modified version most likely is not. Use at your own risk.
