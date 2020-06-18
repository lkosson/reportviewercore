using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ScaleSourceConverter : StringConverter
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
				NamedCollection namedCollection = null;
				if (context.Instance is NamedElement)
				{
					namedElement = (NamedElement)context.Instance;
				}
				if (context.Instance is IList && ((IList)context.Instance).Count > 0)
				{
					namedElement = (NamedElement)((IList)context.Instance)[0];
				}
				if (namedElement != null && namedElement.Common != null && namedElement.ParentElement != null)
				{
					if (namedElement.ParentElement is CircularGauge)
					{
						namedCollection = ((CircularGauge)namedElement.ParentElement).Scales;
					}
					if (namedElement.ParentElement is LinearGauge)
					{
						namedCollection = ((LinearGauge)namedElement.ParentElement).Scales;
					}
				}
				if (namedCollection != null)
				{
					foreach (NamedElement item in namedCollection)
					{
						arrayList.Add(item.Name);
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
