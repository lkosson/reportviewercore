using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class ParentSourceConverter : StringConverter
	{
		internal const string parentNone = "(none)";

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
				if (namedElement != null && namedElement.Common != null)
				{
					arrayList = namedElement.Common.ObjectLinker.GetObjectNames(namedElement);
				}
			}
			arrayList.Add("(none)");
			return new StandardValuesCollection(arrayList);
		}
	}
}
