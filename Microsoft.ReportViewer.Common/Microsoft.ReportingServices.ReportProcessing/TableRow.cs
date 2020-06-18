using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableRow : IDOwner
	{
		private ReportItemCollection m_reportItems;

		private IntList m_IDs;

		private IntList m_colSpans;

		private string m_height;

		private double m_heightValue;

		private Visibility m_visibility;

		[NonSerialized]
		private bool m_startHidden;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private ReportSize m_heightForRendering;

		[NonSerialized]
		private string[] m_renderingModelIDs;

		internal ReportItemCollection ReportItems
		{
			get
			{
				return m_reportItems;
			}
			set
			{
				m_reportItems = value;
			}
		}

		internal IntList IDs
		{
			get
			{
				return m_IDs;
			}
			set
			{
				m_IDs = value;
			}
		}

		internal IntList ColSpans
		{
			get
			{
				return m_colSpans;
			}
			set
			{
				m_colSpans = value;
			}
		}

		internal string Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return m_heightValue;
			}
			set
			{
				m_heightValue = value;
			}
		}

		internal Visibility Visibility
		{
			get
			{
				return m_visibility;
			}
			set
			{
				m_visibility = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return m_renderingModelID;
			}
			set
			{
				m_renderingModelID = value;
			}
		}

		internal ReportSize HeightForRendering
		{
			get
			{
				return m_heightForRendering;
			}
			set
			{
				m_heightForRendering = value;
			}
		}

		internal string[] RenderingModelIDs
		{
			get
			{
				return m_renderingModelIDs;
			}
			set
			{
				m_renderingModelIDs = value;
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

		internal TableRow()
		{
		}

		internal TableRow(int id, int idForReportItems)
			: base(id)
		{
			m_reportItems = new ReportItemCollection(idForReportItems, normal: false);
			m_colSpans = new IntList();
		}

		internal bool Initialize(bool registerRunningValues, int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			int num = 0;
			for (int i = 0; i < m_colSpans.Count; i++)
			{
				num += m_colSpans[i];
			}
			if (numberOfColumns != num)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsWrongNumberOfTableCells, Severity.Error, context.ObjectType, context.ObjectName, "TableCells");
			}
			m_heightValue = context.ValidateSize(ref m_height, "Height");
			tableHeight = Math.Round(tableHeight + m_heightValue, Validator.DecimalPrecision);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: true, tableRowCol: true);
			}
			bool result = m_reportItems.Initialize(context, registerRunningValues, tableColumnVisibility);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
			return result;
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: true);
			}
			m_reportItems.RegisterReceiver(context);
			if (m_visibility != null)
			{
				m_visibility.UnRegisterReceiver(context);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.IDs, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.ColSpans, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.Height, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}
