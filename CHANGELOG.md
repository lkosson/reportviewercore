# upcoming version
 * Removed .NET Core 3.1 and .NET 5 support
 * Added CSV Renderer
 * Added XML Data Exporter

# 15.1.21
 * Added assembly strong name

# 15.1.20
 * Fixed bug causing errors when exporting report with invalid image placeholders
 * Removed dependencies on BinaryFormatter in ResourceManager

# 15.1.19
 * Added .NET 8 version

# 15.1.18
 * Added localized string for WinForms Report Viewer
 * Fixed hyperlink handling in WinForms Report Viewer
 * Added .NET 7 version
 * Removed BinaryFormatter dependencies for .NET 7+

# 15.1.17
 * Fixed broken error messages in NETCore project due to missing string resources

# 15.1.16
 * Fixed ValidValues floating point culture-dependent formatting
 * Custom system-wide culture (LOCALE_CUSTOM_UNSPECIFIED) workaround
 * Multi-target .NET Core 3.1, .NET 5 and .NET 6 due to Microsoft.CodeAnalysis.VisualBasic dependencies

# 15.1.15
 * Fixed race condition in WinForms ReportViewer caused by missing thread abort in RefreshReport
 * Removed SqlClient and OleDb dependencies
 * .NET 6 compatibility

# 15.1.14
 * Added missing System.Text.RegularExpressions.dll assembly reference
 * https support for server-side reports
 * Fixed chart HTML4/5 rendering
 * Fixed invalid PDF missing image placeholder format
 * Fixed InvalidImage rendering in non-WinForms applications

# 15.1.13
 * Fixed BinaryFormatter/ResX issues with PDF export on .NET 5

# 15.1.12
 * Changed HTML4.0 and HTML5 renderer to embed images as data URLs
 * Fixed HTML5 renderer image scaling with javascript disabled
 * Added workaround for aborting background worker thread
 * Added missing ImageRenderer resources
 * Added remote report support to .NETCore version

# 15.1.11
 * Added remote report support to WinForms version

# 15.1.10
 * Added missing resources needed for gauge control

# 15.1.9
 * Added HTML4.0 / HTML5 / MHTML output format

# 15.1.8
 * Added missing resources, fixed resource logical names

# 15.1.7
 * Fixed subreport assembly naming
 * Fixed PDF 1252 character encoding

# 15.1.6
 * Added report assembly unloading support
 * Added missing UI resources

# 15.1.5
 * Added .NET 5 support
 * Added missing WinForms UI resources

# 15.1.4
 * Fixed loading images from external sources

# 15.1.3
 * Fixed IIF report expression
 * Added detailed compilation error message

# 15.1.2
 * Basic cross-platform support
 * Removed unused native method references
 * Renamed ambiguous projects

# 15.1.1
 * Initial version based on ReportViewer 15.0.1404.0