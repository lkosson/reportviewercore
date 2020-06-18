using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ValueSourceConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				NamedElement namedElement = null;
				if (context.Instance is NamedElement)
				{
					namedElement = (NamedElement)context.Instance;
				}
				if (context.Instance is IList && ((IList)context.Instance).Count > 0)
				{
					namedElement = (NamedElement)((IList)context.Instance)[0];
				}
				if (namedElement != null && namedElement.Common != null)
				{
					foreach (InputValue value in namedElement.Common.GaugeCore.Values)
					{
						arrayList.Add(value.Name);
						foreach (CalculatedValue calculatedValue in value.CalculatedValues)
						{
							arrayList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", calculatedValue.InputValueObj.Name, calculatedValue.Name));
						}
					}
				}
			}
			arrayList.Add("(none)");
			return new StandardValuesCollection(arrayList);
		}
	}
}
