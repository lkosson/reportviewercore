using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Corner2005 : ReportObject
	{
		internal class Definition : DefinitionStore<Corner2005, Definition.Properties>
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

		public Corner2005()
		{
		}

		public Corner2005(IPropertyStore propertyStore)
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
