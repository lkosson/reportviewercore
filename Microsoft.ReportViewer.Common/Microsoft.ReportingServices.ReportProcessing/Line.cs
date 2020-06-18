using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Line : ReportItem
	{
		private bool m_slanted;

		private const string ZeroSize = "0mm";

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		internal override ObjectType ObjectType => ObjectType.Line;

		internal bool LineSlant
		{
			get
			{
				return m_slanted;
			}
			set
			{
				m_slanted = value;
			}
		}

		internal Line(ReportItem parent)
			: base(parent)
		{
		}

		internal Line(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.LineStart(m_name);
			base.Initialize(context);
			double heightValue = m_heightValue;
			double widthValue = m_widthValue;
			double topValue = m_topValue;
			double leftValue = m_leftValue;
			if ((0.0 > heightValue && 0.0 <= widthValue) || (0.0 > widthValue && 0.0 <= heightValue))
			{
				m_slanted = true;
			}
			m_heightValue = Math.Abs(heightValue);
			m_widthValue = Math.Abs(widthValue);
			if (0.0 <= heightValue)
			{
				m_topValue = topValue;
			}
			else
			{
				m_topValue = topValue + heightValue;
				if (0.0 > m_topValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeTopHeight, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (0.0 <= widthValue)
			{
				m_leftValue = leftValue;
			}
			else
			{
				m_leftValue = leftValue + widthValue;
				if (0.0 > m_leftValue)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsNegativeLeftWidth, Severity.Error, context.ObjectType, context.ObjectName, null);
				}
			}
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			base.ExprHostID = context.ExprHostBuilder.LineEnd();
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				m_exprHost = reportExprHost.LineHostsRemotable[base.ExprHostID];
				ReportItemSetExprHost(m_exprHost, reportObjectModel);
			}
		}

		internal override void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			if (overwrite)
			{
				m_top = "0mm";
				m_topValue = 0.0;
				m_left = "0mm";
				m_leftValue = 0.0;
			}
			if (m_width == null || (overwrite && m_widthValue > 0.0 && m_widthValue != width))
			{
				m_width = width.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_widthValue = context.ValidateSize(ref m_width, "Width");
			}
			if (m_height == null || (overwrite && m_heightValue > 0.0 && m_heightValue != height))
			{
				m_height = height.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_heightValue = context.ValidateSize(ref m_height, "Height");
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Slanted, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
