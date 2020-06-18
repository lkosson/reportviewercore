using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TextBoxInstance : ReportItemInstance
	{
		internal InstanceInfo InstanceInfo
		{
			get
			{
				if (m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(condition: false, string.Empty);
					return null;
				}
				if (m_instanceInfo is SimpleTextBoxInstanceInfo)
				{
					return (SimpleTextBoxInstanceInfo)m_instanceInfo;
				}
				return (TextBoxInstanceInfo)m_instanceInfo;
			}
		}

		internal TextBoxInstance(ReportProcessing.ProcessingContext pc, TextBox reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			if (reportItemDef.IsSimpleTextBox())
			{
				m_instanceInfo = new SimpleTextBoxInstanceInfo(pc, reportItemDef, this, index);
			}
			else
			{
				m_instanceInfo = new TextBoxInstanceInfo(pc, reportItemDef, this, index);
			}
		}

		internal TextBoxInstance()
		{
		}

		internal SimpleTextBoxInstanceInfo UpgradeToSimpleTextbox(TextBoxInstanceInfo instanceInfo, out bool isSimple)
		{
			isSimple = false;
			TextBox textBox = base.ReportItemDef as TextBox;
			if (textBox.IsSimpleTextBox())
			{
				isSimple = true;
				return (SimpleTextBoxInstanceInfo)(m_instanceInfo = new SimpleTextBoxInstanceInfo(textBox, instanceInfo));
			}
			return null;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, members);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(m_instanceInfo is OffsetInfo);
			if (((TextBox)m_reportItemDef).IsSimpleTextBox(reader.IntermediateFormatVersion))
			{
				return null;
			}
			return reader.ReadTextBoxInstanceInfo((TextBox)m_reportItemDef);
		}

		internal SimpleTextBoxInstanceInfo GetSimpleInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, bool inPageSection)
		{
			Global.Tracer.Assert(((TextBox)m_reportItemDef).IsSimpleTextBox());
			if (m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(chunkManager != null);
				IntermediateFormatReader intermediateFormatReader = null;
				intermediateFormatReader = ((!inPageSection) ? chunkManager.GetReader(((OffsetInfo)m_instanceInfo).Offset) : chunkManager.GetPageSectionInstanceReader(((OffsetInfo)m_instanceInfo).Offset));
				return intermediateFormatReader.ReadSimpleTextBoxInstanceInfo((TextBox)m_reportItemDef);
			}
			return (SimpleTextBoxInstanceInfo)m_instanceInfo;
		}
	}
}
