using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughInformation
	{
		private string m_reportName;

		private DrillthroughParameters m_reportParameters;

		private IntList m_dataSetTokenIDs;

		internal string ReportName
		{
			get
			{
				return m_reportName;
			}
			set
			{
				m_reportName = value;
			}
		}

		internal DrillthroughParameters ReportParameters
		{
			get
			{
				return m_reportParameters;
			}
			set
			{
				m_reportParameters = value;
			}
		}

		internal IntList DataSetTokenIDs
		{
			get
			{
				return m_dataSetTokenIDs;
			}
			set
			{
				m_dataSetTokenIDs = value;
			}
		}

		internal DrillthroughInformation()
		{
		}

		internal DrillthroughInformation(string reportName, DrillthroughParameters reportParameters, IntList dataSetTokenIDs)
		{
			m_reportName = reportName;
			m_reportParameters = reportParameters;
			m_dataSetTokenIDs = dataSetTokenIDs;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DrillthroughParameters));
			memberInfoList.Add(new MemberInfo(MemberName.DataSets, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ResolveDataSetTokenIDs(TokensHashtable dataSetTokenIDs)
		{
			if (dataSetTokenIDs != null && m_dataSetTokenIDs != null)
			{
				DrillthroughParameters drillthroughParameters = new DrillthroughParameters();
				object obj = null;
				for (int i = 0; i < m_dataSetTokenIDs.Count; i++)
				{
					drillthroughParameters.Add(value: (m_dataSetTokenIDs[i] < 0) ? m_reportParameters.GetValue(i) : dataSetTokenIDs[m_dataSetTokenIDs[i]], key: m_reportParameters.GetKey(i));
				}
				m_reportParameters = drillthroughParameters;
			}
		}
	}
}
