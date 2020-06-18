using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemInstance
	{
		private string m_hyperLinkURL;

		private string m_bookmarkLink;

		private string m_label;

		private string m_drillthroughReportName;

		private object[] m_drillthroughParametersValues;

		private BoolList m_drillthroughParametersOmits;

		[NonSerialized]
		private IntList m_dataSetTokenIDs;

		internal string HyperLinkURL
		{
			get
			{
				return m_hyperLinkURL;
			}
			set
			{
				m_hyperLinkURL = value;
			}
		}

		internal string BookmarkLink
		{
			get
			{
				return m_bookmarkLink;
			}
			set
			{
				m_bookmarkLink = value;
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

		internal string DrillthroughReportName
		{
			get
			{
				return m_drillthroughReportName;
			}
			set
			{
				m_drillthroughReportName = value;
			}
		}

		internal object[] DrillthroughParametersValues
		{
			get
			{
				return m_drillthroughParametersValues;
			}
			set
			{
				m_drillthroughParametersValues = value;
			}
		}

		internal BoolList DrillthroughParametersOmits
		{
			get
			{
				return m_drillthroughParametersOmits;
			}
			set
			{
				m_drillthroughParametersOmits = value;
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

		internal ActionItemInstance(ReportProcessing.ProcessingContext pc, ActionItem actionItemDef)
		{
			ParameterValueList drillthroughParameters = actionItemDef.DrillthroughParameters;
			if (drillthroughParameters == null)
			{
				return;
			}
			m_drillthroughParametersValues = new object[drillthroughParameters.Count];
			m_drillthroughParametersOmits = new BoolList(drillthroughParameters.Count);
			m_dataSetTokenIDs = new IntList(drillthroughParameters.Count);
			for (int i = 0; i < drillthroughParameters.Count; i++)
			{
				if (drillthroughParameters[i].Value != null && drillthroughParameters[i].Value.Type == ExpressionInfo.Types.Token)
				{
					m_dataSetTokenIDs.Add(drillthroughParameters[i].Value.IntValue);
				}
				else
				{
					m_dataSetTokenIDs.Add(-1);
				}
			}
		}

		internal ActionItemInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HyperLinkURL, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.BookmarkLink, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParametersOmits, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.BoolList));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
