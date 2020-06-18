using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	[XmlElementClass("Line", typeof(Line))]
	[XmlElementClass("Rectangle", typeof(Rectangle))]
	[XmlElementClass("Textbox", typeof(Textbox))]
	[XmlElementClass("Image", typeof(Image))]
	[XmlElementClass("Subreport", typeof(Subreport))]
	[XmlElementClass("Chart", typeof(Chart))]
	[XmlElementClass("GaugePanel", typeof(GaugePanel))]
	[XmlElementClass("Map", typeof(Map))]
	[XmlElementClass("Tablix", typeof(Tablix))]
	[XmlElementClass("CustomReportItem", typeof(CustomReportItem))]
	internal abstract class ReportItem : ReportElement, IGlobalNamedObject, INamedObject, IShouldSerialize
	{
		internal new class Definition : DefinitionStore<ReportItem, Definition.Properties>
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
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				return (ActionInfo)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize Top
		{
			get
			{
				return base.PropertyStore.GetSize(3);
			}
			set
			{
				base.PropertyStore.SetSize(3, value);
			}
		}

		[DefaultValueConstant("DefaultZeroSize")]
		public ReportSize Left
		{
			get
			{
				return base.PropertyStore.GetSize(4);
			}
			set
			{
				base.PropertyStore.SetSize(4, value);
			}
		}

		public ReportSize Height
		{
			get
			{
				return base.PropertyStore.GetSize(5);
			}
			set
			{
				base.PropertyStore.SetSize(5, value);
			}
		}

		public ReportSize Width
		{
			get
			{
				return base.PropertyStore.GetSize(6);
			}
			set
			{
				base.PropertyStore.SetSize(6, value);
			}
		}

		[DefaultValue(0)]
		[ValidValues(0, int.MaxValue)]
		public int ZIndex
		{
			get
			{
				return base.PropertyStore.GetInteger(7);
			}
			set
			{
				((IntProperty)DefinitionStore<ReportItem, Definition.Properties>.GetProperty(7)).Validate(this, value);
				base.PropertyStore.SetInteger(7, value);
			}
		}

		public Visibility Visibility
		{
			get
			{
				return (Visibility)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression ToolTip
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression DocumentMapLabel
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(11);
			}
			set
			{
				base.PropertyStore.SetObject(11, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression Bookmark
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[DefaultValue("")]
		public string RepeatWith
		{
			get
			{
				return (string)base.PropertyStore.GetObject(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[XmlElement(typeof(RdlCollection<CustomProperty>))]
		public IList<CustomProperty> CustomProperties
		{
			get
			{
				return (IList<CustomProperty>)base.PropertyStore.GetObject(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[DefaultValue("")]
		public string DataElementName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[DefaultValue(DataElementOutputTypes.Auto)]
		[ValidEnumValues("ReportItemDataElementOutputTypes")]
		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return (DataElementOutputTypes)base.PropertyStore.GetInteger(17);
			}
			set
			{
				base.PropertyStore.SetInteger(17, (int)value);
			}
		}

		protected ReportItem()
		{
		}

		internal ReportItem(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			CustomProperties = new RdlCollection<CustomProperty>();
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return true;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			if (!(name == "Top"))
			{
				if (name == "Left" && Left.Value == 0.0 && Left.Type == ReportSize.DefaultType)
				{
					return SerializationMethod.Never;
				}
			}
			else if (Top.Value == 0.0 && Top.Type == ReportSize.DefaultType)
			{
				return SerializationMethod.Never;
			}
			return SerializationMethod.Auto;
		}
	}
}
