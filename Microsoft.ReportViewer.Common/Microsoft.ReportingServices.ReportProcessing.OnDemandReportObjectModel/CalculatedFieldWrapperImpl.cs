using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class CalculatedFieldWrapperImpl : CalculatedFieldWrapper, IStorable, IPersistable
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		private object m_value;

		private bool m_isValueReady;

		private bool m_isVisited;

		private Microsoft.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private bool m_errorOccurred;

		private string m_exceptionMessage;

		[NonSerialized]
		private IErrorContext m_iErrorContext;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override object Value
		{
			get
			{
				if (!m_isValueReady)
				{
					CalculateValue();
				}
				return m_value;
			}
		}

		internal bool ErrorOccurred
		{
			get
			{
				if (!m_isValueReady)
				{
					CalculateValue();
				}
				return m_errorOccurred;
			}
		}

		internal string ExceptionMessage => m_exceptionMessage;

		public int Size => ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_value) + 1 + 1 + ItemSizes.ReferenceSize + ItemSizes.ReferenceSize + 1 + ItemSizes.SizeOf(m_exceptionMessage);

		internal CalculatedFieldWrapperImpl()
		{
		}

		internal CalculatedFieldWrapperImpl(Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef, Microsoft.ReportingServices.RdlExpressions.ReportRuntime reportRT)
		{
			m_fieldDef = fieldDef;
			m_reportRT = reportRT;
			m_iErrorContext = reportRT;
		}

		internal void ResetValue()
		{
			m_isValueReady = false;
			m_isVisited = false;
			m_value = null;
		}

		private void CalculateValue()
		{
			if (m_isVisited)
			{
				m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, ObjectType.Field, m_fieldDef.Name, "Value");
				throw new ReportProcessingException_InvalidOperationException();
			}
			m_isVisited = true;
			Microsoft.ReportingServices.RdlExpressions.VariantResult variantResult = m_reportRT.EvaluateFieldValueExpression(m_fieldDef);
			m_value = variantResult.Value;
			m_errorOccurred = variantResult.ErrorOccurred;
			if (m_errorOccurred)
			{
				m_exceptionMessage = variantResult.ExceptionMessage;
			}
			m_isVisited = false;
			m_isValueReady = true;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_fieldDef);
					writer.Write(value2);
					break;
				}
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.IsValueReady:
					writer.Write(m_isValueReady);
					break;
				case MemberName.IsVisited:
					writer.Write(m_isVisited);
					break;
				case MemberName.ReportRuntime:
				{
					int value = scalabilityCache.StoreStaticReference(m_reportRT);
					writer.Write(value);
					break;
				}
				case MemberName.ErrorOccurred:
					writer.Write(m_errorOccurred);
					break;
				case MemberName.ExceptionMessage:
					writer.Write(m_exceptionMessage);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldDef:
				{
					int id2 = reader.ReadInt32();
					m_fieldDef = (Microsoft.ReportingServices.ReportIntermediateFormat.Field)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Value:
					m_value = reader.ReadVariant();
					break;
				case MemberName.IsValueReady:
					m_isValueReady = reader.ReadBoolean();
					break;
				case MemberName.IsVisited:
					m_isVisited = reader.ReadBoolean();
					break;
				case MemberName.ReportRuntime:
				{
					int id = reader.ReadInt32();
					m_reportRT = (Microsoft.ReportingServices.RdlExpressions.ReportRuntime)scalabilityCache.FetchStaticReference(id);
					m_iErrorContext = m_reportRT;
					break;
				}
				case MemberName.ErrorOccurred:
					m_errorOccurred = reader.ReadBoolean();
					break;
				case MemberName.ExceptionMessage:
					m_exceptionMessage = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CalculatedFieldWrapperImpl;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FieldDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.IsValueReady, Token.Boolean));
				list.Add(new MemberInfo(MemberName.IsVisited, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ReportRuntime, Token.Int32));
				list.Add(new MemberInfo(MemberName.ErrorOccurred, Token.Boolean));
				list.Add(new MemberInfo(MemberName.ExceptionMessage, Token.String));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CalculatedFieldWrapperImpl, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
