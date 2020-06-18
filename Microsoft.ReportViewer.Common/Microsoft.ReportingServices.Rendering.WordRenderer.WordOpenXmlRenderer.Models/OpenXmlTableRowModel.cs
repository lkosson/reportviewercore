namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableRowModel
	{
		private readonly OpenXmlTableRowModel _containingRow;

		private InterleavingWriter _interleavingWriter;

		private OpenXmlTableRowPropertiesModel _rowProperties;

		private int[] _columnWidths;

		public OpenXmlTableRowModel ContainingRow => _containingRow;

		public OpenXmlTableRowPropertiesModel RowProperties => _rowProperties;

		public int[] ColumnWidths
		{
			get
			{
				return _columnWidths;
			}
			set
			{
				_columnWidths = value;
			}
		}

		public OpenXmlTableRowModel(OpenXmlTableRowModel containingRow, InterleavingWriter interleavingWriter)
		{
			_containingRow = containingRow;
			_interleavingWriter = interleavingWriter;
			interleavingWriter.TextWriter.Write("<w:tr>");
			_interleavingWriter.WriteInterleaver(delegate(int index, long location)
			{
				_rowProperties = new OpenXmlTableRowPropertiesModel(index, location);
				return RowProperties;
			});
		}

		public void WriteCloseTag()
		{
			_interleavingWriter.CommitInterleaver(RowProperties);
			_interleavingWriter.TextWriter.Write("</w:tr>");
		}
	}
}
