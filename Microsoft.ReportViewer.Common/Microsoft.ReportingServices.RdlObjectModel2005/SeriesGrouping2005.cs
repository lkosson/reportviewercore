using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class SeriesGrouping2005 : ReportObject
	{
		internal class Definition : DefinitionStore<SeriesGrouping2005, Definition.Properties>
		{
			public enum Properties
			{
				DynamicSeries,
				StaticSeries,
				Style
			}

			private Definition()
			{
			}
		}

		public DynamicSeries2005 DynamicSeries
		{
			get
			{
				return (DynamicSeries2005)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<StaticMember2005>))]
		[XmlArrayItem("StaticMember", typeof(StaticMember2005))]
		public IList<StaticMember2005> StaticSeries
		{
			get
			{
				return (IList<StaticMember2005>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public SeriesGrouping2005()
		{
		}

		public SeriesGrouping2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			StaticSeries = new RdlCollection<StaticMember2005>();
		}
	}
}
