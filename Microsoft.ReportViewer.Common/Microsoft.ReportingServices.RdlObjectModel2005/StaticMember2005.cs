using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class StaticMember2005 : ReportObject
	{
		internal class Definition : DefinitionStore<StaticMember2005, Definition.Properties>
		{
			public enum Properties
			{
				Label,
				LabelLocID
			}

			private Definition()
			{
			}
		}

		public ReportExpression Label
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

		[XmlChildAttribute("Label", "LocID", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string LabelLocID
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

		public StaticMember2005()
		{
		}

		public StaticMember2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
