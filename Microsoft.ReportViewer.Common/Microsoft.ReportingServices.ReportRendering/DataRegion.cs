using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal abstract class DataRegion : ReportItem
	{
		public virtual bool PageBreakAtEnd => ((Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtEnd;

		public virtual bool PageBreakAtStart => ((Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).PageBreakAtStart;

		public virtual bool KeepTogether => ((Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).KeepTogether;

		public virtual bool NoRows => false;

		public string NoRowMessage
		{
			get
			{
				ExpressionInfo noRows = ((Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).NoRows;
				if (noRows != null)
				{
					if (ExpressionInfo.Types.Constant == noRows.Type)
					{
						return noRows.Value;
					}
					return InstanceInfoNoRowMessage;
				}
				return null;
			}
		}

		internal abstract string InstanceInfoNoRowMessage
		{
			get;
		}

		public string DataSetName => ((Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef).DataSetName;

		internal DataRegion(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext)
			: base(null, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public int[] GetRepeatSiblings()
		{
			Microsoft.ReportingServices.ReportProcessing.DataRegion dataRegion = (Microsoft.ReportingServices.ReportProcessing.DataRegion)base.ReportItemDef;
			if (dataRegion.RepeatSiblings == null)
			{
				return new int[0];
			}
			int[] array = new int[dataRegion.RepeatSiblings.Count];
			dataRegion.RepeatSiblings.CopyTo(array);
			return array;
		}
	}
}
