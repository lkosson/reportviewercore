using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class SeriesYValueTypeConverter : EnumConverter
	{
		public SeriesYValueTypeConverter(Type type)
			: base(type)
		{
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object standardValue in base.GetStandardValues(context))
			{
				if (standardValue.ToString() != "String")
				{
					arrayList.Add(standardValue);
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
