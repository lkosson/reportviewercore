using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixHeader : IDOwner, IPersistable
	{
		private string m_size;

		private double m_sizeValue;

		private ReportItem m_cellContents;

		private ReportItem m_altCellContents;

		[NonSerialized]
		private static readonly string[] m_StylesForEmptyRectangleInSubtotals = new string[23]
		{
			"BackgroundColor",
			"BackgroundGradientType",
			"BackgroundGradientEndColor",
			"BackgroundImageMIMEType",
			"BackgroundImageSource",
			"BackgroundImageValue",
			"BackgroundImage",
			"BackgroundRepeat",
			"BorderColor",
			"BorderColorTop",
			"BorderColorBottom",
			"BorderColorRight",
			"BorderColorLeft",
			"BorderStyle",
			"BorderStyleTop",
			"BorderStyleBottom",
			"BorderStyleLeft",
			"BorderStyleRight",
			"BorderWidth",
			"BorderWidthTop",
			"BorderWidthBottom",
			"BorderWidthLeft",
			"BorderWidthRight"
		};

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ReportSize m_sizeForRendering;

		[NonSerialized]
		private List<ReportItem> m_cellContentCollection;

		internal string Size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
			}
		}

		internal double SizeValue
		{
			get
			{
				return m_sizeValue;
			}
			set
			{
				m_sizeValue = value;
			}
		}

		internal ReportSize SizeForRendering
		{
			get
			{
				return m_sizeForRendering;
			}
			set
			{
				m_sizeForRendering = value;
			}
		}

		internal ReportItem CellContents
		{
			get
			{
				return m_cellContents;
			}
			set
			{
				m_cellContents = value;
			}
		}

		internal ReportItem AltCellContents
		{
			get
			{
				return m_altCellContents;
			}
			set
			{
				m_altCellContents = value;
			}
		}

		internal List<ReportItem> CellContentCollection
		{
			get
			{
				if (m_cellContentCollection == null && m_cellContents != null)
				{
					m_cellContentCollection = new List<ReportItem>((m_altCellContents == null) ? 1 : 2);
					if (m_cellContents != null)
					{
						m_cellContentCollection.Add(m_cellContents);
					}
					if (m_altCellContents != null)
					{
						m_cellContentCollection.Add(m_altCellContents);
					}
				}
				return m_cellContentCollection;
			}
		}

		internal TablixHeader()
		{
		}

		internal TablixHeader(int id)
			: base(id)
		{
		}

		internal void Initialize(InitializationContext context, bool isColumn, bool ignoreSize)
		{
			if (m_cellContents != null)
			{
				m_cellContents.Initialize(context);
				if (m_altCellContents != null)
				{
					m_altCellContents.Initialize(context);
				}
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context, bool isClonedDynamic)
		{
			TablixHeader tablixHeader = (TablixHeader)base.PublishClone(context);
			if (m_size != null)
			{
				tablixHeader.m_size = (string)m_size.Clone();
			}
			if (m_cellContents != null)
			{
				if (isClonedDynamic)
				{
					ExpressionInfo meDotValueExpression = null;
					Rectangle rectangle = new Rectangle(context.GenerateID(), context.GenerateID(), context.CurrentDataRegion);
					rectangle.Name = context.CreateUniqueReportItemName(m_cellContents.Name, emptyRectangle: true, isClone: false);
					Style styleClass = m_cellContents.StyleClass;
					if (styleClass != null)
					{
						Style style = new Style(ConstructionPhase.Publishing);
						string[] stylesForEmptyRectangleInSubtotals = m_StylesForEmptyRectangleInSubtotals;
						foreach (string name in stylesForEmptyRectangleInSubtotals)
						{
							AddAttribute(context, styleClass, style, name, meDotValueExpression);
						}
						rectangle.StyleClass = style;
					}
					tablixHeader.m_cellContents = rectangle;
				}
				else
				{
					tablixHeader.m_cellContents = (ReportItem)m_cellContents.PublishClone(context);
					if (m_altCellContents != null)
					{
						Global.Tracer.Assert(tablixHeader.m_cellContents is CustomReportItem);
						tablixHeader.m_altCellContents = (ReportItem)m_altCellContents.PublishClone(context);
						((CustomReportItem)tablixHeader.m_cellContents).AltReportItem = tablixHeader.m_altCellContents;
					}
				}
			}
			return tablixHeader;
		}

		private void AddAttribute(AutomaticSubtotalContext context, Style originalStyle, Style newStyle, string name, ExpressionInfo meDotValueExpression)
		{
			if (originalStyle.GetAttributeInfo(name, out AttributeInfo styleAttribute))
			{
				if (styleAttribute.IsExpression)
				{
					newStyle.AddAttribute(name, (ExpressionInfo)originalStyle.ExpressionList[styleAttribute.IntValue].PublishClone(context));
				}
				else
				{
					newStyle.StyleAttributes.Add(name, styleAttribute.PublishClone(context));
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Size, Token.String));
			list.Add(new MemberInfo(MemberName.SizeValue, Token.Double));
			list.Add(new MemberInfo(MemberName.CellContents, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.AltCellContents, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Size:
					writer.Write(m_size);
					break;
				case MemberName.SizeValue:
					writer.Write(m_sizeValue);
					break;
				case MemberName.CellContents:
					writer.Write(m_cellContents);
					break;
				case MemberName.AltCellContents:
					writer.Write(m_altCellContents);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Size:
					m_size = reader.ReadString();
					break;
				case MemberName.SizeValue:
					m_sizeValue = reader.ReadDouble();
					break;
				case MemberName.CellContents:
					m_cellContents = (ReportItem)reader.ReadRIFObject();
					break;
				case MemberName.AltCellContents:
					m_altCellContents = (ReportItem)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixHeader;
		}
	}
}
