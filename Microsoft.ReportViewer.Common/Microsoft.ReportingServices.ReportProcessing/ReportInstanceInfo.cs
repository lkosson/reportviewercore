using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportInstanceInfo : ReportItemInstanceInfo
	{
		private ParameterInfoCollection m_parameters;

		private string m_reportName;

		private bool m_noRows;

		private int m_bodyUniqueName;

		internal ParameterInfoCollection Parameters
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
			}
		}

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

		internal bool NoRows
		{
			get
			{
				return m_noRows;
			}
			set
			{
				m_noRows = value;
			}
		}

		internal int BodyUniqueName
		{
			get
			{
				return m_bodyUniqueName;
			}
			set
			{
				m_bodyUniqueName = value;
			}
		}

		internal ReportInstanceInfo(ReportProcessing.ProcessingContext pc, Report reportItemDef, ReportInstance owner, ParameterInfoCollection parameters, bool noRows)
			: base(pc, reportItemDef, owner, addToChunk: true)
		{
			m_bodyUniqueName = pc.CreateUniqueName();
			m_reportName = pc.ReportContext.ItemName;
			m_parameters = new ParameterInfoCollection();
			if (parameters != null && parameters.Count > 0)
			{
				parameters.CopyTo(m_parameters);
			}
			m_noRows = noRows;
		}

		internal ReportInstanceInfo(Report reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterInfoCollection));
			memberInfoList.Add(new MemberInfo(MemberName.ReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.BodyUniqueName, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
