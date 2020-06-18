using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class ColumnGrouping2005 : ReportObject
	{
		internal class Definition : DefinitionStore<ColumnGrouping2005, Definition.Properties>
		{
			public enum Properties
			{
				Height,
				DynamicColumns,
				StaticColumns,
				FixedHeader
			}

			private Definition()
			{
			}
		}

		public ReportSize Height
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

		public DynamicColumns2005 DynamicColumns
		{
			get
			{
				return (DynamicColumns2005)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<StaticColumn2005>))]
		[XmlArrayItem("StaticColumn", typeof(StaticColumn2005))]
		public IList<StaticColumn2005> StaticColumns
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

		public ColumnGrouping2005()
		{
		}

		public ColumnGrouping2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Height = Constants.DefaultZeroSize;
			StaticColumns = new RdlCollection<StaticColumn2005>();
		}
	}
}
