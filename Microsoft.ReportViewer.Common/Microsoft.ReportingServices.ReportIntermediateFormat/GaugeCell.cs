using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GaugeCell : Cell, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private List<GaugeInputValue> m_gaugeInputValues;

		[NonSerialized]
		private GaugeCellExprHost m_exprHost;

		protected override bool IsDataRegionBodyCell => true;

		public Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType => Microsoft.ReportingServices.ReportProcessing.ObjectType.GaugeCell;

		public override Microsoft.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => ObjectType;

		protected override Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => Microsoft.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.GaugePanel;

		internal GaugeCell()
		{
		}

		internal GaugeCell(int id, GaugePanel gaugePanel)
			: base(id, gaugePanel)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			_ = context.ExprHostBuilder;
			List<GaugeInputValue> gaugeInputValues = GetGaugeInputValues();
			if (gaugeInputValues != null)
			{
				for (int i = 0; i < gaugeInputValues.Count; i++)
				{
					gaugeInputValues[i].Initialize(context, i);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, memberInfoList);
		}

		internal void SetExprHost(GaugeCellExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			IList<GaugeInputValueExprHost> gaugeInputValueHostsRemotable = m_exprHost.GaugeInputValueHostsRemotable;
			List<GaugeInputValue> gaugeInputValues = GetGaugeInputValues();
			if (gaugeInputValues != null && gaugeInputValueHostsRemotable != null)
			{
				for (int i = 0; i < gaugeInputValues.Count; i++)
				{
					GaugeInputValue gaugeInputValue = gaugeInputValues[i];
					if (gaugeInputValue != null && gaugeInputValue.ExpressionHostID > -1)
					{
						gaugeInputValue.SetExprHost(gaugeInputValueHostsRemotable[gaugeInputValue.ExpressionHostID], reportObjectModel);
					}
				}
			}
			BaseSetExprHost(exprHost, reportObjectModel);
		}

		private List<GaugeInputValue> GetGaugeInputValues()
		{
			if (m_gaugeInputValues == null)
			{
				m_gaugeInputValues = ((GaugePanel)base.DataRegionDef).GetGaugeInputValues();
			}
			return m_gaugeInputValues;
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeCell;
		}
	}
}
