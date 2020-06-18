using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class CategoryGrouping2005 : ReportObject
	{
		internal class Definition : DefinitionStore<CategoryGrouping2005, Definition.Properties>
		{
			public enum Properties
			{
				DynamicCategories,
				StaticCategories
			}

			private Definition()
			{
			}
		}

		public DynamicSeries2005 DynamicCategories
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
		public IList<StaticMember2005> StaticCategories
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

		public CategoryGrouping2005()
		{
		}

		public CategoryGrouping2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			StaticCategories = new RdlCollection<StaticMember2005>();
		}
	}
}
