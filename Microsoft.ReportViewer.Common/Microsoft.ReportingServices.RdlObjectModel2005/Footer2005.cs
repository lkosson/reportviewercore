using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Footer2005 : ReportObject
	{
		internal class Definition : DefinitionStore<Footer2005, Definition.Properties>
		{
			public enum Properties
			{
				TableRows,
				RepeatOnNewPage
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

		public Footer2005()
		{
		}

		public Footer2005(IPropertyStore propertyStore)
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
