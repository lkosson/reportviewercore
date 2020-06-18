using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DesignTimeFieldConverter : StringConverter
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
			if (context != null && context.Instance != null)
			{
				object obj = null;
				obj = ((!(context.Instance is object[])) ? context.Instance : ((object[])context.Instance)[0]);
				if (obj is GroupRule)
				{
					arrayList.Add("(Name)");
					MapCore mapCore = ((GroupRule)obj).GetMapCore();
					if (mapCore != null)
					{
						foreach (Field groupField in mapCore.GroupFields)
						{
							arrayList.Add(groupField.Name);
						}
					}
				}
				else if (obj is PathRuleBase)
				{
					arrayList.Add("(Name)");
					MapCore mapCore2 = ((PathRuleBase)obj).GetMapCore();
					if (mapCore2 != null)
					{
						foreach (Field pathField in mapCore2.PathFields)
						{
							arrayList.Add(pathField.Name);
						}
					}
				}
				else if (obj is ShapeRule)
				{
					arrayList.Add("(Name)");
					MapCore mapCore3 = ((ShapeRule)obj).GetMapCore();
					if (mapCore3 != null)
					{
						foreach (Field shapeField in mapCore3.ShapeFields)
						{
							arrayList.Add(shapeField.Name);
						}
					}
				}
				else if (obj is SymbolRule)
				{
					arrayList.Add("(Name)");
					MapCore mapCore4 = ((SymbolRule)obj).GetMapCore();
					if (mapCore4 != null)
					{
						foreach (Field symbolField in mapCore4.SymbolFields)
						{
							arrayList.Add(symbolField.Name);
						}
					}
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
