using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class TableCell2005 : ReportObject
	{
		internal class Definition : DefinitionStore<TableCell2005, Definition.Properties>
		{
			public enum Properties
			{
				ReportItems,
				ColSpan
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(1)]
		[ValidValues(1, int.MaxValue)]
		public int ColSpan
		{
			get
			{
				return base.PropertyStore.GetInteger(1);
			}
			set
			{
				((IntProperty)DefinitionStore<TableCell2005, Definition.Properties>.GetProperty(1)).Validate(this, value);
				base.PropertyStore.SetInteger(1, value);
			}
		}

		public TableCell2005()
		{
		}

		public TableCell2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ReportItems = new RdlCollection<ReportItem>();
			ColSpan = 1;
		}
	}
}
