using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportDrillthroughInfo
	{
		private TokensHashtable m_rewrittenCommands;

		private DrillthroughHashtable m_drillthroughHashtable;

		internal DrillthroughHashtable DrillthroughInformation
		{
			get
			{
				return m_drillthroughHashtable;
			}
			set
			{
				m_drillthroughHashtable = value;
			}
		}

		internal TokensHashtable RewrittenCommands
		{
			get
			{
				return m_rewrittenCommands;
			}
			set
			{
				m_rewrittenCommands = value;
			}
		}

		internal int Count
		{
			get
			{
				if (m_drillthroughHashtable == null)
				{
					return 0;
				}
				return m_drillthroughHashtable.Count;
			}
		}

		internal ReportDrillthroughInfo()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RewrittenCommands, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.TokensHashtable));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughHashtable, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DrillthroughHashtable));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void AddDrillthrough(string drillthroughId, DrillthroughInformation drillthroughInfo)
		{
			if (m_drillthroughHashtable == null)
			{
				m_drillthroughHashtable = new DrillthroughHashtable();
			}
			m_drillthroughHashtable.Add(drillthroughId, drillthroughInfo);
		}

		internal void AddRewrittenCommand(int id, object value)
		{
			lock (this)
			{
				if (m_rewrittenCommands == null)
				{
					m_rewrittenCommands = new TokensHashtable();
				}
				if (!m_rewrittenCommands.ContainsKey(id))
				{
					m_rewrittenCommands.Add(id, value);
				}
			}
		}
	}
}
