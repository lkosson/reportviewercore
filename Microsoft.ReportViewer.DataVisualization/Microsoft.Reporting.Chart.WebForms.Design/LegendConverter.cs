using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class LegendConverter : NoNameExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null && context.Instance is Chart)
			{
				Chart.controlCurrentContext = context;
			}
			return base.GetPropertiesSupported(context);
		}
	}
}
