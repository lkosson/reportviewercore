using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class DataValueCollection
	{
		private DataValueInstanceList m_instances;

		private DataValueCRIList m_expressions;

		public DataValue this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				string name = null;
				object value = null;
				if (ExpressionInfo.Types.Constant == m_expressions[index].Name.Type)
				{
					name = m_expressions[index].Name.Value;
				}
				else if (m_instances != null)
				{
					name = m_instances[index].Name;
				}
				if (ExpressionInfo.Types.Constant == m_expressions[index].Value.Type)
				{
					value = m_expressions[index].Value.Value;
				}
				else if (m_instances != null)
				{
					value = m_instances[index].Value;
				}
				return new DataValue(name, value);
			}
		}

		public int Count => m_expressions.Count;

		internal DataValueCollection(DataValueCRIList expressions, DataValueInstanceList instances)
		{
			m_expressions = expressions;
			m_instances = instances;
			Global.Tracer.Assert(m_expressions != null);
			Global.Tracer.Assert(m_instances == null || m_instances.Count == m_expressions.Count);
		}
	}
}
