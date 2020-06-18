using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal abstract class MapSpatialElement : ReportObject
	{
		internal class Definition : DefinitionStore<MapSpatialElement, Definition.Properties>
		{
			internal enum Properties
			{
				VectorData,
				MapFields
			}

			private Definition()
			{
			}
		}

		public VectorData VectorData
		{
			get
			{
				return base.PropertyStore.GetObject<VectorData>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<MapField>))]
		public IList<MapField> MapFields
		{
			get
			{
				return (IList<MapField>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public MapSpatialElement()
		{
		}

		internal MapSpatialElement(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapFields = new RdlCollection<MapField>();
		}
	}
}
