using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CheckBox : ReportItem
	{
		private ExpressionInfo m_value;

		private string m_hideDuplicates;

		[NonSerialized]
		private bool m_oldValue;

		[NonSerialized]
		private bool m_hasOldValue;

		internal override ObjectType ObjectType => ObjectType.Checkbox;

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal string HideDuplicates
		{
			get
			{
				return m_hideDuplicates;
			}
			set
			{
				m_hideDuplicates = value;
			}
		}

		internal bool OldValue
		{
			get
			{
				return m_oldValue;
			}
			set
			{
				m_oldValue = value;
				m_hasOldValue = true;
			}
		}

		internal bool HasOldValue
		{
			get
			{
				return m_hasOldValue;
			}
			set
			{
				m_hasOldValue = value;
			}
		}

		internal CheckBox(ReportItem parent)
			: base(parent)
		{
		}

		internal CheckBox(int id, ReportItem parent)
			: base(id, parent)
		{
			m_height = "3.175mm";
			m_width = "3.175mm";
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
			}
			if (m_hideDuplicates != null)
			{
				context.ValidateHideDuplicateScope(m_hideDuplicates, this);
			}
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(condition: false, string.Empty);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.HideDuplicates, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
