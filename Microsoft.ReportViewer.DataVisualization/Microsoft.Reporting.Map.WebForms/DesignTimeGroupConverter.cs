using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignTimeGroupConverter : StringConverter
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
			arrayList.Add("(none)");
			if (context != null && context.Instance != null)
			{
				object obj = null;
				obj = ((!(context.Instance is object[])) ? context.Instance : ((object[])context.Instance)[0]);
				if (obj is Shape)
				{
					MapCore mapCore = ((Shape)obj).GetMapCore();
					if (mapCore != null)
					{
						foreach (Group group in mapCore.Groups)
						{
							arrayList.Add(group.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
