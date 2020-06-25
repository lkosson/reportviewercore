# ReportViewer Core
This project is aimed at porting Microsoft Reporting Services (Report Viewer) to .NET Core 3.1+. This is still work-in-progress and not ready for production use.

# How to use
You should be able to replace references to Report Viewer in your WinForms project with ones provided in this repo and use `Microsoft.Reporting.WinForms.ReportViewer` as usual. For web applications and batch use, easiest way is to:

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
 * SQL Server data sources
 * ODBC Data sources
 * WebForms report preview
 * Spatial SQL types
 
# Sources
Source code for this project comes from decompiling Report Viewer for WinForms, version 15.0.1404.0 using ILSpy. References to .NET Framework assemblies have been updated to NuGet packages and/or removed. System.CodeDom compilation and expression parsing is replaced with Roslyn Visual Basic compiler.

# License
Reporting Services is a free Microsoft product. While decompiling and modifying it for compatibility reasons is legal in my local jurisdiction, redistributing modified version most likely is not. Use at your own risk.
