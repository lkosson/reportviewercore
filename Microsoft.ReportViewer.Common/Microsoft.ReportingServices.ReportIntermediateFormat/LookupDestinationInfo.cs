using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class LookupDestinationInfo : IPersistable
	{
		private bool m_isMultiValue;

		private ExpressionInfo m_destinationExpr;

		private int m_indexInCollection;

		private bool m_usedInSameDataSetTablixProcessing;

		private int m_exprHostID;

		[NonSerialized]
		private LookupDestExprHost m_exprHost;

		[NonSerialized]
		private string m_scope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Scope
		{
			get
			{
				return m_scope;
			}
			set
			{
				m_scope = value;
			}
		}

		internal bool IsMultiValue
		{
			get
			{
				return m_isMultiValue;
			}
			set
			{
				m_isMultiValue = value;
			}
		}

		internal ExpressionInfo DestinationExpr
		{
			get
			{
				return m_destinationExpr;
			}
			set
			{
				m_destinationExpr = value;
			}
		}

		internal int IndexInCollection
		{
			get
			{
				return m_indexInCollection;
			}
			set
			{
				m_indexInCollection = value;
			}
		}

		internal bool UsedInSameDataSetTablixProcessing
		{
			get
			{
				return m_usedInSameDataSetTablixProcessing;
			}
			set
			{
				m_usedInSameDataSetTablixProcessing = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal LookupDestExprHost ExprHost => m_exprHost;

		internal LookupDestinationInfo()
		{
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			LookupDestinationInfo lookupDestinationInfo = (LookupDestinationInfo)MemberwiseClone();
			if (m_destinationExpr != null)
			{
				lookupDestinationInfo.m_destinationExpr = (ExpressionInfo)m_destinationExpr.PublishClone(context);
			}
			return lookupDestinationInfo;
		}

		internal void Initialize(InitializationContext context, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			context.ExprHostBuilder.LookupDestStart();
			if (m_destinationExpr != null)
			{
				m_destinationExpr.LookupInitialize(dataSetName, objectType, objectName, propertyName, context);
				context.ExprHostBuilder.LookupDestExpr(m_destinationExpr);
			}
			m_exprHostID = context.ExprHostBuilder.LookupDestEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.LookupDestExprHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateDestExpr(OnDemandProcessingContext odpContext, IErrorContext errorContext)
		{
			return odpContext.ReportRuntime.EvaluateLookupDestExpression(this, errorContext);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsMultiValue:
					writer.Write(m_isMultiValue);
					break;
				case MemberName.DestinationExpr:
					writer.Write(m_destinationExpr);
					break;
				case MemberName.IndexInCollection:
					writer.Write(m_indexInCollection);
					break;
				case MemberName.UsedInSameDataSetTablixProcessing:
					writer.Write(m_usedInSameDataSetTablixProcessing);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsMultiValue:
					m_isMultiValue = reader.ReadBoolean();
					break;
				case MemberName.DestinationExpr:
					m_destinationExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IndexInCollection:
					m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.UsedInSameDataSetTablixProcessing:
					m_usedInSameDataSetTablixProcessing = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.IsMultiValue, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DestinationExpr, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInSameDataSetTablixProcessing, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupDestinationInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}
	}
}
