using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Drillthrough : ReportObject
	{
		internal class Definition : DefinitionStore<Drillthrough, Definition.Properties>
		{
			internal enum Properties
			{
				ReportName,
				Parameters
			}

			private Definition()
			{
			}
		}

		public ReportExpression ReportName
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

		[XmlElement(typeof(RdlCollection<Parameter>))]
		public IList<Parameter> Parameters
		{
			get
			{
				return (IList<Parameter>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Drillthrough()
		{
		}

		internal Drillthrough(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Parameters = new RdlCollection<Parameter>();
		}
	}
}
