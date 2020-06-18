using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreambookModel : XMLWorkbookModel, IStreambookModel, IWorkbookModel
	{
		private Stream _outputStream;

		private Package _zipPackage;

		public override IWorksheetsModel Worksheets
		{
			get
			{
				if (_manager == null)
				{
					throw new FatalException();
				}
				if (_worksheets == null)
				{
					_worksheets = new XMLStreamsheetsModel(this, _manager);
					_worksheets.NextId = (uint)(_worksheets.Count + 1);
				}
				return _worksheets;
			}
		}

		public Package ZipPackage
		{
			get
			{
				if (_zipPackage == null)
				{
					_zipPackage = Package.Open(_outputStream, FileMode.Create);
				}
				return _zipPackage;
			}
		}

		public XMLStreambookModel(Stream outputStream)
		{
			_outputStream = outputStream;
			_manager = new PartManager(this);
			_nameManager = new XMLDefinedNamesManager((CT_Workbook)_manager.Workbook.Root);
		}

		public void Save()
		{
			_manager.Write();
		}
	}
}
