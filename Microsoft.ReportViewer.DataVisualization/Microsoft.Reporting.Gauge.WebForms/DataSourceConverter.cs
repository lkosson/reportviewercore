using System.Collections;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class DataSourceConverter : ReferenceConverter
	{
		public DataSourceConverter()
			: base(typeof(IListSource))
		{
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		private void FillList(ArrayList list1, StandardValuesCollection collection1)
		{
			foreach (object item in collection1)
			{
				if (item != null && list1.IndexOf(item) == -1)
				{
					list1.Add(item);
				}
			}
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			StandardValuesCollection standardValuesCollection = null;
			FillList(arrayList, standardValues);
			standardValuesCollection = new ReferenceConverter(typeof(IListSource)).GetStandardValues(context);
			FillList(arrayList, standardValuesCollection);
			standardValuesCollection = new ReferenceConverter(typeof(DataView)).GetStandardValues(context);
			FillList(arrayList, standardValuesCollection);
			standardValuesCollection = new ReferenceConverter(typeof(IDbDataAdapter)).GetStandardValues(context);
			FillList(arrayList, standardValuesCollection);
			standardValuesCollection = new ReferenceConverter(typeof(IDataReader)).GetStandardValues(context);
			FillList(arrayList, standardValuesCollection);
			standardValuesCollection = new ReferenceConverter(typeof(IDbCommand)).GetStandardValues(context);
			FillList(arrayList, standardValuesCollection);
			arrayList.Add(null);
			return new StandardValuesCollection(arrayList);
		}
	}
}
