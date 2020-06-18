using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Paragraph : ReportElement
	{
		internal new class Definition : DefinitionStore<Paragraph, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				TextRuns,
				LeftIndent,
				RightIndent,
				HangingIndent,
				ListStyle,
				ListLevel,
				SpaceBefore,
				SpaceAfter
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TextRun>))]
		public IList<TextRun> TextRuns
		{
			get
			{
				return base.PropertyStore.GetObject<IList<TextRun>>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> LeftIndent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> RightIndent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> HangingIndent
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[DefaultValue(ListStyle.None)]
		public ListStyle ListStyle
		{
			get
			{
				return (ListStyle)base.PropertyStore.GetInteger(5);
			}
			set
			{
				base.PropertyStore.SetInteger(5, (int)value);
			}
		}

		[DefaultValue(0)]
		public int ListLevel
		{
			get
			{
				return base.PropertyStore.GetInteger(6);
			}
			set
			{
				base.PropertyStore.SetInteger(6, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> SpaceBefore
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[ReportExpressionDefaultValueConstant(typeof(ReportSize), "DefaultZeroSize")]
		public ReportExpression<ReportSize> SpaceAfter
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		public Paragraph()
		{
		}

		internal Paragraph(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TextRuns = new RdlCollection<TextRun>();
			TextRuns.Add(new TextRun());
		}
	}
}
