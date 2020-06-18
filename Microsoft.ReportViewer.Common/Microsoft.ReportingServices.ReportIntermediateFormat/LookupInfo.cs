using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal class LookupInfo : IPersistable
	{
		private ExpressionInfo m_resultExpr;

		private string m_dataSetName;

		private ExpressionInfo m_sourceExpr;

		private int m_destinationIndexInCollection;

		private string m_name;

		private int m_exprHostID;

		private int m_dataSetIndexInCollection;

		private LookupType m_lookupType;

		[NonSerialized]
		private LookupExprHost m_exprHost;

		[NonSerialized]
		private LookupDestinationInfo m_destinationInfo;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo ResultExpr
		{
			get
			{
				return m_resultExpr;
			}
			set
			{
				m_resultExpr = value;
			}
		}

		internal ExpressionInfo SourceExpr
		{
			get
			{
				return m_sourceExpr;
			}
			set
			{
				m_sourceExpr = value;
			}
		}

		internal int DestinationIndexInCollection
		{
			get
			{
				return m_destinationIndexInCollection;
			}
			set
			{
				m_destinationIndexInCollection = value;
			}
		}

		internal int DataSetIndexInCollection
		{
			get
			{
				return m_dataSetIndexInCollection;
			}
			set
			{
				m_dataSetIndexInCollection = value;
			}
		}

		internal LookupType LookupType
		{
			get
			{
				return m_lookupType;
			}
			set
			{
				m_lookupType = value;
			}
		}

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
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

		internal LookupExprHost ExprHost => m_exprHost;

		internal LookupDestinationInfo DestinationInfo
		{
			get
			{
				return m_destinationInfo;
			}
			set
			{
				m_destinationInfo = value;
			}
		}

		internal LookupInfo()
		{
		}

		internal bool ReturnFirstMatchOnly()
		{
			return m_lookupType != LookupType.LookupSet;
		}

		internal string GetAsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Enum.GetName(typeof(LookupType), m_lookupType));
			stringBuilder.Append("(");
			bool appendSeperator = false;
			AppendWithSeperator(stringBuilder, m_sourceExpr, ref appendSeperator);
			AppendWithSeperator(stringBuilder, m_destinationInfo.DestinationExpr, ref appendSeperator);
			AppendWithSeperator(stringBuilder, m_resultExpr, ref appendSeperator);
			if (!string.IsNullOrEmpty(m_destinationInfo.Scope))
			{
				if (appendSeperator)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("\"");
				stringBuilder.Append(m_destinationInfo.Scope);
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			LookupInfo lookupInfo = (LookupInfo)MemberwiseClone();
			lookupInfo.m_name = context.CreateLookupID(m_name);
			if (m_resultExpr != null)
			{
				lookupInfo.m_resultExpr = (ExpressionInfo)m_resultExpr.PublishClone(context);
			}
			if (m_sourceExpr != null)
			{
				lookupInfo.m_sourceExpr = (ExpressionInfo)m_sourceExpr.PublishClone(context);
			}
			lookupInfo.m_destinationInfo = (LookupDestinationInfo)m_destinationInfo.PublishClone(context);
			return lookupInfo;
		}

		private void AppendWithSeperator(StringBuilder sb, ExpressionInfo expr, ref bool appendSeperator)
		{
			if (expr != null)
			{
				if (appendSeperator)
				{
					sb.Append(", ");
				}
				sb.Append(expr.OriginalText);
				appendSeperator = true;
			}
		}

		internal void Initialize(InitializationContext context, string dataSetName, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			context.ExprHostBuilder.LookupStart();
			if (m_resultExpr != null)
			{
				m_resultExpr.LookupInitialize(dataSetName, objectType, objectName, propertyName, context);
				context.ExprHostBuilder.LookupResultExpr(m_resultExpr);
			}
			if (m_sourceExpr != null)
			{
				m_sourceExpr.Initialize(propertyName, context);
				context.ExprHostBuilder.LookupSourceExpr(m_sourceExpr);
			}
			ExprHostID = context.ExprHostBuilder.LookupEnd();
		}

		internal void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				m_exprHost = reportExprHost.LookupExprHostsRemotable[ExprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateSourceExpr(Microsoft.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			return runtime.EvaluateLookupSourceExpression(this);
		}

		internal Microsoft.ReportingServices.RdlExpressions.VariantResult EvaluateResultExpr(Microsoft.ReportingServices.RdlExpressions.ReportRuntime runtime)
		{
			return runtime.EvaluateLookupResultExpression(this);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ResultExpr:
					writer.Write(m_resultExpr);
					break;
				case MemberName.DataSetName:
					writer.Write(m_dataSetName);
					break;
				case MemberName.SourceExpr:
					writer.Write(m_sourceExpr);
					break;
				case MemberName.DestinationIndexInCollection:
					writer.Write(m_destinationIndexInCollection);
					break;
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.DataSetIndexInCollection:
					writer.Write(m_dataSetIndexInCollection);
					break;
				case MemberName.LookupType:
					writer.WriteEnum((int)m_lookupType);
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
				case MemberName.ResultExpr:
					m_resultExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataSetName:
					m_dataSetName = reader.ReadString();
					break;
				case MemberName.SourceExpr:
					m_sourceExpr = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IsMultiValue:
					if (reader.ReadBoolean())
					{
						m_lookupType = LookupType.LookupSet;
					}
					else
					{
						m_lookupType = LookupType.Lookup;
					}
					break;
				case MemberName.DestinationIndexInCollection:
					m_destinationIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataSetIndexInCollection:
					m_dataSetIndexInCollection = reader.ReadInt32();
					break;
				case MemberName.LookupType:
					m_lookupType = (LookupType)reader.ReadEnum();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ResultExpr, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
				list.Add(new MemberInfo(MemberName.SourceExpr, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
				list.Add(new ReadOnlyMemberInfo(MemberName.IsMultiValue, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DestinationIndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.Name, Token.String));
				list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
				list.Add(new MemberInfo(MemberName.DataSetIndexInCollection, Token.Int32));
				list.Add(new MemberInfo(MemberName.LookupType, Token.Enum));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupInfo, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_Declaration;
		}
	}
}
