using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class ChartDataSourceConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Container != null)
			{
				foreach (IComponent component in context.Container.Components)
				{
					if (ChartImage.IsValidDataSource(component))
					{
						arrayList.Add(component.Site.Name);
					}
				}
			}
			arrayList.Add("(none)");
			return new StandardValuesCollection(arrayList);
		}
	}
}
