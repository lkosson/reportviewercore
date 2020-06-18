using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParametersGridCellDefinitionList : ArrayList
	{
		public new ParameterGridLayoutCellDefinition this[int index] => (ParameterGridLayoutCellDefinition)base[index];

		public ParametersGridCellDefinitionList()
		{
		}

		public ParametersGridCellDefinitionList(int capacity)
			: base(capacity)
		{
		}
	}
}
