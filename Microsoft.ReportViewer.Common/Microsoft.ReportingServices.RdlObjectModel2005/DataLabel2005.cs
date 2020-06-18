using Microsoft.ReportingServices.RdlObjectModel;
using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.ComponentModel;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class DataLabel2005 : ChartDataLabel
	{
		internal new class Definition : DefinitionStore<DataLabel2005, Definition.Properties>
		{
			public enum Properties
			{
				Style = 9,
				Value,
				Visible,
				Position,
				Rotation
			}

			private Definition()
			{
			}
		}

		public new Style2005 Style
		{
			get
			{
				return (Style2005)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Value
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(10);
			}
			set
			{
				base.PropertyStore.SetObject(10, value);
			}
		}

		[DefaultValue(false)]
		public new bool Visible
		{
			get
			{
				return base.PropertyStore.GetBoolean(11);
			}
			set
			{
				base.PropertyStore.SetBoolean(11, value);
			}
		}

		[DefaultValue(0)]
		public new int Rotation
		{
			get
			{
				return base.PropertyStore.GetInteger(13);
			}
			set
			{
				base.PropertyStore.SetInteger(13, value);
			}
		}

		[XmlChildAttribute("Value", "LocID", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string ValueLocID
		{
			get
			{
				return (string)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public DataLabel2005()
		{
		}

		public DataLabel2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
