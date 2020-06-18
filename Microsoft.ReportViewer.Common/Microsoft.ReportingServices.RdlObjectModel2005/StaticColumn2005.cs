using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class StaticColumn2005 : ReportObject
	{
		internal class Definition : DefinitionStore<StaticColumn2005, Definition.Properties>
		{
			public enum Properties
			{
				ReportItems
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

		public StaticColumn2005()
		{
		}

		public StaticColumn2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ReportItems = new RdlCollection<ReportItem>();
		}
	}
}
