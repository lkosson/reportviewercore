using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class LabelData : IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_dataSetName;

		private List<string> m_keyFields;

		private string m_label;

		internal string DataSetName
		{
			get
			{
				return m_dataSetName;
			}
			set
			{
				m_dataSetName = value;
			}
		}

		internal List<string> KeyFields
		{
			get
			{
				return m_keyFields;
			}
			set
			{
				m_keyFields = value;
			}
		}

		internal string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}

		internal void Initialize(Tablix tablix, InitializationContext context)
		{
			context.ValidateSliderLabelData(tablix, this);
		}

		[SkipMemberStaticValidation(MemberName.Key)]
		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.Key, Token.String, Lifetime.RemovedIn(200)));
			list.Add(new MemberInfo(MemberName.Label, Token.String));
			list.Add(new MemberInfo(MemberName.KeyFields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String, Lifetime.AddedIn(200)));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(m_dataSetName);
					break;
				case MemberName.Key:
					writer.Write(m_keyFields[0]);
					break;
				case MemberName.Label:
					writer.Write(m_label);
					break;
				case MemberName.KeyFields:
					writer.WriteListOfPrimitives(m_keyFields);
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
				case MemberName.DataSetName:
					m_dataSetName = reader.ReadString();
					break;
				case MemberName.Key:
				{
					string item = reader.ReadString();
					m_keyFields = new List<string>(1);
					m_keyFields.Add(item);
					break;
				}
				case MemberName.Label:
					m_label = reader.ReadString();
					break;
				case MemberName.KeyFields:
					m_keyFields = reader.ReadListOfPrimitives<string>();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LabelData;
		}
	}
}
