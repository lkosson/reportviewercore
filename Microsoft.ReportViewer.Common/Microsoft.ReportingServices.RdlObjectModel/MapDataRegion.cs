using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapDataRegion : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<MapDataRegion, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				DataSetName,
				Filters,
				MapMember,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[DefaultValue("")]
		public string DataSetName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Filter>))]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapMember MapMember
		{
			get
			{
				return (MapMember)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public MapDataRegion()
		{
		}

		internal MapDataRegion(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			Report ancestor = GetAncestor<Report>();
			if (ancestor != null)
			{
				DataSet dataSetByName = ancestor.GetDataSetByName(DataSetName);
				if (dataSetByName != null && !dependencies.Contains(dataSetByName))
				{
					dependencies.Add(dataSetByName);
				}
			}
		}
	}
}
