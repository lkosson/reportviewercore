using System;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class AltReportItem : ReportObject
	{
		internal class Definition : DefinitionStore<AltReportItem, Definition.Properties>
		{
			internal enum Properties
			{
				ReportItem
			}

			private Definition()
			{
			}
		}

		public ReportItem ReportItem
		{
			get
			{
				return (ReportItem)base.PropertyStore.GetObject(0);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		public AltReportItem()
		{
		}

		internal AltReportItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
