using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class RowGrouping2005 : ReportObject
	{
		internal class Definition : DefinitionStore<RowGrouping2005, Definition.Properties>
		{
			public enum Properties
			{
				Width,
				DynamicRows,
				StaticRows,
				FixedHeader
			}

			private Definition()
			{
			}
		}

		public ReportSize Width
		{
			get
			{
				return base.PropertyStore.GetSize(0);
			}
			set
			{
				base.PropertyStore.SetSize(0, value);
			}
		}

		public DynamicRows2005 DynamicRows
		{
			get
			{
				return (DynamicRows2005)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<StaticColumn2005>))]
		[XmlArrayItem("StaticRow", typeof(StaticColumn2005))]
		public IList<StaticColumn2005> StaticRows
		{
			get
			{
				return (IList<StaticColumn2005>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[DefaultValue(false)]
		public bool FixedHeader
		{
			get
			{
				return base.PropertyStore.GetBoolean(3);
			}
			set
			{
				base.PropertyStore.SetBoolean(3, value);
			}
		}

		public RowGrouping2005()
		{
		}

		public RowGrouping2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Width = Constants.DefaultZeroSize;
		}
	}
}
