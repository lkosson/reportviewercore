using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Visibility
	{
		private ExpressionInfo m_hidden;

		private string m_toggle;

		private bool m_recursiveReceiver;

		[NonSerialized]
		private ToggleItemInfo m_toggleItemInfo;

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal string Toggle
		{
			get
			{
				return m_toggle;
			}
			set
			{
				m_toggle = value;
			}
		}

		internal bool RecursiveReceiver
		{
			get
			{
				return m_recursiveReceiver;
			}
			set
			{
				m_recursiveReceiver = value;
			}
		}

		internal void Initialize(InitializationContext context, bool isContainer, bool tableRowCol)
		{
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				if (tableRowCol)
				{
					context.ExprHostBuilder.TableRowColVisibilityHiddenExpressionsExpr(m_hidden);
				}
				else
				{
					context.ExprHostBuilder.GenericVisibilityHidden(m_hidden);
				}
			}
			m_toggleItemInfo = RegisterReceiver(context, isContainer);
		}

		internal ToggleItemInfo RegisterReceiver(InitializationContext context, bool isContainer)
		{
			if (context.RegisterHiddenReceiver)
			{
				return context.RegisterReceiver(m_toggle, this, isContainer);
			}
			return null;
		}

		internal void UnRegisterReceiver(InitializationContext context)
		{
			if (m_toggleItemInfo != null)
			{
				context.UnRegisterReceiver(m_toggle, m_toggleItemInfo);
			}
		}

		internal static SharedHiddenState GetSharedHidden(Visibility visibility)
		{
			if (visibility == null)
			{
				return SharedHiddenState.Never;
			}
			if (visibility.Toggle == null)
			{
				if (visibility.Hidden == null)
				{
					return SharedHiddenState.Never;
				}
				if (ExpressionInfo.Types.Constant == visibility.Hidden.Type)
				{
					if (visibility.Hidden.BoolValue)
					{
						return SharedHiddenState.Always;
					}
					return SharedHiddenState.Never;
				}
			}
			return SharedHiddenState.Sometimes;
		}

		internal static bool HasToggle(Visibility visibility)
		{
			if (visibility == null)
			{
				return false;
			}
			if (visibility.Toggle == null)
			{
				return false;
			}
			return true;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Hidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Toggle, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RecursiveReceiver, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static bool IsOnePassHierarchyVisible(ReportItem reportItem)
		{
			if (IsOnePassVisible(reportItem))
			{
				if (reportItem.Parent != null)
				{
					return IsOnePassHierarchyVisible(reportItem.Parent);
				}
				return true;
			}
			return false;
		}

		private static bool IsOnePassVisible(ReportItem reportItem)
		{
			if (reportItem == null)
			{
				return false;
			}
			if (reportItem.Visibility == null)
			{
				return true;
			}
			if (reportItem.Visibility.Toggle != null)
			{
				return false;
			}
			if (reportItem.Visibility.Hidden != null)
			{
				if (ExpressionInfo.Types.Constant == reportItem.Visibility.Hidden.Type)
				{
					return !reportItem.Visibility.Hidden.BoolValue;
				}
				return !reportItem.StartHidden;
			}
			return true;
		}

		internal static bool IsVisible(ReportItem reportItem)
		{
			return IsVisible(reportItem, null, null);
		}

		internal static bool IsVisible(ReportItem reportItem, ReportItemInstance reportItemInstance, ReportItemInstanceInfo reportItemInstanceInfo)
		{
			if (reportItem == null)
			{
				return false;
			}
			bool startHidden = reportItemInstance != null && reportItemInstanceInfo != null && reportItemInstanceInfo.StartHidden;
			return IsVisible(reportItem.Visibility, startHidden);
		}

		internal static bool IsVisible(Visibility visibility, bool startHidden)
		{
			if (visibility == null)
			{
				return true;
			}
			if (visibility.Toggle != null)
			{
				return true;
			}
			if (visibility.Hidden != null)
			{
				if (ExpressionInfo.Types.Constant == visibility.Hidden.Type)
				{
					return true;
				}
				return !startHidden;
			}
			return true;
		}

		internal static bool IsVisible(SharedHiddenState state, bool hidden, bool hasToggle)
		{
			if (state == SharedHiddenState.Always)
			{
				return true;
			}
			if (SharedHiddenState.Never == state)
			{
				return true;
			}
			if (hasToggle)
			{
				return true;
			}
			return !hidden;
		}

		internal static bool IsTableCellVisible(bool[] tableColumnsVisible, int startIndex, int colSpan)
		{
			Global.Tracer.Assert(startIndex >= 0 && colSpan > 0 && tableColumnsVisible != null && startIndex + colSpan <= tableColumnsVisible.Length);
			bool flag = false;
			for (int i = 0; i < colSpan; i++)
			{
				if (flag)
				{
					break;
				}
				flag |= tableColumnsVisible[startIndex + i];
			}
			return flag;
		}
	}
}
