using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class CheckBox : ReportItem
	{
		public bool Value
		{
			get
			{
				ExpressionInfo value = ((Microsoft.ReportingServices.ReportProcessing.CheckBox)base.ReportItemDef).Value;
				if (value.Type == ExpressionInfo.Types.Constant)
				{
					return value.BoolValue;
				}
				if (base.ReportItemInstance != null)
				{
					return ((CheckBoxInstanceInfo)base.InstanceInfo).Value;
				}
				return false;
			}
		}

		public bool HideDuplicates => ((Microsoft.ReportingServices.ReportProcessing.CheckBox)base.ReportItemDef).HideDuplicates != null;

		public bool Duplicate
		{
			get
			{
				if (base.ReportItemInstance == null)
				{
					return false;
				}
				return ((CheckBoxInstanceInfo)base.InstanceInfo).Duplicate;
			}
		}

		internal CheckBox(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.CheckBox reportItemDef, CheckBoxInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}
	}
}
