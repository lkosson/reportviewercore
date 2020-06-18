using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ParameterDataSource : IPersistable, IParameterDataSource
	{
		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_valueFieldIndex = -1;

		private int m_labelFieldIndex = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public int DataSourceIndex
		{
			get
			{
				return m_dataSourceIndex;
			}
			set
			{
				m_dataSourceIndex = value;
			}
		}

		public int DataSetIndex
		{
			get
			{
				return m_dataSetIndex;
			}
			set
			{
				m_dataSetIndex = value;
			}
		}

		public int ValueFieldIndex
		{
			get
			{
				return m_valueFieldIndex;
			}
			set
			{
				m_valueFieldIndex = value;
			}
		}

		public int LabelFieldIndex
		{
			get
			{
				return m_labelFieldIndex;
			}
			set
			{
				m_labelFieldIndex = value;
			}
		}

		internal ParameterDataSource()
		{
		}

		internal ParameterDataSource(int dataSourceIndex, int dataSetIndex)
		{
			m_dataSourceIndex = dataSourceIndex;
			m_dataSetIndex = dataSetIndex;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSourceIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataSetIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.ValueFieldIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.LabelFieldIndex, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSourceIndex:
					writer.Write(m_dataSourceIndex);
					break;
				case MemberName.DataSetIndex:
					writer.Write(m_dataSetIndex);
					break;
				case MemberName.ValueFieldIndex:
					writer.Write(m_valueFieldIndex);
					break;
				case MemberName.LabelFieldIndex:
					writer.Write(m_labelFieldIndex);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSourceIndex:
					m_dataSourceIndex = reader.ReadInt32();
					break;
				case MemberName.DataSetIndex:
					m_dataSetIndex = reader.ReadInt32();
					break;
				case MemberName.ValueFieldIndex:
					m_valueFieldIndex = reader.ReadInt32();
					break;
				case MemberName.LabelFieldIndex:
					m_labelFieldIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterDataSource;
		}
	}
}
