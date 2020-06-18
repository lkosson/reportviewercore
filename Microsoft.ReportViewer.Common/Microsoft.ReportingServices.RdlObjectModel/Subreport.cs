using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Subreport : ReportItem
	{
		internal new class Definition : DefinitionStore<Subreport, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				ReportName,
				Parameters,
				NoRowsMessage,
				MergeTransactions,
				KeepTogether,
				OmitBorderOnPageBreak,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public string ReportName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(18);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(18, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Parameter>))]
		public IList<Parameter> Parameters
		{
			get
			{
				return (IList<Parameter>)base.PropertyStore.GetObject(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression NoRowsMessage
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[DefaultValue(false)]
		public bool MergeTransactions
		{
			get
			{
				return base.PropertyStore.GetBoolean(21);
			}
			set
			{
				base.PropertyStore.SetBoolean(21, value);
			}
		}

		[DefaultValue(false)]
		public bool KeepTogether
		{
			get
			{
				return base.PropertyStore.GetBoolean(22);
			}
			set
			{
				base.PropertyStore.SetBoolean(22, value);
			}
		}

		[DefaultValue(false)]
		public bool OmitBorderOnPageBreak
		{
			get
			{
				return base.PropertyStore.GetBoolean(23);
			}
			set
			{
				base.PropertyStore.SetBoolean(23, value);
			}
		}

		public Subreport()
		{
		}

		internal Subreport(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			ReportName = "";
			Parameters = new RdlCollection<Parameter>();
		}
	}
}
