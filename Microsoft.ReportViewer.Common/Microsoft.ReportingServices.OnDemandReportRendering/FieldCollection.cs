using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class FieldCollection : ReportElementCollectionBase<Field>
	{
		private Field[] m_collection;

		private Microsoft.ReportingServices.ReportIntermediateFormat.DataSet m_dataSetdef;

		public override int Count => m_dataSetdef.NonCalculatedFieldCount;

		public override Field this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_collection == null)
				{
					m_collection = new Field[Count];
				}
				if (m_collection[index] == null)
				{
					m_collection[index] = new Field(m_dataSetdef.Fields[index]);
				}
				return m_collection[index];
			}
		}

		public Field this[string name] => this[GetFieldIndex(name)];

		internal FieldCollection(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef)
		{
			m_dataSetdef = dataSetDef;
		}

		public int GetFieldIndex(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return -1;
			}
			for (int i = 0; i < Count; i++)
			{
				if (string.Equals(name, m_dataSetdef.Fields[i].Name, StringComparison.Ordinal))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
