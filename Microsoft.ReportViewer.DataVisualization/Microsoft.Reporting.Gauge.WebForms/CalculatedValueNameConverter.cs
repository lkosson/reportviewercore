using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class CalculatedValueNameConverter : StringConverter
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
				if (context.Instance is CalculatedValue)
				{
					namedElement = (CalculatedValue)context.Instance;
				}
				if (namedElement == null && context.Instance is Array)
				{
					Array array = (Array)context.Instance;
					if (array.Length > 0 && array.GetValue(0) is CalculatedValue)
					{
						namedElement = (CalculatedValue)array.GetValue(0);
					}
				}
				if (namedElement != null && namedElement.Collection != null)
				{
					arrayList.Add(namedElement.Collection.parent.Name);
					foreach (NamedElement item in namedElement.Collection)
					{
						if (item.Name != namedElement.Name)
						{
							arrayList.Add(item.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
