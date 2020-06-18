using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixInstanceInfo : ReportItemInstanceInfo
	{
		private NonComputedUniqueNames m_cornerNonComputedNames;

		private string m_noRows;

		internal NonComputedUniqueNames CornerNonComputedNames
		{
			get
			{
				return m_cornerNonComputedNames;
			}
			set
			{
				m_cornerNonComputedNames = value;
			}
		}

		internal string NoRows
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

		internal MatrixInstanceInfo(ReportProcessing.ProcessingContext pc, Matrix reportItemDef, MatrixInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: false)
		{
			if (0 < reportItemDef.CornerReportItems.Count && !reportItemDef.CornerReportItems.IsReportItemComputed(0))
			{
				m_cornerNonComputedNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, reportItemDef.CornerReportItems[0]);
			}
			reportItemDef.CornerNonComputedUniqueNames = m_cornerNonComputedNames;
			if (!pc.DelayAddingInstanceInfo)
			{
				if (reportItemDef.FirstInstance)
				{
					pc.ChunkManager.AddInstanceToFirstPage(this, owner, pc.InPageSection);
					reportItemDef.FirstInstance = false;
				}
				else
				{
					pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
				}
			}
			m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal MatrixInstanceInfo(Matrix reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CornerNonComputedNames, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
