using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SimpleTextBoxInstanceInfo : InstanceInfo
	{
		private string m_formattedValue;

		private object m_originalValue;

		[NonSerialized]
		private ReportItem m_reportItemDef;

		internal string FormattedValue
		{
			get
			{
				return m_formattedValue;
			}
			set
			{
				m_formattedValue = value;
			}
		}

		internal object OriginalValue
		{
			get
			{
				return m_originalValue;
			}
			set
			{
				m_originalValue = value;
			}
		}

		internal SimpleTextBoxInstanceInfo(ReportProcessing.ProcessingContext pc, TextBox reportItemDef, TextBoxInstance owner, int index)
		{
			m_reportItemDef = reportItemDef;
			ReportProcessing.RuntimeRICollection.ResetSubtotalReferences(pc);
			if (pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, reportItemDef, owner, index, pc.InPageSection);
			}
		}

		internal SimpleTextBoxInstanceInfo(TextBox reportItemDef)
		{
			m_reportItemDef = reportItemDef;
		}

		internal SimpleTextBoxInstanceInfo(TextBox reportItemDef, TextBoxInstanceInfo instanceInfo)
		{
			m_reportItemDef = reportItemDef;
			m_originalValue = instanceInfo.OriginalValue;
			m_formattedValue = instanceInfo.FormattedValue;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.FormattedValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.OriginalValue, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
