using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ReportItemInstanceInfo : InstanceInfo, IShowHideReceiver
	{
		protected object[] m_styleAttributeValues;

		protected bool m_startHidden;

		protected string m_label;

		protected string m_bookmark;

		protected string m_toolTip;

		protected DataValueInstanceList m_customPropertyInstances;

		[NonSerialized]
		protected ReportItem m_reportItemDef;

		internal object[] StyleAttributeValues
		{
			get
			{
				return m_styleAttributeValues;
			}
			set
			{
				m_styleAttributeValues = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal ReportItem ReportItemDef
		{
			get
			{
				return m_reportItemDef;
			}
			set
			{
				m_reportItemDef = value;
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

		internal string Bookmark
		{
			get
			{
				return m_bookmark;
			}
			set
			{
				m_bookmark = value;
			}
		}

		internal string ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return m_customPropertyInstances;
			}
			set
			{
				m_customPropertyInstances = value;
			}
		}

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, int index)
		{
			ConstructorHelper(pc, reportItemDef, owner);
			if (pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, reportItemDef, owner, index, pc.InPageSection);
			}
			reportItemDef.StartHidden = m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, int index, bool customCreated)
		{
			if (!customCreated)
			{
				ConstructorHelper(pc, reportItemDef, owner);
			}
			else
			{
				m_reportItemDef = reportItemDef;
			}
			if (pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, reportItemDef, owner, index, pc.InPageSection);
			}
			reportItemDef.StartHidden = m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner, bool addToChunk)
		{
			ConstructorHelper(pc, reportItemDef, owner);
			if (addToChunk && pc.ChunkManager != null && !pc.DelayAddingInstanceInfo)
			{
				pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
			}
			reportItemDef.StartHidden = m_startHidden;
		}

		protected ReportItemInstanceInfo(ReportItem reportItemDef)
		{
			m_reportItemDef = reportItemDef;
		}

		private void ConstructorHelper(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef, ReportItemInstance owner)
		{
			m_reportItemDef = reportItemDef;
			Style styleClass = reportItemDef.StyleClass;
			if (styleClass != null && styleClass.ExpressionList != null && 0 < styleClass.ExpressionList.Count)
			{
				m_styleAttributeValues = new object[styleClass.ExpressionList.Count];
			}
			ReportProcessing.RuntimeRICollection.EvalReportItemAttr(reportItemDef, owner, this, pc);
			if (reportItemDef.CustomProperties != null)
			{
				m_customPropertyInstances = reportItemDef.CustomProperties.EvaluateExpressions(reportItemDef.ObjectType, reportItemDef.Name, null, pc);
			}
		}

		internal object GetStyleAttributeValue(int index)
		{
			Global.Tracer.Assert(m_styleAttributeValues != null && 0 <= index && index < m_styleAttributeValues.Length);
			return m_styleAttributeValues[index];
		}

		void IShowHideReceiver.ProcessReceiver(ReportProcessing.ProcessingContext context, int uniqueName)
		{
			m_startHidden = context.ProcessReceiver(uniqueName, m_reportItemDef.Visibility, m_reportItemDef.ExprHost, m_reportItemDef.ObjectType, m_reportItemDef.Name);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Bookmark, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ToolTip, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
