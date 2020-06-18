using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class MapElementView : MapView
	{
		internal new class Definition : DefinitionStore<MapElementView, Definition.Properties>
		{
			internal enum Properties
			{
				Zoom,
				LayerName,
				MapBindingFieldPairs,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public ReportExpression LayerName
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

		[XmlElement(typeof(RdlCollection<MapBindingFieldPair>))]
		public IList<MapBindingFieldPair> MapBindingFieldPairs
		{
			get
			{
				return (IList<MapBindingFieldPair>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public MapElementView()
		{
		}

		internal MapElementView(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			MapBindingFieldPairs = new RdlCollection<MapBindingFieldPair>();
		}
	}
}
