using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ParameterDataSource : IParameterDataSource
	{
		private int m_dataSourceIndex = -1;

		private int m_dataSetIndex = -1;

		private int m_valueFieldIndex = -1;

		private int m_labelFieldIndex = -1;

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
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSourceIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataSetIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ValueFieldIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.LabelFieldIndex, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
