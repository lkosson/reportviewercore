using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLWorksheetsModel : IWorksheetsModel, IEnumerable<IWorksheetModel>, IEnumerable
	{
		private readonly Worksheets _interface;

		protected readonly XMLWorkbookModel Workbook;

		protected readonly PartManager Manager;

		protected List<IWorksheetModel> Worksheets;

		private uint _nextId;

		internal uint NextId
		{
			get
			{
				return _nextId++;
			}
			set
			{
				_nextId = value;
			}
		}

		public Worksheets Interface => _interface;

		public int Count => SheetModels.Count;

		private IList<IWorksheetModel> SheetModels
		{
			get
			{
				if (Worksheets == null)
				{
					Worksheets = new List<IWorksheetModel>();
				}
				return Worksheets;
			}
		}

		public XMLWorksheetsModel(XMLWorkbookModel workbook, PartManager manager)
		{
			Workbook = workbook;
			Manager = manager;
			_interface = new Worksheets(this);
		}

		public IWorksheetModel GetWorksheet(int position)
		{
			if (position < 0 || position >= Count)
			{
				throw new FatalException();
			}
			return SheetModels[position];
		}

		public int getSheetPosition(string name)
		{
			if (SheetModels.Count == 0)
			{
				return -1;
			}
			for (int i = 0; i < SheetModels.Count; i++)
			{
				if (SheetModels[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual IStreamsheetModel CreateStreamsheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			throw new FatalException();
		}

		public IEnumerator<IWorksheetModel> GetEnumerator()
		{
			return SheetModels.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
