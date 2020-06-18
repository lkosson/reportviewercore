using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[SkipStaticValidation]
	internal class ParameterImplWrapper : IPersistable
	{
		private ParameterImpl m_odpParameter;

		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ParameterImpl WrappedParameterImpl
		{
			get
			{
				return m_odpParameter;
			}
			set
			{
				m_odpParameter = value;
			}
		}

		internal ParameterImplWrapper()
		{
			m_odpParameter = new ParameterImpl();
		}

		internal ParameterImplWrapper(ParameterImpl odpParameter)
		{
			m_odpParameter = odpParameter;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.Object));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveArray, Token.String));
			list.Add(new MemberInfo(MemberName.IsMultiValue, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Prompt, Token.String));
			list.Add(new MemberInfo(MemberName.IsUserSupplied, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(m_odpParameter.GetValues());
					break;
				case MemberName.Label:
					writer.Write(m_odpParameter.GetLabels());
					break;
				case MemberName.IsMultiValue:
					writer.Write(m_odpParameter.IsMultiValue);
					break;
				case MemberName.Prompt:
					writer.Write(m_odpParameter.Prompt);
					break;
				case MemberName.IsUserSupplied:
					writer.Write(m_odpParameter.IsUserSupplied);
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
				case MemberName.Value:
					m_odpParameter.SetValues(reader.ReadVariantArray());
					break;
				case MemberName.Label:
					m_odpParameter.SetLabels(reader.ReadStringArray());
					break;
				case MemberName.IsMultiValue:
					m_odpParameter.SetIsMultiValue(reader.ReadBoolean());
					break;
				case MemberName.Prompt:
					m_odpParameter.SetPrompt(reader.ReadString());
					break;
				case MemberName.IsUserSupplied:
					m_odpParameter.SetIsUserSupplied(reader.ReadBoolean());
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Parameter;
		}
	}
}
