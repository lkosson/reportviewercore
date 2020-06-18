using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportElementInstance : BaseInstance, IPersistable
	{
		[NonSerialized]
		protected ReportElement m_reportElementDef;

		protected StyleInstance m_style;

		private static readonly Declaration m_Declaration = GetDeclaration();

		public virtual StyleInstance Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new StyleInstance(m_reportElementDef, m_reportElementDef.ReportScope, m_reportElementDef.RenderingContext);
				}
				return m_style;
			}
		}

		internal ReportElement ReportElementDef => m_reportElementDef;

		internal ReportElementInstance(ReportElement reportElementDef)
			: base(reportElementDef.ReportScope)
		{
			m_reportElementDef = reportElementDef;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}

		protected override void ResetInstanceCache()
		{
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			Serialize(writer);
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			Deserialize(reader);
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return GetObjectType();
		}

		internal virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Style)
				{
					writer.Write(Style);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		internal virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Style)
				{
					reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		internal virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportElementInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Style, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportElementInstance, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}
	}
}
