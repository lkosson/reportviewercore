namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableModel
	{
		private readonly OpenXmlTableModel _containingTable;

		private InterleavingWriter _interleavingWriter;

		private OpenXmlTablePropertiesModel _tableProperties;

		private bool _writtenProperties;

		private OpenXmlTableGridModel _tableGrid;

		public OpenXmlTableModel ContainingTable => _containingTable;

		public OpenXmlTablePropertiesModel TableProperties => _tableProperties;

		public OpenXmlTableGridModel TableGrid => _tableGrid;

		public OpenXmlTableModel(OpenXmlTableModel containingTable, InterleavingWriter interleavingWriter, AutoFit autofit)
		{
			_containingTable = containingTable;
			_interleavingWriter = interleavingWriter;
			_interleavingWriter.TextWriter.Write("<w:tbl>");
			_tableProperties = new OpenXmlTablePropertiesModel(autofit);
		}

		public void WriteProperties()
		{
			if (!_writtenProperties)
			{
				_tableProperties.Write(_interleavingWriter.TextWriter);
				_interleavingWriter.WriteInterleaver(delegate(int index, long location)
				{
					_tableGrid = new OpenXmlTableGridModel(index, location);
					return TableGrid;
				});
				_writtenProperties = true;
			}
		}

		public void WriteCloseTag()
		{
			WriteProperties();
			_interleavingWriter.CommitInterleaver(_tableGrid);
			_interleavingWriter.TextWriter.Write("</w:tbl>");
		}
	}
}
