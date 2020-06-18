using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Header2005 : ReportObject
	{
		internal class Definition : DefinitionStore<Header2005, Definition.Properties>
		{
			public enum Properties
			{
				TableRows,
				RepeatOnNewPage,
				FixedHeader
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TableRow2005>))]
		[XmlArrayItem("TableRow", typeof(TableRow2005))]
		public IList<TableRow2005> TableRows
		{
			get
			{
				return (IList<TableRow2005>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue(false)]
		public bool RepeatOnNewPage
		{
			get
			{
				return base.PropertyStore.GetBoolean(1);
			}
			set
			{
				base.PropertyStore.SetBoolean(1, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedHeader
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		public Header2005()
		{
		}

		public Header2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TableRows = new RdlCollection<TableRow2005>();
		}
	}
}
