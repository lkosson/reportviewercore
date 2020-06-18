using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PageBreakProperties : IStorable, IPersistable
	{
		private bool m_pageBreakAtStart;

		private bool m_pageBreakAtEnd;

		private bool m_resetPageNumber;

		private object m_source;

		private static Declaration m_declaration = GetDeclaration();

		public bool PageBreakAtStart
		{
			get
			{
				return m_pageBreakAtStart;
			}
			set
			{
				m_pageBreakAtStart = value;
			}
		}

		public bool PageBreakAtEnd
		{
			get
			{
				return m_pageBreakAtEnd;
			}
			set
			{
				m_pageBreakAtEnd = value;
			}
		}

		public bool ResetPageNumber
		{
			get
			{
				return m_resetPageNumber;
			}
			set
			{
				m_resetPageNumber = value;
			}
		}

		public object Source => m_source;

		public int Size => Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 3;

		internal PageBreakProperties()
		{
		}

		private PageBreakProperties(PageBreak pageBreak, object source)
		{
			PageBreakLocation breakLocation = pageBreak.BreakLocation;
			m_pageBreakAtStart = (breakLocation == PageBreakLocation.Start || breakLocation == PageBreakLocation.StartAndEnd);
			m_pageBreakAtEnd = (breakLocation == PageBreakLocation.End || breakLocation == PageBreakLocation.StartAndEnd);
			m_resetPageNumber = pageBreak.Instance.ResetPageNumber;
			m_source = source;
		}

		internal static PageBreakProperties Create(PageBreak pageBreak, object source, PageContext pageContext)
		{
			if (pageBreak.BreakLocation != 0)
			{
				if (!pageBreak.Instance.Disabled)
				{
					return new PageBreakProperties(pageBreak, source);
				}
				pageContext.Common.TracePageBreakIgnoredDisabled(source);
			}
			return null;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageBreakAtStart:
					writer.Write(m_pageBreakAtStart);
					break;
				case MemberName.PageBreakAtEnd:
					writer.Write(m_pageBreakAtEnd);
					break;
				case MemberName.ResetPageNumber:
					writer.Write(m_resetPageNumber);
					break;
				case MemberName.Source:
				{
					int value = scalabilityCache.StoreStaticReference(m_source);
					writer.Write(value);
					break;
				}
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageBreakAtStart:
					m_pageBreakAtStart = reader.ReadBoolean();
					break;
				case MemberName.PageBreakAtEnd:
					m_pageBreakAtEnd = reader.ReadBoolean();
					break;
				case MemberName.ResetPageNumber:
					m_resetPageNumber = reader.ReadBoolean();
					break;
				case MemberName.Source:
				{
					int id = reader.ReadInt32();
					m_source = scalabilityCache.FetchStaticReference(id);
					break;
				}
				default:
					RSTrace.RenderingTracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.PageBreakProperties;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
				list.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ResetPageNumber, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				return new Declaration(ObjectType.PageBreakProperties, ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
