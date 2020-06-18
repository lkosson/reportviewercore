using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignTimeLegendConverter : StringConverter
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
				if (obj is RuleBase)
				{
					MapCore mapCore = ((RuleBase)obj).GetMapCore();
					if (mapCore != null)
					{
						foreach (Legend legend3 in mapCore.Legends)
						{
							arrayList.Add(legend3.Name);
						}
					}
				}
				else if (obj is SymbolRule)
				{
					MapCore mapCore2 = ((SymbolRule)obj).GetMapCore();
					if (mapCore2 != null)
					{
						foreach (Legend legend4 in mapCore2.Legends)
						{
							arrayList.Add(legend4.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
