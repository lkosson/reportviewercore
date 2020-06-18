using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ReportBody : PageItemContainer
	{
		[StaticReference]
		private new Body m_source;

		private double m_originalWidth;

		private static Declaration m_declaration = GetDeclaration();

		internal override string SourceID => m_source.ID;

		internal override string SourceUniqueName => m_source.Instance.UniqueName;

		internal override double OriginalLeft => 0.0;

		internal override double OriginalWidth => m_originalWidth;

		internal override Style SharedStyle => m_source.Style;

		internal override StyleInstance NonSharedStyle => m_source.Instance.Style;

		internal override byte RPLFormatType => 6;

		public override int Size => base.Size + Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 8;

		internal ReportBody()
		{
		}

		internal ReportBody(Body source, ReportSize width)
			: base(null)
		{
			m_itemPageSizes = new ItemSizes(0.0, 0.0, width.ToMillimeters(), source.Height.ToMillimeters());
			m_originalWidth = m_itemPageSizes.Width;
			m_source = source;
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			base.UnresolvedKTV = (base.UnresolvedKTH = false);
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLBody();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			return new RPLBody(props as RPLItemProps);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BodySource:
				{
					int value = scalabilityCache.StoreStaticReference(m_source);
					writer.Write(value);
					break;
				}
				case MemberName.Width:
					writer.Write(m_originalWidth);
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.BodySource:
				{
					int id = reader.ReadInt32();
					m_source = (Body)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.Width:
					m_originalWidth = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ReportBody;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.BodySource, Token.Int32));
				list.Add(new MemberInfo(MemberName.Width, Token.Double));
				return new Declaration(ObjectType.ReportBody, ObjectType.PageItemContainer, list);
			}
			return m_declaration;
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			CreateChildren(m_source.ReportItemCollection, pageContext);
		}
	}
}
