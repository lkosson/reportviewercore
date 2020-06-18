using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapSpatialDataSet : MapSpatialData
	{
		internal class Definition : DefinitionStore<MapSpatialDataSet, Definition.Properties>
		{
			internal enum Properties
			{
				DataSetName,
				SpatialField,
				MapFieldNames,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression DataSetName
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression SpatialField
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression>))]
		[XmlArrayItem("MapFieldName", typeof(ReportExpression))]
		public IList<ReportExpression> MapFieldNames
		{
			get
			{
				return (IList<ReportExpression>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapSpatialDataSet()
		{
		}

		internal MapSpatialDataSet(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapFieldNames = new RdlCollection<ReportExpression>();
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			Report ancestor = GetAncestor<Report>();
			if (ancestor != null)
			{
				DataSet dataSetByName = ancestor.GetDataSetByName(DataSetName.Expression);
				if (dataSetByName != null && !dependencies.Contains(dataSetByName))
				{
					dependencies.Add(dataSetByName);
				}
			}
		}
	}
}
