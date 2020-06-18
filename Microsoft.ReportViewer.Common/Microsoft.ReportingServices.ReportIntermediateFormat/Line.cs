using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Line : ReportItem, IPersistable
	{
		private bool m_slanted;

		private const string ZeroSize = "0mm";

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.Line;

		internal bool LineSlant
		{
			get
			{
				return m_slanted;
			}
			set
			{
				m_slanted = value;
			}
		}

		internal Line(ReportItem parent)
			: base(parent)
		{
		}

		internal Line(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.LineStart(m_name);
			base.Initialize(context);
			double heightValue = m_heightValue;
			double widthValue = m_widthValue;
			double topValue = m_topValue;
			double leftValue = m_leftValue;
			if ((0.0 > ReportItem.RoundSize(heightValue) && 0.0 <= ReportItem.RoundSize(widthValue)) || (0.0 > ReportItem.RoundSize(widthValue) && 0.0 <= ReportItem.RoundSize(heightValue)))
			{
				m_slanted = true;
			}
			m_heightValue = Math.Abs(heightValue);
			m_widthValue = Math.Abs(widthValue);
			if (0.0 <= heightValue)
			{
				m_topValue = topValue;
			}
			else
			{
				m_topValue = topValue + heightValue;
				if (0.0 > m_topValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeTopHeight, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (0.0 <= widthValue)
			{
				m_leftValue = leftValue;
			}
			else
			{
				m_leftValue = leftValue + widthValue;
				if (0.0 > m_leftValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeLeftWidth, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (m_visibility != null)
			{
				m_visibility.Initialize(context);
			}
			base.ExprHostID = context.ExprHostBuilder.LineEnd();
			return true;
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			if (overwrite)
			{
				m_top = "0mm";
				m_topValue = 0.0;
				m_left = "0mm";
				m_leftValue = 0.0;
			}
			if (m_width == null || (overwrite && m_widthValue > 0.0 && m_widthValue != width))
			{
				m_width = width.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_widthValue = context.ValidateSize(ref m_width, "Width");
			}
			if (m_height == null || (overwrite && m_heightValue > 0.0 && m_heightValue != height))
			{
				m_height = height.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_heightValue = context.ValidateSize(ref m_height, "Height");
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return base.PublishClone(context);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Slanted, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Line, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Slanted)
				{
					writer.Write(m_slanted);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Slanted)
				{
					m_slanted = reader.ReadBoolean();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Line;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.LineHostsRemotable[base.ExprHostID];
				ReportItemSetExprHost(m_exprHost, reportObjectModel);
			}
		}
	}
}
