# This project is not supported/developed by Microsoft

This project is aimed at porting Microsoft SQL Server Reporting Services (Report Viewer) to .NET 6. Sources and examples are available at https://github.com/lkosson/reportviewercore/

# Basic usage


    Stream reportDefinition; // your RDLC from file or resource
    IEnumerable dataSource; // your datasource for the report
    
    LocalReport report = new LocalReport();
    report.LoadReportDefinition(reportDefinition);
    report.DataSources.Add(new ReportDataSource("source", dataSource));
    report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
    byte[] pdf = report.Render("PDF");
